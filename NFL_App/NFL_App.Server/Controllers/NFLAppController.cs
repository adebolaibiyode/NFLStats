using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NFL_App.Models;
using NFL_App.Server.MessageQueue;
using NFL_App.Server.Services;

namespace NFL_App.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NFLAppController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public NFLAppController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;            
        }

        [HttpGet("nflteamstat/{teamName}")]
        public async Task<ActionResult<NFLTeamStats>> GetTeamStats(int teamName)
        {
            string requestUri = $"https://sports.snoozle.net/search/nfl/searchHandler?fileType=inline&statType=teamStats&season=2020&teamName={teamName}";

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    MatchUpStats matchUpStats = JsonConvert.DeserializeObject<MatchUpStats>(json);
                    var jsonUtilities = new JsonUtilities();
                    if(matchUpStats !=null && matchUpStats.matchUpStats.Count > 0)
                        jsonUtilities.SaveJson(matchUpStats,"TeamData" + teamName.ToString() );
                    return Ok(matchUpStats);
                }
                return NotFound();
            }
            catch (HttpRequestException e)
            {
                // Log the exception 
                return StatusCode(500, "Error accessing the NFL stats service");
            }
        }

        [HttpGet("getteamstat/{teamName}")]
        public async Task<ActionResult<NFLTeamStats>> QTeamStats(string teamName)
        {
            try
            {
                var jsonUtilities = new JsonUtilities();
                string json = jsonUtilities.ReadJsonString("TeamData" + teamName.ToString() + ".json");
                string query = "$.matchUpStats[?(@.homeStats.GameDate == 'Sep 13, 2020')]";
                NFLTeamStats teamstats = jsonUtilities.GetTeamStats(json, query);
                //MatchUpStats matchUpStats = jsonUtilities.GetNFLStats(json);
                if (teamstats != null)
                    return Ok(teamstats);

                return NotFound();
            }
            catch (HttpRequestException e)
            {
                // Log the exception (implementation depends on the logging framework being used)
                return StatusCode(500, "Error accessing the NFL stats service");
            }
        }

        [HttpPost("getteamstat")]
        public async Task<ActionResult<NFLTeamStats>> QueryTeamStats([FromBody] TeamStatsRequest request)
        {
            try
            {
                var jsonUtilities = new JsonUtilities();
                string json = jsonUtilities.ReadJsonString($"TeamData{request.TeamName}.json");   
                var gameDate = DateTime.Parse(request.Query);
                string query = $"$.matchUpStats[?(@.homeStats.GameDate == '{gameDate.ToString("MMM dd, yyyy")}')]";
                NFLTeamStats teamStats = jsonUtilities.GetTeamStats(json, query);
                MatchUpStats matchUpStats = new MatchUpStats();
                List<NFLTeamStats> nFLTeamStats = new List<NFLTeamStats>();
                nFLTeamStats.Add(teamStats);
                matchUpStats.matchUpStats = nFLTeamStats;

                if (teamStats != null)
                    return Ok(matchUpStats);
                else
                    return NotFound("Team stats not found.");
            }
            catch (Exception e)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("fetchTeamStats")]
        public async Task<ActionResult> FetchTeamStats([FromBody] TeamStatsRequest request)
        {
            var serviceBusManager = new ServiceBusManager();
            await serviceBusManager.SendMessageAsync(request);

            return Accepted(); // Acknowledge that the request has been received and will be processed
        }

    }
}
