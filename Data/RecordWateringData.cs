using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;

namespace WaterMyGarden.Data
{
    public interface IRecordWateringData
    {
        Task<string> AddWateringData(RecordWateringRequest recordWateringRequest);
        Task<string> DeleteDate(RecordWateringRequest recordWateringRequest);

        Task<List<string>> ShowAllDates();
    }
    public class RecordWateringData : IRecordWateringData
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _tableName = "Watering_My_Garden";

        public RecordWateringData()
        {
            var awsCredentials = new BasicAWSCredentials("AKIA4T4OCILUI2NQVOMT", "iLmmKfz4PEVIsDx7lR0e54ZsS2LjvAkCrWOu2C+2");
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.APSoutheast2  // Set your AWS region
            };

            _dynamoDbClient = new AmazonDynamoDBClient(awsCredentials, config);
        }

        public async Task<string> AddWateringData(RecordWateringRequest recordWateringRequest)
        {
            try
            {
                var item = new Dictionary<string, AttributeValue>
                {
                    { "date", new AttributeValue { S = recordWateringRequest.date } } // Partition key is "date"
                };

                var request = new PutItemRequest
                {
                    TableName = _tableName,
                    Item = item
                };

                await _dynamoDbClient.PutItemAsync(request);
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding date: " + ex.Message);
                return "Fail";
            }
        }

        public async Task<string> DeleteDate(RecordWateringRequest recordWateringRequest)
        {
            try
            {
                var request = new DeleteItemRequest
                {
                    TableName = _tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "date", new AttributeValue { S = recordWateringRequest.date } } // Partition key is "date"
                    }
                };

                await _dynamoDbClient.DeleteItemAsync(request);
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting date: " + ex.Message);
                return "Fail";
            }
        }

        public async Task<List<string>> ShowAllDates()
        {
            try
            {
                var request = new ScanRequest
                {
                    TableName = _tableName
                };

                var response = await _dynamoDbClient.ScanAsync(request);

                Console.WriteLine("Dates in DynamoDB:");
                List<string> wateringDate = new List<string>();
                foreach (var item in response.Items)
                {
                    wateringDate.Add(item["date"].S);
                    Console.WriteLine(item["date"].S); // Assuming the partition key is "date"
                }
                return wateringDate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching data: " + ex.Message);
                return new List<string>();
            }
        }
    }
}
