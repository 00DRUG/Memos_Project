using Memos_Project.Services;  

class Program
{
    static async Task Main(string[] args)
    {
        var api = ApiService.Instance;
        const string targetPlanetName = "Kashyyyk";

        // Fetch all data asynchronously
        var allPeople = await api.GetJSON_AllPeopleAsync();
        var allVehicles = await api.GetJSON_AllVehiclesAsync();
        var allPlanets = await api.GetJSON_AllPlanetsAsync();

        Console.WriteLine("First Approach:\n");
        var pilotsFromPlanet = await api.GetString_CharactersPilotsFromPlanetAsync(allPeople, allVehicles, targetPlanetName);
        foreach (var pilot in pilotsFromPlanet)
        {
            Console.WriteLine(pilot);
        }

        Console.WriteLine("\nSecond Approach (LINQ):\n");
        var pilotsFromPlanetLinq = await api.GetString_CharactersPilotsFromPlanet_LINQ(allPeople, allVehicles, allPlanets, targetPlanetName);
        foreach (var pilot in pilotsFromPlanetLinq)
        {
            Console.WriteLine(pilot);
        }
    }
}
