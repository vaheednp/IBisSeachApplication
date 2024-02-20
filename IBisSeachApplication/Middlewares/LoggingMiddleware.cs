namespace API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Log request information
            var request = context.Request;
            _logger.LogInformation($"Request: {request.Method} {request.Path}");

            await _next(context);

            // Log response information
            var response = context.Response;
            _logger.LogInformation($"Response: {response.StatusCode}");
        }
    }
}
