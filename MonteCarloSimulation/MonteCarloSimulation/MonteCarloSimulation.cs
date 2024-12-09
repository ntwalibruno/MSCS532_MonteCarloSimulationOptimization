using System;
using System.Diagnostics;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Windows.Forms;
using OxyPlot.Legends;

class MonteCarloSimulation
{
    static Random random = new Random();

    public static double EstimatePi(int numPoints)
    {
        int pointsInsideCircle = 0;

        for (int i = 0; i < numPoints; i++)
        {
            double x = random.NextDouble(); // Random x coordinate between 0 and 1
            double y = random.NextDouble(); // Random y coordinate between 0 and 1

            // Check if the point is inside the quarter circle
            if (x * x + y * y <= 1)
            {
                pointsInsideCircle++;
            }
        }

        // Estimate Pi: (pointsInsideCircle / numPoints) * 4
        return (4.0 * pointsInsideCircle) / numPoints;
    }

    public void RunSimulation()
    {
        Console.WriteLine("Monte Carlo Simulation to Estimate Pi");

        int numPoints = 1000000000;  // Number of points for the simulation

        // Start stopwatch to measure runtime
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Start Process to measure CPU usage
        Process process = Process.GetCurrentProcess();
        var initialCpuUsage = process.TotalProcessorTime;

        // Run the simulation
        double estimatedPi = EstimatePi(numPoints);

        // Stop stopwatch after simulation completes
        stopwatch.Stop();
        var finalCpuUsage = process.TotalProcessorTime;

        // Calculate CPU time used
        TimeSpan cpuUsage = finalCpuUsage - initialCpuUsage;

        // Output results
        Console.WriteLine($"Estimated Pi after {numPoints} points: {estimatedPi}");
        Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"CPU Time: {cpuUsage.TotalMilliseconds} ms");
        Console.WriteLine($"CPU Cores Used: {Environment.ProcessorCount}");

        Console.WriteLine();
        Console.WriteLine();
        
        // Generate performance graph
        GeneratePerformanceGraph(stopwatch.ElapsedMilliseconds, cpuUsage.TotalMilliseconds, Environment.ProcessorCount);
    }

    // Inside the MonteCarloSimulation class
    private void GeneratePerformanceGraph(double elapsedTime, double cpuTime, int cpuCores)
    {
        var plotModel = new PlotModel { Title = "Performance Graph" };

        var elapsedTimeSeries = new LineSeries { Title = "Elapsed Time (ms)" };
        elapsedTimeSeries.Points.Add(new DataPoint(0, 0));
        elapsedTimeSeries.Points.Add(new DataPoint(1, elapsedTime));

        var cpuTimeSeries = new LineSeries { Title = "CPU Time (ms)" };
        cpuTimeSeries.Points.Add(new DataPoint(0, 0));
        cpuTimeSeries.Points.Add(new DataPoint(1, cpuTime));

        var cpuCoresSeries = new LineSeries { Title = "CPU Cores Used" };
        cpuCoresSeries.Points.Add(new DataPoint(0, 0));
        cpuCoresSeries.Points.Add(new DataPoint(1, cpuCores));

        plotModel.Series.Add(elapsedTimeSeries);
        plotModel.Series.Add(cpuTimeSeries);
        plotModel.Series.Add(cpuCoresSeries);

        plotModel.Legends.Add(new Legend
        {
            LegendPosition = LegendPosition.TopRight,
            LegendPlacement = LegendPlacement.Outside,
            LegendOrientation = LegendOrientation.Horizontal
        });

        var plotView = new PlotView { Model = plotModel };
        var form = new Form { ClientSize = new System.Drawing.Size(800, 600) };
        form.Controls.Add(plotView);
        plotView.Dock = DockStyle.Fill;
        Application.Run(form);
    }
}
