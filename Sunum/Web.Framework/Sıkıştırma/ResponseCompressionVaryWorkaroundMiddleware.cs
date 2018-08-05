using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Core;

namespace Web.Framework.Sıkıştırma
{
    public partial class ResponseCompressionVaryWorkaroundMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor
        
        public ResponseCompressionVaryWorkaroundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods
        
        public async Task Invoke(Microsoft.AspNetCore.Http.HttpContext context, IWebYardımcısı webHelper)
        {
            var accept = context.Request.Headers[HeaderNames.AcceptEncoding];
            if (!StringValues.IsNullOrEmpty(accept))
            {
                context.Response.Headers.Append(HeaderNames.Vary, HeaderNames.AcceptEncoding);
            }
            await _next(context);
        }

        #endregion
    }
}