using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SafeMind.WebAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Deixa a requisição seguir o seu fluxo normal pela API
                await _next(context);
            }
            catch (Exception ex)
            {
                // Se ALGO der errado em qualquer lugar do sistema, o erro cai aqui!
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500; // 500 = Internal Server Error

                var response = new 
                { 
                    erro = "Ocorreu uma falha interna no servidor. A nossa equipe já foi notificada.",
                    detalhe = ex.Message 
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}