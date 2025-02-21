CREATE OR ALTER PROCEDURE [dbo].[CameraViewsStatisticsReport] @BegDate DATETIME,
                                                              @EndDate DATETIME,
                                                              @CameraId UNIQUEIDENTIFIER,
                                                              @UserGroupIds dbo.ParamTableGuids READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
    ELSE
        SET @shallFilterUserGroups = 0;

    DECLARE @tmp_Live TABLE
                      (
                          [Type]        VARCHAR(45),
                          Token         UNIQUEIDENTIFIER,
                          [ClientIp]    VARCHAR(45),
                          Action        VARCHAR(45),
                          ApplicationId UNIQUEIDENTIFIER,
                          realtime      datetime,
                          UserId        UNIQUEIDENTIFIER
                      );

    DECLARE @tmp_Archive TABLE
                         (
                             [Type]        VARCHAR(45),
                             Token         UNIQUEIDENTIFIER,
                             [ClientIp]    VARCHAR(45),
                             Action        VARCHAR(45),
                             ApplicationId UNIQUEIDENTIFIER,
                             realtime      datetime,
                             UserId        UNIQUEIDENTIFIER
                         );

    DECLARE @tmp_Tokens TABLE
                        (
                            TokenId UNIQUEIDENTIFIER,
                            UserId  UNIQUEIDENTIFIER,
                            [Type]  VARCHAR(45)
                        );

    INSERT INTO @tmp_Tokens
    SELECT t.Id, t.UserId, t.ActivityType
    FROM MoscowVideo_Tokens t
    WHERE t.CameraId = @CameraId
      AND t.CreationDate >= @BegDate
      AND t.CreationDate <= @EndDate
      AND t.ActivityType IN (
                             'Live',
                             'Archive'
        )

    INSERT INTO @tmp_Live
    SELECT 'Лайв',
           lv.TokenId,
           lv.ClientIp,
           lv.[Action],
           ae.ApplicationId,
           DATEADD(s, lv.ActionDate / 1000, '1970-01-01 03:00:00'),
           mt.UserId
    FROM Audit_LiveView lv
             JOIN Audit_AuditEntry AS ae ON ae.Id = lv.Id
             JOIN @tmp_Tokens mt ON mt.TokenId = lv.TokenId AND mt.[Type] = 'Live'

    INSERT INTO @tmp_Archive
    SELECT 'Архив',
           av.TokenId,
           av.ClientIp,
           av.[Action],
           ae.ApplicationId,
           DATEADD(s, av.ActionDate / 1000, '1970-01-01 03:00:00'),
           mt.UserId
    FROM Audit_ArchiveView av
             JOIN Audit_AuditEntry AS ae ON ae.Id = av.Id
             JOIN @tmp_Tokens mt ON mt.TokenId = av.TokenId AND mt.[Type] = 'Archive'
    WHERE av.[Action] IN (
                          'start',
                          'stop'
        )

    SELECT tl.realtime 'Date',
           (CASE
                WHEN tl.[Action] = 'start' THEN 'Start'
                WHEN tl.[Action] = 'stop' THEN 'Stop'
               END)    'Action',
           u.Fio       'UserFullName',
           ug.Fullname 'UserGroupName',
           u.Ad        'ActiveDirectoryLogin',
           u.Sudir     'SudirLogin',
           tl.[Type]   'Type',
           tl.ClientIp 'Ip',
           (CASE
                WHEN tl.ApplicationId = 'D412D576-7F4A-4BED-91B5-717409EA8249' THEN 'Внутренний портал Netris'
                WHEN tl.ApplicationId = 'BEEBF8E0-8FCA-4AA6-8D96-225A664B8D03' THEN 'other'
                WHEN tl.ApplicationId = 'CECDFF74-9C3A-4A38-9EBD-340316300C23' THEN 'Внешний портал Netris'
                WHEN tl.ApplicationId = 'F4588627-C3FF-4114-BCFF-02FCE39545D7' THEN 'Технологический портал'
               END)    'PortalType'
    FROM @tmp_Live tl
             JOIN AggregateTables_Users u ON u.UserId = tl.UserId
             JOIN AggregateTables_UserGroups ug ON u.UserGroupId = ug.Id AND
                                                   (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)

    UNION

    SELECT ta.realtime 'Date',
           (CASE
                WHEN ta.[Action] = 'start' THEN 'Start'
                WHEN ta.[Action] = 'stop' THEN 'Stop'
               END)    'Action',
           u.Fio       'UserFullName',
           ug.Fullname 'UserGroupName',
           u.Ad        'ActiveDirectoryLogin',
           u.Sudir     'SudirLogin',
           ta.[Type]   'Type',
           ta.ClientIp 'Ip',
           (CASE
                WHEN ta.ApplicationId = 'D412D576-7F4A-4BED-91B5-717409EA8249' THEN 'Внутренний портал Netris'
                WHEN ta.ApplicationId = 'BEEBF8E0-8FCA-4AA6-8D96-225A664B8D03' THEN 'other'
                WHEN ta.ApplicationId = 'CECDFF74-9C3A-4A38-9EBD-340316300C23' THEN 'Внешний портал Netris'
                WHEN ta.ApplicationId = 'F4588627-C3FF-4114-BCFF-02FCE39545D7' THEN 'Технологический портал'
               END)    'PortalType'
    FROM @tmp_Archive ta
             JOIN AggregateTables_Users u ON u.UserId = ta.UserId
             JOIN AggregateTables_UserGroups ug ON u.UserGroupId = ug.Id AND
                                                   (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
    WHERE ta.[Action] IN (
                          'start',
                          'stop'
        )
    ORDER BY [Date], [Type] DESC

END