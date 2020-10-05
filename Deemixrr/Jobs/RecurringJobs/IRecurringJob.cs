using System.Threading.Tasks;

using Hangfire.Server;

namespace Deemixrr.Jobs.RecurringJobs
{
    public interface IRecurringJob
    {
        Task Execute(PerformContext context);
    }


    public interface IRecurringJob<T>
    {
        Task Execute(T param, PerformContext context);
    }
}