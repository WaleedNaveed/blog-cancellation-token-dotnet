namespace CancellationTokenDemo.Implementations
{
    public sealed class TimeoutDemo
    {
        public async Task<object> RunWithTimeoutAsync(int timeoutMs, CancellationToken outer)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(outer);
            cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                // Simulated IO (3s)
                await SimulatedIoAsync(3000, cts.Token);
                sw.Stop();
                return new { Completed = true, ElapsedMs = sw.ElapsedMilliseconds, TimedOut = false };
            }
            catch (OperationCanceledException)
            {
                sw.Stop();
                return new { Completed = false, ElapsedMs = sw.ElapsedMilliseconds, TimedOut = true };
            }
        }

        // Linked tokens: timeout + client abort
        public async Task<object> LinkedSearchAsync(int timeoutMs, CancellationToken clientAbort)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs));
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, clientAbort);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                // Simulate a long-running operation in 10 steps (each ~400ms)
                for (int i = 0; i < 10; i++)
                {
                    linked.Token.ThrowIfCancellationRequested();
                    await Task.Delay(400, linked.Token);
                }
                sw.Stop();
                return new { Completed = true, ElapsedMs = sw.ElapsedMilliseconds, Reason = "All steps done" };
            }
            catch (OperationCanceledException)
            {
                sw.Stop();

                string reason;
                if (timeout.IsCancellationRequested && !clientAbort.IsCancellationRequested)
                {
                    reason = "Timeout";
                    Console.WriteLine($"Request cancelled due to TIMEOUT");
                }
                else if (clientAbort.IsCancellationRequested)
                {
                    reason = "ClientAborted";
                    Console.WriteLine($"Request cancelled by CLIENT");
                }
                else
                {
                    reason = "LinkedCancel";
                    Console.WriteLine($"Request cancelled for unknown linked reason after {sw.ElapsedMilliseconds}ms");
                }

                return new { Completed = false, ElapsedMs = sw.ElapsedMilliseconds, Reason = reason };
            }
        }

        private static async Task SimulatedIoAsync(int ms, CancellationToken ct)
            => await Task.Delay(ms, ct);
    }
}
