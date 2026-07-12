using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PropertyChanged;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Globalization;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// MTBILogControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MTBILogControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public LogSearcher.LogSearcherBase LogSearcherObject { get; set; }

        public Func<double, string> TimeCollectionXFormatter => x => new DateTime((long)x).ToString("yyyy-MM-dd");

        public Func<double, string> TimeCollectionYFormatter => x => x.ToString();

        public MTBILogControl()
        {
            InitializeComponent();
        }

        private void PieChart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }

    public class LogToTimeCollection : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var logs = (IEnumerable<LogSearcher.MTBILogInfo>)value;
                return new SeriesCollection
                    {
                        new StackedAreaSeries
                        {
                            Title = "Run",
                            Values = new ChartValues<DateTimePoint>(logs.Select(x => new DateTimePoint(x.Date, x.RunTime.TotalHours)))
                        },

                        new StackedAreaSeries
                        {
                            Title = "RunDown",
                            Values = new ChartValues<DateTimePoint>(logs.Select(x => new DateTimePoint(x.Date, x.RunDownTime.TotalHours)))
                        },

                        new StackedAreaSeries
                        {
                            Title = "Stop",
                            Values = new ChartValues<DateTimePoint>(logs.Select(x => new DateTimePoint(x.Date, x.StopTime.TotalHours)))
                        },

                        new StackedAreaSeries
                        {
                            Title = "Alarm",
                            Values = new ChartValues<DateTimePoint>(logs.Select(x => new DateTimePoint(x.Date, x.AlarmTime.TotalHours)))
                        },
                    };
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LogToMTBICollection : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var logs = (IEnumerable<LogSearcher.MTBILogInfo>)value;
                return new SeriesCollection
                    {
                        new LineSeries
                        {
                            ScalesYAt = 0,
                            Title = "MTBI",
                            Values = new ChartValues<DateTimePoint>(logs.Select(x => new DateTimePoint(x.Date, x.MTBI.TotalHours)))
                        },

                        new LineSeries
                        {
                            ScalesYAt = 1,
                            Title = "Alarm",
                            Values = new ChartValues<DateTimePoint>(logs.Select(x => new DateTimePoint(x.Date, x.AlarmCount)))
                        }
                    };
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MTBIInfoToPieCollection : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            try
            {
                var mtbiInfo = (LogSearcher.MTBILogInfo)value;
                return new SeriesCollection
                    {
                        new PieSeries
                        {
                            Title = "Run",
                            DataLabels = true,
                            LabelPoint = chartPoint => $"{chartPoint.Y.ToString("F2")}H ({chartPoint.Participation:P})",
                            Values = new ChartValues<double> {mtbiInfo.RunTime.TotalHours}
                        },

                       new PieSeries
                        {
                            Title = "RunDown",
                            DataLabels = true,
                            LabelPoint = chartPoint => $"{chartPoint.Y.ToString("F2")}H ({chartPoint.Participation:P})",
                            Values = new ChartValues<double> {mtbiInfo.RunDownTime.TotalHours}
                        },

                       new PieSeries
                        {
                            Title = "Stop",
                            DataLabels = true,
                            LabelPoint = chartPoint => $"{chartPoint.Y.ToString("F2")}H ({chartPoint.Participation:P})",
                            Values = new ChartValues<double> {mtbiInfo.StopTime.TotalHours}
                        },

                       new PieSeries
                        {
                            Title = "Alarm",
                            DataLabels = true,
                            LabelPoint = chartPoint => $"{chartPoint.Y.ToString("F2")}H ({chartPoint.Participation:P})",
                            Values = new ChartValues<double> {mtbiInfo.AlarmTime.TotalHours}
                        }
                    };
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
