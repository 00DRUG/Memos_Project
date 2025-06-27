using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;

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
        public async Task<List<string>> GetString_PilotsFromPlanetAsync(List<JObject> peopleJson, List<JObject> VehiclesJson, string planetName)
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
        public async Task<List<string>> GetString_PilotsFromPlanet_LINQ(
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


        // Search parameter approach
        // Method with search parameter to get the URL of a planet by its name
        public async Task<string?> GetPlanetUrlByNameAsync_SearchBased(string planetName)
        {
            string url = $"https://swapi.dev/api/planets/?search={planetName}";
            JObject? json = await GetJsonAsync(url);

            var results = json?["results"] as JArray;
            if (results != null)
            {
                foreach (var planet in results)
                {
                    if (planet?["name"]?.ToString().Equals(planetName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return planet["url"]?.ToString(); 
                    }
                }
            }

            return null;
        }
        // Method to get ships and vehicles with pilots from a specific planet
        public async Task<List<JObject>?> GetJobject_ShipsOfPilotsFromPlanetAsync_SearchBased(string planetName)
        {
            var resultShips = new HashSet<JObject>();
            //var visitedPilotUrls = new HashSet<string>(); Comment because pilot can have multiple ships or/and vehicles


            string? planetUrl = await GetPlanetUrlByNameAsync_SearchBased(planetName);
            if (planetUrl == null)
            {
                Console.WriteLine($"Planet '{planetName}' not found.");
                return null;
            }
            var allShips = (await GetJSON_AllVehiclesAsync()).Concat(await GetJSON_AllStarshipsAsync());

            foreach (var ship in allShips)
            {
                var pilots = ship["pilots"] as JArray;
                if (pilots == null || pilots.Count == 0)
                    continue;

                foreach (var pilotUrlToken in pilots)
                {
                    string? pilotUrl = pilotUrlToken?.ToString();
                    /*if (string.IsNullOrEmpty(pilotUrl) || visitedPilotUrls.Contains(pilotUrl))
                        continue;

                    visitedPilotUrls.Add(pilotUrl);
                    */ // Comment because pilot can have multiple ships or/and vehicles
                    var pilotJson = await GetJsonAsync(pilotUrl);
                    if (pilotJson == null) continue;

                    string? homeworldUrl = pilotJson["homeworld"]?.ToString();
                    if (homeworldUrl == planetUrl)
                    {
                        resultShips.Add(ship);
                        break; //Break here for efficiency, as we only need one pilot from the exact planet
                    }
                }
            }

            return resultShips.ToList();
        }

        public static async Task MeasureFuncPerformanceAsync(Func<Task> targetFunc)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long beforeMemory = GC.GetTotalMemory(true);
            var stopwatch = Stopwatch.StartNew();

            await targetFunc();

            stopwatch.Stop();
            long afterMemory = GC.GetTotalMemory(true);
            long memoryUsed = afterMemory - beforeMemory;

            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Memory used: {memoryUsed / 1024.0:F2} KB");
        }

    }
}
