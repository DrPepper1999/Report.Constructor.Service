CREATE OR ALTER PROCEDURE [dbo].[ArchiveDownloadsReport] @timestart DATETIME,
                                                         @timeend DATETIME,
                                                         @UserGroupIds dbo.ParamTableGuids READONLY
AS
BEGIN

    DECLARE @timestartLocal datetime = @timestart
    DECLARE @timeendLocal datetime = @timeend
    DECLARE @shallFilterUserGroups BIT;

    IF EXISTS (SELECT 1 FROM @UserGroupIds)
        SET @shallFilterUserGroups = 1;
    ELSE
        SET @shallFilterUserGroups = 0;

    SELECT arr.ArchiveNumber                    'OrderNumber',
           arr.Title                            'CameraName',
           cast(arr.BeginTime AS smallDATETIME) 'Start',
           arr.EndTime                          'End',
           arr.FileSize                         'FileSizeInGb',
           arr.fio                              'FullName',
           arr.[action]                         'Action',
           arr.DateDownload                     'DateDownload',
           arr.Reason                           'Reason'
    from (SELECT ao.ArchiveNumber,
                 c.Title,
                 ae.BeginTime,
                 ae.EndTime,
                 ROUND(cast(at.FileSize AS REAL) / 1024, 2) FileSize,
                 u.LastName + ' ' + u.FirstName +
                 ' ' + u.MiddleName                         fio,
                 'Скачивание архива'                        [action],
                 adh.DateDownload,
                 adh.Reason
          FROM (SELECT ArchiveOrderId, TimeStamp AS DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveActionsHistories
                WHERE ActionType = 1
                UNION
                SELECT ArchiveOrderId, DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveDownloadedHistory) AS adh
                   JOIN MoscowVideo_ArchiveOrders AS ao
                        ON ao.Id = adh.ArchiveOrderId
                   JOIN MoscowVideo_ArchiveEntries AS ae
                        ON ae.Id = ao.ArchiveEntryId
                   JOIN ArchiveBalancingDb_ArchiveTask at
                        ON at.Id = ae.TaskId
                   JOIN MoscowVideo_Users AS u ON u.Id = adh.UserId
                   JOIN MoscowVideo_Cameras AS c ON c.Id = ae.CameraId
          WHERE adh.DateDownload >= @timestartLocal
            AND adh.DateDownload <= @timeendLocal --AND adh.UserId = ao.UserId --AND ao.IsAuthor = 1
            AND (u.UsergroupId IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
          UNION
          SELECT ao.ArchiveNumber,
                 c.Title,
                 ae.BeginTime,
                 ae.EndTime,
                 ROUND(cast(at.FileSize AS REAL) / 1024, 2) FileSize,
                 u.LastName + ' ' + u.FirstName + ' '
                     + u.MiddleName                         fio,
                 'Получение ссылки'                         [action],
                 adh.DateDownload,
                 adh.Reason
          FROM (SELECT ArchiveOrderId, TimeStamp AS DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveActionsHistories
                WHERE ActionType = 2
                UNION
                SELECT ArchiveOrderId, DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveDownloadedHistory) AS adh
                   JOIN MoscowVideo_ArchiveOrders AS ao
                        ON ao.Id = adh.ArchiveOrderId
                   JOIN MoscowVideo_ArchiveEntries AS ae
                        ON ae.Id = ao.ArchiveEntryId
                   JOIN ArchiveBalancingDb_ArchiveTask at
                        ON at.Id = ae.TaskId
                   JOIN MoscowVideo_Users AS u ON u.Id = adh.UserId
                   JOIN MoscowVideo_Cameras AS c ON c.Id = ae.CameraId
          WHERE adh.DateDownload >= @timestartLocal
            AND adh.DateDownload <= @timeendLocal --AND adh.UserId = ao.UserId --AND ao.IsAuthor = 0
            AND (u.UsergroupId IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
          UNION
          SELECT ao.ArchiveNumber,
                 c.Title,
                 ae.BeginTime,
                 ae.EndTime,
                 ROUND(cast(at.FileSize AS REAL) / 1024, 2) FileSize,
                 u.LastName + ' ' + u.FirstName + ' '
                     + u.MiddleName                         fio,
                 'Продление'                                [action],
                 adh.DateDownload,
                 adh.Reason
          FROM (SELECT ArchiveOrderId, TimeStamp AS DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveActionsHistories
                WHERE ActionType = 3
                UNION
                SELECT ArchiveOrderId, DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveDownloadedHistory) AS adh
                   JOIN MoscowVideo_ArchiveOrders AS ao
                        ON ao.Id = adh.ArchiveOrderId
                   JOIN MoscowVideo_ArchiveEntries AS ae
                        ON ae.Id = ao.ArchiveEntryId
                   JOIN ArchiveBalancingDb_ArchiveTask at
                        ON at.Id = ae.TaskId
                   JOIN MoscowVideo_Users AS u ON u.Id = adh.UserId
                   JOIN MoscowVideo_Cameras AS c ON c.Id = ae.CameraId
          WHERE adh.DateDownload >= @timestartLocal
            AND adh.DateDownload <= @timeendLocal --AND adh.UserId != ao.UserId --AND ao.IsAuthor = 0
            AND (u.UsergroupId IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
          union
          SELECT ao.ArchiveNumber,
                 c.Title,
                 ae.BeginTime,
                 ae.EndTime,
                 ROUND(cast(at.FileSize AS REAL) / 1024, 2) FileSize,
                 u.LastName + ' ' + u.FirstName + ' '
                     + u.MiddleName                         fio,
                 'Просмотр'                                 [action],
                 adh.DateDownload,
                 adh.Reason
          FROM (SELECT ArchiveOrderId, TimeStamp AS DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveActionsHistories
                WHERE ActionType = 4
                UNION
                SELECT ArchiveOrderId, DateDownload, Reason, UserId
                FROM MoscowVideo_ArchiveDownloadedHistory) AS adh
                   JOIN MoscowVideo_ArchiveOrders AS ao
                        ON ao.Id = adh.ArchiveOrderId
                   JOIN MoscowVideo_ArchiveEntries AS ae
                        ON ae.Id = ao.ArchiveEntryId
                   JOIN ArchiveBalancingDb_ArchiveTask at
                        ON at.Id = ae.TaskId
                   JOIN MoscowVideo_Users AS u ON u.Id = adh.UserId
                   JOIN MoscowVideo_Cameras AS c ON c.Id = ae.CameraId
          WHERE adh.DateDownload >= @timestartLocal
            AND adh.DateDownload <= @timeendLocal --AND adh.UserId != ao.UserId --AND ao.IsAuthor = 1
            AND (u.UsergroupId IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
          union
          SELECT ao.ArchiveNumber,
                 c.Title,
                 ae.BeginTime,
                 ae.EndTime,
                 ROUND(cast(at.FileSize AS REAL) / 1024, 2),
                 u.LastName + ' ' + u.FirstName + ' '
                     + u.MiddleName,
                 'Заказ архива',
                 ae.DateCreated,
                 ao.Reason
          FROM MoscowVideo_ArchiveOrders AS ao
                   JOIN MoscowVideo_ArchiveEntries AS ae
                        ON ae.Id = ao.ArchiveEntryId
                   JOIN ArchiveBalancingDb_ArchiveTask at
                        ON at.Id = ae.TaskId
                   JOIN MoscowVideo_Users AS u ON u.Id = ao.UserId
                   JOIN MoscowVideo_Cameras AS c
                        ON c.Id = ae.CameraId
          WHERE ae.DateCreated >= @timestartLocal
            AND ae.DateCreated <= @timeendLocal
            AND (u.UsergroupId IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
            AND ao.IsAuthor = 1
          UNION
          SELECT ao.ArchiveNumber,
                 c.Title,
                 ae.BeginTime,
                 ae.EndTime,
                 ROUND(cast(at.FileSize AS REAL) / 1024, 2),
                 u.LastName + ' ' + u.FirstName + ' '
                     + u.MiddleName,
                 'Повторный заказ',
                 ae.DateCreated,
                 ao.Reason
          FROM MoscowVideo_ArchiveOrders AS ao
                   JOIN MoscowVideo_ArchiveEntries AS ae
                        ON ae.Id = ao.ArchiveEntryId
                   JOIN ArchiveBalancingDb_ArchiveTask at
                        ON at.Id = ae.TaskId
                   JOIN MoscowVideo_Users AS u ON u.Id = ao.UserId
                   JOIN MoscowVideo_Cameras AS c
                        ON c.Id = ae.CameraId
          WHERE ae.DateCreated >= @timestartLocal
            AND ae.DateCreated <= @timeendLocal
            AND (u.UsergroupId IN (SELECT Value FROM @UserGroupIds) OR @shallFilterUserGroups = 0)
            AND ao.IsAuthor = 0) arr
    ORDER BY arr.DateDownload
END