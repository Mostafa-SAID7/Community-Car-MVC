using CommunityCar.Application.Common.Interfaces.Repositories.Dashboard.Reports;
using CommunityCar.Domain.Entities.Dashboard.Reports;
using CommunityCar.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace CommunityCar.Infrastructure.Persistence.Repositories.Dashboard.Reports;

public class ReportScheduleRepository : IReportScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public ReportScheduleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReportSchedule>> GetAllAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(rs => rs.Report)
            .OrderByDescending(rs => rs.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReportSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(rs => rs.Report)
            .FirstOrDefaultAsync(rs => rs.Id == id, cancellationToken);
    }

    public async Task<List<ReportSchedule>> GetActiveSchedulesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(rs => rs.Report)
            .Where(rs => rs.IsActive)
            .OrderBy(rs => rs.NextRun)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<ReportSchedule>> GetDueSchedulesAsync(DateTime currentTime, CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules
            .Include(rs => rs.Report)
            .Where(rs => rs.IsActive && rs.NextRun <= currentTime)
            .OrderBy(rs => rs.NextRun)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReportSchedule> CreateAsync(ReportSchedule schedule, CancellationToken cancellationToken = default)
    {
        _context.ReportSchedules.Add(schedule);
        await _context.SaveChangesAsync(cancellationToken);
        return schedule;
    }

    public async Task<ReportSchedule> UpdateAsync(ReportSchedule schedule, CancellationToken cancellationToken = default)
    {
        _context.ReportSchedules.Update(schedule);
        await _context.SaveChangesAsync(cancellationToken);
        return schedule;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await GetByIdAsync(id, cancellationToken);
        if (schedule == null) return false;

        _context.ReportSchedules.Remove(schedule);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReportSchedules.CountAsync(cancellationToken);
    }
}