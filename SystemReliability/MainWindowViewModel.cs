using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Data;
using Extreme.Mathematics.Calculus;
using OxyPlot;
using OxyPlot.Series;

namespace SystemReliability
{
    public class MainWindowViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Time { get; set; } = "100";

        public string Lambda0 { get; set; } = "0.002";

        public string Lambda1 { get; set; } = "0.003";

        public string Lambda2 { get; set; } = "0.0002";

        public string Lambda3 { get; set; } = "0.0001";

        public string Beta { get; set; } = "1.5";

        public string Alpha1 { get; set; } = "75";

        public string Alpha2 { get; set; } = "1";

        public string Alpha3 { get; set; } = "6";

        public string K { get; set; } = "5";

        public int Progress { get; set; }

        public int ProgressMax { get; set; }

        //private double _time => double.Parse(Time);

        //private double _lambda0 => double.Parse(Lambda0);

        //private double _lambda1 => double.Parse(Lambda1);

        //private double _lambda2 => double.Parse(Lambda2);

        //private double _lambda3 => double.Parse(Lambda3);

        //private double _beta => double.Parse(Beta);

        //private long _alpha1 => long.Parse(Alpha1);

        //private long _alpha2 => long.Parse(Alpha2);

        //private long _alpha3 => long.Parse(Alpha3);

        //private long _k => long.Parse(K);

        private SynchronizationContext _synchronizationContext;

        private Dictionary<long, long> _cachedFactorial = new Dictionary<long, long>();

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public ObservableCollection<KeyValuePair<long, double>> Calculations { get; set; }

        public RelayCommand CalculateCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public PlotModel PlotModel { get; set; }

        public LineSeries LineSeries { get; set; } = new LineSeries { MarkerType = MarkerType.Circle};

        public MainWindowViewModel()
        {
            CalculateCommand = new RelayCommand(ExecuteCalculateCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            _synchronizationContext = SynchronizationContext.Current;
            PlotModel = new PlotModel();
            PlotModel.Series.Add(LineSeries);
        }

        private void ExecuteCancelCommand()
        {
            CancellationTokenSource.Cancel();
        }

        private void ExecuteCalculateCommand()
        {
            CancellationTokenSource = new CancellationTokenSource();
            Task.Run(CalculateReadinessCoefficient, CancellationTokenSource.Token);
            //Task.Run(CalculateSystemBeingInAllStatesDuration, CancellationTokenSource.Token);
        }

        private void CalculateReadinessCoefficient()
        {
            Calculations = new ObservableCollection<KeyValuePair<long, double>>();
            double time = double.Parse(Time);
            double _lambda0 = double.Parse(Lambda0);
            double _lambda1 = double.Parse(Lambda1);
            double _lambda2 = double.Parse(Lambda2);
            double _lambda3 = double.Parse(Lambda3);
            double _beta = double.Parse(Beta);
            long _alpha1 = long.Parse(Alpha1);
            long _alpha2 = long.Parse(Alpha2);
            long _alpha3 = long.Parse(Alpha3);
            long _k = long.Parse(K);

            int numberOfCalculations = 10;
            int step = (int)Math.Ceiling((double)time / numberOfCalculations);

            ProgressMax = numberOfCalculations;
            Progress = 0;

            for (long _time = step, i = 0; _time <= time; _time += step, i++)
            {
                double result = Math.Exp(-_lambda0 * _time);

                double sum1Result = 0;
                List<long> x3s = new List<long>();

                for (long x3 = _k; x3 <= _alpha1 * _alpha2 * _alpha3; x3++)
                {
                    x3s.Add(x3);
                }

                foreach (long x3 in x3s.AsParallel())
                {
                    if (CancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }
                    double sum2Result = 0;
                    for (long x1 = (long)Math.Ceiling(Math.Ceiling((double)x3 / _alpha3) / _alpha2); x1 <= _alpha1; x1++)
                    {
                        double sum3Result = 0;
                        
                        for (long x2 = (long)Math.Ceiling((double)x3 / _alpha3); x2 <= _alpha2 * x1; x2++)
                        {
                            sum3Result += (NoRepeatCombinations(_alpha2 * x1, x2)
                                          * Math.Exp(-_lambda2 * x2 * _time)
                                          * Math.Pow(1 - Math.Exp(-_lambda2 * _time), _alpha2 * x1 - x2)
                                          * NoRepeatCombinations(_alpha3 * x2, x3)
                                          * Math.Exp(-_lambda3 * x3 * Math.Pow(_time, _beta))
                                          * Math.Pow(1 - Math.Exp(-_lambda3 * Math.Pow(_time, _beta)), _alpha3 * x2 - x3));
                        }

                        sum2Result += (NoRepeatCombinations(_alpha1, x1)
                                       * Math.Exp(-_lambda1 * x1 * _time)
                                       * Math.Pow(1 - Math.Exp(-_lambda1 * _time), _alpha1 - x1))
                                       * sum3Result;
                    }

                    sum1Result += sum2Result;
                }
                result *= sum1Result;
                Progress++;
                if (i == 6)
                {
                    return;
                }
                LineSeries.Points.Add(new DataPoint(i, result));
                PlotModel.InvalidatePlot(false);
            }
        }

        private void CalculateSystemBeingInAllStatesDuration()
        {
            Calculations = new ObservableCollection<KeyValuePair<long, double>>();
            double time = double.Parse(Time);
            double _lambda0 = double.Parse(Lambda0);
            double _lambda1 = double.Parse(Lambda1);
            double _lambda2 = double.Parse(Lambda2);
            double _lambda3 = double.Parse(Lambda3);
            double _beta = double.Parse(Beta);
            long _alpha1 = long.Parse(Alpha1);
            long _alpha2 = long.Parse(Alpha2);
            long _alpha3 = long.Parse(Alpha3);
            long _k = long.Parse(K);

            SimpsonIntegrator simpson = new SimpsonIntegrator
            {
                RelativeTolerance = 1e-5
            };
            double result = simpson.Integrate(Integral(_lambda0, _lambda1, _lambda2, 1, 1, 1, 1, 1, 1, 1, 1), 0, double.MaxValue);
            MessageBox.Show(result.ToString(CultureInfo.InvariantCulture));
        }

        private Func<double, double> Integral(double lambda0, double lambda1, double lambda2, int x1, int x2, int x3, int j1, int j2, int j3, int t, int beta)
        {
            double res = Math.Exp(-(lambda0 + lambda1) * t);
            return Math.Sin;
        }

        private long NoRepeatCombinations(long n, long m)
        {
            return (long)(Factorial(n) / (double)(Factorial(m) * Factorial(n - m)));
        }

        private long Factorial(long n)
        {
            if (_cachedFactorial.TryGetValue(n, out long value))
            {
                return value;
            }
            long result = 1;
            for (long i = 1; i <= n; i++)
            {
                result *= i;
            }

            _cachedFactorial.Add(n, result);

            return result;
        }
    }
}
