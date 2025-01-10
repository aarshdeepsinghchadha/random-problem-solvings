using System.Diagnostics;

public class Program
{
    static void Main()
    {
        var datasetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "data.csv");
        var creatorsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "creators.csv");

        if (!File.Exists(datasetPath))
        {
            Console.WriteLine("data.csv is missing!");
            throw new FileNotFoundException("data.csv is required but missing.");
        }
        else
        {
            Console.WriteLine("data.csv exists.");
        }

        if (!File.Exists(creatorsPath))
        {
            Console.WriteLine("creators.csv is missing!");
            throw new FileNotFoundException("creators.csv is required but missing.");
        }
        else
        {
            Console.WriteLine("creators.csv exists.");
        }

        var historicalData = ReadCsv(datasetPath);
        var creators = ReadCsv(creatorsPath);

        var (avgDeltaTech, avgDeltaEntertainment) = CalculateAverageDeltas(historicalData);

        var evolvedCreators = SimulateEvolution(creators, avgDeltaTech, avgDeltaEntertainment, weeks: 4);

        int highestTechIndex = evolvedCreators.Select((creator, index) => (creator[0], index))
                                              .OrderByDescending(x => x.Item1)
                                              .First().index;

        int highestEntertainmentIndex = evolvedCreators.Select((creator, index) => (creator[1], index))
                                                       .OrderByDescending(x => x.Item1)
                                                       .First().index;

        var focusSwitches = IdentifyFocusSwitches(creators, evolvedCreators);


        Console.WriteLine($"Creator with highest tech value after 4 weeks: {(highestTechIndex != -1 ? highestTechIndex.ToString() : "None")}");
        Console.WriteLine($"Creator with highest entertainment value after 4 weeks: {(highestEntertainmentIndex != -1 ? highestEntertainmentIndex.ToString() : "None")}");

        Console.WriteLine("Creators switching focus (tech to entertainment): " +
                          (focusSwitches.techToEntertainment.Count > 0
                              ? string.Join(", ", focusSwitches.techToEntertainment)
                              : "None"));

        Console.WriteLine("Creators switching focus (entertainment to tech): " +
                          (focusSwitches.entertainmentToTech.Count > 0
                              ? string.Join(", ", focusSwitches.entertainmentToTech)
                              : "None"));
    }

    static List<double[]> ReadCsv(string filePath)
    {
        return File.ReadLines(filePath)
                   .Select(line => line.Split(',').Select(double.Parse).ToArray())
                   .ToList();
    }


    static (double avgDeltaTech, double avgDeltaEntertainment) CalculateAverageDeltas(List<double[]> data)
    {
        double totalDeltaTech = 0;
        double totalDeltaEntertainment = 0;
        int count = data.Count;

        foreach(var row in data)
        {
            totalDeltaTech += row[2] - row[0];
            totalDeltaEntertainment += row[3] - row[1];
        }

        return (totalDeltaTech, totalDeltaEntertainment);
    }

    static List<double[]> SimulateEvolution(List<double[]> creators, double avgDeltaTech, double avgDeltaEntertainment, int weeks)
    {
        return creators.Select(creator =>
        {
            double tech = creator[0];
            double entertainment = creator[1];

            for (int i = 0; i < weeks; i++)
            {
                tech += avgDeltaTech;
                entertainment += avgDeltaEntertainment;
            }

            return new double[] { tech, entertainment };
        }).ToList();
    }

    static (List<int> techToEntertainment, List<int> entertainmentToTech) IdentifyFocusSwitches(List<double[]> initial, List<double[]> final)
    {
        var techToEntertainment = new List<int>();
        var entertainmentToTech = new List<int>();

        for (int i = 0; i < initial.Count; i++)
        {
            bool initiallyTechFocused = initial[i][0] > initial[i][1];
            bool finallyTechFocused = final[i][0] > final[i][1];

            if (initiallyTechFocused && !finallyTechFocused)
            {
                techToEntertainment.Add(i);
            }
            else if (!initiallyTechFocused && finallyTechFocused)
            {
                entertainmentToTech.Add(i);
            }
        }

        return (techToEntertainment, entertainmentToTech);
    }

}