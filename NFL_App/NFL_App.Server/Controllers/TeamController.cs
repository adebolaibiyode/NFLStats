using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NFL_App.Models;
using NFL_App.Server.Services;

namespace NFL_App.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TeamController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET: api/Team/FetchAllTeamsData
        [HttpGet("FetchAllTeamsData")]
        public async Task<IActionResult> FetchAllTeamsData()
        {
            var baseAddress = "https://sports.snoozle.net/search/nfl/searchHandler";
          
            var allTeamData = new List<TeamData>();

            for(int i = 1; i < 33; i++)
            {
                string requestUri = $"{baseAddress}?fileType=inline&statType=teamStats&season=2020&teamName={i}";
                MatchUpStats teamData = await FetchTeamData(requestUri);
                if (teamData == null)
                    continue;
                var teamStats = teamData.matchUpStats[0];
                int gamesWon = 0;
                int gamesLost = 0;

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
                    TeamName = teamStats.visStats.TeamCode.ToString() == i.ToString()? teamStats.visTeamName : teamStats.homeTeamName,
                    TeamCode = i.ToString(),
                    GamesWon = gamesWon,
                    GamesLost = gamesLost                  
                };
                allTeamData.Add(team);
            
            }
            var teamInfo = new TeamInfo
            {
                nflTeams = allTeamData
            };
            var jsonUtilities = new JsonUtilities();
            jsonUtilities.SaveJson(teamInfo, "AllTeamData");
            FetchAllTeamsDataBus();
            return Ok(allTeamData);
        }

        // GET: api/Team/FetchTeamData -- Fetch team data method.
        private async Task<MatchUpStats> FetchTeamData(string requestUri)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(requestUri);
            MatchUpStats matchUpStats = JsonConvert.DeserializeObject<MatchUpStats>(response);
            var jsonUtilities = new JsonUtilities();
            if (matchUpStats != null && matchUpStats.matchUpStats.Count > 0)
                return matchUpStats;
            else
                return null;
            
        }

        // GET: api/Team/FetchAllTeamsDataBus -- Fetch all teams data method.
        [HttpGet("FetchAllTeamsDataBus")]
        public async Task<IActionResult> FetchAllTeamsDataBus()
        {
            var teamFetchRequest = new { RequestType = "" };
            var queueName ="FetchAllTeamsData";
            var queueId = Guid.NewGuid().ToString();
            var AzureServiceBusConnectionString = "Endpoint=sb://devserviceb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=3TpDtskSRdwF7YqqDQAL4oZRO6PY/4XCM+ASbIsD/7Y=";
            var serviceBusPublisher = new AzureServiceBusPublisher(
                AzureServiceBusConnectionString,
                queueName);

            await serviceBusPublisher.SendMessageAsync(teamFetchRequest);

            return Accepted("Request to fetch all teams data has been queued.");
        }

    }
}
