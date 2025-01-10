using MathNet.Numerics.LinearAlgebra;

class ProgramV1
{
    static void Mainv1(string[] args)
    {
        Console.WriteLine("Checking the keys for unlocking the library...");

        var inputKeys1 = new List<List<double>>
        {
            new List<double> { 1.0, 0.0, 0.0 },
            new List<double> { 0.0, 1.0, 0.0 },
            new List<double> { 0.0, 0.0, 1.0 }
        };
        bool result1 = CanUnlockLibrary(inputKeys1);
        Console.WriteLine($"Test Case 1: Can unlock library: {result1}");

        var inputKeys2 = new List<List<double>>
        {
            new List<double> { 2, 0, 0 },
            new List<double> { 0, 2, 0 },
            new List<double> { 4, 4, 0 }
        };
        bool result2 = CanUnlockLibrary(inputKeys2);
        Console.WriteLine($"Test Case 2: Can unlock library: {result2}");
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
