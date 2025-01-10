public static class MatrixExtensions
{
    public static double[] GetColumn(this double[,] matrix, int columnIndex)
    {
        int rows = matrix.GetLength(0);
        double[] column = new double[rows];
        for (int i = 0; i < rows; i++)
        {
            column[i] = matrix[i, columnIndex];
        }
        return column;
    }
}

class Program
{
    static void Main()
    {
        string dataFilePath = Path.Combine("Data", "data.csv");
        string creatorsFilePath = Path.Combine("Data", "creators.csv");

        List<double[]> historicalData = ReadData(dataFilePath);

        List<double[]> XTech = historicalData.Select(row => new double[] { 1, row[0], row[1] }).ToList();
        List<double> YTech = historicalData.Select(row => row[2]).ToList();
        List<double[]> XEnt = historicalData.Select(row => new double[] { 1, row[0], row[1] }).ToList();
        List<double> YEnt = historicalData.Select(row => row[3]).ToList();

        double[] techCoefficients = LinearRegression(XTech, YTech);
        double[] entCoefficients = LinearRegression(XEnt, YEnt);

        List<double[]> creators = ReadData(creatorsFilePath);

        List<double[]> predictions = SimulateEvolution(creators, techCoefficients, entCoefficients, weeks: 4);

        int maxTechIndex = GetMaxIndex(predictions, 0);
        Console.WriteLine($"Creator with highest technical depth after 4 weeks: {maxTechIndex}");

        int maxEntIndex = GetMaxIndex(predictions, 1);
        Console.WriteLine($"Creator with highest entertainment value after 4 weeks: {maxEntIndex}");

        var switches = IdentifyFocusSwitches(creators, predictions);
        Console.WriteLine("Creators switching from tech to entertainment: " + string.Join(", ", switches.TechToEnt));
        Console.WriteLine("Creators switching from entertainment to tech: " + string.Join(", ", switches.EntToTech));
    }

    static List<double[]> ReadData(string filePath)
    {
        return File.ReadAllLines(filePath)
                   .Select(line => line.Split(',').Select(double.Parse).ToArray())
                   .ToList();
    }

    static double[] LinearRegression(List<double[]> X, List<double> Y)
    {
        int n = X.Count;
        int m = X[0].Length;

        double[,] designMatrix = new double[n, m];
        double[] targetVector = new double[n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                designMatrix[i, j] = X[i][j];
            }
            targetVector[i] = Y[i];
        }

        double[,] Xt = Transpose(designMatrix);
        double[,] XtX = MatrixMultiply(Xt, designMatrix);
        double[,] XtXInv = MatrixInverse(XtX);
        double[,] XtY = MatrixMultiply(Xt, ToColumnVector(targetVector));
        double[,] coefficientsMatrix = MatrixMultiply(XtXInv, XtY);

        return coefficientsMatrix.GetColumn(0);
    }

    static List<double[]> SimulateEvolution(List<double[]> creators, double[] techCoefficients, double[] entCoefficients, int weeks)
    {
        List<double[]> finalStates = new List<double[]>();
        foreach (var creator in creators)
        {
            double tech = creator[0];
            double ent = creator[1];
            for (int week = 0; week < weeks; week++)
            {
                double nextTech = techCoefficients[0] + techCoefficients[1] * tech + techCoefficients[2] * ent;
                double nextEnt = entCoefficients[0] + entCoefficients[1] * tech + entCoefficients[2] * ent;
                tech = nextTech;
                ent = nextEnt;
            }
            finalStates.Add(new double[] { tech, ent });
        }
        return finalStates;
    }

    static int GetMaxIndex(List<double[]> data, int index)
    {
        return data.Select((value, idx) => new { Value = value[index], Index = idx })
                   .OrderByDescending(x => x.Value)
                   .FirstOrDefault()?.Index ?? -1;
    }

    static (List<int> TechToEnt, List<int> EntToTech) IdentifyFocusSwitches(List<double[]> initial, List<double[]> final)
    {
        List<int> techToEnt = new List<int>();
        List<int> entToTech = new List<int>();

        for (int i = 0; i < initial.Count; i++)
        {
            bool initialTechFocused = initial[i][0] > initial[i][1];
            bool finalTechFocused = final[i][0] > final[i][1];

            if (initialTechFocused && !finalTechFocused)
                techToEnt.Add(i);
            else if (!initialTechFocused && finalTechFocused)
                entToTech.Add(i);
        }

        return (techToEnt, entToTech);
    }

    static double[,] Transpose(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        double[,] transposed = new double[cols, rows];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                transposed[j, i] = matrix[i, j];
        return transposed;
    }

    static double[,] MatrixMultiply(double[,] a, double[,] b)
    {
        int aRows = a.GetLength(0);
        int aCols = a.GetLength(1);
        int bRows = b.GetLength(0);
        int bCols = b.GetLength(1);

        if (aCols != bRows)
            throw new ArgumentException("Incompatible matrix dimensions.");

        double[,] result = new double[aRows, bCols];
        for (int i = 0; i < aRows; i++)
            for (int j = 0; j < bCols; j++)
                for (int k = 0; k < aCols; k++)
                    result[i, j] += a[i, k] * b[k, j];
        return result;
    }

    static double[,] MatrixInverse(double[,] matrix)
    {
        if (matrix.GetLength(0) != 3 || matrix.GetLength(1) != 3)
            throw new ArgumentException("Matrix must be 3x3.");

        double det = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1])
                    - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[1, 2] * matrix[2, 0])
                    + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]);

        if (det == 0)
            throw new InvalidOperationException("Matrix is singular and cannot be inverted.");

        double[,] inverse = new double[3, 3];
        inverse[0, 0] = (matrix[1, 1] * matrix[2, 2] - matrix[1, 2] * matrix[2, 1]) / det;
        inverse[0, 1] = (matrix[0, 2] * matrix[2, 1] - matrix[0, 1] * matrix[2, 2]) / det;
        inverse[0, 2] = (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]) / det;
        inverse[1, 0] = (matrix[1, 2] * matrix[2, 0] - matrix[1, 0] * matrix[2, 2]) / det;
        inverse[1, 1] = (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]) / det;
        inverse[1, 2] = (matrix[0, 2] * matrix[1, 0] - matrix[0, 0] * matrix[1, 2]) / det;
        inverse[2, 0] = (matrix[1, 0] * matrix[2, 1] - matrix[1, 1] * matrix[2, 0]) / det;
        inverse[2, 1] = (matrix[0, 1] * matrix[2, 0] - matrix[0, 0] * matrix[2, 1]) / det;
        inverse[2, 2] = (matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0]) / det;

        return inverse;
    }

    static double[,] ToColumnVector(double[] vector)
    {
        double[,] columnVector = new double[vector.Length, 1];
        for (int i = 0; i < vector.Length; i++)
            columnVector[i, 0] = vector[i];
        return columnVector;
    }
}