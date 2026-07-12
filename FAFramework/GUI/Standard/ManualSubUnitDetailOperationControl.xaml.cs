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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using FAFramework.Utility;

namespace FAFramework.GUI.Standard
{
    /// <summary>
    /// ManualSubUnitDetailOperationControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualSubUnitDetailOperationControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private Dictionary<string, PartDefineForManualOperation> _partDefineList;
        public Dictionary<string, PartDefineForManualOperation> PartDefineList
        {
            get { return _partDefineList; }
            set
            {
                _partDefineList = value;
                NotifyPropertyChanged("PartDefineList");
            }
        }

        public ManualSubUnitDetailOperationControl()
        {
            InitializeComponent();
        }

        public void ClickOperation(object sender, RoutedEventArgs e)
        {
            var tag = (sender as Button).Tag;
            if (tag == null) return;

            var owner = System.Windows.Application.Current.Windows.OfType<System.Windows.Window>().SingleOrDefault(x => x.IsActive);
            PartActionWindow dlg = new PartActionWindow();
            dlg.PartDefine = tag as PartDefineForManualOperation;
            dlg.Owner = owner;
            dlg.ShowDialog();
        }
    }
}
