using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Math_Utils
{
    /// <summary>
    /// This class describes what a x,y pair should look like. Using the constructor to create datapoints, it makes a convenient
    /// way to create these pairs, then using thet getter properties to review those records. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class XYPair<T>
    {
        private T _x { get; set; }
        private T _y { get; set; }

        /// <summary>
        /// Construct an XY pair record
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public XYPair(T x, T y)
        {
            _x = x;
            _y = y;
        }

        public T X { 
            get
            {
                return _x;
            }
        }
        public T Y
        {
            get
            {
                return _y;
            }
        }

    }
    /// <summary>
    /// This class helps to predict x or y points are based on a set of sample data in x,y pairs. You only get two interfaces,
    /// Find X corrdinate method or Find Y coordinate method. It should solve a problem like this. 
    /// 
    /// I have a set of data samples that are x,y points. 
    /// { (1, 11) (3, 16) (4, 15) (6, 20) (8, 18) }
    /// I want to know what the x value would be when y reaches 0. You then use the FindXPosition() method to locate what that
    /// x value would be when y = 0, using the least squares regression line formulas for the slope (m) and intercept (b), plug in the known y value 0,
    /// and it will calculate the x result. 
    /// </summary>
    internal class Linear_Regression
    {
        //resource https://statisticsbyjim.com/regression/least-squares-regression-line/

        /// <summary>
        /// Here we will hold the sum of each x,y pair in a data set for E(x*y), E(x), E(y), and E(x^2). This is a reference table.
        /// Where E is sigma and we add each result of each instance within the ().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Table<T>
        {
            public T xy { get; set; }
            public T x_squared { get; set; }
            public T sum_x { get; set; }
            public T sum_y { get; set; }
        }
        /// <summary>
        /// This holds the relationship model of your samples. This will be a record for that sample set's slope and intercept so we can analyze
        /// and make predictions using the y = mx + b formula. In particular, this class holds the m and b comonents or slope and intercept. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Formula<T>
        {
            private T _slope { get; set; }
            private T _offset { get; set; }
            public Formula(T slope, T offset)
            {
                _slope = slope;
                _offset = offset;
            }

            public T Slope { 
                get
                {
                    return _slope;
                }
            }
            public T Offset
            {
                get
                {
                    return _offset;
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Linear_Regression()
        {

        }
        /// <summary>
        /// Using a sample set, for each x,y pair, we will calculate and sum x*y, x, y, and x^2. Then this is stored into a Table class object. 
        /// </summary>
        /// <param name="samples"></param>
        /// <returns>Table class object filled with results of calculation.</returns>
        private Table<double> BuildTable(XYPair<double>[] samples)
        {
            Table<double> table = new Table<double>();
            foreach (var sample in samples)
            {
                table.xy += (sample.X * sample.Y);
                table.x_squared += (sample.X * sample.X);
                table.sum_x += sample.X;
                table.sum_y += sample.Y;
            }
            return table;
        }
        /// <summary>
        ///  m = N(E(xy)) - E(x)E(y) / N(E(x^2)) - (E(x))^2 where N is number of samples and E represents a sum
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="table"></param>
        /// <returns> returns a slope or m in the y = mx + b formula</returns>
        private double FindSlope(XYPair<double>[] samples,Table<double> table)
        {
            int count = samples.Length;
            double m = ((count*table.xy)-(table.sum_x* table.sum_y)) / ((count* table.x_squared)-(table.sum_x * table.sum_x));
            return m;
        }
        /// <summary>
        /// b = E(y) - m(E(x)) / N  where N is number of samples and E represents a sum and m is the slope
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="slope"></param>
        /// <param name="table"></param>
        /// <returns>returns the intercept or b in the y = mx + b formula</returns>
        private double FindIntercept(XYPair<double>[] samples, double slope,Table<double>table)
        {
            double b = (table.sum_y-(slope*table.sum_x)) / (samples.Length);
            return b;
        }
        /// <summary>
        /// Function needs a sample set of x,y pairs. For each pair in the set, it will build a table needed for the calculating 
        /// the slope and intercept. 
        /// </summary>
        /// <param name="samples"></param>
        /// <returns>A new Formula class object that hold the slope and offset based on the samples provided in the input.</returns>
        private Formula<double> CalculateLinearRegression(XYPair<double>[] samples)
        {
            Table<double> table = BuildTable(samples);
            double slope = FindSlope(samples, table);
            double offset = FindIntercept(samples,slope,table);
            return new Formula<double>(slope, offset);
        }
        /// <summary>
        /// You give it the y value you want to find a corresponding x value for and the samples of x,y pairs that you have in a buffer. 
        /// We will then calculate a linear regression line based on your samples using the least squares regression line formula.
        /// 
        /// This is useful for trying to predict coordinate points that are outside of your data set. 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="y"></param>
        /// <returns>x coordinate position</returns>
        public double FindXPosition(XYPair<double>[] samples, double y)
        {
            double x = 0;
            Formula<double> result = CalculateLinearRegression(samples);
            x = (y-result.Offset) /(result.Slope);
            return x;
        }
        /// <summary>
        /// You give it the x value you want to find a corresponding y value for and the samples of x,y pairs that you have in a buffer. 
        /// We will then calculate a linear regression line based on your samples using the least squares regression line formula.   
        /// 
        /// This is useful for trying to predict coordinate points that are outside of your data set. 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="x"></param>
        /// <returns>y coordinate position</returns>
        public double FindYPosition(XYPair<double>[] samples, double x)
        {
            double y = 0;
            Formula<double> result = CalculateLinearRegression(samples);
            y = (result.Slope * x) + result.Offset; // y=mx+b
            return y;
        }

    }
}
