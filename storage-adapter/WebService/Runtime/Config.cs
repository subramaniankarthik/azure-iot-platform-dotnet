﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Runtime;
using Microsoft.Azure.IoTSolutions.StorageAdapter.Services.Helpers;
using Mmm.Platform.IoT.Common.Services.Helpers;
using Mmm.Platform.IoT.Common.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.StorageAdapter.WebService.Runtime
{
    public interface IConfig
    {
        /// <summary>Web service listening port</summary>
        int Port { get; }

        /// <summary>Service layer configuration</summary>
        IServicesConfig ServicesConfig { get; }
    }

    /// <summary>Web service configuration</summary>
    public class Config : IConfig
    {
        private const string GLOBAL_KEY = "Global:";
        private const string COSMOSDB_KEY = GLOBAL_KEY + "CosmosDb:";
        private const string APPLICATION_KEY = "StorageAdapter:";
        private const string PORT_KEY = APPLICATION_KEY + "webservicePort";
        private const string STORAGE_TYPE_KEY = APPLICATION_KEY + "storageType";
        private const string DOCUMENT_DB_RUS_KEY = APPLICATION_KEY + "documentDBRUs";
        private const string APP_CONFIG_CONNECTION_STRING_KEY = "PCS_APPLICATION_CONFIGURATION";
        private const string APP_INSIGHTS_INSTRUMENTATION_KEY = "Global:instrumentationKey";
        private const string EXTERNAL_DEPENDENCIES = "ExternalDependencies:";
        private const string USER_MANAGEMENT_URL_KEY = EXTERNAL_DEPENDENCIES + "authWebServiceUrl";
        private const string AUTH_REQUIRED_KEY = "AuthRequired";

        private const string COSMOS_CONNECTION_STRING_KEY = "documentDBConnectionString";

        /// <summary>Web service listening port</summary>
        public int Port { get; }

        /// <summary>Service layer configuration</summary>
        public IServicesConfig ServicesConfig { get; }

        public Config(IConfigData configData)
        {
            this.Port = configData.GetInt(PORT_KEY);

            var storageType = configData.GetString(STORAGE_TYPE_KEY).ToLowerInvariant();
            var appConfigConnectionString = configData.GetString(APP_CONFIG_CONNECTION_STRING_KEY);
            if (storageType == "documentdb" &&
                (string.IsNullOrEmpty(appConfigConnectionString)
                 || appConfigConnectionString.StartsWith("${")
                 || appConfigConnectionString.Contains("...")))
            {
                // In order to connect to the storage, the service requires a connection
                // string for Document Db. The value can be found in the Azure Portal.
                // The connection string can be stored in the 'appsettings.ini' configuration
                // file, or in the PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING environment variable.
                // When working with VisualStudio, the environment variable can be set in the
                // WebService project settings, under the "Debug" tab.
                throw new Exception("The service configuration is incomplete. " +
                                    "Please provide your App Config connection string. " +
                                    "For more information, see the environment variables " +
                                    "used in project properties and the 'PCS_APPLICATION_CONFIGURATION' " +
                                    "value in the 'appsettings.ini' configuration file.");
            }
            AppConfigurationHelper appConfig = new AppConfigurationHelper(appConfigConnectionString);
            AppInsightsExceptionHelper.Initialize(configData.GetString(APP_INSIGHTS_INSTRUMENTATION_KEY));
            this.ServicesConfig = new ServicesConfig
            {
                StorageType = configData.GetString(STORAGE_TYPE_KEY),
                DocumentDbConnString = configData.GetString(COSMOS_CONNECTION_STRING_KEY),
                DocumentDbDatabase = configData.GetString($"{APPLICATION_KEY}{configData.GetString(STORAGE_TYPE_KEY)}"),
                DocumentDbRUs = configData.GetInt(DOCUMENT_DB_RUS_KEY),
                UserManagementApiUrl = configData.GetString(USER_MANAGEMENT_URL_KEY),
                AppConfig = appConfig
            };
        }
    }
}
