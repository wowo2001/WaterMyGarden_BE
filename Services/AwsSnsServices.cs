using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using WaterMyGarden.Data;

namespace WaterMyGarden.Services
{
    public interface IAwsSnsServices
    {
        Task SendSNS();
    }

    public class AwsSnsServices: IAwsSnsServices
    {

        private readonly IWeatherService _weatherService;

        public AwsSnsServices(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }
        public async Task SendSNS()
        {
            var snsClient = new AmazonSimpleNotificationServiceClient(
                "AKIA4T4OCILUI2NQVOMT",
                "iLmmKfz4PEVIsDx7lR0e54ZsS2LjvAkCrWOu2C+2",
                Amazon.RegionEndpoint.APSoutheast2);

            string phoneNumber = "+610479035870";

            string message = await buildSNSMessage();

            // Send the SMS
            var request = new PublishRequest
            {
                PhoneNumber = phoneNumber,
                Message = message
            };

            try
            {
                var response = await snsClient.PublishAsync(request);
                Console.WriteLine($"Message sent! Message ID: {response.MessageId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
        private async Task<string> buildSNSMessage()
        {
            string message = null;
            DateTime currentDate = DateTime.Now;
            bool isRainOrWatering = false;
            for (int i = 0; i <= 3; i++)
            {
                string previousDay = currentDate.AddDays(-i).ToString("yyyy-MM-dd");
                message = message + previousDay + Environment.NewLine;
                var response = await _weatherService.GetWeather();
                foreach(DayWeather dayWeather in response.weather)
                {
                    if (previousDay == dayWeather.date)
                    {
                        message = message + "weather: " + dayWeather.weather + Environment.NewLine;
                        message = message + "watering the garden: " + dayWeather.wateringGarden + Environment.NewLine;
                        if (dayWeather.weather == "Rainy")
                        {
                            isRainOrWatering = true;
                        }
                        if (dayWeather.wateringGarden == true)
                        {
                            isRainOrWatering = true;
                        }
                    }
                
                }
            }
            if (isRainOrWatering)
            {
                message = "You don't need to water your garden today" + Environment.NewLine + message;
            }
            else {
                message = "You need to water your garden today" + Environment.NewLine + message;
            }
            return message;


        }
    }
}
