using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using System.Threading;
using OxyPlot.Legends;

class MonteCarloSimulationOptimized
{
    private static ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(() => new Random());

    // Method to estimate Pi using Monte Carlo method (parallel version)
    public static double EstimatePiParallel(int numPoints)
    {
        int pointsInsideCircle = 0;

        int chunkSize = numPoints / Environment.ProcessorCount;

        Parallel.For(0, Environment.ProcessorCount, (i) =>
        {
            int localCount = 0;

            // Each thread calculates its portion of the points
            for (int j = 0; j < chunkSize; j++)
            {
                double x = threadRandom.Value.NextDouble(); // Random x coordinate between 0 and 1
                double y = threadRandom.Value.NextDouble(); // Random y coordinate between 0 and 1

                // Check if the point is inside the quarter circle
                if (x * x + y * y <= 1)
                {
                    localCount++;
                }
            }

            System.Threading.Interlocked.Add(ref pointsInsideCircle, localCount);
        });

        return (4.0 * pointsInsideCircle) / numPoints;
    }

    public void RunSimulation()
    {
        Console.WriteLine("Monte Carlo Simulation to Estimate Pi (Parallel)");

        int numPoints = 1000000000;  // Increased points for better parallel performance

        Stopwatch stopwatch = Stopwatch.StartNew();

        Process process = Process.GetCurrentProcess();
        var initialCpuUsage = process.TotalProcessorTime;

        double estimatedPi = EstimatePiParallel(numPoints);

        stopwatch.Stop();
        var finalCpuUsage = process.TotalProcessorTime;

        TimeSpan cpuUsage = finalCpuUsage - initialCpuUsage;

        Console.WriteLine($"Estimated Pi after {numPoints} points: {estimatedPi}");
        Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"CPU Time: {cpuUsage.TotalMilliseconds} ms");
        Console.WriteLine($"CPU Cores Used: {Environment.ProcessorCount}");

        // Generate performance graph
        GeneratePerformanceGraph(stopwatch.ElapsedMilliseconds, cpuUsage.TotalMilliseconds, Environment.ProcessorCount);
    }

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
        var form = new System.Windows.Forms.Form { ClientSize = new System.Drawing.Size(800, 600) };
        form.Controls.Add(plotView);
        plotView.Dock = System.Windows.Forms.DockStyle.Fill;
        System.Windows.Forms.Application.Run(form);
    }
}
