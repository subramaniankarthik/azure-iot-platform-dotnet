﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.IoTSolutions.Auth;
using Microsoft.Azure.IoTSolutions.UIConfig.Services.Helpers;
using Microsoft.Azure.IoTSolutions.UIConfig.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.UIConfig.Services.External
{
    public interface IDeviceTelemetryClient
    {
        Task UpdateRuleAsync(RuleApiModel rule, string etag);
    }

    public class DeviceTelemetryClient : IDeviceTelemetryClient
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly string serviceUri;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string TENANT_HEADER = "ApplicationTenantID";
        private const string TENANT_ID = "TenantID";
        public DeviceTelemetryClient(
            IHttpClientWrapper httpClient,
            IServicesConfig config,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpClient = httpClient;
            this.serviceUri = config.TelemetryApiUrl;
            this._httpContextAccessor = httpContextAccessor;


            string tenantId = this._httpContextAccessor.HttpContext.Request.GetTenant();
            this.httpClient.SetHeaders(new Dictionary<string, string> {{TENANT_HEADER, tenantId}});
        }

        public async Task UpdateRuleAsync(RuleApiModel rule, string etag)
        {
            rule.ETag = etag;

            await this.httpClient.PutAsync($"{this.serviceUri}/rules/{rule.Id}", $"Rule {rule.Id}", rule);
        }
    }
}
