using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using finTrack.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace finTrack.Middlewares
{
    public class JwtMiddleware : IMiddleware
    {
        private readonly TokenService _jwtSecurityTokenHandler;

        public JwtMiddleware(TokenService jwtSecurityTokenHandler){
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string token = context.Request?.Cookies["TOKEN"] ?? "";

            if(!token.IsNullOrEmpty()){
            
                try
                {
                    var userId = await _jwtSecurityTokenHandler.ValidateJwtTokenAndGetUserID(token);
                    context.Items["UserID"] = userId;

                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("Unauthorized");
                }

            
            }
            await next(context);
        }
    }
}