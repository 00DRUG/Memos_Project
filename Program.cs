using Memos_Project.Extensions;
using Newtonsoft.Json.Linq;
using System.IO.Pipes;
var api = API_Manipulator.Instance;

List<JObject> people = await api.GetAllPeopleAsync();
