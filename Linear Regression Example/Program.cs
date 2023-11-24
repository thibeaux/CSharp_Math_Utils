using System;
using Math_Utils;


void main(string[] args)
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
    new XYPair<double>(156000, 263),
    new XYPair<double>(163000, 310),
    new XYPair<double>(155500, 258),
    new XYPair<double>(156500, 265),
    new XYPair<double>(143000, 200),
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

    output = linear_Regression.FindXPosition(xyPairs2, 0);
    Console.WriteLine("what is the x place when y = 0, x = " + output);

}

main(args);
