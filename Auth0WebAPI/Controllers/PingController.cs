using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0WebAPI.Models;
using Google.Apis.PeopleService.v1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Claim = Auth0WebAPI.Models.Claim;

namespace Auth0WebAPI.Controllers
{
    [Route("api")]
    public class PingController : Controller
    {
        private ManagementAPI _managementAPI;
        public PingController()
        {
            _managementAPI = new ManagementAPI();
        }

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "Pong";
        }

        [Authorize]
        [HttpGet]
        [Route("ping/secure")]
        public string PingSecured()
        {
            return "All good. You only get this message if you are authenticated.";
        }

        [Authorize]
        [HttpGet("claims")]
        public List<Claim> Claims()
        {
            return GetClaims().ToList();
        }

        [Authorize]
        [HttpGet("connections")]
        public async Task<int?> UserProfile()
        {
            int connectionCount = 0;
            int? result = null;
            List<Claim> claims = this.GetClaims().ToList();
            string userId = claims.FirstOrDefault(claim => claim.Type.EndsWith("nameidentifier")).Value;

            string googleAccessToken = await _managementAPI.GetGoogleAccessToken(userId);

            if (googleAccessToken != null)
            {
                GooglePeople googlePeopleClient = new GooglePeople(googleAccessToken);
                IList<Person> connections = googlePeopleClient.GetGoogleConnections();
                connectionCount = connections.Count;
                bool success = _managementAPI.UpdateUserGoogleConnectionCount(connectionCount, userId);
                if (success)
                {
                    result = connectionCount;
                }
            }
            return result;
        }

        /// <summary>
        /// Get Logged IN User Claims
        ///  TODO: Move this to a class that can be used in multiple places
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Claim> GetClaims()
        {
            var claims = User.Claims.Select(c =>
                new
                    Claim(
                        c.Type,
                        c.Value
                    ));
            return claims;
        }
    }
}