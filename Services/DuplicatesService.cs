
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
                array[i] = random.Next(1, 5); // Range can be different 
            }
        }
        public List<int> GetDuplicates()
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
        public void PrintDuplicates()
        {
            var duplicates = GetDuplicates();
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
    }
}
