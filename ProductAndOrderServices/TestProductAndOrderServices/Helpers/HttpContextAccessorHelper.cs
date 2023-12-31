﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TestProductAndOrderServices.Helpers
{
    public class HttpContextAccessorHelper
    {
        public HttpContextAccessor HttpContextAccessor { get; }
        public HttpContextAccessorHelper()
        {
            HttpContextAccessor = new HttpContextAccessor();

            var claims = new List<Claim>
            {
                new Claim("BankAccountId", "64dcd34fe55c1e2ee8460999"),
                new Claim(ClaimTypes.NameIdentifier, "64dcd34fe55c1e2ee8460991")
            };
            var identity = new ClaimsIdentity(claims, "mock");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            HttpContextAccessor.HttpContext = httpContext;
        }
    }
}
