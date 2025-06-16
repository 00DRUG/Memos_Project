
namespace Memos_Project.Services
{
    public class DuplicatesService
    {
        const int size = 1000000;
        int[] array { get; set; } = new int[size];


        public DuplicatesService()
        {
            FillArray();
        }
        public void FillArray()
        {
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                array[i] = random.Next(1, 100); // Range can be different 
            }
        }
        /// Method to find duplicates in the array using HashSet
        public List<int> GetDuplicates_HashSet()
        {
            HashSet<int> duplicates = new HashSet<int>();
            HashSet<int> seen = new HashSet<int>();
            foreach (var number in array)
            {
                if (!seen.Add(number))
                {
                    duplicates.Add(number);
                }
            }
            return duplicates.ToList();
        }
        public void PrintDuplicates_HashSet()
        {
            var duplicates = GetDuplicates_HashSet();
            if (duplicates.Count == 0)
            {
                Console.WriteLine("No duplicates found.");
            }
            else
            {
                Console.WriteLine("Duplicates found:");
                foreach (var duplicate in duplicates)
                {
                    Console.Write(duplicate + " ");
                }
            }
            Console.WriteLine();

        }
        /// Method to find duplicates in the array using Dictionary
        public Dictionary<int, int> GetDuplicates_Dictionary()
        {
            Dictionary<int, int> counts = new Dictionary<int, int>();

            foreach (int number in array)
            {
                if (counts.ContainsKey(number))
                    counts[number]++;
                else
                    counts[number] = 1;
            }
            return counts.Where(kvp => kvp.Value > 1)
                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public void PrintDuplicates_Dictionary()
        {
            var duplicates = GetDuplicates_Dictionary();
            if (duplicates.Count == 0)
            {
                Console.WriteLine("No duplicates found.");
            }
            else
            {
                Console.WriteLine("Duplicates found:");
                foreach (var kvp in duplicates)
                {
                    Console.WriteLine($"Value: {kvp.Key}, Count: {kvp.Value}");
                }
            }
        }
    }
}
