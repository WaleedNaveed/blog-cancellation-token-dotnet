namespace CancellationTokenDemo.Implementations
{
    public sealed class ParallelTasksDemo
    {
        public async Task<object> ScrapeManyAsync(int n, int delayMs, CancellationToken requestAborted)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(requestAborted);
            var token = cts.Token;

            var tasks = Enumerable.Range(1, n).Select(i => ScrapeOneAsync(i, delayMs, token)).ToArray();

            try
            {
                // If any task throws, we cancel all and rethrow to shape the response.
                var all = Task.WhenAll(tasks);
                await all;
                return new
                {
                    Completed = true,
                    Results = tasks.Select(t => t.Result).ToArray()
                };
            }
            catch (Exception ex)
            {
                cts.Cancel();

                var results = tasks
                    .Where(t => t.IsCompletedSuccessfully)
                    .Select(t => t.Result)
                    .ToArray();

                return new
                {
                    Completed = false,
                    Canceled = true,
                    SucceededCount = results.Length,
                    Message = $"One or more tasks canceled/failed → others canceled ({ex.GetType().Name})"
                };
            }
        }

        private static async Task<object> ScrapeOneAsync(int id, int delayMs, CancellationToken ct)
        {
            // Randomly fail one to demonstrate cascade cancel
            var rnd = Random.Shared.Next(1, 10);
            var steps = Random.Shared.Next(5, 9);

            for (int i = 0; i < steps; i++)
            {
                ct.ThrowIfCancellationRequested();
                await Task.Delay(delayMs, ct);
                if (rnd == 7 && i == 2) // sometimes fail
                    throw new InvalidOperationException($"Scraper {id} hit anti-bot.");
            }

            return new { Worker = id, Steps = steps, PerStepDelayMs = delayMs };
        }
    }
}
