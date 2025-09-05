namespace CancellationTokenDemo.Models
{
    public class JobItem
    {
        public Guid JobId { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = default!;
        public string Status { get; set; } = "Pending";
    }
}
