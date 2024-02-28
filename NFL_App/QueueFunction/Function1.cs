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
using Azure;
using QueueFunction.Services;


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
           
            var baseAddress = "https://sports.snoozle.net/search/nfl/searchHandler";

            var allTeamData = new List<TeamData>();

            for (int i = 1; i <= 32; i++)
            {
                string requestUri = $"{baseAddress}?fileType=inline&statType=teamStats&season=2020&teamName={i}";

                MatchUpStats teamData = await FetchTeamData(requestUri);
                log.LogInformation($"Fetched data for team {i}: {teamData}");

                if (teamData == null)
                {
                    log.LogError($"Failed to fetch data for team {i}.");
                    continue;
                }
                var teamStats = teamData.matchUpStats[0];
                int gamesWon = 0;
                int gamesLost = 0;
                try
                {
                    // Loop through the matchUpStats and count the games won and lost
                    for (int j = 0; j < teamData.matchUpStats.Count; j++)
                    {
                        if (teamData.matchUpStats[j].visStats.TeamCode == i)
                        {
                            if (teamData.matchUpStats[j].visStats.Score > teamData.matchUpStats[j].homeStats.Score)
                                gamesWon++;
                            else
                                gamesLost++;
                        }
                        else
                        {
                            if (teamData.matchUpStats[j].homeStats.TeamCode == i)
                            {
                                if (teamData.matchUpStats[j].homeStats.Score > teamData.matchUpStats[j].visStats.Score)
                                    gamesWon++;
                                else
                                    gamesLost++;
                            }
                        }
                    }
                    var team = new TeamData
                    {
                        TeamName = teamStats.visStats.TeamCode.ToString() == i.ToString() ? teamStats.visTeamName : teamStats.homeTeamName,
                        TeamCode = i.ToString(),
                        GamesWon = gamesWon,
                        GamesLost = gamesLost
                    };
                    allTeamData.Add(team);                   
                }
                catch (HttpRequestException ex)
                {
                    log.LogError($"An error occurred while fetching data for team {i}: {ex.Message}");
                }
            }
            var teamInfo = new TeamInfo
            {
                nflTeams = allTeamData
            };
            var jsonUtilities = new JsonUtilities();
            jsonUtilities.SaveJson(teamInfo, "AllTeamData");
            log.LogInformation($"Saved data for all teams: {teamInfo}");
            //return Ok(allTeamData);
        }

        private static async Task<MatchUpStats> FetchTeamData(string requestUri)
        {
            //var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(requestUri);
            MatchUpStats matchUpStats = JsonConvert.DeserializeObject<MatchUpStats>(response);
            
            if (matchUpStats != null && matchUpStats.matchUpStats.Count > 0)
                return matchUpStats;
            else
                return null;

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
