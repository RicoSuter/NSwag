namespace NSwag.Sample.Common
{
    public class WeatherForecast
    {
        public Station Station { get; set; }

        public string DateFormatted { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF
        {
            get
            {
                return 32 + (int)(TemperatureC / 0.5556);
            }
        }
    }
}
