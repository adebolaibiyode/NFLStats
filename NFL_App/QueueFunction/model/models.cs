using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueFunction.model
{
    public class MatchUpStats
    {
        public List<NFLTeamStats>? matchUpStats { get; set; }
    }
    public class NFLTeamStats
    {
        public   string neutral { get; set; } = "false";
        public string visTeamName { get; set; }
        public GameStats visStats { get; set; }
        public  string homeTeamName { get; set; }
        public GameStats homeStats { get; set; }
        public string isFinal { get; set; } = "false";
        public DateTime date { get; set; } = DateTime.MinValue;
    }

    public class GameStats
    {
        public string StatIdCode { get; set; }
        public string GameCode { get; set; }
        public int TeamCode { get; set; }
        public string GameDate { get; set; }
        public int RushYds { get; set; }
        public int RushAtt { get; set; }
        public int PassYds { get; set; }
        public int PassAtt { get; set; }
        public int PassComp { get; set; }
        public int Penalties { get; set; }
        public int PenaltYds { get; set; }
        public int FumblesLost { get; set; }
        public int InterceptionsThrown { get; set; }
        public int FirstDowns { get; set; }
        public int ThridDownAtt { get; set; }
        public int ThirdDownConver { get; set; }
        public int FourthDownAtt { get; set; }
        public int FourthDownConver { get; set; }
        public int TimePoss { get; set; }
        public int Score { get; set; }
    }

    public class TeamStatsRequest
    {
        public  string TeamName { get; set; }
        public string? Query { get; set; }
    }

    public class TeamData
    {
        public  string TeamName { get; set; }
        public  string TeamCode { get; set; }
        public int GamesWon { get; set; } = 0;
        public int GamesLost { get; set; } = 0;
    }

    public class TeamInfo
    {
        public List<TeamData>? nflTeams { get; set; }
    }

}

