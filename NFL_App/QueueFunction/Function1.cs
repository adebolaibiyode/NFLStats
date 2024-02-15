using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using QueueFunction.model;
using Microsoft.Azure.WebJobs.Extensions;   


namespace QueueFunction
{
    public static class FetchTeamDataFunction
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("FetchTeamData")]
        public static async Task Run(
            [ServiceBusTrigger("FetchAllTeamsData", Connection = "ServiceBusConnection")]
           string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"Processing queue item: {myQueueItem}");

            // Assuming the queue item might be a simple command or could contain specific parameters
            // For simplicity, let's say myQueueItem could be just a trigger to start the process
            var baseAddress = "https://sports.snoozle.net/search/nfl/searchHandler";

            var allTeamData = new List<TeamData>();

            for (int i = 1; i <= 32; i++)
            {
                string requestUri = $"{baseAddress}?fileType=inline&statType=teamStats&season=2020&teamName={i}";
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var teamData = JsonConvert.DeserializeObject(json); // Assuming deserialization to a known type

                        // Process the fetched data (e.g., store it in a database or further processing)
                        // For demonstration, we'll log the fetched data
                        log.LogInformation($"Fetched data for team {i}: {json}");

                        // Here you would add your logic to store the fetched data
                        // For example, saving it to a database or sending it to another queue for further processing
                    }
                    else
                    {
                        log.LogError($"Failed to fetch data for team {i}. HTTP status: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    log.LogError($"An error occurred while fetching data for team {i}: {ex.Message}");
                }
            }
        }
    }
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
