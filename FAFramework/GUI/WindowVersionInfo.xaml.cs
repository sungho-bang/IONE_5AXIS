using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.IO;

namespace FAFramework.GUI
{
    /// <summary>
    /// WindowVersionInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WindowVersionInfo : Window
    {
        static public string Version
        {
            get
            {
                string text;
                try
                {
                    StreamResourceInfo info = App.GetResourceStream(new Uri("VersionHistory.txt", UriKind.Relative));
                    StreamReader reader = new StreamReader(info.Stream);
                    text = reader.ReadLine();
                    return text.Trim();
                }
                catch
                {
                    return "";
                }
            }
        }

        public WindowVersionInfo()
        {
            InitializeComponent();

            //SetText();

            string text;
            try
            {
                StreamResourceInfo info = App.GetResourceStream(new Uri("VersionHistory.txt", UriKind.Relative));
                StreamReader reader = new StreamReader(info.Stream);
                text = reader.ReadToEnd();
            }
            catch
            {
                return;
            }

            textBlock1.Text = text;
        }

        //private void SetText()
        //{
        //    var resource = Manager.StringResourceManager.Instance.Resource.GUI;

        //    buttonClose.Content = resource.Close;
        //}

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
