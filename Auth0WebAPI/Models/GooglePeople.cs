using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;

namespace Auth0WebAPI.Models
{
    public class GooglePeople
    {
        private PeopleServiceService _peopleService;
        public GooglePeople(string accessToken)
        {
            _peopleService = new
                PeopleServiceService(new BaseClientService.Initializer()
                {
                    ApplicationName = "Pizza42",
                });
            _peopleService.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        }

        /// <summary>
        /// Get Goolge Connections
        /// </summary>
        /// <returns></returns>
        public IList<Person> GetGoogleConnections()
        {
            PeopleResource.ConnectionsResource.ListRequest peopleRequest =
                _peopleService.People.Connections.List("people/me");
            peopleRequest.PersonFields = "names,emailAddresses";
            ListConnectionsResponse connectionsResponse = peopleRequest.Execute();
            return connectionsResponse.Connections;
        }
    }
}
