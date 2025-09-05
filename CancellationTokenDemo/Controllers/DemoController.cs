using CancellationTokenDemo.Implementations;
using CancellationTokenDemo.Interfaces;
using CancellationTokenDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace CancellationTokenDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly ReportService _reportService;
        private readonly TimeoutDemo _timeoutDemo;
        private readonly ParallelTasksDemo _parallelTasksDemo;
        private readonly FileWriteDemo _fileWriteDemo;
        private readonly IJobQueue _queue;

        public DemoController(ReportService reportService,
            TimeoutDemo timeoutDemo,
            ParallelTasksDemo parallelTasksDemo,
            FileWriteDemo fileWriteDemo,
            IJobQueue queue)
        {
            _reportService = reportService;
            _timeoutDemo = timeoutDemo;
            _parallelTasksDemo = parallelTasksDemo;
            _fileWriteDemo = fileWriteDemo;
            _queue = queue;
        }

        // 1) Client-initiated cancellation via HttpContext.RequestAborted
        [HttpGet("reports/generate")]
        public async Task<IActionResult> GenerateReport(int rows = 200, int delayMs = 25, CancellationToken token = default)
        {
            try
            {
                var summary = await _reportService.GenerateReportAsync(rows, delayMs, token);
                return Ok(summary);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Request cancelled by client");
                return StatusCode(StatusCodes.Status499ClientClosedRequest, new { Message = "Client cancelled the request." });
            }
        }

        // 2) Timeout (CancelAfter)
        [HttpPost("timeout/run")]
        public async Task<IActionResult> RunWithTimeout(int timeoutMs = 2000, CancellationToken token = default)
        {
            var result = await _timeoutDemo.RunWithTimeoutAsync(timeoutMs, token);
            return Ok(result);
        }

        // 3) Linked tokens (timeout + client cancel)
        [HttpPost("linked/search")]
        public async Task<IActionResult> LinkedSearch(int timeoutMs = 1000, CancellationToken token = default)
        {
            var result = await _timeoutDemo.LinkedSearchAsync(timeoutMs, token);
            return Ok(result);
        }

        // 4) Parallel tasks + cooperative cancellation 
        [HttpPost("parallel/scrape")]
        public async Task<IActionResult> ScrapeMany(int n = 5, int delayMs = 800, CancellationToken token = default)
        {
            var result = await _parallelTasksDemo.ScrapeManyAsync(n, delayMs, token);
            return Ok(result);
        }

        // 5) Cleanup on cancel (file write) 
        [HttpPost("files/write")]
        public async Task<IActionResult> WriteLargeFile(int sizeMb = 25, int chunkMs = 200, CancellationToken token = default)
        {
            var result = await _fileWriteDemo.WriteLargeFileAsync(sizeMb, chunkMs, token);
            return Ok(result);
        }

        // 6) Channel + Worker: enqueue & inspect jobs 
        [HttpPost("queue/enqueue")]
        public async Task<IActionResult> EnqueueJobs(int count = 3, CancellationToken token = default)
        {
            var jobs = new List<JobItem>();
            for (int i = 1; i <= count; i++)
            {
                var job = new JobItem { FileName = $"DEMO_{i:00}.dat" };
                await _queue.EnqueueAsync(job, token);
                jobs.Add(job);
            }

            return Ok(new { Enqueued = jobs.Select(j => j.JobId) });
        }
    }
}
