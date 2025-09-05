using CancellationTokenDemo.Interfaces;
using CancellationTokenDemo.Models;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace CancellationTokenDemo.Implementations
{
    public sealed class ChannelJobQueue : IJobQueue
    {
        private readonly Channel<JobItem> _channel = Channel.CreateUnbounded<JobItem>();

        public ValueTask EnqueueAsync(JobItem job, CancellationToken ct = default)
            => _channel.Writer.WriteAsync(job, ct);

        public async IAsyncEnumerable<JobItem> ReadAllAsync([EnumeratorCancellation] CancellationToken ct)
        {
            while (await _channel.Reader.WaitToReadAsync(ct))
                while (_channel.Reader.TryRead(out var job))
                    yield return job;
        }
    }
}
