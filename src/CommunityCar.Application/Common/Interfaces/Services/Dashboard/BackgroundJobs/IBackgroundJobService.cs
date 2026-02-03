using System;
using System.Threading.Tasks;

namespace CommunityCar.Application.Common.Interfaces.Services.Dashboard.BackgroundJobs;

public interface IBackgroundJobService
{
    Task<string> EnqueueAsync<T>(string methodName, T parameters) where T : class;
    Task<string> ScheduleAsync<T>(string methodName, T parameters, TimeSpan delay) where T : class;
    Task<string> ScheduleAsync<T>(string methodName, T parameters, DateTimeOffset scheduleAt) where T : class;
    Task<string> RecurringAsync<T>(string jobId, string methodName, T parameters, string cronExpression) where T : class;
    Task<bool> DeleteAsync(string jobId);
    Task<bool> CancelAsync(string jobId);
}


