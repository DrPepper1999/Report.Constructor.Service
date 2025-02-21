using AuditReportEnums;
using AuditReportFilters;
using AuditReportServiceGenerated;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MapsterMapper;
using Microsoft.Extensions.Options;
using Report.Constructor.Core.Options;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.DAL.ReportOrdersDb.Models.Enums;
using Report.Constructor.DAL.ReportOrdersDb.Models.ReportItems;

namespace AuditAdapter;

internal sealed class AuditRepository : IAuditRepository
{
    private readonly ReportsOptions _options;
    private readonly AuditReportService.AuditReportServiceClient _auditReportServiceClient;
    private readonly IMapper _mapper;

    public AuditRepository(
        IOptions<ReportsOptions> options, 
        AuditReportService.AuditReportServiceClient auditReportServiceClient,
        IMapper mapper)
    {
        _options = options.Value;
        _auditReportServiceClient = auditReportServiceClient;
        _mapper = mapper;
    }
    
    public Task<List<UserControlReportItem>> GetUserControls(DateTime startDateTime, DateTime endDateTime)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.UserControl,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            {
                TimeStart = startDateTime.ToUniversalTime().ToTimestamp(),
                TimeEnd = endDateTime.ToUniversalTime().ToTimestamp()
            }
        };

        return GetAllReportItems<UserControlReportItem>(reportDataRequest);
    }
    
    public Task<List<UserGroupControlReportItem>> GetUserGroupControls(DateTime startDateTime, DateTime endDateTime)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.UserGroupControl,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            {
                TimeStart = startDateTime.ToUniversalTime().ToTimestamp(),
                TimeEnd = endDateTime.ToUniversalTime().ToTimestamp()
            }
        };
        
        return GetAllReportItems<UserGroupControlReportItem>(reportDataRequest);
    }
    
    public Task<List<UserEditsReportItem>> GetUserEdits(DateTime startDateTime, DateTime endDateTime)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.UserEdits,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            {
                TimeStart = startDateTime.ToUniversalTime().ToTimestamp(),
                TimeEnd = endDateTime.ToUniversalTime().ToTimestamp()
            }
        };

        return GetAllReportItems<UserEditsReportItem>(reportDataRequest);
    }
    
    public Task<List<UserGroupViewsReportItem>> GetUserGroupViews(DateTime startDateTime, DateTime endDateTime)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.OperationGetLiveUrl,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            {
                TimeStart = startDateTime.ToUniversalTime().ToTimestamp(),
                TimeEnd = endDateTime.ToUniversalTime().ToTimestamp()
            }
        };

        return GetAllReportItems<UserGroupViewsReportItem>(reportDataRequest);
    }

    public Task<List<UserViewsReportItem>> GetUserViews(DateTime startDateTime, DateTime endDateTime)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.OperationGetLiveUrl,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            {
                TimeStart = startDateTime.ToUniversalTime().ToTimestamp(),
                TimeEnd = endDateTime.ToUniversalTime().ToTimestamp()
            }
        };

        return GetAllReportItems<UserViewsReportItem>(reportDataRequest);
    }

    public Task<List<OperationPlayerActionReportItem>> GetOperationPlayerActionItems(
         Guid? userId, string? accessType, DateTime start, DateTime end)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.OperationPlayerAction,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            OperationPlayerActionFilter = new OperationPlayerActionFilter
            {
                UserId = userId.ToString(),
                ActivityType = accessType,
                TimeIntervalFilter = new TimeIntervalFilter
                {
                    TimeStart = start.ToUniversalTime().ToTimestamp(),
                    TimeEnd = end.ToUniversalTime().ToTimestamp()
                }
            }
        };
        
        return GetAllReportItems<OperationPlayerActionReportItem>(reportDataRequest);
    }

    public Task<List<OperationSharedLinkActionReportItem>> GetOperationSharedLinkActionData(
        DateTime dateFrom, DateTime dateTo)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.OperationSharedLinkAction,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            {
                TimeStart = dateFrom.ToUniversalTime().ToTimestamp(),
                TimeEnd = dateTo.ToUniversalTime().ToTimestamp()
            }
        };

        return GetAllReportItems<OperationSharedLinkActionReportItem>(reportDataRequest);
    }

    public Task<List<OperationOrderArchiveReportItem>> GetOperationOrderArchiveData(DateTime dateFrom, DateTime dateTo)
    {
        var auditReportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.OperationOrderArchive,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            TimeIntervalFilter = new TimeIntervalFilter
            { 
                TimeStart = dateFrom.ToUniversalTime().ToTimestamp(),
                TimeEnd = dateTo.ToUniversalTime().ToTimestamp()
            }
        };

        return GetAllReportItems<OperationOrderArchiveReportItem>(auditReportDataRequest);
    }

    public Task<List<OperationScreenshotJobCrudReportItem>> GetOperationScreenshotJobCrudData(DateTime dateFrom, DateTime dateTo, CrudOperationType operationType)
    {
        var reportDataRequest = new GetReportDataRequest
        {
            ReportType = ReportType.OperationScreenshotJobCrud,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            OperationScreenshotJobCrudFilter = new OperationScreenshotJobCrudFilter
            {
                OperationType = (int)operationType,
                TimeIntervalFilter = new TimeIntervalFilter
                {
                    TimeStart = dateFrom.ToUniversalTime().ToTimestamp(),
                    TimeEnd = dateTo.ToUniversalTime().ToTimestamp()
                }
            }
        };

        return GetAllReportItems<OperationScreenshotJobCrudReportItem>(reportDataRequest);
    }

    public Task<List<LiveViewReportItem>> GetLiveViewData(DateTime dateFrom, DateTime dateTo, IEnumerable<Guid> tokenIds)
    {
        var filters = tokenIds
            .Chunk(5000)
            .Select(tokenIdChunk => new GetReportDataRequest
            {
                ReportType = ReportType.LiveView,
                Page = 1,
                PageSize = _options.AuditReportPageSize,
                TokenIdsFilter = new TokenIdsFilter
                {
                    TokenIds = { tokenIdChunk.Select(id => id.ToString()) },
                    TimeIntervalFilter = new TimeIntervalFilter
                    {
                        TimeStart = dateFrom.ToUniversalTime().ToTimestamp(),
                        TimeEnd = dateTo.ToUniversalTime().ToTimestamp()
                    }
                }
            });

        return GetReportItemsByRequests<LiveViewReportItem>(filters);
    }

    public Task<List<ArchiveViewReportItem>> GetArchiveViewData(
        DateTime dateFrom,
        DateTime dateTo,
        IEnumerable<Guid> tokenIds, 
        IEnumerable<Report.Constructor.Core.Enums.ArchiveViewActionType> actionTypes)
    {
        var filters = tokenIds
            .Chunk(5000)
            .Select(tokenIdChunk => new GetReportDataRequest
            {
                ReportType = ReportType.ArchiveView,
                Page = 1,
                PageSize = _options.AuditReportPageSize,
                ArchiveViewFilter = new ArchiveViewFilter
                {
                    Actions = { actionTypes.Cast<ArchiveViewActionType>() },
                    TokenIdsFilter = new TokenIdsFilter
                    {
                        TokenIds = { tokenIdChunk.Select(id => id.ToString()) },
                        TimeIntervalFilter = new TimeIntervalFilter
                        {
                            TimeStart = dateFrom.ToUniversalTime().ToTimestamp(),
                            TimeEnd = dateTo.ToUniversalTime().ToTimestamp()
                        }
                    }
                }
            });

        return GetReportItemsByRequests<ArchiveViewReportItem>(filters);
    }
    
    public Task<List<ArchiveViewReportItem>> GetArchiveView(Guid tokenId, DateTimeOffset start, DateTimeOffset end)
    {
        var filter = new GetReportDataRequest
        {
            ReportType = ReportType.ArchiveView,
            Page = 1,
            PageSize = _options.AuditReportPageSize,
            ArchiveViewFilter = new ArchiveViewFilter
            {
                TokenIdsFilter = new TokenIdsFilter
                {
                    TokenIds = { new [] { tokenId.ToString() } },
                    TimeIntervalFilter = new TimeIntervalFilter
                    {
                        TimeStart = start.ToUniversalTime().ToTimestamp(),
                        TimeEnd = end.ToUniversalTime().ToTimestamp()
                    }
                }
            }
        };

        return GetReportItemsByRequests<ArchiveViewReportItem>(new []{ filter });
    }

    private Task<List<TItem>> GetAllReportItems<TItem>(GetReportDataRequest request)
    {
        var pageRequests = Enumerable
            .Range(1, (int)request.Page)
            .Select(i =>
            {
                var clone = request.Clone();
                clone.Page = i;
                return clone;
            });
        
        return GetReportItemsByRequests<TItem>(pageRequests);
    }
    
    private async Task<List<TItem>> GetReportItemsByRequests<TItem>(IEnumerable<GetReportDataRequest> requests)
    {
        var items = new List<TItem>();

        var callOptions = new CallOptions(deadline: DateTime.UtcNow.Add(_options.AuditDataRetrievingTimeout));
        
        foreach (var request in requests)
        {
            var reportData = await _auditReportServiceClient.GetReportDataAsync(request, callOptions);
            
            if (reportData.DataCase != GetReportDataResponse.DataOneofCase.None)
            {
                items.AddRange(_mapper.Map<IEnumerable<TItem>>(reportData));
            }
        }
        
        return items;
    }
}