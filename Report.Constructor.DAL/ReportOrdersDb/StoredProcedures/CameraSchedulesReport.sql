CREATE OR ALTER PROCEDURE [dbo].[CameraSchedulesReport] @timestart DATETIME,
                                                        @timeend DATETIME,
                                                        @UserGroupIds dbo.ParamTableGuids READONLY
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
ELSE
        SET @shallFilterUserGroups = 0;

SELECT cast(position.CreateDate as SMALLDATETIME)                  'CreationDate',
    position.servtype                                           'ServiceType',
    position.Title                                              'Camera',
    position.ShortName                                          'District',
    position.DisplayName                                        'Address',
    position.fio                                                'FullName',
    u.Ad                                                        'ADLogin',
    u.Sudir                                                     'SudirLogin',
    position.rod                                                'ParentGroup',
    position.FullName                                           'Group',
    position.rej                                                'Mode',
    position.Name                                               'Schedule',
    position.tip                                                'Type',
    position.pos                                                'PresetName',
    position.Position                                           'VNObject',
    cast(position.TimeStart AS TIME(0))                         'TransitionTime',
    cast(position.TimeEnd AS TIME(0))                           'ReturnToGDPTime',
    DATEDIFF(second, position.TimeStart, position.TimeEnd) / 60 'DurationInMinutes',
    cast(position.DeathTime as SMALLDATETIME)                   'EndDate',
    (CASE
         WHEN position.BlockControl = 1 THEN 'Да'
         WHEN position.BlockControl = 0 THEN 'Нет'
        END)                                                    'ControlLock'
from (SELECT pos.CreateDate,
             pos.DeathTime,
             ug2.FullName                                        rod,
             cso.Name                                            Position,
                 cp.Title                                            pos,
                 d.ShortName,
                 pos.BlockControl,
                 st.Name                                             servtype,
                 (sn.Name + ' ' + a.AddressName)                     DisplayName,
                 c.Title,
                 u.Id                                                Userid,
                 u.LastName + ' ' + u.FirstName + ' ' + u.MiddleName fio,
                 ug.FullName,
                 pos.Name,
                 'DefaultShedule'                                    rej,
                 'PatrolSchedule'                                    tip,
                 stl.TimeStart,
                 stl.TimeEnd
      FROM (SELECT cst.CreateDate, cst.DeathTime, cst.BlockControl, cst.Id, cst.UserId, cst.Name, COUNT(1) co
          FROM MoscowVideo_CameraScheduleTasks AS cst
          JOIN MoscowVideo_CameraScheduleTaskPositions AS cstp
          ON cstp.CameraScheduleTaskId = cst.Id
          WHERE cst.[Type] = 1
          AND cst.[State] != 2
          AND cst.[CreateDate] BETWEEN @timestart AND @timeend
          GROUP BY cst.CreateDate, cst.DeathTime, cst.BlockControl, cst.Id, cst.UserId, cst.Name) pos
          JOIN MoscowVideo_CameraScheduleTaskPositions AS cstp
      ON cstp.CameraScheduleTaskId = pos.Id
          JOIN MoscowVideo_CameraPositions_PersonalPositions AS cppp
          ON cppp.Id = cstp.PersonalPositionId
          JOIN MoscowVideo_CameraPositions AS cp
          ON cp.Id = cppp.Id
          left JOIN MoscowVideo_CameraPositions_CameraSurveillanceObjects AS cpcso
          ON cpcso.PositionId = cp.Id
          left JOIN MoscowVideo_CameraSurveillanceObjects AS cso
          ON cso.Id = cpcso.SurveillanceObjectId
          JOIN MoscowVideo_Cameras AS c
          ON c.Id = cp.CameraId
          JOIN MoscowVideo_Users AS u
          ON u.Id = pos.UserId
          JOIN MoscowVideo_Schedule AS s
          ON s.Id = cstp.ScheduleId
          JOIN MoscowVideo_ScheduleTimeLine AS stl
          ON stl.ScheduleId = s.Id
          JOIN MoscowVideo_UserGroups AS ug
          ON ug.Id = u.UserGroupId
          JOIN MoscowVideo_UserGroups AS ug2
          ON ug2.Id = ug.ParentUserGroupId
          JOIN MoscowVideo_Addresses AS a
          ON a.Id = c.AddressId
          JOIN MoscowVideo_Districts AS d
          ON d.Id = a.DistrictId
          JOIN MoscowVideo_StreetNames AS sn
          ON sn.Id = a.StreetId
          JOIN MoscowVideo_ServiceTypes AS st
          ON st.Id = c.ServiceTypeId
      WHERE pos.co > 1 AND (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
      UNION all
      SELECT pos.CreateDate,
          pos.DeathTime,
          ug2.FullName     rod,
          cso.Name         Position,
          cp.Title         pos,
          d.ShortName,
          pos.BlockControl,
          st.Name,
          sn.Name + ' ' + a.AddressName,
          c.Title,
          u.Id,
          u.LastName + ' ' + u.FirstName + ' ' + u.MiddleName,
          ug.FullName,
          pos.Name,
          'DefaultShedule' rej,
          'SchedulewithOnePosition',
          stl.TimeStart,
          stl.TimeEnd
      FROM (SELECT cst.CreateDate, cst.DeathTime, cst.BlockControl, cst.Id, cst.UserId, cst.Name, COUNT(1) co
          FROM MoscowVideo_CameraScheduleTasks AS cst
          JOIN MoscowVideo_CameraScheduleTaskPositions AS cstp
          ON cstp.CameraScheduleTaskId = cst.Id
          WHERE cst.[Type] = 1
          AND cst.[State] != 2
          AND cst.[CreateDate] BETWEEN @timestart AND @timeend
          GROUP BY cst.CreateDate, cst.DeathTime, cst.BlockControl, cst.Id, cst.UserId, cst.Name) pos
          JOIN MoscowVideo_CameraScheduleTaskPositions AS cstp
      ON cstp.CameraScheduleTaskId = pos.Id
          JOIN MoscowVideo_CameraPositions_PersonalPositions AS cppp
          ON cppp.Id = cstp.PersonalPositionId
          JOIN MoscowVideo_CameraPositions AS cp
          ON cp.Id = cppp.Id
          left JOIN MoscowVideo_CameraPositions_CameraSurveillanceObjects AS cpcso
          ON cpcso.PositionId = cp.Id
          left JOIN MoscowVideo_CameraSurveillanceObjects AS cso
          ON cso.Id = cpcso.SurveillanceObjectId
          JOIN MoscowVideo_Cameras AS c
          ON c.Id = cp.CameraId
          JOIN MoscowVideo_Users AS u
          ON u.Id = pos.UserId
          JOIN MoscowVideo_Schedule AS s
          ON s.Id = cstp.ScheduleId
          JOIN MoscowVideo_ScheduleTimeLine AS stl
          ON stl.ScheduleId = s.Id
          JOIN MoscowVideo_UserGroups AS ug
          ON ug.Id = u.UserGroupId
          JOIN MoscowVideo_UserGroups AS ug2
          ON ug2.Id = ug.ParentUserGroupId
          JOIN MoscowVideo_Addresses AS a
          ON a.Id = c.AddressId
          JOIN MoscowVideo_Districts AS d
          ON d.Id = a.DistrictId
          JOIN MoscowVideo_StreetNames AS sn
          ON sn.Id = a.StreetId
          JOIN MoscowVideo_ServiceTypes AS st
          ON st.Id = c.ServiceTypeId
      WHERE pos.co = 1 AND (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
      UNION ALL
      SELECT pos.CreateDate,
          pos.DeathTime,
          ug2.FullName   rod,
          cso.Name       Position,
          cp.Title       pos,
          d.ShortName,
          pos.BlockControl,
          st.Name,
          sn.Name + ' ' + a.AddressName,
          c.Title,
          u.Id,
          u.LastName + ' ' + u.FirstName + ' ' + u.MiddleName,
          ug.FullName,
          pos.Name,
          'UserSchedule' rej,
          'SchedulewithOnePosition',
          stl.TimeStart,
          stl.TimeEnd
      FROM (SELECT cst.CreateDate, cst.DeathTime, cst.BlockControl, cst.Id, cst.UserId, cst.Name, COUNT(1) co
          FROM MoscowVideo_CameraScheduleTasks AS cst
          JOIN MoscowVideo_CameraScheduleTaskPositions AS cstp
          ON cstp.CameraScheduleTaskId = cst.Id
          WHERE cst.[Type] = 2
          AND cst.[State] != 2
          AND cst.[CreateDate] BETWEEN @timestart AND @timeend
          GROUP BY cst.CreateDate, cst.DeathTime, cst.BlockControl, cst.Id, cst.UserId, cst.Name) pos
          JOIN MoscowVideo_CameraScheduleTaskPositions AS cstp
      ON cstp.CameraScheduleTaskId = pos.Id
          JOIN MoscowVideo_CameraPositions_PersonalPositions AS cppp
          ON cppp.Id = cstp.PersonalPositionId
          JOIN MoscowVideo_CameraPositions AS cp
          ON cp.Id = cppp.Id
          left JOIN MoscowVideo_CameraPositions_CameraSurveillanceObjects AS cpcso
          ON cpcso.PositionId = cp.Id
          left JOIN MoscowVideo_CameraSurveillanceObjects AS cso
          ON cso.Id = cpcso.SurveillanceObjectId
          JOIN MoscowVideo_Cameras AS c
          ON c.Id = cp.CameraId
          JOIN MoscowVideo_Users AS u
          ON u.Id = pos.UserId
          JOIN MoscowVideo_Schedule AS s
          ON s.Id = cstp.ScheduleId
          JOIN MoscowVideo_ScheduleTimeLine AS stl
          ON stl.ScheduleId = s.Id
          JOIN MoscowVideo_UserGroups AS ug
          ON ug.Id = u.UserGroupId
          JOIN MoscowVideo_UserGroups AS ug2
          ON ug2.Id = ug.ParentUserGroupId
          JOIN MoscowVideo_Addresses AS a
          ON a.Id = c.AddressId
          JOIN MoscowVideo_Districts AS d
          ON d.Id = a.DistrictId
          JOIN MoscowVideo_StreetNames AS sn
          ON sn.Id = a.StreetId
          JOIN MoscowVideo_ServiceTypes AS st
          ON st.Id = c.ServiceTypeId
      WHERE ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0
      UNION ALL
      SELECT sj.CreationDate,
          sj.DeathTime,
          ug2.FullName rod,
          cso.Name     Position,
          cp.Title     pos,
          d.ShortName,
          1,
          st.Name,
          sn.Name + ' ' + a.AddressName,
          c.Title,
          u.id,
          u.LastName + ' ' + u.FirstName + ' ' + u.MiddleName,
          ug.FullName,
          sj.[Description],
          'Скриншоты',
          'Задание с одной позицией',
          stl.TimeStart,
          DATEADD(minute, 1, stl.TimeStart)
      FROM MoscowVideo_ScreenshotJob AS sj
          JOIN MoscowVideo_ScreenshotJobCameraPositions AS sjccp
      ON sjccp.ScreenshotJobId = sj.Id
          JOIN MoscowVideo_CameraPositions AS cp
          ON cp.Id = sjccp.PositionId
          left JOIN MoscowVideo_CameraPositions_CameraSurveillanceObjects AS cpcso
          ON cpcso.PositionId = cp.Id
          left JOIN MoscowVideo_CameraSurveillanceObjects AS cso
          ON cso.Id = cpcso.SurveillanceObjectId
          JOIN MoscowVideo_Schedule AS s
          ON s.Id = sj.ScheduleId
          JOIN MoscowVideo_ScheduleTimeLine AS stl
          ON stl.ScheduleId = s.Id
          JOIN MoscowVideo_Cameras AS c
          ON c.Id = cp.CameraId
          JOIN MoscowVideo_Addresses AS a
          ON a.Id = c.AddressId
          JOIN MoscowVideo_Users AS u
          ON u.Id = sj.UserId
          JOIN MoscowVideo_UserGroups AS ug
          ON ug.Id = u.UserGroupId
          JOIN MoscowVideo_UserGroups AS ug2
          ON ug2.Id = ug.ParentUserGroupId
          JOIN MoscowVideo_Districts AS d
          ON d.Id = a.DistrictId
          JOIN MoscowVideo_StreetNames AS sn
          ON sn.Id = a.StreetId
          JOIN MoscowVideo_ServiceTypes AS st
          ON st.Id = c.ServiceTypeId
      WHERE sj.CreationDate BETWEEN @timestart AND @timeend AND sj.IsActive = 1 AND (ug.Id IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
        AND cp.IsActive = 1) position
             JOIN AggregateTables_Users u ON position.Userid = u.userid
ORDER BY position.Title

END