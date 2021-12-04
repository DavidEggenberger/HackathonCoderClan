using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebAPI.GitHub
{
    public class GitHubAPIClient
    {
        private HttpClient httpClient;
        public GitHubAPIClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Add("User-Agent", "request");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        }
        public void SetAuthorizationHeader(string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
        }
        public Task<GitHubRoot> GetGitHubRootAsync()
        {
            return httpClient.GetFromJsonAsync<GitHubRoot>("/user");
        }
    }
}
