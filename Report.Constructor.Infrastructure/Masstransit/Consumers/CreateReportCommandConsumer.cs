using MassTransit;
using Microsoft.Extensions.Logging;
using Report.Constructor.Application.Models.Commands;
using Report.Constructor.DAL.Interfaces.Repositories;
using Report.Constructor.Infrastructure.Interfaces.ReportConstruction;

namespace Report.Constructor.Infrastructure.Masstransit.Consumers;

internal sealed class CreateReportCommandConsumer : IConsumer<CreateReportCommand>
{
    private readonly IReportOrderRepository _reportOrderRepository;
    private readonly IReportConstructor _reportConstructor;
    private readonly ILogger<CreateReportCommandConsumer> _logger;

    public CreateReportCommandConsumer(IReportOrderRepository reportOrderRepository,
        ILogger<CreateReportCommandConsumer> logger, IReportConstructor reportConstructor)
    {
        _reportOrderRepository = reportOrderRepository;
        _logger = logger;
        _reportConstructor = reportConstructor;
    }

    public async Task Consume(ConsumeContext<CreateReportCommand> context)
    {
        var createReportCommand = context.Message;
        var orderId = createReportCommand.OrderId;

        try
        {
            await _reportOrderRepository.SetProcessingStatusToReportOrder(orderId);
            
            var reportData = await _reportConstructor.ConstructReport(
                createReportCommand.ReportType, createReportCommand.ReportFilter);
            
            await _reportOrderRepository.CompleteReportOrder(orderId, reportData);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create report for order {OrderId}", orderId);
            await _reportOrderRepository.SetFailedStatusToReportOrder(orderId, "Внутренняя ошибка сервера.");
        }
    }
}