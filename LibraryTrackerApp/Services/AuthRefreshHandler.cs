using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LibraryTrackerApp.Services
{
    public class AuthRefreshHandler : DelegatingHandler
    {

        private readonly AuthService _authService;

        public AuthRefreshHandler(AuthService authService)
        {
            _authService = authService;            
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = _authService.GetValidAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await _authService.RefreshTokenUserAsync();

                if (refreshed)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.AccessToken);
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            return response;
        }
        
    }
}