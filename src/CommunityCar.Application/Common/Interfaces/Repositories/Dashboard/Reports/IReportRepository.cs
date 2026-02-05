using CommunityCar.Domain.Entities.Dashboard.Reports;

namespace CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports;

public interface IReportRepository
{
    Task<List<Report>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Report>> GetByTypeAsync(string reportType, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<List<Report>> GetByStatusAsync(string status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<Report> CreateAsync(Report report, CancellationToken cancellationToken = default);
    Task<Report> UpdateAsync(Report report, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<List<Report>> GetExpiredReportsAsync(CancellationToken cancellationToken = default);
}