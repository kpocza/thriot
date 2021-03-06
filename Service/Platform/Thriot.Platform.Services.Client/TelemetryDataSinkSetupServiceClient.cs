﻿using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Thriot.Platform.Services.Client
{
    public class TelemetryDataSinkSetupServiceClient : ITelemetryDataSinkSetupServiceClient
    {
        private static string _serviceUrl;
        private static string _apiKey;
        private const string TelemetrySetupServiceApiKey = "X-TelemetrySetupServiceApiKey";

        public void Setup(string serviceUrl, string apiKey)
        {
            _apiKey = apiKey;
            _serviceUrl = serviceUrl;
        }

        public TelemetryDataSinksMetadataDtoClient GetTelemetryDataSinksMetadata()
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers.Add(TelemetrySetupServiceApiKey, _apiKey);

                var str = wc.DownloadString(_serviceUrl + "/metadata");
                return JsonConvert.DeserializeObject<TelemetryDataSinksMetadataDtoClient>(str);
            }
        }

        public void PrepareAndValidateIncoming(TelemetryDataSinksParametersDtoClient telemetryDataSinksParameters)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                wc.Headers.Add(TelemetrySetupServiceApiKey, _apiKey);
                
                var str = JsonConvert.SerializeObject(telemetryDataSinksParameters, Formatting.None);
                wc.UploadString(_serviceUrl + "/prepareAndValidate", str);
            }
        }
    }
}