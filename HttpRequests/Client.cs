using HttpRequests;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace HttpRequests
{
    public class Client
    {
        ILogger _logger;
        IHttpClientFactory _clientFactory;
        AsyncRetryPolicy _retryPolicy;
        private readonly int maxRetries = 3;

        public Client(ILogger logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(maxRetries, times =>
                TimeSpan.FromMilliseconds(times * 100));
        }

        public async Task<ShowLookupTvMazeApi> GetShowLookupResponse(string tvMazeRootUrl, string ImdbIds)
        {
            var client = _clientFactory.CreateClient();
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var uri = new Uri($"{tvMazeRootUrl}/lookup/shows?imdb=tt{ImdbIds}");
                var response = await client.GetAsync(uri);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.Information($"{tvMazeRootUrl} is unresponsive after {maxRetries} tries");
                    var showLookUpTvMazeApi = new ShowLookupTvMazeApi();
                    showLookUpTvMazeApi.Name = "";
                    return showLookUpTvMazeApi;
                }
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ShowLookupTvMazeApi>(responseString);
            });
        }
    }
}
