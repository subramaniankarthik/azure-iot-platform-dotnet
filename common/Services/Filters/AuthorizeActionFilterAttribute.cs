﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Mmm.Platform.IoT.Common.Services;
using Mmm.Platform.IoT.Common.Services.Exceptions;

namespace Mmm.Platform.IoT.Common.Services.Filters
{
    public class AuthorizeActionFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string allowedAction;

        public AuthorizeActionFilterAttribute(string allowedAction)
        {
            this.allowedAction = allowedAction;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool isAuthorized = this.IsValidAuthorization(context.HttpContext, this.allowedAction);

            if (!isAuthorized)
            {
                throw new NotAuthorizedException($"Current user is not authorized to perform this action: '{this.allowedAction}'");
            }
            else
            {
                await next();
            }
        }

        /// <summary>
        /// Validate allowed actions of current user based on role claims against the declared actions
        /// in the Authorize attribute of controller. The allowed action is case insensitive.
        /// </summary>
        /// <param name="httpContext">current context of http request</param>
        /// <param name="allowedAction">allowed action required by controller</param>
        /// <returns>true if validatation succeed</returns>
        private bool IsValidAuthorization(HttpContext httpContext, string allowedAction)
        {
            if (!httpContext.Request.GetAuthRequired() || !httpContext.Request.IsExternalRequest()) return true;

            if (allowedAction == null || !allowedAction.Any()) return true;

            var userAllowedActions = httpContext.Request.GetCurrentUserAllowedActions();
            if (userAllowedActions == null || !userAllowedActions.Any())
            {
                return false;
            }

            // validation succeeds if any required action occurs in the current user's allowed allowedAction
            return userAllowedActions.Select(a => a.ToLowerInvariant())
               .Contains(this.allowedAction.ToLowerInvariant());
        }
    }
}
