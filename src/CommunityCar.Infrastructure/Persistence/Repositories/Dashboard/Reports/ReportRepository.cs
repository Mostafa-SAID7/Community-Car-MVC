using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports;
using CommunityCar.Domain.Entities.Dashboard.Reports;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Dashboard.Reports;

public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;

    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Report>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Schedules)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Schedules)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Report>> GetByTypeAsync(string reportType, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Schedules)
            .Where(r => r.Type.ToString().Equals(reportType, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Report>> GetByStatusAsync(string status, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Include(r => r.Schedules)
            .Where(r => r.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Report> CreateAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Add(report);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<Report> UpdateAsync(Report report, CancellationToken cancellationToken = default)
    {
        _context.Reports.Update(report);
        await _context.SaveChangesAsync(cancellationToken);
        return report;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var report = await GetByIdAsync(id, cancellationToken);
        if (report == null) return false;

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reports.CountAsync(cancellationToken);
    }

    public async Task<int> GetCountByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase))
            .CountAsync(cancellationToken);
    }

    public async Task<List<Report>> GetExpiredReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reports
            .Where(r => r.ExpiresAt.HasValue && r.ExpiresAt.Value < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }
}