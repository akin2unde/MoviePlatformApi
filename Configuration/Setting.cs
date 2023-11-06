namespace MoviePlatformApi.Configuration
{
    public class Setting
    {

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public int Skip { get; set; } = 0;
        public int Limit { get; set; } = 20;
        public bool IsProduction { get; set; } = true;
    }
}