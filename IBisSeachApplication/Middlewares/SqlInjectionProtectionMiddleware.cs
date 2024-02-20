namespace API.Middlewares
{
    public class SqlInjectionProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public SqlInjectionProtectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Sanitize input data to prevent SQL injection
            foreach (var key in context.Request.Query.Keys)
            {
                var value = context.Request.Query[key];
                if (IsSqlInjection(value))
                {
                    // Log or handle the SQL injection attempt
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("SQL injection attempt detected.");
                    return;
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }

        private bool IsSqlInjection(string value)
        {
            // Implement logic to detect SQL injection patterns
            // Example: Check for common SQL injection keywords or patterns
            return value.Contains("SELECT") || value.Contains("DROP") || value.Contains("INSERT") || value.Contains("UPDATE") || value.Contains("DELETE");
        }
    }
}
