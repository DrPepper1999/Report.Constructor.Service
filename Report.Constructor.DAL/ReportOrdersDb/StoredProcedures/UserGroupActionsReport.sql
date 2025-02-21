CREATE OR ALTER PROCEDURE [dbo].[UserGroupActionsReport] @BegDate DATETIME,
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
                           ParentGroupName NVARCHAR(200),
                           GroupName       NVARCHAR(200),
                           UserId          UNIQUEIDENTIFIER,
                           UserGroupId     UNIQUEIDENTIFIER,
                           [Delete]        BIT,
                           ad              NVARCHAR(200),
                           base            NVARCHAR(200),
                           sudir           NVARCHAR(200),
                           fio             NVARCHAR(200)
                       );

    DECLARE @user_all TABLE
                      (
                          ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                          co BIGINT
                      );

    DECLARE @user_act TABLE
                      (
                          ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                          co BIGINT
                      );

    DECLARE @user_new TABLE
                      (
                          ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                          co BIGINT
                      );

    DECLARE @user_del TABLE
                      (
                          ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                          co BIGINT
                      );

    DECLARE @tmp_view_live TABLE
                           (
                               ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                               co BIGINT,
                               ti BIGINT
                           );

    DECLARE @uni_live TABLE
                      (
                          ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                          co BIGINT
                      );

    DECLARE @uni_arh TABLE
                     (
                         ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                         co BIGINT
                     );

    DECLARE @tmp_view_arh TABLE
                          (
                              ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                              co BIGINT,
                              ti BIGINT
                          );

    DECLARE @tmp_camera_controll TABLE
                                 (
                                     ui  UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                                     ucc BIGINT,
                                     co  BIGINT
                                 );

    DECLARE @tmp_active TABLE
                        (
                            ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                            co BIGINT
                        );

    DECLARE @tmp_act_day TABLE
                         (
                             ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                             co BIGINT
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

    WITH C (GroupID, GroupName, ParentGroupID, ParentGroupName, NestingLevel, Line) AS
             (SELECT ug.Id,
                     ug.FullName,
                     ug.Id,
                     ug.FullName,
                     1,
                     ROW_NUMBER() OVER (PARTITION BY ug.ParentUserGroupId ORDER BY ug.id)
              FROM AggregateTables_UserGroups AS ug WITH (NOLOCK)
              WHERE ug.id IN (SELECT Value FROM @UserGroupIds)
                 OR @shallFilterUserGroups = 0 --= @usergroupid 
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
           c.GroupID,
           u.[Delete],
           u.ad,
           u.base,
           u.sudir,
           u.fio
    FROM c AS C
             JOIN AggregateTables_Users AS u ON c.GroupID = u.UserGroupId

    INSERT INTO @user_all
    SELECT tu.UserGroupId, COUNT(1) co
    FROM @tmp_users tu
    WHERE tu.UserId IS NOT null
    GROUP BY tu.UserGroupId
    union
    SELECT tu.UserGroupId, 0
    FROM @tmp_users tu
    WHERE tu.UserId IS null

    INSERT INTO @user_del
    SELECT tu.UserGroupId, COUNT(1) co
    FROM @tmp_users tu
    WHERE tu.[Delete] = 1
    GROUP BY tu.UserGroupId

    INSERT INTO @user_new
    SELECT fuck.UserGroupId, COUNT(1)
    FROM (SELECT MIN(act.[Date]) AS [date], act.UserId, act.UserGroupId
          FROM (SELECT lv.[Date], lv.UserId, u.UserGroupId
                FROM AggregateTables_LiveView AS lv
                         JOIN AggregateTables_Users AS u ON u.UserId = lv.UserId
                UNION
                SELECT av.[Date], av.UserId, u.UserGroupId
                FROM AggregateTables_ArchiveView AS av
                         JOIN AggregateTables_Users AS u ON u.UserId = av.UserId
                union
                SELECT a.[Date], a.UserId, u.UserGroupId
                FROM AggregateTables_Active AS a
                         JOIN AggregateTables_Users AS u ON u.UserId = a.UserId) act
          GROUP BY act.UserGroupId, act.UserId) fuck
    WHERE fuck.[date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND fuck.[date] < @enddate
    GROUP BY fuck.UserGroupId


    insert INTO @tmp_view_live
    SELECT lv.UserId, sum(cast(lv.[Count] AS BIGINT)), SUM(cast(lv.[Time] AS BIGINT))
    FROM AggregateTables_LiveView AS lv
    WHERE lv.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND lv.[Date] < @EndDate
    GROUP BY lv.UserId

    INSERT INTO @user_act
    SELECT actu.UserGroupId, COUNT(1) co
    FROM (SELECT tvl.ui, tu.UserGroupId
          from @tmp_view_live AS tvl
                   JOIN @tmp_users AS tu ON tu.UserId = tvl.ui
          UNION
          SELECT tva.ui, tu.UserGroupId
          from @tmp_view_arh AS tva
                   JOIN @tmp_users AS tu ON tu.UserId = tva.ui
          UNION
          SELECT a.UserId, tu.UserGroupId
          FROM AggregateTables_Active AS a
                   JOIN @tmp_users AS tu ON tu.UserId = a.UserId
          WHERE a.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND a.[Date] < @enddate) actu
    GROUP BY actu.UserGroupId

    INSERT INTO @uni_live
    SELECT lu.UserId, COUNT(1)
    FROM (SELECT distinct lv.UserId, lv.CamId
          FROM AggregateTables_LiveView AS lv
          WHERE lv.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND lv.[Date] < @EndDate) lu
    GROUP BY lu.UserId

    INSERT INTO @uni_arh
    SELECT au.UserId, COUNT(1)
    FROM (SELECT distinct av.UserId, av.CamId
          FROM AggregateTables_ArchiveView AS av
          WHERE av.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND av.[Date] < @EndDate) au
    GROUP BY au.UserId

    insert INTO @tmp_view_arh
    SELECT av.UserId, sum(cast(av.[Count] AS BIGINT)), SUM(cast(av.[Time] AS BIGINT))
    FROM AggregateTables_ArchiveView AS av
    WHERE av.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND av.[Date] < @EndDate
    GROUP BY av.UserId;

    WITH UsersCamerasAndControlCount
             AS
             (SELECT cc.UserId, cc.CamId, u.UserGroupId, SUM(cc.[Count]) co
              FROM AggregateTables_CamerasControll AS cc
                       JOIN @tmp_users u ON u.UserId = cc.UserId
              WHERE cc.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @BegDate)))
                AND cc.[Date] < @EndDate
              GROUP BY cc.UserId, cc.CamId, u.UserGroupId),
         UsersTotalControlCountByUserAndCameraEnumeration
             AS
             (SELECT UserId,
                     SUM(a.co) OVER (PARTITION BY UserId, UserGroupId)       AS [co],
                     ROW_NUMBER() OVER (PARTITION BY CamId ORDER BY [CamId]) AS [ucc]
              FROM UsersCamerasAndControlCount a)
    INSERT
    INTO @tmp_camera_controll
    SELECT UserId,
           SUM(CASE WHEN ucc = 1 THEN 1 ELSE 0 END) AS [ucc],
           MIN(co)                                  AS [co]
    FROM UsersTotalControlCountByUserAndCameraEnumeration b
    GROUP BY b.UserId

    INSERT INTO @tmp_active
    SELECT a.UserId, SUM(CAST(a.[Count] AS BIGINT))
    FROM AggregateTables_Active AS a
    WHERE a.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND a.[Date] < @endDate
    GROUP BY a.UserId

    INSERT INTO @tmp_act_day
    SELECT ad.UserId, COUNT(1)
    from (SELECT DISTINCT a.[Date], a.UserId
          FROM AggregateTables_Active AS a
          WHERE a.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND a.[Date] < @EndDate
          UNION
          SELECT DISTINCT lv.[Date], lv.UserId
          FROM AggregateTables_LiveView AS lv
          WHERE lv.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND lv.[Date] < @EndDate
          UNION
          SELECT DISTINCT av.[Date], av.UserId
          FROM AggregateTables_ArchiveView AS av
          WHERE av.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
            AND av.[Date] < @EndDate) ad
    GROUP BY ad.UserId

    INSERT INTO @tmp_arh
    SELECT ao.UserId AS ui, COUNT(1) AS co, SUM(DATEDIFF(ss, ae.BeginTime, ae.EndTime)) AS ti
    FROM MoscowVideo_ArchiveEntries ae
             JOIN MoscowVideo_ArchiveOrders AS ao
                  ON ao.ArchiveEntryId = ae.Id
    WHERE ao.DateCreated BETWEEN DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate))) AND @endDate
    GROUP BY ao.UserId

    INSERT INTO @tmp_problem
    SELECT pr.UserId AS ui, COUNT(1) AS co
    FROM MoscowVideo_ProblemTickets AS pt
             JOIN MoscowVideo_ProblemReports AS pr ON pr.ProblemTicketId = pt.Id
    WHERE pr.[Date] >= DATEADD(dd, 0, (DATEDIFF(dd, 0, @begDate)))
      AND pr.[Date] < @endDate
    GROUP BY pr.UserId

    SELECT u.ParentGroupName                                                                                    AS 'ParentGroup',
           u.GroupName                                                                                          AS 'Group',
           uall.co                                                                                              AS 'UserCount',
           isnull(udel.co, 0)                                                                                   AS 'DeletedUsers',
           isnull(uac.co, 0)                                                                                    AS 'ActiveUserCount',
           sum(ISNULL(ac.co, 0) + ISNULL(vl.co, 0) + ISNULL(varh.co, 0) +
               ISNULL(cc.co, 0))                                                                                AS 'Activity',
           sum(ISNULL(vl.co, 0) + ISNULL(varh.co, 0))                                                           AS 'ViewCount',
           sum(ISNULL(vl.ti, 0) + ISNULL(varh.ti, 0)) / 60                                                      AS 'DurationInMinutes',             --длительность просмотров в мин
           sum(ISNULL(ul.co, 0))                                                                                AS 'UniqueCameras',
           sum(ISNULL(cc.co, 0))                                                                                AS 'Controls',
           sum(ISNULL(cc.ucc, 0))                                                                               AS 'UniqueCamerasByControls',--Уникальных камер по управлениям
           sum(ISNULL(vl.co, 0))                                                                                AS 'LiveViews',
           sum(ISNULL(varh.co, 0))                                                                              AS 'ArchiveViews',
           sum(ISNULL(ar.co, 0))                                                                                AS 'OrderedArchives',
           isnull(uac.co, 0) /
           isnull(NULLIF(DATEDIFF(DAY, @begDate, @endDate), 0), 1)                                              AS 'AvgDailyUserAudience',
           sum(ISNULL(vl.ti, 0)) / 60 /
           isnull(NULLIF(DATEDIFF(DAY, @begDate, @endDate), 0), 1)                                              AS 'AvgDailyLiveViewDuration',
           sum(ISNULL(varh.ti, 0)) / 60 /
           isnull(NULLIF(DATEDIFF(DAY, @begDate, @endDate), 0), 1)                                              AS 'AvgDailyArchiveViewDuration',
           sum(ISNULL(vl.co, 0) + ISNULL(varh.co, 0)) /
           isnull(NULLIF(DATEDIFF(DAY, @begDate, @endDate), 0), 1)                                              AS 'AvgDailyCameraOpens',--Среднее дневное кол во открытий камер
           sum(ISNULL(vl.ti, 0)) / 60                                                                           AS 'LiveDurationInMinutes',--Длительность Live, мин
           sum(ISNULL(varh.ti, 0)) / 60                                                                         AS 'ArchiveDurationInMinutes',--Длительность АРХИВ, мин
           sum(ISNULL(ar.ti, 0)) / 60                                                                           AS 'OrderedArchiveDurationInMinutes'--Длительность заказаных архивов, мин
    FROM @tmp_users u
             LEFT JOIN @tmp_active AS ac ON u.UserId = ac.Ui
             LEFT JOIN @tmp_camera_controll AS cc ON u.UserId = cc.ui
             LEFT JOIN @tmp_view_live AS vl ON u.UserId = vl.ui
             LEFT JOIN @tmp_view_arh AS varh ON u.UserId = varh.ui
             LEFT JOIN @tmp_act_day AS av ON u.UserId = av.ui
             LEFT JOIN @uni_live AS ul ON ul.ui = u.UserId
             LEFT JOIN @uni_arh AS ua ON ua.ui = u.UserId
             LEFT JOIN @tmp_arh AS ar ON u.UserId = ar.ui
             LEFT JOIN @tmp_problem AS prcam ON u.UserId = prcam.ui
             LEFT JOIN @user_all AS uall ON uall.ui = u.UserGroupId
             LEFT JOIN @user_del AS udel ON udel.ui = u.UserGroupId
             LEFT JOIN @user_act AS uac ON uac.ui = u.UserGroupId
             LEFT JOIN @user_new AS un ON un.ui = u.UserGroupId
    GROUP BY un.co, uac.co, udel.co, uall.co, u.ParentGroupName, u.GroupName,
             isnull(uac.co, 0) / isnull(NULLIF(DATEDIFF(DAY, @begDate, @endDate), 0), 1)
END