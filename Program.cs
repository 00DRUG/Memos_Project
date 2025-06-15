using Memos_Project.Extensions;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;
var api = API_Manipulator.Instance;
string planetName = "Kashyyyk";
List<string> people = await api.GetString_CharactersPilotsFromPlanetAsync(planetName);
foreach (string person in people)
{
    Console.WriteLine(person);
}