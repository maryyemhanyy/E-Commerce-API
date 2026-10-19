using E_Commerce.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Drawing.Text;
using System.Text;

namespace E_Commerce.Attributes
{
    public class RedisCacheAttribure : ActionFilterAttribute
    {
        private readonly int durationInSeconds;
        public RedisCacheAttribure(int durationInSeconds)
        {
            this.durationInSeconds = durationInSeconds;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheServices>();
            var cacheKey = GenerateCacheKey(context.HttpContext.Request);
            var cachedResponse = await cacheService.GetAsync(cacheKey);
            if(!string.IsNullOrEmpty(cachedResponse))
            {
              context.Result = new ContentResult
              {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = StatusCodes.Status200OK
              };
                return;
            }
             var executedContext = await next.Invoke();
            if(executedContext.Result is OkObjectResult {Value : not null } ok)
            {
               await cacheService.SetAsync(cacheKey, ok.Value, TimeSpan.FromSeconds(durationInSeconds));
            }
            return;

        }

        private static string GenerateCacheKey(HttpRequest request)
        {
          var key = new StringBuilder();
          key.Append(request.Path).Append("?");
                foreach (var (k, v) in request.Query.OrderBy(x => x.Key))
                {
                    key.Append(k).Append("=").Append(v);
                }
                return key.ToString();
        }
    }
}
