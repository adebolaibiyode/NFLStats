using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NFL_App.Models;

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
                    return Ok(matchUpStats);
                }
                return NotFound();
            }
            catch (HttpRequestException e)
            {
                // Log the exception (implementation depends on the logging framework being used)
                return StatusCode(500, "Error accessing the NFL stats service");
            }
        }
    }
}
