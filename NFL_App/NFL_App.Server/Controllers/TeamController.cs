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
                var team = new TeamData
                {
                    TeamName = teamStats.visStats.TeamCode.ToString() == i.ToString()? teamStats.visTeamName : teamStats.homeTeamName,
                    TeamCode = i.ToString()
                };
                allTeamData.Add(team);
            
            }
            var teamInfo = new TeamInfo
            {
                nflTeams = allTeamData
            };
            var jsonUtilities = new JsonUtilities();
            jsonUtilities.SaveJson(teamInfo, "AllTeamData");
            return Ok(allTeamData);
        }

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
    }
}
