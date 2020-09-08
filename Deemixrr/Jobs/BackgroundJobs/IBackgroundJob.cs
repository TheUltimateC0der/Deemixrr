using System.Threading.Tasks;

namespace Deemixrr.Jobs.BackgroundJobs
{
    public interface IBackgroundJob
    {
        Task Execute();
    }


    public interface IBackgroundJob<T>
    {
        Task Execute(T param, bool queueNext = false);
    }
}