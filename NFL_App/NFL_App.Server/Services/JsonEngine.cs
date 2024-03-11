
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NFL_App.Models;
using System.IO;

namespace NFL_App.Server.Services
{
    // This class is used to query and manipulate JSON data
    public class JsonQueryEngine
    {
        public JToken Query(string jsonString, string query)
        {
            // Parse the JSON string into a JObject
            var jsonObject = JObject.Parse(jsonString);

            // Use SelectToken with a JSONPath query string to query the JObject
            return jsonObject.SelectToken(query);
        }
    }

    // This class is used to write JSON data to a file
    public class JsonFileWriter
    {
        // Save the JSON object for MatchupStat to a file
        public void SaveJsonToFile(MatchUpStats jsonObject, string filePath)
        {
            // Convert the object to a JSON string
            string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

            // Check directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON string to a file
            File.WriteAllText(filePath, jsonString);
        }

        // Save the JSON object for TeamInfo to a file
        public void SaveJsonToFile(TeamInfo jsonObject, string filePath)
        {
            // Convert the object to a JSON string
            string jsonString = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

            // Check directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON string to a file
            File.WriteAllText(filePath, jsonString);
        }
    }

    // This class is used to read JSON data from a file
    public class JsonFileReader
    {
        // Read the JSON data from a file
        public string ReadJsonFromFile(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file was not found.", filePath);
            }

            // Read and return the content of the file
            return File.ReadAllText(filePath);
        }
    }
}
