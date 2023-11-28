using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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

        private double _MAPE_error { get; set; } = 0;
        public double MAPE_Error
        {
            get
            {
                return _MAPE_error;
            }
            //set
            //{
            //    _MAPE_error = value;
            //}
        }
        private double _MAD_error { get; set; } = 0;
        public double MAD_Error
        {
            get
            {
                return _MAD_error;
            }
            //set
            //{
            //    _MAD_error = value; 
            //}
        }
        private double _MSE_error { get; set; } = 0;
        public double MSE_Error
        {
            get
            {
                return _MSE_error;
            }
            //set
            //{
            //    _MSE_error = value; 
            //}
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
        /// After calculating the linear regression components, we can optionally call this method to test
        /// the accuracy of this calulation. 
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="forecast"></param>
        /// <param name="CalculateX">If true, then we will test the X component for accuracy, if false we will test the Y component, Y this is default. </param>
        /// <exception cref="ArgumentException"></exception>
        private void FindError(XYPair<double>[] actual, XYPair<double>[] forecast,bool CalculateX = false)
        {
            double absoluteDeviation = 0;
            double sumAbsoluteDeviations = 0;
            double sumAbsolutePercentage = 0;
            double sumMeanSquare = 0;
            if (actual.Length != forecast.Length)
            {
                throw new ArgumentException("The actual and forecast arrays must have the same length.");
            }
            if (!CalculateX)
            {
                for (int i = 0; i < actual.Length; i++)
                {
                    // Mean Absolute Deviation (MAD)
                    absoluteDeviation = Math.Abs(actual[i].Y - forecast[i].Y);
                    sumAbsoluteDeviations += absoluteDeviation;
                    //Mean Absolute Percentage Error (MAPE)
                    sumAbsolutePercentage = Math.Abs((actual[i].Y - forecast[i].Y) / (actual[i].Y));
                }
            }
            else
            {
                for (int i = 0; i < actual.Length; i++)
                {
                    // Mean Absolute Deviation (MAD)
                    absoluteDeviation = Math.Abs(actual[i].X - forecast[i].X);
                    sumAbsoluteDeviations += absoluteDeviation;
                    //Mean Absolute Percentage Error (MAPE)
                    sumAbsolutePercentage = Math.Abs((actual[i].X - forecast[i].X) / (actual[i].X));
                }
            }
            // Mean square error(MSE)
            sumMeanSquare = absoluteDeviation * absoluteDeviation;
            _MAD_error = sumAbsoluteDeviations / actual.Length;
            _MAPE_error = ((sumAbsolutePercentage / forecast.Length) * 100);
            _MSE_error = ((sumAbsolutePercentage / forecast.Length));
        }
        /// <summary>
        /// You give it the y value you want to find a corresponding x value for and the samples of x,y pairs that you have in a buffer. 
        /// We will then calculate a linear regression line based on your samples using the least squares regression line formula.
        /// 
        /// This is useful for trying to predict coordinate points that are outside of your data set. 
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="y"></param>
        /// <param name="calculateError">Default to false, if true, then, based on your provided samples, we will genereate a test and genereate your error states.</param>
        /// <returns>x coordinate position</returns>
        public double FindXPosition(XYPair<double>[] samples, double y, bool calculateError=false, bool enableErrorCorrection = false)
        {
            double x = 0;
            Formula<double> result = CalculateLinearRegression(samples);

            if(calculateError) 
            {
                XYPair<double>[] forecast = new XYPair<double>[samples.Length];
                int i = 0;
                foreach (XYPair<double> sample in samples)
                {
                    // create forecast list
                    x = (sample.Y - result.Offset) / (result.Slope);
                    forecast[i] = new XYPair<double>(x, sample.Y);
                    i++;
                }

                FindError(samples, forecast,true); // true refers to us wanting to genereate test and it's results using the X attribute
            }
            double error = 0;
            if (enableErrorCorrection)
                error = this.MSE_Error;
            x = (y-result.Offset+error) /(result.Slope);
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
        /// <param name="calculateError">Default to false, if true, then, based on your provided samples, we will genereate a test and genereate your error states.</param>
        /// <returns>y coordinate position</returns>
        public double FindYPosition(XYPair<double>[] samples, double x, bool calculateError = false, bool enableErrorCorrection = false)
        {
            double y = 0;
            Formula<double> result = CalculateLinearRegression(samples);

            if (calculateError)
            {
                XYPair<double>[] forecast = new XYPair<double>[samples.Length];
                int i = 0;
                foreach (XYPair<double> sample in samples)
                {
                    // create forecast list
                    y = (result.Slope * sample.X) + result.Offset;
                    forecast[i] = new XYPair<double>(sample.X, y);
                    i++;
                }

                FindError(samples, forecast);
            }
            double error = 0;
            if (enableErrorCorrection)
                error = this.MSE_Error;
            y = (result.Slope * x) + result.Offset + error; // y=mx+b
            return y;
        }

    }

    /// <summary>
    /// Represents a buffer for storing and managing data points of type T.
    /// </summary>
    /// <typeparam name="T">Type of data points in the buffer.</typeparam>
    internal class SampleBuffer<T>
    {
        private List<T> _buffer;
        private int _maxBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleBuffer{T}"/> class with a specified maximum buffer size.
        /// </summary>
        /// <param name="maxBufferSize">The maximum size of the buffer.</param>
        public SampleBuffer(int maxBufferSize)
        {
            if (maxBufferSize <= 0)
            {
                throw new ArgumentException("Maximum buffer size must be greater than zero.", nameof(maxBufferSize));
            }

            _buffer = new List<T>();
            _maxBufferSize = maxBufferSize;
        }

        /// <summary>
        /// Gets a value indicating whether the buffer is full.
        /// </summary>
        public bool IsBufferFull
        {
            get { return _buffer.Count == _maxBufferSize; }
        }

        /// <summary>
        /// Adds a data point to the buffer.
        /// </summary>
        /// <param name="dataPoint">The data point to add.</param>
        public void AddDataPoint(T dataPoint)
        {
            if (IsBufferFull)
            {
                throw new InvalidOperationException("Buffer is full. Cannot add more data points.");
            }

            _buffer.Add(dataPoint);
        }

        /// <summary>
        /// Clears the last data point from the buffer.
        /// </summary>
        /// <returns>The removed data point.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the buffer is empty.</exception>
        public T ClearDataPoint()
        {
            if (_buffer.Count == 0)
            {
                throw new InvalidOperationException("Buffer is empty. Cannot clear data point.");
            }

            T removedDataPoint = _buffer[_buffer.Count - 1];
            _buffer.RemoveAt(_buffer.Count - 1);

            return removedDataPoint;
        }
        /// <summary>
        /// Clears all data points from the buffer.
        /// </summary>
        public void ClearAll()
        {
            _buffer.Clear();
        }
        /// <summary>
        /// Gets the buffer as an array.
        /// </summary>
        /// <returns>The buffer as an array.</returns>
        public T[] GetBufferAsArray()
        {
            return _buffer.ToArray();
        }
    }

    /// <summary>
    /// This class encapsulates the functionality for detecting outliers in a dataset using the standard deviation method.
    /// </summary>
    public class OutlierDetector
    {
        /// <summary>
        /// An array of double values representing the dataset.
        /// </summary>
        private double[] _data { get;set; }
        public double[] Data
        {
            get { return _data; }
            private set{_data = value;}
        }
        /// <summary>
        /// The mean (average) of the dataset.
        /// </summary>
        private double _mean { get; set; }
        public double Mean
        {
            get
            {
                return _mean;
            }
            private set
            {
                _mean = value;
            }
        }
        /// <summary>
        /// The standard deviation of the dataset.
        /// </summary>
        private double _standardDeviation { get; set; }
        public double StandardDeviation {
            get { return _standardDeviation; } 
            private set { _standardDeviation= value; } 
        }
        /// <summary>
        /// A multiplier determining the threshold for identifying outliers.
        /// </summary>
        private double _threshold;
        public double Threshold {
            get { return _threshold; } 
            private set { _threshold = value; }
        }   

        /// <summary>
        /// Constructor that initializes the class with the dataset and an optional threshold.
        /// </summary>
        /// <param name="data">An array of double values representing the dataset.</param>
        /// <param name="threshold">An optional multiplier determining the threshold for identifying outliers.</param>
        public OutlierDetector(double[] data, double threshold = 2.0)
        {
            _data = data;
            _threshold = threshold;
            CalculateStatistics();
        }
        /// <summary>
        /// Returns a new list containing the data without outliers, where each data point is within the specified threshold of the mean.
        /// </summary>
        /// <returns>A list containing the data without outliers.</returns>
        public List<double> GetDataWithoutOutliers()
        {
            return _data.Where(value => Math.Abs(value - _mean) <= _threshold * _standardDeviation).ToList();
        }
        /// <summary>
        /// Detects and returns a list of outliers in the dataset based on the specified threshold.
        /// </summary>
        /// <returns>A list of double values representing the outliers in the dataset.</returns>
        public List<double> DetectOutliers()
        {
            return _data.Where(value => Math.Abs(value - _mean) > _threshold * _standardDeviation).ToList();
        }

        /// <summary>
        /// Calculates the mean and standard deviation of the dataset.
        /// </summary>
        private void CalculateStatistics()
        {
            _mean = _data.Average();
            _standardDeviation = CalculateStandardDeviation();
        }

        /// <summary>
        /// Calculates the standard deviation of the dataset.
        /// </summary>
        /// <returns>The standard deviation of the dataset.</returns>
        private double CalculateStandardDeviation()
        {
            double sumSquaredDifferences = _data.Sum(value => Math.Pow(value - _mean, 2));
            return Math.Sqrt(sumSquaredDifferences / _data.Length);
        }
    }


    /// <summary>
    /// Represents an outlier detector for a specific type T using x, y pairs.
    /// </summary>
    /// <typeparam name="T">The type of values for x and y in the x, y pair.</typeparam>
    internal class XYOutlierDetector<T>
    {
        private SampleBuffer<XYPair<T>> _sampleBuffer;
        private Func<XYPair<T>, double> _valueExtractor;
        // Expose _mean and _standardDeviation with public getter properties
        public double Mean { get; private set; }
        public double StandardDeviation { get; private set; }

        private double _threshold;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlierDetector{T}"/> class.
        /// </summary>
        /// <param name="sampleBuffer">The sample buffer containing x, y pairs.</param>
        /// <param name="valueExtractor">A function to extract numerical values from x, y pairs.</param>
        /// <param name="threshold">The threshold for identifying outliers.</param>
        public XYOutlierDetector(SampleBuffer<XYPair<T>> sampleBuffer, Func<XYPair<T>, double> valueExtractor, double threshold = 2.0)
        {
            _sampleBuffer = sampleBuffer ?? throw new ArgumentNullException(nameof(sampleBuffer));
            _valueExtractor = valueExtractor ?? throw new ArgumentNullException(nameof(valueExtractor));
            _threshold = threshold;
            CalculateStatistics();
        }
        /// <summary>
        /// Detects outliers in the sample buffer based on the specified threshold.
        /// </summary>
        /// <returns>A list of numerical values representing outliers.</returns>
        public XYPair<T>[] DetectOutliers()
        {
            return _sampleBuffer.GetBufferAsArray()
                .Where(pair => Math.Abs(_valueExtractor(pair) - Mean) > _threshold * StandardDeviation)
                .ToArray();
        }

        /// <summary>
        /// Returns a new array containing the data without outliers, where each data point is within the specified threshold of the mean.
        /// </summary>
        /// <returns>An array containing the data without outliers.</returns>
        public XYPair<T>[] GetDataWithoutOutliers()
        {
            return _sampleBuffer.GetBufferAsArray()
                .Where(pair => Math.Abs(_valueExtractor(pair) - Mean) <= _threshold * StandardDeviation)
                .ToArray();
        }

        private void CalculateStatistics()
        {
            double[] data = _sampleBuffer.GetBufferAsArray().Select(_valueExtractor).ToArray();
            Mean = data.Average();
            StandardDeviation = CalculateStandardDeviation(data);
        }

        private double CalculateStandardDeviation(double[] values)
        {
            double sumSquaredDifferences = values.Sum(value => Math.Pow(value - Mean, 2));
            return Math.Sqrt(sumSquaredDifferences / values.Length);
        }
    }
}
