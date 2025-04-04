using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WaterMyGarden.Data;
namespace WaterMyGarden.Services
{
    public interface IWeatherService
    { 
        Task<Weather> GetWeather();
        Task<ActionResult<string>> RecordWatering(RecordWateringRequest recordWateringRequest);

    }
    public class WeatherService: IWeatherService
    {
        private readonly IRecordWateringData _recordWateringData;

        public WeatherService(IRecordWateringData recordWateringData)
        {
            _recordWateringData = recordWateringData;
        }


        public async Task<ActionResult<string>> RecordWatering(RecordWateringRequest recordWateringRequest)
        {
            if (recordWateringRequest.operate == "add")
            {
                return await _recordWateringData.AddWateringData(recordWateringRequest);
            }
            else if (recordWateringRequest.operate == "remove")
            {
                return await _recordWateringData.DeleteDate(recordWateringRequest);
            }
            else {
                return "Fail";
            }

        }
        public async Task<Weather> GetWeather()
        {
            List<string> wateringDate = await _recordWateringData.ShowAllDates();
            Weather weather = new Weather();
            

            string baseUrl = "https://api.open-meteo.com/v1/forecast";

            DateTime currentDate = DateTime.Now;

            DateTime firstDayOfPreviousMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-1);

            // First day of the current month
            DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

            // Last day of the current month
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Formatting the datesformattedFirstDay = firstDayOfMonth.ToString("yyyy-MM-dd");
            string formattedPreviousMonthFirstDay = firstDayOfPreviousMonth.ToString("yyyy-MM-dd");
            string formattedFirstDay = firstDayOfMonth.ToString("yyyy-MM-dd");
            string formattedLastDay = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd");

            var parameters = new Dictionary<string, string>
            {
                { "latitude", "-33.8116" },
                { "longitude", "151.1054" },
                { "daily", "rain_sum,showers_sum" },
                { "timezone", "Australia/Sydney" },
                { "start_date", formattedPreviousMonthFirstDay },
                { "end_date", formattedLastDay }
            };
            var url = BuildUrlWithParams(baseUrl, parameters);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send GET request and get the response as a string
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response body as a string
                        string responseData = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response into a C# object
                        var weatherForecast = JsonConvert.DeserializeObject<WeatherForecast>(responseData.ToString());

                        // Print out the rain and shower sums for each day
                        for (int i = 0; i < weatherForecast.Daily.Time.Count; i++)
                        {
                            DayWeather dayWeather = new DayWeather();
                            dayWeather.date = weatherForecast.Daily.Time[i];
                            if (weatherForecast.Daily.Rain_Sum[i] + weatherForecast.Daily.Showers_Sum[i] > 1)
                            {
                                dayWeather.weather = "Rainy";
                            }
                            else {
                                dayWeather.weather = "Sunny";
                            }
                            dayWeather.wateringGarden = wateringDate.Contains(dayWeather.date);
                            weather.weather.Add(dayWeather);
                        }
                    }
                    else
                    {
                        // Handle non-success status codes (e.g., 400, 500)
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network issues, invalid JSON, etc.)
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return weather;
        }

        private static string BuildUrlWithParams(string baseUrl, Dictionary<string, string> parameters)
        {
            var uriBuilder = new UriBuilder(baseUrl);
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            // Add each parameter to the query string
            foreach (var param in parameters)
            {
                query[param.Key] = param.Value;
            }

            // Set the query string for the UriBuilder
            uriBuilder.Query = query.ToString();

            return uriBuilder.ToString();
        }
    }
}
