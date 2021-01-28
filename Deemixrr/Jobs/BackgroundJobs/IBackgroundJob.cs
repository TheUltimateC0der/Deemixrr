using System.Threading.Tasks;

using Hangfire.Server;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public interface IBackgroundJob
    {
        Task Execute(PerformContext context);
    }


    public interface IBackgroundJob<T>
    {
        Task Execute(T param, PerformContext context);
    }
}