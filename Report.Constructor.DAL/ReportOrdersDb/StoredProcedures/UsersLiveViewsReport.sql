CREATE OR ALTER PROCEDURE [dbo].[UsersLiveViewsReport] @dateFrom DATETIME,
                                                       @dateTo DATETIME,
                                                       @UserGroupIds dbo.ParamTableGuids READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
    ELSE
        SET @shallFilterUserGroups = 0;

    SELECT lv.[Date]       'Date',
           u.fio           'UserFullName',
           ug.FullName     'UserGroupName',
           ug2.FullName    'ParentGroupName',
           Sum(lv.[Count]) 'ViewsCount'
    FROM AggregateTables_LiveView lv
             JOIN AggregateTables_Users u ON lv.UserId = u.UserId
             JOIN AggregateTables_UserGroups ug ON u.UserGroupId = ug.Id AND
                                                   (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
             LEFT JOIN AggregateTables_UserGroups ug2 ON ug.ParentUserGroupId = ug2.Id
    WHERE lv.[Date] BETWEEN @dateFrom AND @dateTo
    GROUP BY lv.[Date], u.fio, ug.FullName, ug2.FullName

END