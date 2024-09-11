namespace MigrationApp.Core.Entities
{
    public class EndpointOptions
    {
        public Uri Url { get; set; } = new Uri("https://default.tableau.cloud");

        public string SiteContentUrl { get; set; } = string.Empty;

        public string AccessTokenName { get; set; } = string.Empty;

        // Access token configuration should use a secure configuration system.
        public string AccessToken { get; set; } = string.Empty;

        public EndpointOptions()
        {
        }

        public EndpointOptions(string url, string siteContentUrl, string accessTokenName, string accessToken)
        {
            Url = new Uri(url);
            SiteContentUrl = siteContentUrl;
            AccessTokenName = accessTokenName;
            AccessToken = accessToken;
        }

        public bool IsValid()
        {
            return Url != null && !string.IsNullOrEmpty(AccessTokenName) && !string.IsNullOrEmpty(AccessToken);
        }
    }
}
