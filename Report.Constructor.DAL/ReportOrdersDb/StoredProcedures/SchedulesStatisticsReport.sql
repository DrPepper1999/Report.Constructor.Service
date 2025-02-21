CREATE OR ALTER   PROCEDURE [dbo].[SchedulesStatisticsReport] @BegDate DATETIME,
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
    declare @TMP_TimeLine2 TABLE
                           (
                               ScheduleId UNIQUEIDENTIFIER,
                               timestart  DATETIME,
                               timeend    DATETIME,
                               timePeriod TIME(7)
                           );
    declare @TMP_TimeLine TABLE
                          (
                              ScheduleId UNIQUEIDENTIFIER,
                              timestart  DATETIME
                          );
    declare @ScheduleId uniqueidentifier;
    declare @Timestart DATETIME;
    declare @Timeend DATETIME;
    DECLARE @Period TIME(7);
    DECLARE @kolvo TABLE
                   (
                       ScheduleId UNIQUEIDENTIFIER,
                       ti         TIME,
                       kol        INT
                   );

	DECLARE @ScheduleTimeLine TABLE (
	scheduleid uniqueidentifier,
	Timestart DATETIME,
	TimeEnd DATETIME,
	Period TIME(7)
	);

	INSERT INTO @ScheduleTimeLine
	SELECT stl.ScheduleId, stl.TimeStart, stl.Timeend, stl.period
	FROM MoscowVideo_ScheduleTimeLine stl

    INSERT INTO @TMP_TimeLine
    SELECT stl.ScheduleId, stl.TimeStart
    FROM MoscowVideo_ScheduleTimeLine AS stl
             JOIN MoscowVideo_ScreenshotJob AS sj ON sj.ScheduleId = stl.ScheduleId AND sj.DeathTime >= GETDATE() AND sj.Errors = 0
    WHERE stl.Period IS NULL;

	INSERT INTO @TMP_TimeLine2
    SELECT 
	stl.ScheduleId, cast(stl.TimeStart AS DATETime), 
	CASE 
		WHEN stl.TimeEnd IS NULL THEN '1900-01-01 23:59:59'	
		WHEN stl.TimeEnd IS not NULL THEN cast(stl.TimeEnd AS DATETime) 
	END, stl.Period
    FROM MoscowVideo_ScheduleTimeLine AS stl
             JOIN MoscowVideo_ScreenshotJob AS sj ON sj.ScheduleId = stl.ScheduleId AND sj.DeathTime >= GETDATE() AND sj.Errors = 0
    WHERE stl.TimeEnd IS NULL
    AND stl.Period IS NOT NULL;

	DECLARE SIMPLE_CURSOR CURSOR FOR
    SELECT stl.ScheduleId, stl.TimeStart, stl.TimeEnd, stl.TimePeriod
    FROM @TMP_TimeLine2 AS stl;

    OPEN SIMPLE_CURSOR;

    FETCH NEXT FROM SIMPLE_CURSOR
        INTO @ScheduleId, @Timestart,@Timeend, @Period;

    WHILE @@FETCH_STATUS = 0
        BEGIN
            WHILE @Timestart < @Timeend
                BEGIN

                    INSERT INTO @TMP_TimeLine VALUES (@ScheduleId, cast(@Timestart AS TIME(7)))
                    SET @Timestart = @TimeStart + cast(@Period AS DATETIME)
                END
            FETCH NEXT FROM SIMPLE_CURSOR INTO @ScheduleId, @Timestart,@Timeend, @Period;
        END

    CLOSE SIMPLE_CURSOR
    DEALLOCATE SIMPLE_CURSOR;

    INSERT INTO @kolvo
    SELECT kol.ScheduleId, stl.period, MAX(kol.num)
    FROM (SELECT ttl.ScheduleId,
                 ttl.timestart,
                 ROW_NUMBER() OVER (partition BY ttl.ScheduleId ORDER BY ttl.timestart) num
          FROM @TMP_TimeLine AS ttl) kol
          JOIN @ScheduleTimeLine AS stl ON stl.ScheduleId = kol.ScheduleId
    GROUP BY kol.ScheduleId, stl.period

    SELECT sj.[Description]                                          'Name',
           cast(cast(sj.CreationDate AS DATETIME2) AS SMALLDATETIME) 'CreationDate',
           cast(sj.DeathTime AS smallDATETIME)                       'EndDate',
           (CASE
                WHEN sj.IsActive = 1 THEN 'Active'
                ELSE 'Inactive' end)                                 'Activity',
           sj.fio                                                    'FullName',
           us.Ad                                                     'ADLogin',
           us.Sudir                                                  'SudirLogin',
           sj.Rod                                                    'ParentGroup',
           sj.FullName                                               'Group',
           (CASE
                WHEN sj.[Type] != 100 THEN sj.Name
                ELSE 'Filter' END)                                   'CameraGroup',
           isnull(sj.ImageKeepTime, 0)                               'ArchiveStorageDays',                     --Хранение архива, дней
           cast(isnull(sj.ti, '00:00:00.000') AS NVARCHAR(8))        'Periodicity',
           sj.Kol                                                    'NumberOfTimePeriodsPerDay',--Кол-во временных периодов в сутки
           sj.co                                                     'NumberOfCamerasInSchedule',
           sj.co * sj.kol                                            'NumberOfRequestedScreenshotsInLastDay',--Кол-во запрошенных скриншотов за последние сутки
           isnull(sjr.co, 0)                                         'SuccessfullyPreparedScreenshotsInLastDay'--Успешно подготовлено скриншотов за прошедшие сутки
    FROM (SELECT sj.[Description],
                 ug2.FullName                                rod,
                 cg.[Type],
                 sj.id,
                 [@kolvo].kol,
                 cg.Name,
                 [@kolvo].ti,
                 sj.CreationDate,
                 ISNULL(sj.DeathTime, '2015-12-31')          DeathTime,
                 sj.ImageKeepTime,
                 sj.IsActive,
                 u.Id                                        userid,
                 u.LastName + ' ' +
                 + ' ' + u.FirstName + ' ' + u.MiddleName AS fio,
                 ug.FullName,
                 COUNT(1)                                    co
          FROM MoscowVideo_ScreenshotJob AS sj
                   JOIN MoscowVideo_Schedule AS s
                        ON s.Id = sj.ScheduleId
                   JOIN @kolvo ON [@kolvo].ScheduleId = sj.ScheduleId
                   JOIN MoscowVideo_Users AS u ON u.Id = sj.UserId
                   JOIN MoscowVideo_UserGroups AS ug ON ug.Id = u.UserGroupId AND
                                                         (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
                   LEFT JOIN MoscowVideo_UserGroups AS ug2 ON ug2.Id = ug.ParentUserGroupId
                   JOIN MoscowVideo_CameraGroups AS cg ON cg.Id = sj.CameraGroupId
                   JOIN MoscowVideo_Cameras_CameraGroups AS cmcg on cmcg.CameraGroupId = cg.Id
          GROUP BY cg.[Type], ug2.FullName, cg.Name, [@kolvo].ti, [@kolvo].kol, sj.Id, sj.[Description],
                   sj.CreationDate, sj.DeathTime, sj.IsActive, u.Id,
                   u.LastName + ' ' +
                   + ' ' + u.FirstName + ' ' + u.MiddleName, ug.FullName, sj.ImageKeepTime) sj
             LEFT JOIN
         (SELECT sjr.ScreenshotJobId, COUNT(1) co
          FROM MoscowVideo_ScreenshotJobResults AS sjr
		  JOIN MoscowVideo_ScreenshotJob sj ON sj.Id = sjr.ScreenshotJobId
          WHERE sjr.JobTime >= DATEADD(DAY, -1, (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))))
            AND sjr.JobTime <= (SELECT DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE())))
			AND sj.DeathTime >= GETDATE()
          GROUP BY sjr.ScreenshotJobId) sjr ON sj.Id = sjr.ScreenshotJobId
             JOIN AggregateTables_Users us ON us.userid = sj.userid
END