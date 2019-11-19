﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using Newtonsoft.Json;

namespace Mmm.Platform.IoT.Common.Services.Http
{
    /* <summary>
     * This class is a Extension of HttpRequest class which is used by StorageAdapterClientTest
     * to validate the URL and data model using overloaded "Check" methods
     * </summary>     */
    internal static class HttpRequestExtensions
    {
        public static bool Check(this IHttpRequest request, string uri)
        {
            return request.Uri.ToString() == uri;
        }

        public static bool Check<T>(this IHttpRequest request, string uri, Func<T, bool> validator)
        {
            if (request.Uri.ToString() != uri)
            {
                return false;
            }

            if (validator == null)
            {
                return true;
            }

            var model = JsonConvert.DeserializeObject<T>(request.Content.ReadAsStringAsync().Result);
            return validator(model);
        }
    }
}