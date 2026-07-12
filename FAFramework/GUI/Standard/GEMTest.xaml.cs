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
using System.IO;
using System.ComponentModel;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// GEMTest.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GEMTest : UserControl, INotifyPropertyChanged
    {
        readonly int MaxLineCount = 300;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public FAFramework.Utility.CommandHandler SendEventReportCommand
        {
            get;
            set;
        }

        public static readonly DependencyProperty LogUpdateProperty = DependencyProperty.Register("LogUpdate",
            typeof(bool), typeof(GEMTest));

        public bool LogUpdate
        {
            get { return (bool)this.GetValue(LogUpdateProperty); }
            set
            {
                this.SetValue(LogUpdateProperty, this);
            }
        }

        public string Log { get; private set; }

        public bool AutoLogUpdate { get; set; }

        public int UpdateInterval { get; set; }

        public GEMTest()
        {
            SendEventReportCommand = new Utility.CommandHandler(
                delegate (object param)
                {
                    try
                    {
                        if (param == null) return;

                        var eventReport = param as GEM.EventReport;
                        foreach (var item in eventReport.SVIDDefine)
                            GEM.GEMManager.Instance.GEM.SetCurrentStatusValue(item.ID, item.Value);

                        GEM.GEMManager.Instance.GEM.SendEventReport(eventReport.CEID.ID);
                    }
                    catch (Exception e)
                    {
                        Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                    }
                },
                true);

            GEM.GEMManager.Instance.GEM.OnSecsMsgIn +=
                delegate
                {
                    LoadGEMLog();
                };

            GEM.GEMManager.Instance.GEM.OnSecsMsgOut +=
                delegate
                {
                    LoadGEMLog();
                };

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick +=
                delegate
                {
                    if (AutoLogUpdate && UpdateInterval > 0)
                    {
                        if (stopwatch.ElapsedMilliseconds > UpdateInterval * 1000)
                        {
                            LoadGEMLog();
                            stopwatch.Restart();
                        }
                    }
                };
            stopwatch.Start();
            timer.Start();

            InitializeComponent();
        }

        private void LoadGEMLog()
        {
            try
            {
                if (!AutoLogUpdate) return;
                if (!LogUpdate) return;
                int lineCount = MaxLineCount;
                var logFile = GEM.GEMManager.Instance.LogFileName;
                if (File.Exists(logFile))
                {
                    var allLines = File.ReadAllLines(logFile);
                    if (allLines.Length < MaxLineCount)
                        lineCount = allLines.Length;

                    Log = string.Join("\n", allLines, allLines.Length - lineCount, lineCount);
                }
            }
            catch
            {

            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.ScrollToEnd();
        }
    }
}
