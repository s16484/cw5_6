using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw5.Middlewares
{
    public class LoggingMiddleware
    {

        private readonly RequestDelegate _next;
        private const string FILE_NAME = "requestsLog.txt";

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method.ToString();
                string querystring = context.Request?.QueryString.ToString();
                string bodyStr = "";

                using (StreamReader reader
                 = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                }

                File.AppendAllText(FILE_NAME,
                    "\n------- "+DateTime.Now.ToString()+ " -------" + 
                    "\nMetoda: " + method + 
                    "\nŚcieżka: " + path + 
                    "\nCiało żądania: " + bodyStr + 
                    "\nQuery: " + querystring);

                await _next(context);

            }
        }
    }
}
