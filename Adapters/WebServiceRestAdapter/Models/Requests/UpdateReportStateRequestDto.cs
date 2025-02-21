using WebServiceRestAdapter.MoscowVideoRestApi;

namespace WebServiceRestAdapter.Models.Requests
{
    public sealed class UpdateReportStateRequestDto
    {
        public required Guid ReportId { get; set; }
        public required FileState ReportState { get; set; }
        public string? ErrorMessage { get; set; }
    }
}