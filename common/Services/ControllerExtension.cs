using System;
using Microsoft.AspNetCore.Mvc;

namespace Mmm.Platform.IoT.Common.Services
{
    public static class ControllerExtension
    {
        public static string GetTenantId(this Controller controller)
        {
            try
            {
                string tenantId = controller.HttpContext.Request.GetTenant();
                if (String.IsNullOrEmpty(tenantId))
                {
                    throw new Exception("Request Tenant was null or empty.");
                }
                return tenantId;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to get the tenantId from the user claims or the request headers.", e);
            }
        }

        public static string GetClaimsUserId(this Controller controller)
        {
            try
            {
                return controller.HttpContext.Request.GetCurrentUserObjectId();
            }
            catch (Exception e)
            {
                throw new Exception("A request was sent to an API endpoint that requires a userId, but the userId was not passed through the url nor was it available in the user Claims.", e);
            }
        }
    }
}