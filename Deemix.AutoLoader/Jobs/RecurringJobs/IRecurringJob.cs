using System.Threading.Tasks;

namespace Deemix.AutoLoader.Jobs.RecurringJobs
{
    public interface IRecurringJob
    {
        Task Execute();
    }


    public interface IRecurringJob<T>
    {
        Task Execute(T param);
    }
}