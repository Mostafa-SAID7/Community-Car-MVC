using CommunityCar.Domain.Entities.Dashboard.Reports;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports;

public interface IReportScheduleRepository
{
    Task<List<ReportSchedule>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<ReportSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ReportSchedule>> GetActiveSchedulesAsync(CancellationToken cancellationToken = default);
    Task<List<ReportSchedule>> GetDueSchedulesAsync(DateTime currentTime, CancellationToken cancellationToken = default);
    Task<ReportSchedule> CreateAsync(ReportSchedule schedule, CancellationToken cancellationToken = default);
    Task<ReportSchedule> UpdateAsync(ReportSchedule schedule, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}