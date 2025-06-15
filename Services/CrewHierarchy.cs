using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memos_Project.Services
{
    public class CrewMember
    {
        public string Name { get; set; }
        public List<CrewMember> Subordinates { get; set; } = new List<CrewMember>();
        public CrewMember(string name)
        {
            Name = name;
        }
    }
    public class CrewHierarchy
    {
        public CrewMember Captain { get; set; }
        public CrewHierarchy()
        {
            Captain = new CrewMember("Jean Luc Pickard");

            var riker = new CrewMember("William Riker");
            var troi = new CrewMember("Deana Troi");
            var forge = new CrewMember("Jordi La Forge");
            Captain.Subordinates.AddRange(new[] { riker, troi, forge });

            var mog = new CrewMember("Worf son of Mog");
            var guinan = new CrewMember("Guinan");
            var crusher = new CrewMember("Beverly Crusher");
            riker.Subordinates.AddRange(new[] { mog, guinan, crusher });
            

            var lwaxana = new CrewMember("Lwaxana Troi");
            var barkley = new CrewMember("Reginald Barkley");
            troi.Subordinates.AddRange(new[] { lwaxana, barkley });


            var data = new CrewMember("Mr. Data");
            var brien = new CrewMember("Miles O'Brien");
            forge.Subordinates.AddRange(new[] { data, brien });

            var wesley = new CrewMember("Weslley Crusher");
            var ogawa = new CrewMember("Alyssa Ogawa");
            crusher.Subordinates.AddRange(new[] { wesley, ogawa });

            var yar = new CrewMember("Tasha Yar");
            var ehleyr = new CrewMember("K'Ehleyr");
            mog.Subordinates.AddRange(new[] { yar, ehleyr });

            var rozhenko = new CrewMember("Alexander Rozhenko");
            ehleyr.Subordinates.Add(rozhenko);

            var bashir = new CrewMember("Julian Bashir");
            ogawa.Subordinates.Add(bashir);
        }
        public void PrintCrewHierarchy(CrewMember member, int level = 0)
        {
            if (member == null) return;

            Console.WriteLine(new string('-', level) + member.Name);
            foreach (var subordinate in member.Subordinates)
            {
                PrintCrewHierarchy(subordinate, level + 1);
            }
        }
        public CrewMember? FindCrewMemberByName(CrewMember root, string name)
        {
            if (root.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return root;

            foreach (var sub in root.Subordinates)
            {
                var result = FindCrewMemberByName(sub, name);
                if (result != null)
                    return result;
            }

            return null;
        }
        public void PrintCrewHierarchyFromName(CrewMember root, string name)
        {
            var member = FindCrewMemberByName(root, name);
            if (member != null)
            {
                PrintCrewHierarchy(member);
            }
            else
            {
                Console.WriteLine($"Crew member '{name}' not found.");
            }
        }


    }
}
