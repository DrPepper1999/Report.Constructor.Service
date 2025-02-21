CREATE OR ALTER PROCEDURE [dbo].[LinksUsageReport] @OperationSharedLinkActionItems dbo.ParamTableOperationSharedLinkActionReportItem READONLY,
                                                   @UserGroupIds dbo.ParamTableGuids READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
    ELSE
        SET @shallFilterUserGroups = 0;

    SELECT st.Name                       'ServiceType',
           subt.Name                     'ServiceSubtype',
           cam.Title                     'Camera',
           d.ShortName                   'District',
           sn.Name + ' ' + a.AddressName 'Address',
           isnull(CrLiLive.co, 0)        'GeneratedLiveLinks',
           isnull(CrLiArh.co, 0)         'GeneratedArchiveLinks',
           isnull(GoLiLive.co, 0)        'LiveLinkClicks',
           isnull(GoLiArh.co, 0)         'ArchiveLinkClicks',
           isnull(avtor.co, 0)           'FinishedWithAuthorization'
    FROM (SELECT distinct c.id, c.Title
          FROM @OperationSharedLinkActionItems AS osla
                   JOIN AggregateTables_Cameras AS c ON c.id = osla.CameraId) cam
             left JOIN
         (SELECT c.id, COUNT(1) co
          FROM @OperationSharedLinkActionItems AS osla
                   JOIN AggregateTables_Cameras AS c ON c.id = osla.CameraId
                   JOIN AggregateTables_Users u ON u.Userid = osla.UserId
                   JOIN AggregateTables_UserGroups ug ON u.UserGroupId = ug.Id
              AND (ug.Id IN (SELECT Value FROM @UserGroupIds)
                  OR @shallFilterUserGroups = 0)
          WHERE osla.ActionType = 1
            AND osla.LinkType = 1 --сгенерировано ссылок на live
          GROUP BY c.id) CrLiLive ON CrLiLive.id = cam.id
             LEFT JOIN
         (SELECT c.id, COUNT(1) co
          FROM @OperationSharedLinkActionItems AS osla
                   JOIN AggregateTables_Cameras AS c ON c.id = osla.CameraId
                   JOIN AggregateTables_Users u ON u.Userid = osla.UserId
                   JOIN AggregateTables_UserGroups ug ON u.UserGroupId = ug.Id
              AND (ug.Id IN (SELECT Value FROM @UserGroupIds)
                  OR @shallFilterUserGroups = 0)
          WHERE osla.ActionType = 1
            AND osla.LinkType = 2 --сгенерировано ссылок на архив
          GROUP BY c.id) CrLiArh ON CrLiArh.id = cam.id
             LEFT JOIN
         (SELECT c.id, COUNT(1) co
          FROM @OperationSharedLinkActionItems AS osla
                   JOIN AggregateTables_Cameras AS c ON c.id = osla.CameraId
                   JOIN AggregateTables_Users u ON u.Userid = osla.UserId
                   JOIN AggregateTables_UserGroups ug
                        ON u.UserGroupId = ug.Id AND (ug.Id IN (SELECT Value FROM @UserGroupIds)
                            OR @shallFilterUserGroups = 0)
          WHERE osla.ActionType = 2
            AND osla.LinkType = 2 --переходов в архив
          GROUP BY c.id) GoLiArh ON GoLiArh.id = cam.id
             LEFT join
         (SELECT c.id, COUNT(1) co
          FROM @OperationSharedLinkActionItems AS osla
                   JOIN AggregateTables_Cameras AS c ON c.id = osla.CameraId
                   JOIN AggregateTables_Users u ON u.Userid = osla.UserId
                   JOIN AggregateTables_UserGroups ug
                        ON u.UserGroupId = ug.Id AND (ug.Id IN (SELECT Value FROM @UserGroupIds)
                            OR @shallFilterUserGroups = 0)
          WHERE osla.ActionType = 2
            AND osla.LinkType = 1 --переходов в live
          GROUP BY c.id) GoLiLive ON GoLiLive.id = cam.id
             LEFT JOIN
         (SELECT c.id, COUNT(1) co
          FROM @OperationSharedLinkActionItems AS osla
                   JOIN AggregateTables_Cameras AS c ON c.id = osla.CameraId
                   JOIN AggregateTables_Users u ON u.Userid = osla.UserId
                   JOIN AggregateTables_UserGroups ug
                        ON u.UserGroupId = ug.Id AND (ug.Id IN (SELECT Value FROM @UserGroupIds)
                            OR @shallFilterUserGroups = 0)
          WHERE osla.ActionType = 3 --завершилось авторизацией
          GROUP BY c.id) avtor ON avtor.id = cam.id
             JOIN MoscowVideo_Cameras c ON c.id = cam.id
             JOIN MoscowVideo_Addresses AS a
                  ON a.Id = c.AddressId
             JOIN MoscowVideo_Districts AS d
                  ON d.Id = a.DistrictId
             JOIN MoscowVideo_Streets AS sn
                  ON sn.Id = a.StreetId
             JOIN MoscowVideo_ServiceTypes AS st
                  ON st.Id = c.ServiceTypeId
             left outer JOIN MoscowVideo_ServiceTypes AS subt
                             ON st.Id = c.SubtypeId
    WHERE (CrLiLive.co IS NOT NULL OR CrLiArh.co IS NOT NULL OR GoLiLive.co IS NOT NULL OR GoLiArh.co IS NOT NULL OR
           avtor.co IS NOT NULL)
    ORDER BY CrLiLive.co desc
END