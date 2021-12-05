using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient.Authentication
{
    public class AuthorizedHandler : DelegatingHandler
    {
        private readonly HostAuthenticationStateProvider _authenticationStateProvider;

        public AuthorizedHandler(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = (HostAuthenticationStateProvider)authenticationStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            HttpResponseMessage responseMessage;

            if (!authState.User.Identity.IsAuthenticated)
            {
                responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                responseMessage = await base.SendAsync(request, cancellationToken);
            }

            if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
            {
                _authenticationStateProvider.SignIn();
            }

            return responseMessage;
        }
    }
}
