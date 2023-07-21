namespace MoviePlatformApi.Configuration
{
    public class Setting
    {
        public string DefaultToken { get; set; }
        public string OMDbApiKey { get; set; }
        public string BaseUrl { get; set; }
        public int Skip { get; set; } = 0;
        public int Limit { get; set; } = 20;
        public bool IsProduction { get; set; } = true;
    }
}