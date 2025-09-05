using CancellationTokenDemo.Implementations;
using CancellationTokenDemo.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI registrations
builder.Services.AddSingleton<IJobQueue, ChannelJobQueue>();
builder.Services.AddSingleton<ReportService>();
builder.Services.AddSingleton<TimeoutDemo>();
builder.Services.AddSingleton<ParallelTasksDemo>();
builder.Services.AddSingleton<FileWriteDemo>();
builder.Services.AddHostedService<BackgroundWorker>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

