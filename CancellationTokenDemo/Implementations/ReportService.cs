namespace CancellationTokenDemo.Implementations
{
    public sealed class ReportService
    {
        public async Task<object> GenerateReportAsync(int rows, int delayMs, CancellationToken ct)
        {
            var started = DateTimeOffset.UtcNow;
            var processed = 0;

            try
            {
                for (int i = 0; i < rows; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    await Task.Delay(delayMs, ct); // simulate row processing
                    processed++;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Report generation cancelled");
                throw;
            }

            return new
            {
                StartedAt = started,
                CompletedAt = DateTimeOffset.UtcNow,
                RowsProcessed = processed,
                DelayPerRowMs = delayMs
            };
        }
    }
}
