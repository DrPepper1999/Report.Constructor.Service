CREATE OR ALTER PROCEDURE [dbo].[CreatedScreenshotsReport] @BegDate DATETIME,
                                                           @EndDate DATETIME,
                                                           @OperationScreenshotJobCrudItems dbo.ParamTableOperationScreenshotJobCrud READONLY,
                                                           @UserGroupIds dbo.ParamTableGuids READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
    ELSE
        SET @shallFilterUserGroups = 0;

    DECLARE @tmp_users TABLE
                       (
                           ParentGroupName NVARCHAR(200),
                           GroupName       NVARCHAR(200),
                           UserId          UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                           [Delete]        BIT,
                           ad              NVARCHAR(100),
                           base            NVARCHAR(100),
                           sudir           NVARCHAR(100),
                           fio             NVARCHAR(200)
                       );
    DECLARE @tmp_cam TABLE
                     (
                         ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                         co BIGINT
                     );
    DECLARE @tmp_cam_act TABLE
                         (
                             ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                             co BIGINT
                         );
    DECLARE @tmp_act_job TABLE
                         (
                             ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                             co BIGINT
                         );
    DECLARE @tmp_crud_job TABLE
                          (
                              ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                              co BIGINT
                          );
    DECLARE @tmp_result TABLE
                        (
                            ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                            co BIGINT
                        );


    WITH C (GroupID, GroupName, ParentGroupID, ParentGroupName, NestingLevel, Line) AS
             (SELECT ug.Id,
                     ug.FullName,
                     ug.Id,
                     ug.FullName,
                     1,
                     ROW_NUMBER() OVER (PARTITION BY ug.ParentUserGroupId ORDER BY ug.id)
              FROM AggregateTables_UserGroups AS ug WITH (NOLOCK)
              WHERE (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
              UNION ALL
              SELECT ugg.Id, ugg.FullName, ugg.ParentUserGroupId, c.GroupName, (c.NestingLevel + 1), c.Line
              FROM AggregateTables_UserGroups AS ugg,
                   c
              WHERE c.GroupID = ugg.ParentUserGroupId)


    INSERT
    INTO @tmp_users
    SELECT c.ParentGroupName,
           c.GroupName,
           u.UserId,
           u.[Delete],
           u.ad,
           u.base,
           u.sudir,
           u.fio
    FROM AggregateTables_Users AS u
             JOIN c ON c.GroupID = u.UserGroupId

--камер в расписаниях
    INSERT INTO @tmp_cam
    SELECT osjc.UserId, COUNT(1)
      FROM @OperationScreenshotJobCrudItems AS osjc
      JOIN MoscowVideo_ScreenshotJob AS sj ON sj.Id = osjc.JobId
      JOIN MoscowVideo_Cameras_CameraGroups AS ccg ON sj.CameraGroupId = ccg.CameraGroupId
     GROUP BY osjc.UserId

--активных расписаний
    INSERT INTO @tmp_act_job
    SELECT osjc.UserId, COUNT(1)
      FROM @OperationScreenshotJobCrudItems AS osjc
      JOIN MoscowVideo_ScreenshotJob AS sj ON sj.Id = osjc.JobId
     WHERE sj.IsActive = 1
     GROUP BY osjc.UserId

--создано расписаний
    INSERT INTO @tmp_crud_job
    SELECT osjc.UserId, COUNT(1)
      FROM @OperationScreenshotJobCrudItems AS osjc
      JOIN MoscowVideo_Users AS u ON u.Id = osjc.UserId
     GROUP BY osjc.UserId

--камер в активных расписаниях
    INSERT INTO @tmp_cam_act
    SELECT osjc.UserId, COUNT(1)
      FROM @OperationScreenshotJobCrudItems AS osjc
      JOIN MoscowVideo_ScreenshotJob AS sj ON sj.Id = osjc.JobId
      JOIN MoscowVideo_Cameras_CameraGroups AS ccg ON sj.CameraGroupId = ccg.CameraGroupId
     WHERE sj.IsActive = 1
    GROUP BY osjc.UserId

--Кол-во снимков с активых расписаний за период
    INSERT INTO @tmp_result
    SELECT osjc.UserId, COUNT(1)
      FROM @OperationScreenshotJobCrudItems osjc
      JOIN MoscowVideo_ScreenshotJob AS sj ON sj.Id = osjc.JobId
      JOIN MoscowVideo_ScreenshotJobResults AS sjr ON sjr.ScreenshotJobId = sj.Id
    WHERE sj.IsActive = 1
      AND sjr.RegisteredDate >= @begdate
      AND sjr.RegisteredDate < @enddate
    GROUP BY osjc.UserId

    SELECT u.ParentGroupName AS 'ParentGroup',
           u.GroupName       AS 'Group',
           (CASE
                WHEN u.[Delete] = 1 THEN 'Deleted'
                WHEN u.[Delete] = 0 THEN 'Active'
               END)          AS 'Status',
           u.ad              AS 'ADLogin',
           u.base            AS 'BasicLogin',
           u.sudir           AS 'SudirLogin',
           u.fio             AS 'FullName',
           isnull(cj.co, 0)  AS 'CreatedSchedulesCount',
           isnull(taj.co, 0) AS 'ActiveSchedulesCount',
           isnull(tc.co, 0)  AS 'CamerasInNewSchedulesCount',
           isnull(tca.co, 0) AS 'CamerasInActiveSchedulesCount',
           isnull(tr.co, 0)  AS 'SnapshotsFromActiveSchedulesInPeriod'
    FROM @tmp_users u
             left JOIN @tmp_crud_job cj ON cj.ui = u.UserId
             left JOIN @tmp_act_job AS taj ON taj.ui = u.UserId
             left JOIN @tmp_cam AS tc ON tc.ui = u.UserId
             left JOIN @tmp_cam_act AS tca ON tca.ui = u.UserId
             LEFT JOIN @tmp_result AS tr ON tr.ui = u.UserId


END