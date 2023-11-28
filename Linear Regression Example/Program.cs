using System;
using Math_Utils;


void LinearRegressionTest()
{
    XYPair<double>[] xyPairs = new XYPair<double>[]
    {
    new XYPair<double>(1, 11),
    new XYPair<double>(3, 16),
    new XYPair<double>(4, 15),
    new XYPair<double>(6, 20),
    new XYPair<double>(8, 18),

    };
    XYPair<double>[] xyPairs2 = new XYPair<double>[]
    {
    new XYPair<double>(120514, 236),
    new XYPair<double>(138401, 223.6),
    new XYPair<double>(150544, 320.2),
    new XYPair<double>(166628, 290),
    new XYPair<double>(166908, 304),
    new XYPair<double>(195876, 372),
    };


    Linear_Regression linear_Regression = new Linear_Regression();

    double output = linear_Regression.FindYPosition(xyPairs,5);
    if (output > 16 && output < 17)
        Console.WriteLine("PASS: y = " + output);
    else
        Console.WriteLine("FAIL: y = " + output + "\r\nEXPECTED: 16.637");

    output = 0;

    output = linear_Regression.FindXPosition(xyPairs, 15);
    if (output > 3 && output < 5)
        Console.WriteLine("PASS: x = " + output);
    else
        Console.WriteLine("FAIL: x = " + output + "\r\nEXPECTED: 4");
    double goalY = 0, goalX = 195876;
    output = linear_Regression.FindXPosition(xyPairs2, goalY,true,true);
    Console.WriteLine("With Error Correction:");
    Console.WriteLine("what is the x place when y = " + goalY + ", x = " + output);
    Console.WriteLine("MAD = " + linear_Regression.MAD_Error + " MAPE = " + linear_Regression.MAPE_Error + " MSE = " + linear_Regression.MSE_Error);
    output = linear_Regression.FindXPosition(xyPairs2, goalY,true);
    Console.WriteLine("Without Error Correction:");
    Console.WriteLine("what is the x place when y = " + goalY + ", x = " + output);
    Console.WriteLine("MAD = " + linear_Regression.MAD_Error + " MAPE = " + linear_Regression.MAPE_Error + " MSE = " + linear_Regression.MSE_Error);
    
    
    
    Console.WriteLine();
    output = linear_Regression.FindYPosition(xyPairs2, goalX, true);
    Console.WriteLine("what is the y place when x = " + goalX + ", y = " + output);
    Console.WriteLine("MAD = " + linear_Regression.MAD_Error + " MAPE = " + linear_Regression.MAPE_Error + " MSE = " + linear_Regression.MSE_Error);

}

static void OutlierTest()
{
        // Sample data
        double[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 100 };

        // Create an instance of the OutlierDetector class
        OutlierDetector outlierDetector = new OutlierDetector(data);

        // Detect outliers
        List<double> outliers = outlierDetector.DetectOutliers();

        // Get data without outliers
        List<double> dataWithoutOutliers = outlierDetector.GetDataWithoutOutliers();

        // Output the results
        Console.WriteLine("Data: " + string.Join(", ", data));
        Console.WriteLine("Mean: " + outlierDetector.Mean);
        Console.WriteLine("Standard Deviation: " + outlierDetector.StandardDeviation);
        Console.WriteLine("Outliers: " + string.Join(", ", outliers));
        Console.WriteLine("Data without Outliers: " + string.Join(", ", dataWithoutOutliers));
}

static void XYOutlierTest()
{
    // Create a sample buffer of XYPair<double>
    var sampleBuffer = new SampleBuffer<XYPair<double>>(maxBufferSize: 10);

    // Generate sample data with outliers
    Random random = new Random();
    for (int i = 0; i < 9; i++)
    {
        double x = random.Next(1, 10);
        double y = random.Next(1, 10);
        sampleBuffer.AddDataPoint(new XYPair<double>(x, y));
    }

    // Add an outlier
    sampleBuffer.AddDataPoint(new XYPair<double>(100, 100));

    // Create an OutlierDetector using the Y values for outlier detection
    var outlierDetector = new XYOutlierDetector<double>(
        sampleBuffer,
        pair => pair.Y,
        threshold: 1.0
    );

    // Detect outliers
    XYPair<double>[] outliers = outlierDetector.DetectOutliers();

    // Get data without outliers
    XYPair<double>[] dataWithoutOutliers = outlierDetector.GetDataWithoutOutliers();

    // Output the results
    Console.WriteLine("\nOutliers:");
    foreach (var dataPoint in outliers)
    {
        Console.WriteLine($"X: {dataPoint.X}, Y: {dataPoint.Y}");
    }

    Console.WriteLine("\nData without Outliers:");
    foreach (var dataPoint in dataWithoutOutliers)
    {
        Console.WriteLine($"X: {dataPoint.X}, Y: {dataPoint.Y}");
    }

}
LinearRegressionTest();
OutlierTest();
XYOutlierTest();