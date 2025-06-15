using Memos_Project.Services;  

class Program
{
    static async Task Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Menu");
            Console.WriteLine("Task 1: Find duplicates in 10000000 int array");
            Console.WriteLine("Task 2: Find ships with pilot from Kashyyk");
            Console.WriteLine("Task 3: ");

            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    var duplicatesService = new DuplicatesService();
                    duplicatesService.PrintDuplicates();
                    break;
                case "2":
                    await FindShipsWithPilotFromKashyykAsync();
                    break;
                case "3":
                    var service = new CrewHierarchy();

                    Console.Write("Enter crew member name: ");
                    string name = Console.ReadLine() ?? "";

                    service.PrintCrewHierarchyFromName(service.Captain, name);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

        }
    }

    public static async Task FindShipsWithPilotFromKashyykAsync()
    {
        var api = ApiService.Instance;
        const string targetPlanetName = "Kashyyyk";
        // Fetch all data asynchronously  
        var allPeople = await api.GetJSON_AllPeopleAsync();
        var allVehicles = await api.GetJSON_AllVehiclesAsync();
        var allPlanets = await api.GetJSON_AllPlanetsAsync();
        Console.WriteLine("All ships whose pilot is from the planet Kashyyyk :\n");
        var pilotsFromPlanet1 = await api.GetString_ShipsOfPilotsFromPlanetAsync(allPeople, targetPlanetName);
        foreach (var pilot in pilotsFromPlanet1)
        {
            Console.WriteLine(pilot);
        }
    }


}
