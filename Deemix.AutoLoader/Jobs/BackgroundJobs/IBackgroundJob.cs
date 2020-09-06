using System.Threading.Tasks;

namespace Deemix.AutoLoader.Jobs.BackgroundJobs
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