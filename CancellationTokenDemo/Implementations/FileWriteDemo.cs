namespace CancellationTokenDemo.Implementations
{
    public sealed class FileWriteDemo
    {
        public async Task<object> WriteLargeFileAsync(int sizeMb, int chunkMs, CancellationToken ct)
        {
            var fileName = $"ct-demo-{Guid.NewGuid():N}.bin";
            var tmpPath = Path.Combine(Path.GetTempPath(), fileName);

            var totalBytes = sizeMb * 1024L * 1024L;
            var chunk = new byte[1024 * 1024]; // 1MB
            Random.Shared.NextBytes(chunk);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var written = 0L;

            try
            {
                await using var fs = new FileStream(
                    tmpPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true);

                while (written < totalBytes)
                {
                    ct.ThrowIfCancellationRequested();

                    var remaining = (int)Math.Min(chunk.Length, totalBytes - written);
                    await fs.WriteAsync(chunk.AsMemory(0, remaining), ct);
                    written += remaining;

                    // flush so partial data really goes to disk
                    await fs.FlushAsync(ct);

                    // simulate work per chunk
                    await Task.Delay(chunkMs, ct);
                }

                sw.Stop();

                return new
                {
                    FileName = fileName,
                    SizeMB = sizeMb,
                    ElapsedMs = sw.ElapsedMilliseconds,
                    Canceled = false
                };
            }
            catch (OperationCanceledException)
            {
                sw.Stop();

                if (File.Exists(tmpPath))
                {
                    try { File.Delete(tmpPath); } catch { }
                }

                Console.WriteLine($"Request cancelled by CLIENT");

                return new
                {
                    FileName = fileName,
                    SizeMB = sizeMb,
                    ElapsedMs = sw.ElapsedMilliseconds,
                    Canceled = true
                };
            }
        }
    }
}
