using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FALibrary;

namespace FAFramework.ConfigClasses
{
    [Serializable]
    public class EquipmentStateConfigGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [FAAttribute("")]
        public EquipmentStateConfig InitializeStateConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig AutoRunStateConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig AutoStopStateConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig ErrorStateConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig WarningStateConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig RunDownStateConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig SuspendedConfig { get; set; }
        [FAAttribute("")]
        public EquipmentStateConfig NotifyMessageConfig { get; set; }

        public EquipmentStateConfigGroup()
        {
            InitializeStateConfig = new EquipmentStateConfig();
            AutoRunStateConfig = new EquipmentStateConfig();
            AutoStopStateConfig = new EquipmentStateConfig();
            ErrorStateConfig = new EquipmentStateConfig();
            WarningStateConfig = new EquipmentStateConfig();
            RunDownStateConfig = new EquipmentStateConfig();
            SuspendedConfig = new EquipmentStateConfig();
            NotifyMessageConfig = new EquipmentStateConfig();
        }
    }
}
