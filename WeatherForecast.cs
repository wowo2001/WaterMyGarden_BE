namespace WaterMyGarden
{
    public class WeatherForecast
    {
        public string Timezone { get; set; }
        public string Timezone_Abbreviation { get; set; }
        public double Elevation { get; set; }
        public DailyData Daily { get; set; }
    }

    public class DailyData
    {
        public List<string> Time { get; set; }
        public List<double> Rain_Sum { get; set; }
        public List<double> Showers_Sum { get; set; }
    }
}
