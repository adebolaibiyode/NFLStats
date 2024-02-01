namespace NFL_App.Models
{

    public class MatchUpStats
    {
        public List<NFLTeamStats>? matchUpStats { get; set; }
    }
    public class NFLTeamStats
    {
        public required string neutral { get; set; } = "false";
        public required string visTeamName { get; set; }
        public required GameStats visStats { get; set; }
        public required string homeTeamName { get; set; }
        public required GameStats homeStats { get; set; }
        public required string isFinal { get; set; } = "false";
        public required DateTime date { get; set; } = DateTime.MinValue;
    }

    public class GameStats
    {
        public required string StatIdCode { get; set; }
        public required string GameCode { get; set; }
        public int TeamCode { get; set; }
        public required string GameDate { get; set; }
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

}
