using System.Net;

namespace MasterApi
{
    public class HttpHelper
    {
        public static string InvokeApi(string url )
        {
            return RedirectRequest( url ).Result;
        }
        private static async Task<string> RedirectRequest(string url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 5,
                UseCookies = true
            };

            using( var httpClient = new HttpClient( httpClientHandler ) )
            {
                var request = new HttpRequestMessage( HttpMethod.Get, new Uri( url ) );

                try
                {
                    var response = await httpClient.SendAsync( request );

                    if( response.StatusCode == HttpStatusCode.Redirect ||
                        response.StatusCode == HttpStatusCode.Moved ||
                        response.StatusCode == HttpStatusCode.RedirectKeepVerb )
                    {
                        var redirectedUrl = response.Headers.Location;
                        var redirectedRequest = new HttpRequestMessage( HttpMethod.Get, redirectedUrl );
                        var redirectedResponse = await httpClient.SendAsync( redirectedRequest );

                        return await redirectedResponse.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
                catch( HttpRequestException ex )
                {
                    Console.WriteLine( $"Request failed with error: {ex.Message}" );
                    return ex.Message + url;
                }
            }
        }
    }
}
