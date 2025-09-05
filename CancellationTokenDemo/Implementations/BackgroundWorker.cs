using CancellationTokenDemo.Interfaces;

namespace CancellationTokenDemo.Implementations
{
    public sealed class BackgroundWorker : BackgroundService
    {
        private readonly IJobQueue _queue;

        public BackgroundWorker(IJobQueue queue)
        {
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Background worker started.");

            try
            {
                await foreach (var job in _queue.ReadAllAsync(stoppingToken))
                {
                    Console.WriteLine($"Processing {job.FileName}...");

                    try
                    {
                        // Simulate work
                        for (int i = 0; i < 5; i++)
                        {
                            stoppingToken.ThrowIfCancellationRequested();
                            await Task.Delay(500, stoppingToken);
                        }

                        job.Status = "Completed";
                        Console.WriteLine($"Job {job.FileName} done.");
                    }
                    catch (OperationCanceledException)
                    {
                        job.Status = "Canceled";
                        Console.WriteLine($"Job {job.FileName} canceled.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Worker stopping (host shutdown).");
            }
        }
    }
}
