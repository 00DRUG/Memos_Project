using Newtonsoft.Json.Linq;

namespace Memos_Project.Services
{
    public class ApiService
    {
        // Disable SSL certificate validation
        private static readonly HttpClientHandler _handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };

        static readonly Lazy<ApiService> _instanceHolder =
            new Lazy<ApiService>(() => new ApiService(_handler));

        private readonly HttpClient _client;

        private ApiService(HttpClientHandler handler)
        {
            _client = new HttpClient(handler);
        }

        public static ApiService Instance => _instanceHolder.Value;

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

        public async Task<List<JObject>> GetJSON_AllPeopleAsync()
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
        //Dictionary<string, string> is used to store planet URLs and their names  - extra function for the LINQ approach 
        public async Task<Dictionary<string, string>> GetJSON_AllPlanetsAsync()
        {
            var planetsDict = new Dictionary<string, string>();
            string? nextUrl = "https://swapi.dev/api/planets/";

            while (!string.IsNullOrEmpty(nextUrl))
            {
                var json = await GetJsonAsync(nextUrl);
                if (json == null) break;
                foreach (var planet in json["results"] ?? Enumerable.Empty<JToken>())
                {
                    if (planet is JObject planetObj)
                    {
                        string? name = planetObj["name"]?.ToString();
                        string? url = planetObj["url"]?.ToString();
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                        {
                            planetsDict[url] = name;
                        }
                    }
                }

                nextUrl = json["next"]?.ToString(); // Get the url to next json page
            }

            return planetsDict;
        }
        public async Task<List<JObject>> GetJSON_AllVehiclesAsync()
        {
            var allVehicles = new List<JObject>();
            string? nextUrl = "https://swapi.dev/api/vehicles/";

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
                            allVehicles.Add(personObj);
                        }
                    }
                }

                nextUrl = json["next"]?.ToString(); // Get the url to next json page
            }

            return allVehicles;
        }
        public async Task<List<JObject>> GetJSON_AllStarshipsAsync()
        {
            var allStarships = new List<JObject>();
            string? nextUrl = "https://swapi.dev/api/starships/";

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
                            allStarships.Add(personObj);
                        }
                    }
                }

                nextUrl = json["next"]?.ToString(); // Get the url to next json page
            }

            return allStarships;
        }
        // Function to get the name of a planet from its URL
        public async Task<string?> GetPlanetNameAsync(string planetUrl)
        {
            var json = await GetJsonAsync(planetUrl);
            return json["name"]?.ToString();
        }
        // Function to get the name of a ship from its URL
        public async Task<string?> GetShipNameAsync(string shipUrl)
        {
            var json = await GetJsonAsync(shipUrl);
            return json["name"]?.ToString();
        }
        // Additional function to search people by their homeworld planet name
        public async Task<List<string>> GetString_CharactersFromPlanetAsync(List<JObject> peopleJson, string planetName)
        {
            List<string> matchingCharacters = new List<string>();

            foreach (var person in peopleJson ?? Enumerable.Empty<JToken>())
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
        // Function to get characters who are pilots of vehicles from a specific planet
        public async Task<List<string>> GetString_CharactersPilotsFromPlanetAsync(List<JObject> peopleJson, List<JObject> VehiclesJson, string planetName)
        {
            List<string> matchingCharacters = new List<string>();
            foreach (var vehicle in VehiclesJson ?? Enumerable.Empty<JToken>())
            {
                var pilotsArray = vehicle["pilots"] as JArray;
                if (pilotsArray == null || pilotsArray.Count == 0)
                    continue;
                foreach (var pilotUrlToken in pilotsArray)
                {
                    string? pilotUrl = pilotUrlToken?.ToString();
                    if (string.IsNullOrEmpty(pilotUrl))
                        continue;
                    var pilot = peopleJson.FirstOrDefault(p => p["url"]?.ToString() == pilotUrl);
                    if (pilot == null)
                        continue;
                    string? homeworldUrl = pilot["homeworld"]?.ToString();
                    string? currentPlanetName = await GetPlanetNameAsync(homeworldUrl ?? string.Empty);
                    if (currentPlanetName == planetName)
                    {
                        string? characterName = pilot["name"]?.ToString();
                        if (!string.IsNullOrEmpty(characterName))
                        {
                            matchingCharacters.Add(characterName);
                        }
                    }
                }
            }
            return matchingCharacters;

        }
        // Function to get characters who are pilots of vehicles from a specific planet using LINQ
        public async Task<List<string>> GetString_CharactersPilotsFromPlanet_LINQ(
    List<JObject> people,
    List<JObject> vehicles,
    Dictionary<string, string> planetUrlToName,
    string targetPlanetName)
        {
            var pilotUrls = vehicles
                .AsParallel()
                .SelectMany(v => v["pilots"] ?? Enumerable.Empty<JToken>())
                .Select(p => p.ToString())
                .Distinct()
                .ToHashSet();

            var result = people
                .AsParallel()
                .Where(p => pilotUrls.Contains(p["url"]?.ToString()))
                .Where(p =>
                {
                    var homeworldUrl = p["homeworld"]?.ToString();
                    return homeworldUrl != null &&
                           planetUrlToName.TryGetValue(homeworldUrl, out var name) &&
                           name == targetPlanetName;
                })
                .Select(p => p["name"]?.ToString())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            return result;
        }
        public async Task<List<string>> GetString_ShipsOfPilotsFromPlanetAsync(List<JObject> peopleJson, string planetName)
        {
            var shipNames = new HashSet<string>();

            foreach (var person in peopleJson ?? Enumerable.Empty<JObject>())
            {
                string? homeworldUrl = person["homeworld"]?.ToString();
                string? currentPlanetName = await GetPlanetNameAsync(homeworldUrl ?? string.Empty);

                if (currentPlanetName == planetName)
                {
                    var vehicleUrls = person["vehicles"]?.ToObject<List<string>>() ?? new List<string>();
                    var starshipUrls = person["starships"]?.ToObject<List<string>>() ?? new List<string>();
                    var allShipUrls = vehicleUrls.Concat(starshipUrls);

                    foreach (var shipUrl in allShipUrls)
                    {
                        string? shipName = await GetShipNameAsync(shipUrl);
                        if (!string.IsNullOrEmpty(shipName))
                        {
                            shipNames.Add(shipName);
                        }
                    }
                }
            }

            return shipNames.ToList();
            
        }
        // Additional methods for POST, PUT, DELETE can be added here  
    }
}
