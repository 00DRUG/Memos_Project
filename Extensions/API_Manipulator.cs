using Newtonsoft.Json.Linq;

namespace Memos_Project.Extensions
{
    public class API_Manipulator
    {
        // Disable SSL certificate validation
        private static readonly HttpClientHandler _handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        static readonly Lazy<API_Manipulator> _instanceHolder =
            new Lazy<API_Manipulator>(() => new API_Manipulator(_handler));

        private readonly HttpClient _client;

        private API_Manipulator(HttpClientHandler handler)
        {
            _client = new HttpClient(handler);
        }

        public static API_Manipulator Instance => _instanceHolder.Value;

        public async Task<JObject?> GetJsonAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return JObject.Parse(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return null;
            }
        }

        public async Task<List<JObject>> GetAllPeopleAsync()
        {
            var allPeople = new List<JObject>();
            string? nextUrl = "https://swapi.dev/api/people/";

            while (!string.IsNullOrEmpty(nextUrl))
            {
                var json = await GetJsonAsync(nextUrl);
                if (json == null) break;

                var results = json["results"] as JArray;
                if (results != null)
                {
                    foreach (var person in results)
                    {
                        if (person is JObject personObj)
                        {
                            allPeople.Add(personObj);
                        }
                    }
                }

                nextUrl = json["next"]?.ToString(); // Get the url to next json page
            }

            return allPeople;
        }
        public async Task<string?> GetPlanetNameAsync(string planetUrl)
        {
            var json = await GetJsonAsync(planetUrl);
            return json["name"]?.ToString();
        }
        public async Task<List<string>> GetCharactersFromPlanetAsync(string planetName)
        {
            var peopleJson = await GetJsonAsync("https://swapi.dev/api/people/");
            List<string> matchingCharacters = new List<string>();

            foreach (var person in peopleJson["results"] ?? Enumerable.Empty<JToken>())
            {
                string? homeworldUrl = person["homeworld"]?.ToString();
                string? currentPlanetName = await GetPlanetNameAsync(homeworldUrl ?? string.Empty);

                if (currentPlanetName == planetName)
                {
                    string? characterName = person["name"]?.ToString();
                    if (!string.IsNullOrEmpty(characterName))
                    {
                        matchingCharacters.Add(characterName);
                    }
                }
            }

            return matchingCharacters;
        }
        // Additional methods for POST, PUT, DELETE can be added here  
    }
}
