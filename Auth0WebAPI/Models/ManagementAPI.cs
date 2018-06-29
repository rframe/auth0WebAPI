using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace Auth0WebAPI.Models
{
    public class ManagementAPI
    {
        private ManagementApiClient _client;
        private readonly string _managementToken = Startup.Configuration["Auth0:ManagementToken"];
        private readonly string _managementDomain = Startup.Configuration["Auth0:ManagementDomain"];
        public ManagementAPI()
        {
            _client = new ManagementApiClient(_managementToken, new Uri(_managementDomain));
        }

        /// <summary>
        /// Get User by UserId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUser(string userId)
        {
            return await _client.Users.GetAsync(userId);
        }

        /// <summary>
        /// Update Goolge Connections Count
        /// </summary>
        /// <param name="count"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateUserGoogleConnectionCount(int count, string userId)
        {
            var client = new RestClient($"{_managementDomain}users/{userId}");
            var request = new RestRequest(Method.PATCH);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", $"Bearer {_managementToken}");
            request.AddParameter("application/json", "{\"user_metadata\": {\"googleConnections\": \"" + count + "\"}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.IsSuccessful;
        }

        /// <summary>
        /// Get Google Access Tokens
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetGoogleAccessToken(string userId)
        {
            string accessToken = null;
            User user = await GetUser(userId);

            foreach (Identity userIdentity in user?.Identities)
            {
                if (userIdentity.Provider == "google-oauth2")
                {
                    accessToken = userIdentity.AccessToken;
                    string refreshToken = userIdentity.RefreshToken;

                    PeopleServiceService peopleService = new
                        PeopleServiceService(new BaseClientService.Initializer()
                        {
                                ApplicationName = "Pizza42",
                        });
                    peopleService.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    PeopleResource.ConnectionsResource.ListRequest peopleRequest =
                        peopleService.People.Connections.List("people/me");
                    peopleRequest.PersonFields = "names,emailAddresses";
                    ListConnectionsResponse connectionsResponse = peopleRequest.Execute();
                    IList<Person> connections = connectionsResponse.Connections;
                    int connectionCount = connections.Count;
                }
            }

            return accessToken;
        }
    }
}
