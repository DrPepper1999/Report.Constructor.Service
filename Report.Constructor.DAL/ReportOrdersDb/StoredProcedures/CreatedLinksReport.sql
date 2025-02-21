CREATE OR ALTER PROCEDURE [dbo].[CreatedLinksReport] @BegDate DATETIME,
                                                     @EndDate DATETIME,
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
                           UserId          UNIQUEIDENTIFIER,
                           [Delete]        BIT,
                           ad              NVARCHAR(100),
                           base            NVARCHAR(100),
                           sudir           NVARCHAR(100),
                           fio             NVARCHAR(250)
                       );
    DECLARE @tmp_create_live_link TABLE
                                  (
                                      ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                                      co BIGINT
                                  );
    DECLARE @tmp_create_arch_link TABLE
                                  (
                                      ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                                      co BIGINT
                                  );
    DECLARE @tmp_view_live_link TABLE
                                (
                                    ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                                    co BIGINT
                                );
    DECLARE @tmp_view_arch_link TABLE
                                (
                                    ui UNIQUEIDENTIFIER PRIMARY KEY CLUSTERED,
                                    co BIGINT
                                );
    DECLARE @tmp_avtorise TABLE
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
    DECLARE @tmp_unicam TABLE
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
    FROM AggregateTables_Users u
             JOIN c ON c.GroupID = u.UserGroupId

    INSERT INTO @tmp_unicam
    SELECT share.UserId, COUNT(1) co
    FROM (SELECT distinct osla.UserId, osla.CameraId
          FROM Audit_OperationSharedLinkAction AS osla
                   JOIN Audit_AuditEntry AS ae ON osla.Id = ae.Id
          WHERE osla.ActionType = 1
            and ae.[TimeStamp] >= @BegDate
            AND ae.[TimeStamp] < @EndDate) share
    GROUP BY share.UserId

    INSERT INTO @tmp_create_live_link
    SELECT osla.UserId, COUNT(distinct osla.id)
    FROM Audit_AuditEntry AS ae
             JOIN Audit_OperationSharedLinkAction AS osla ON osla.Id = ae.Id
             JOIN Audit_EntryValues AS ev ON ev.Entry_Id = ae.Id
    WHERE ae.[TimeStamp] >= @BegDate
      AND ae.[TimeStamp] < @EndDate
      AND osla.LinkType = 1
      AND osla.ActionType = 1 --пользователь создал линк на Live
    GROUP BY osla.UserId

    INSERT INTO @tmp_create_arch_link
    SELECT osla.UserId, COUNT(distinct osla.id)
    FROM Audit_AuditEntry AS ae
             JOIN Audit_OperationSharedLinkAction AS osla ON osla.Id = ae.Id
             JOIN Audit_EntryValues AS ev ON ev.Entry_Id = ae.Id
    WHERE ae.[TimeStamp] >= @BegDate
      AND ae.[TimeStamp] < @EndDate
      AND osla.LinkType = 2
      AND osla.ActionType = 1 --пользователь создал линк на Архив
    GROUP BY osla.UserId

    INSERT INTO @tmp_view_live_link
    SELECT osla.UserId, COUNT(1)
    FROM Audit_AuditEntry AS ae
             JOIN Audit_OperationSharedLinkAction AS osla ON osla.Id = ae.Id
             JOIN Audit_EntryValues AS ev ON ev.Entry_Id = ae.Id
    WHERE ae.[TimeStamp] >= @BegDate
      AND ae.[TimeStamp] < @EndDate
      AND osla.LinkType = 1
      AND osla.ActionType = 2 --Просмотренно ссылок Live
    GROUP BY osla.UserId

    INSERT INTO @tmp_view_arch_link
    SELECT osla.UserId, COUNT(1)
    FROM Audit_AuditEntry AS ae
             JOIN Audit_OperationSharedLinkAction AS osla ON osla.Id = ae.Id
             JOIN Audit_EntryValues AS ev ON ev.Entry_Id = ae.Id
    WHERE ae.[TimeStamp] >= @BegDate
      AND ae.[TimeStamp] < @EndDate
      AND osla.LinkType = 1
      AND osla.ActionType = 2 --Просмотренно ссылок Архив
    GROUP BY osla.UserId

    insert into @tmp_avtorise
    SELECT osla.UserId, COUNT(1)
    FROM Audit_AuditEntry AS ae
             JOIN Audit_OperationSharedLinkAction AS osla ON osla.Id = ae.Id
             JOIN Audit_EntryValues AS ev ON ev.Entry_Id = ae.Id
    WHERE ae.[TimeStamp] >= @BegDate
      AND ae.[TimeStamp] < @EndDate
      AND osla.ActionType = 3 --Завершилось авторизацией
    GROUP BY osla.UserId


    SELECT u.ParentGroupName  AS 'ParentGroup',
           u.GroupName        AS 'Group',
           (CASE
                WHEN u.[Delete] = 1 THEN 'Deleted'
                WHEN u.[Delete] = 0 THEN 'Active'
               END)           AS 'Status',
           u.ad               AS 'ADLogin',
           u.base             AS 'BasicLogin',
           u.sudir            AS 'SudirLogin',
           u.fio              AS 'FullName',
           isnull(uc.co, 0)   AS 'UniqueCamerasCount',
           isnull(tcll.co, 0) AS 'GeneratedLiveLinksCount',
           isnull(tcal.co, 0) AS 'GeneratedArchiveLinksCount',
           isnull(tvll.co, 0) AS 'LiveLinkClicks',
           isnull(tval.co, 0) AS 'ArchiveLinkClicks',
           isnull(ta.co, 0)   AS 'AuthenticatedLinkClicks'
    FROM @tmp_users u
             LEFT JOIN @tmp_create_live_link AS tcll ON u.UserId = tcll.Ui
             LEFT JOIN @tmp_create_arch_link AS tcal ON u.UserId = tcal.ui
             LEFT JOIN @tmp_view_live_link AS tvll ON u.UserId = tvll.ui
             LEFT JOIN @tmp_view_arch_link AS tval ON u.UserId = tval.ui
             LEFT JOIN @tmp_avtorise AS ta ON u.UserId = ta.ui
             LEFT JOIN @tmp_unicam AS uc ON uc.ui = u.UserId

END