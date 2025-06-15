using Memos_Project.Extensions;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;
var api = API_Manipulator.Instance;
string planetName = "Kashyyyk";
List<string> people1 = await api.GetString_CharactersPilotsFromPlanetAsync(planetName);
foreach (string person in people1)
{
    Console.WriteLine(person);
}

List<string> people2 = await api.GetString_CharactersPilotsFromPlanet_LINQ();