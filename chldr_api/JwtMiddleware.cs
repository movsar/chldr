﻿using Microsoft.AspNetCore.Mvc;

namespace chldr_api
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestedPath = context.Request.Path.Value;
            var r = context.Request;
            var isLoginMutation = context.Request.Method == "POST" && requestedPath.StartsWith("/graphql") && requestedPath.EndsWith("/loginUser");

            if (!isLoginMutation && !context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (!isLoginMutation)
            {
                var token = authorizationHeader.ToString().Replace("Bearer ", "");
                // TODO: Validate the access token using a library like JWTBearerAuthentication or IdentityServer
                // You can store the validated token in the HttpContext for later use
                context.Items["access_token"] = token;
            }

            await _next(context);
        }
    }
}
