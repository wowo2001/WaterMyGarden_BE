namespace WaterMyGarden
{
    public class Weather
    {
        public List<DayWeather> weather { get; set; }
        public Weather()
        {
            weather = new List<DayWeather>();
        }
    }

    public class DayWeather()
    {
        public string date { get; set; }
        public string weather { get; set; }
        public bool wateringGarden { get; set; }
    }
}
