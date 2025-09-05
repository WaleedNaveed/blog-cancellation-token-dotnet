using CancellationTokenDemo.Models;

namespace CancellationTokenDemo.Interfaces
{
    public interface IJobQueue
    {
        ValueTask EnqueueAsync(JobItem job, CancellationToken ct = default);
        IAsyncEnumerable<JobItem> ReadAllAsync(CancellationToken ct);
    }
}
