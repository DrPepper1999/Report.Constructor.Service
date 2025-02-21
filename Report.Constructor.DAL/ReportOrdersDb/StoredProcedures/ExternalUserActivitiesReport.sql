CREATE OR ALTER PROCEDURE [dbo].[ExternalUserActivitiesReport] @UserId UNIQUEIDENTIFIER,
                                                               @CameraIdsParam dbo.[ParamTableGuids] ReadOnly,
                                                               @CamsIsEmpty BIT = 0,
                                                               @ExtLogins dbo.ParamTableVars ReadOnly,
                                                               @ExLgIsEmpty BIT = 0,
                                                               @ActionType NVARCHAR(1000),
                                                               @DateFrom DATETIME,
                                                               @DateTo DATETIME,
                                                               @skip INT = 0,
                                                               @take INT = 20,
                                                               @sortColumn VARCHAR(MAX) = 'fio',
                                                               @sortDirection INT = 1
AS
BEGIN
    IF EXISTS(SELECT * FROM @CameraIdsParam)
        SET @CamsIsEmpty = 1;
    IF EXISTS(SELECT * FROM @ExtLogins)
        SET @ExLgIsEmpty = 1;

    SELECT opa.UserId,
           u.fio,
           CameraId,
           c.Title,
           COALESCE(AD, SUDIR)                 AS user_name,
           (CASE
                WHEN Action = 'start' THEN 'Начало просмотра'
                WHEN Action = 'abseek' THEN 'Поиск фрагмента (в календаре или ползунком)'
                WHEN Action = 'scale' THEN 'Шаг перемотки'
                WHEN Action = 'play' THEN 'Проигрывание видео'
                WHEN Action = 'shift' THEN 'Сдвиг стрелкой'
                WHEN Action = 'rplay' THEN 'Проигрывание видео назад'
                WHEN Action = 'pause' THEN 'Пауза'
               END)                            AS Action
            ,
           TimeStamp,
           (CASE
                WHEN ae.ApplicationId = 'D412D576-7F4A-4BED-91B5-717409EA8249' THEN 'Внутренний портал Netris'
                WHEN ae.ApplicationId = 'BEEBF8E0-8FCA-4AA6-8D96-225A664B8D03' THEN 'other'
                WHEN ae.ApplicationId = 'CECDFF74-9C3A-4A38-9EBD-340316300C23' THEN 'Внешний портал Netris'
                WHEN ae.ApplicationId = 'F4588627-C3FF-4114-BCFF-02FCE39545D7' THEN 'Технологический портал'
               END)                            AS Portal,
           actType.Value                       as ActionType,
           JSON_VALUE(ev.Value, '$.ext_login') AS ExtLogin
    FROM Audit_OperationPlayerAction opa
             JOIN Audit_AuditEntry ae ON ae.Id = opa.Id
             JOIN Audit_EntryValues ev ON opa.Id = ev.Entry_Id
             JOIN Audit_EntryValues actType ON opa.Id = actType.Entry_Id
             LEFT JOIN AggregateTables_Users u ON opa.UserId = u.UserId
             LEFT JOIN AggregateTables_Cameras c ON opa.CameraId = c.Id
    WHERE ISJSON(ev.Value) > 0
      AND ISJSON(actType.Value) = 0
      AND ((@UserId IS NULL) OR (opa.UserId = @UserId))
      AND ((@ActionType IS NULL) OR (actType.Value = @ActionType))
      AND (@CamsIsEmpty = 0 OR opa.CameraId IN (SELECT Value FROM @CameraIdsParam))
      AND (@ExLgIsEmpty = 0 OR (JSON_VALUE(ev.Value, '$.ext_login') IN (SELECT Value COLLATE Cyrillic_General_CI_AI FROM @ExtLogins)))
      AND ((@DateFrom IS NULL) OR (@DateFrom < ae.TimeStamp))
      AND ((@DateTo IS NULL) OR (@DateTo > ae.TimeStamp))
      AND JSON_VALUE(ev.Value, '$.ext_login') IS NOT NULL
    ORDER BY CASE WHEN @sortDirection = 1 AND @sortColumn = 'UserId' THEN opa.UserId END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'fio' THEN fio END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'CameraId' THEN CameraId END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'Title' THEN Title END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'user_name' THEN COALESCE(AD, SUDIR) END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'Action' THEN Action END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'TimeStamp' THEN TimeStamp END DESC,
             CASE WHEN @sortDirection = 1 AND @sortColumn = 'ActionType' THEN actType.Value END DESC,
             CASE
                 WHEN @sortDirection = 1 AND @sortColumn = 'ExtLogin' THEN JSON_VALUE(ev.Value, '$.ext_login') END DESC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'UserId' THEN opa.UserId END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'fio' THEN fio END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'CameraId' THEN CameraId END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'Title' THEN Title END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'user_name' THEN COALESCE(AD, SUDIR) END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'Action' THEN Action END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'TimeStamp' THEN TimeStamp END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'ActionType' THEN actType.Value END ASC,
             CASE WHEN @sortDirection = 0 AND @sortColumn = 'ExtLogin' THEN JSON_VALUE(ev.Value, '$.ext_login') END ASC
    OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY
END