CREATE OR ALTER PROCEDURE [dbo].[EchdUserActionsReport] @BegDate DATETIME,
                                                        @EndDate DATETIME,
                                                        @UserGroupIds dbo.ParamTableGuids READONLY
AS
    SET NOCOUNT ON
BEGIN

    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
    ELSE
        SET @shallFilterUserGroups = 0;

    DECLARE @tmp_users TABLE
                       (
                           ParentGroupName NVARCHAR(MAX),
                           GroupName       NVARCHAR(MAX),
                           UserId          UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                           [Delete]        BIT,
                           ad              NVARCHAR(MAX),
                           base            NVARCHAR(MAX),
                           sudir           NVARCHAR(MAX),
                           fio             NVARCHAR(MAX)
                       );
    DECLARE @tmp_active TABLE
                        (
                            ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                            co BIGINT
                        );
    DECLARE @tmp_view_uni TABLE
                          (
                              ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                              co BIGINT
                          );
    DECLARE @tmp_camera_controll TABLE
                                 (
                                     ui  UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                                     ucc BIGINT,
                                     co  BIGINT
                                 );
    DECLARE @tmp_view_live TABLE
                           (
                               ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                               co BIGINT,
                               ti BIGINT
                           );
    DECLARE @tmp_view_arh TABLE
                          (
                              ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                              co BIGINT,
                              ti BIGINT
                          );
    DECLARE @tmp_arh TABLE
                     (
                         ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                         co BIGINT,
                         ti BIGINT
                     );
    DECLARE @tmp_problem TABLE
                         (
                             ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                             co BIGINT
                         );
    DECLARE @tmp_act_day TABLE
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
              WHERE (ug.Id IN (SELECT Value FROM @UserGroupIds)
                  OR @shallFilterUserGroups = 0)
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

    INSERT INTO @tmp_active
    SELECT aa.UserId AS ui, SUM(aa.[Count]) AS co
    FROM AggregateTables_AllActive AS aa
    WHERE aa.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND aa.[Date] < @endDate
    GROUP BY aa.UserId


    INSERT INTO @tmp_view_uni
    SELECT uni.ui AS ui, COUNT(1) AS co
    FROM (SELECT DISTINCT ca.UserId AS ui, ca.CameraId AS ci
          FROM AggregateTables_CameraActivities AS ca
          WHERE ca.TimeStart >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND ca.TimeStart < @endDate) AS uni
    GROUP BY uni.ui;
    WITH tmpCamerasControll
             AS
             (SELECT cc.UserId, cc.CamId, SUM(cc.[Count]) AS co
              FROM AggregateTables_CamerasControll AS cc WITH (NOLOCK)
              WHERE cc.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @BegDate)))
                AND cc.[Date] < @EndDate
              GROUP BY cc.UserId, cc.CamId)
    INSERT
    INTO @tmp_camera_controll
    SELECT DISTINCT t.UserId,
                    SUM(1) OVER (PARTITION BY t.UserId),
                    SUM([co]) OVER (PARTITION BY t.UserId)
    FROM tmpCamerasControll t

    insert INTO @tmp_view_live
    SELECT lv.UserId AS ui, SUM(lv.[Count]) AS co, SUM(lv.[Time]) AS ti
    FROM AggregateTables_LiveView AS lv
    WHERE lv.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND lv.[Date] < @endDate
    GROUP BY lv.UserId

    INSERT INTO @tmp_view_arh
    SELECT av.UserId AS ui, SUM(av.[Count]) AS co, SUM(av.[Time]) AS ti
    FROM AggregateTables_ArchiveView AS av
    WHERE av.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND av.[Date] < @endDate
    GROUP BY av.UserId

    INSERT INTO @tmp_arh
    SELECT ao.UserId AS ui, COUNT(1) AS co, SUM(DATEDIFF(ss, ae.BeginTime, ae.EndTime)) AS ti
    FROM MoscowVideo_ArchiveEntries ae
             JOIN MoscowVideo_ArchiveOrders AS ao
                  ON ao.ArchiveEntryId = ae.Id
    WHERE ae.DateCreated BETWEEN DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate))) AND @endDate
    GROUP BY ao.UserId

    INSERT INTO @tmp_problem
    SELECT pr.UserId AS ui, COUNT(1) AS co
    FROM MoscowVideo_ProblemTickets AS pt
             JOIN MoscowVideo_ProblemReports AS pr ON pr.ProblemTicketId = pt.Id
    WHERE pr.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND pr.[Date] < @endDate
    GROUP BY pr.UserId

    INSERT INTO @tmp_act_day
    SELECT aa.UserId ui, COUNT(1) co
    FROM AggregateTables_AllActive AS aa
    WHERE aa.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND aa.[Date] < @endDate
    GROUP BY aa.UserId


    SELECT u.ParentGroupName                               AS 'ParentGroup',
           u.GroupName                                     AS 'Group',
           (CASE
                WHEN u.[Delete] = 1 THEN 'Deleted'
                WHEN u.[Delete] = 0 THEN 'Active'
               END)                                        AS 'Status',
           u.ad                                            AS 'ADLogin',
           u.base                                          AS 'BasicLogin',
           u.sudir                                         AS 'SudirLogin',
           u.fio                                           AS 'FullName',
           isnull(av.co, 0)                                AS 'ActiveDays',
           ISNULL(ac.co, 0)                                AS 'Activity',
           ISNULL(vl.co, 0) + ISNULL(varh.co, 0)           AS 'TotalViews',
           ISNULL(vl.ti, 0) / 60 + ISNULL(varh.ti, 0) / 60 AS 'DurationInMinutes',               --Длительностью, мин
           ISNULL(vu.co, 0)                                AS 'UniqueCameras',
           ISNULL(cc.co, 0)                                AS 'Controls',
           ISNULL(cc.ucc, 0)                               AS 'UniqueCamerasByControls',--Уникальных камер по управлениям
           ISNULL(vl.co, 0)                                AS 'LiveViews',
           ISNULL(vl.ti, 0) / 60                           AS 'LiveDurationInMinutes',           --Длительность Live, мин
           ISNULL(varh.co, 0)                              AS 'ArchiveViews',
           ISNULL(varh.ti, 0) / 60                         AS 'ArchiveDurationInMinutes',        --Длительность АРХИВ, мин
           ISNULL(ar.co, 0)                                AS 'OrderedArchives',
           ISNULL(ar.ti, 0) / 60                           AS 'OrderedArchiveDurationInMinutes', --Длительность заказаных архивов, мин              
           ISNULL(prcam.co, 0)                             AS 'PortalComplaints'
    FROM @tmp_users u
             LEFT JOIN @tmp_active AS ac ON u.UserId = ac.Ui
             LEFT JOIN @tmp_act_day AS av ON u.UserId = av.ui
             LEFT JOIN @tmp_view_uni AS vu ON u.UserId = vu.ui
             LEFT JOIN @tmp_camera_controll AS cc ON u.UserId = cc.ui
             LEFT JOIN @tmp_view_live AS vl ON u.UserId = vl.ui
             LEFT JOIN @tmp_view_arh AS varh ON u.UserId = varh.ui
             LEFT JOIN @tmp_arh AS ar ON u.UserId = ar.ui
             LEFT JOIN @tmp_problem AS prcam ON u.UserId = prcam.ui

END