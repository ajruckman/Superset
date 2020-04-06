#nullable enable

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Superset.Logging
{
    public sealed class SplunkExporter
    {
        private readonly HttpClient _httpClient;
        private readonly string     _url;

        public readonly string Host;
        public readonly bool   HTTPS;
        public readonly bool   IgnoreCertificate;
        public readonly string Index;
        public readonly int    Port;
        public readonly string Server;
        public readonly string Source;
        public readonly string SourceType;

        public SplunkExporter(
            string  server,
            string  token,
            string  index,
            string  source,
            bool    https             = false,
            bool    ignoreCertificate = false,
            int     port              = 8088,
            string  sourceType        = "_json",
            string? host              = null
        )
        {
            Server            = server;
            Port              = port;
            HTTPS             = https;
            IgnoreCertificate = ignoreCertificate;
            Index             = index;
            Source            = source;
            SourceType        = sourceType;
            Host              = host ?? Environment.MachineName;

            _url = $"{(HTTPS ? "https" : "http")}://{Server}:{Port}/services/collector";

            if (!HTTPS || !IgnoreCertificate)
            {
                _httpClient = new HttpClient();
            }
            else
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                httpClientHandler.ServerCertificateCustomValidationCallback +=
                    (message, certificate, chain, errors) => true;
                _httpClient = new HttpClient(httpClientHandler);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Splunk", token);
        }

        public void Send(Message message)
        {
            string? exception = null;
            if (message.Exception != null)
            {
                exception = "";
                using StringReader reader = new StringReader(message.Exception.ToString());

                for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    exception += line + "\n";
            }

            SplunkEvent splunkEvent = new SplunkEvent
            {
                Event = new Fields
                {
                    {"Time", message.Time},
                    {"Level", message.Level.ToString()},
                    {"Text", message.Text},
                    {"Meta", message.Meta},
                    {"Exception", exception},
                    {"MemberName", message.MemberName},
                    {"SourceFilePath", message.SourceFilePath},
                    {"SourceLineNumber", message.SourceLineNumber}
                },
                Host       = Host,
                Index      = Index,
                Source     = Source,
                SourceType = SourceType
            };

            string content = JsonConvert.SerializeObject(splunkEvent);

            HttpRequestMessage request = new HttpRequestMessage(
                HttpMethod.Post,
                _url
            );
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task   = _httpClient.SendAsync(request);
            HttpResponseMessage       result = task.Result;

            if (task.Exception != null)
                throw task.Exception;

            if (!result.IsSuccessStatusCode)
                Console.WriteLine("Failed to send message to Splunk: " + result.Content.ReadAsStringAsync().Result);
        }

        public struct SplunkEvent
        {
            [JsonProperty("event")]
            public Fields Event { get; set; }

            [JsonProperty("host")]
            public string Host { get; set; }

            [JsonProperty("index")]
            public string Index { get; set; }

            [JsonProperty("source")]
            public string Source { get; set; }

            [JsonProperty("sourcetype")]
            public string SourceType { get; set; }

            [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
            public DateTime? Time { get; set; }
        }
    }
}