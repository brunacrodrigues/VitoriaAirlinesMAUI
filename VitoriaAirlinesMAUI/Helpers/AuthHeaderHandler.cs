using System.Net.Http.Headers;

namespace VitoriaAirlinesMAUI.Helpers
{
    /// <summary>
    /// Custom HTTP message handler that adds a Bearer token to the Authorization header
    /// of each outgoing HTTP request if a token is stored in preferences.
    /// </summary>
    public class AuthHeaderHandler : DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request with an Authorization header if a token exists.
        /// </summary>
        /// <param name="request">The outgoing HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>The HTTP response message from the inner handler.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = Preferences.Get("Token", string.Empty);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
