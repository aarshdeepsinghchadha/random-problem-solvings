using MathNet.Numerics.LinearAlgebra;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Checking the keys for unlocking the library...");

        Console.Write("Enter the number of keys: ");
        int numberOfKeys = int.Parse(Console.ReadLine());

        Console.Write("Enter the number of dimensions for each key: ");
        int dimensions = int.Parse(Console.ReadLine());

        var inputKeys = new List<List<double>>();

        for (int i = 0; i < numberOfKeys; i++)
        {
            Console.WriteLine($"Enter the values for key {i + 1} (comma-separated): ");
            var key = Console.ReadLine()
                             .Split(',')
                             .Select(double.Parse)
                             .ToList();

            if (key.Count != dimensions)
            {
                Console.WriteLine($"Error: The key must have {dimensions} dimensions.");
                return;
            }

            inputKeys.Add(key);
        }

        bool result = CanUnlockLibrary(inputKeys);
        Console.WriteLine($"Can unlock library: {result}");
    }

    public static bool CanUnlockLibrary(List<List<double>> keys, double tolerance = 1e-10)
    {
        if (keys == null || keys.Count == 0 || keys[0].Count == 0)
        {
            throw new ArgumentException("Keys cannot be null or empty.");
        }

        int rows = keys.Count;
        int cols = keys[0].Count;

        var keysArray = keys.Select(row => row.ToArray()).ToArray();

        var matrix = Matrix<double>.Build.DenseOfRowArrays(keysArray);

        var svd = matrix.Svd(true);

        int rank = 0;
        foreach (var value in svd.S)
        {
            if (value > tolerance)
            {
                rank++;
            }
        }

        return rank == cols;
    }
}
