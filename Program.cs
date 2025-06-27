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
            Console.WriteLine("Task 3: Create a data representation of the crew of the Enterprise");

            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    HandleDuplicateTask();
                    break;
                case "2":
                    await FindShipsWithPilotFromKashyykAsync();
                    break;
                case "3":
                    HandleCrewHierarchyTask();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

        }
    }
    static void HandleDuplicateTask()
    {
        var duplicatesService = new DuplicatesService();

        Console.WriteLine("HashSet method:\n");
        duplicatesService.PrintDuplicates_HashSet();

        Console.WriteLine("Dictionary method:\n");
        duplicatesService.PrintDuplicates_Dictionary();
    }
    public static async Task FindShipsWithPilotFromKashyykAsync()
    {
        var api = ApiService.Instance;
        const string targetPlanetName = "Kashyyyk";

        Console.WriteLine("== Measuring new function JObject return ==");
        await ApiService.MeasureFuncPerformanceAsync(async () =>
        {
            var ships = await api.GetJobject_ShipsOfPilotsFromPlanetAsync_SearchBased(targetPlanetName);

            if (ships != null)
            {
                foreach (var ship in ships)
                {
                    Console.WriteLine($"- {ship["name"]}");
                }
            }
            else
            {
                Console.WriteLine("No ships found.");
            }
        });

        Console.WriteLine("== Measuring new function String return ==");
        await ApiService.MeasureFuncPerformanceAsync(async () =>
        {
            var ships = await api.GetString_ShipsOfPilotsFromPlanetAsync_SearchBased(targetPlanetName);

            if (ships != null)
            {
                foreach (var ship in ships)
                {
                    Console.WriteLine($"- {ship}");
                }
            }
            else
            {
                Console.WriteLine("No ships found.");
            }
        });

        Console.WriteLine("== Measuring old function ==");
        await ApiService.MeasureFuncPerformanceAsync(async () =>
        {
            var people = await api.GetJSON_AllPeopleAsync();
            var resultShips = await api.GetString_ShipsOfPilotsFromPlanetAsync(people, targetPlanetName);

            foreach (var ship in resultShips)
            {
                Console.WriteLine($"- {ship}");
            }
        });
    }

    static void HandleCrewHierarchyTask()
    {
        var crewService = new CrewHierarchy();

        Console.Write("Enter crew member name: ");
        string name = Console.ReadLine()?.Trim() ?? "";
        var member = crewService.FindCrewMemberByName(crewService.Captain, name);
        if (member == null)
        {
            Console.WriteLine($" Crew member '{name}' not found.");
            return;
        }
        // Print the hierarchy of the selected crew member
        Console.WriteLine($"\n Crew hierarchy under '{member.Name}':");
        crewService.PrintCrewHierarchy(member);

        // Get all subordinates of the selected crew member as a list of CrewMember objects
        var subordinates = crewService.GetAllSubordinates(member);
        Console.WriteLine($"\n All subordinates of '{member.Name}':");
        if (subordinates.Any())
        {
            foreach (var sub in subordinates)
            {
                Console.WriteLine($"- {sub.Name}");
            }
        } 
        else
        {
            Console.WriteLine($"No subordinates found for '{member.Name}'.");
        }


        // Get the infection toward the captain for the selected crew member with the subordinates of the member 
        Console.WriteLine($"\n All infected of '{member.Name}':");
        var infected = crewService.GetAllInfected(crewService.Captain, name);
        crewService.PrintInfectedHierarchyFromCaptain(infected);
    }


}
