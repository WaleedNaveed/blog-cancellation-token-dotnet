# CancellationToken Demos for ASP.NET Core

This repository contains the demo code used in the blog post [CancellationToken in ASP.NET Core – A Complete Guide](https://wntech.hashnode.dev/cancellation-token-in-aspnet-core).  
It’s a compact, copy-paste-friendly sample project that demonstrates real-world CancellationToken patterns:

**Scenarios covered**
- Client-initiated cancellation via `HttpContext.RequestAborted`
- Timeout handling with `CancellationTokenSource.CancelAfter()`
- Linking tokens (timeout + client abort)
- Parallel tasks with cooperative cancellation
- Cleanup on cancel (large file write example)
- Background worker with queue and graceful shutdown

## Quick start (requirements)
- .NET 8 SDK installed
- Run from repository root

```bash
dotnet restore
dotnet build
dotnet run
```

By default the app runs on https://localhost:7269.

## Example Endpoints

- **Client cancel / RequestAborted**  
  `GET https://localhost:7269/api/Demo/reports/generate?rows=200&delayMs=25`

- **CancelAfter timeout**  
  `POST https://localhost:7269/api/Demo/timeout/run?timeoutMs=2000`

- **Linked tokens (timeout + client abort)**  
  `POST https://localhost:7269/api/Demo/linked/search?timeoutMs=1000`

- **Parallel cooperative cancellation**  
  `POST https://localhost:7269/api/Demo/parallel/scrape?n=5&delayMs=800`

- **Cleanup on cancel (file write)**  
  `POST https://localhost:7269/api/Demo/files/write?sizeMb=25&chunkMs=100`

- **Background worker + queue**  
  `POST https://localhost:7269/api/Demo/queue/enqueue?count=3`

---

## Notes

- Use **Postman** to demonstrate client-abort reliably.  
  (Browsers/Swagger may not immediately close the underlying TCP connection.)
- The code is intentionally small and readable so you can copy individual demos into your own projects.

---

## Blog

Read the full write-up with explanations and screenshots here:  
[CancellationToken in ASP.NET Core – A Complete Guide](https://wntech.hashnode.dev/cancellation-token-in-aspnet-core)

---

## Contributing

Small improvements, bugfixes, or clearer examples are welcome — open a PR.
