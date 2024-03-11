using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QueueFunction.model;
using System;
using System.IO;

namespace QueueFunction.Services
{
    public class JsonUtilities
    {
        // This method reads a JSON file and returns the JSON string
        public MatchUpStats GetNFLStats(string jsonString)
        {
            // Parse the JSON string into a JObject
            var jsonObject = JObject.Parse(jsonString);

            // Use SelectToken with a JSONPath query string to query the JObject
            var teamStats = jsonObject.SelectToken("teamStats");
            var teamStatsJson = teamStats.ToString();
            var teamStatsObject = JsonConvert.DeserializeObject<MatchUpStats>(teamStatsJson);
            return teamStatsObject;
        }

        // This method reads a JSON file and returns the JSON string
        public NFLTeamStats GetTeamStats(string jsonString, string query)
        {
            // Parse the JSON string into a JObject
            var jsonObject = JObject.Parse(jsonString);

            // Use SelectToken with a JSONPath query string to query the JObject
            var teamStats = jsonObject.SelectToken(query);
            var teamStatsJson = teamStats.ToString();
            var teamStatsObject = JsonConvert.DeserializeObject<NFLTeamStats>(teamStatsJson);
            return teamStatsObject;
        }

        // This method reads a JSON file and returns the JSON string
        public static NFLTeamStats GetTeamStats(string jsonString, string query, string query2)
        {
            // Parse the JSON string into a JObject
            var jsonObject = JObject.Parse(jsonString);

            // Use SelectToken with a JSONPath query string to query the JObject
            var teamStats = jsonObject.SelectToken(query);
            var teamStatsJson = teamStats.ToString();
            var teamStatsObject = JsonConvert.DeserializeObject<NFLTeamStats>(teamStatsJson);
            return teamStatsObject;
        }

        // This method reads a JSON file and returns the JSON string
        public void SaveJson(MatchUpStats jsonObject, string filename)
        {
            var jsonFileWriter = new JsonFileWriter();
            jsonFileWriter.SaveJsonToFile(jsonObject, Path.Combine("..", "Data", filename + ".json"));

        }

        // This method reads parsed JSON file and returns the JSON string
        public void SaveJson(TeamInfo jsonObject, string filename)
        {
            var jsonFileWriter = new JsonFileWriter();
            jsonFileWriter.SaveJsonToFile(jsonObject, Path.Combine("..", "Data", filename + ".json"));

        }
        // This method reads a JSON file and returns the JSON string
        public string ReadJsonString(string filename)
        {
            var jsonFileReader = new JsonFileReader();
            try
            {
                return jsonFileReader.ReadJsonFromFile(Path.Combine("..", "Data", filename));
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return null; 
            }
        }
    }
}

