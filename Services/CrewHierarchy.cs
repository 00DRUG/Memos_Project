

namespace Memos_Project.Services
{
    public class CrewMember
    {
        public string Name { get; set; }
        public CrewMember? Commander { get; set; }
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
            riker.Commander = Captain;
            troi.Commander = Captain;
            forge.Commander = Captain;

            var mog = new CrewMember("Worf son of Mog");
            var guinan = new CrewMember("Guinan");
            var crusher = new CrewMember("Beverly Crusher");
            riker.Subordinates.AddRange(new[] { mog, guinan, crusher });
            mog.Commander = riker;
            guinan.Commander = riker;
            crusher.Commander = riker;


            var lwaxana = new CrewMember("Lwaxana Troi");
            var barkley = new CrewMember("Reginald Barkley");
            troi.Subordinates.AddRange(new[] { lwaxana, barkley });
            barkley.Commander = troi;
            lwaxana.Commander = troi;

            var data = new CrewMember("Mr. Data");
            var brien = new CrewMember("Miles O'Brien");
            forge.Subordinates.AddRange(new[] { data, brien });
            data.Commander = forge;
            brien.Commander = forge;

            var wesley = new CrewMember("Weslley Crusher");
            var ogawa = new CrewMember("Alyssa Ogawa");
            crusher.Subordinates.AddRange(new[] { wesley, ogawa });
            wesley.Commander = crusher;
            ogawa.Commander = crusher;  

            var yar = new CrewMember("Tasha Yar");
            var ehleyr = new CrewMember("K'Ehleyr");
            mog.Subordinates.AddRange(new[] { yar, ehleyr });
            yar.Commander = mog;
            ehleyr.Commander = mog;

            var rozhenko = new CrewMember("Alexander Rozhenko");
            ehleyr.Subordinates.Add(rozhenko);
            rozhenko.Commander = ehleyr;

            var bashir = new CrewMember("Julian Bashir");
            ogawa.Subordinates.Add(bashir);
            bashir.Commander = ogawa;
        }
        /// Testing function to print the crew hierarchy starting from the captain
        public void PrintCrewHierarchy(CrewMember member, int level = 0)
        {
            if (member == null) return;

            Console.WriteLine(new string('-', level) + member.Name);
            foreach (var subordinate in member.Subordinates)
            {
                PrintCrewHierarchy(subordinate, level + 1);
            }
        }
        /// Method to print the hierarchy of a crew member upwards to the captain
        public void PrintHierarchyUpwards(CrewMember member)
        {
            while (member != null)
            {
                Console.WriteLine(member.Name);
                member = member.Commander!; 
            }
        }
        /// Method to find a crew member by name in the hierarchy
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
        /// UNUSED METHOD Method to print the crew hierarchy starting from a specific crew member by name 
        public void PrintCrewHierarchyFromName(CrewMember Captain, string name)
        {
            var member = FindCrewMemberByName(Captain, name);
            if (member != null)
            {
                PrintCrewHierarchy(member);
            }
            else
            {
                Console.WriteLine($"Crew member '{name}' not found.");
            }
        }
        /// Method to get a list of infected crew members until the captain
        public List<string> GetInfectedUntilCaptain(CrewMember Captain, string name)
        {
            var infected = new List<string>();
            var member = FindCrewMemberByName(Captain, name);

            if (member == null)
                return infected;

            while (member != null)
            {
                infected.Add(member.Name);

                if (member.Name == Captain.Name)
                    break;

                member = member.Commander;
            }

            return infected;
        }
        /// Method to get all subordinates of a crew member recursively
        public List<CrewMember> GetAllSubordinates(CrewMember member)
        {
            var result = new List<CrewMember>();

            foreach (var subordinate in member.Subordinates)
            {
                result.Add(subordinate);
                result.AddRange(GetAllSubordinates(subordinate));
            }

            return result;
        }
        // New approach that subordinates can be infected as well 
        private void SpreadInfectionUpwards(CrewMember? member, HashSet<CrewMember> infected)
        {
            while (member != null && infected.Add(member))
            {
                member = member.Commander;
            }
        }
        private void SpreadInfectionDownwards(CrewMember member, HashSet<CrewMember> infected)
        {
            foreach (var subordinate in member.Subordinates)
            {
                if (infected.Add(subordinate))
                {
                    SpreadInfectionDownwards(subordinate, infected);
                }
            }
        }
        public List<CrewMember> GetAllInfected(CrewMember captain, string infectedName)
        {
            var infectedSet = new HashSet<CrewMember>();
            var start = FindCrewMemberByName(captain, infectedName);
            if (start == null)
                return infectedSet.ToList();

            SpreadInfectionUpwards(start, infectedSet);
            SpreadInfectionDownwards(start, infectedSet);

            return infectedSet.ToList();
        }
        public void PrintInfectedHierarchyFromCaptain(List<CrewMember> infected)
        {
            PrintInfectedHierarchyRecursive(Captain, infected, 0);
        }

        private void PrintInfectedHierarchyRecursive(CrewMember member, List<CrewMember> infected, int level)
        {
            if (member == null || !infected.Contains(member))
                return;

            Console.WriteLine(new string('-', level) + member.Name);

            foreach (var subordinate in member.Subordinates)
            {
                PrintInfectedHierarchyRecursive(subordinate, infected, level + 1);
            }
        }


    }
}
