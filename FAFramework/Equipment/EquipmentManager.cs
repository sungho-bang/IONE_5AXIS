using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;

namespace FAFramework.Equipment
{
    public partial class EquipmentManager
    {
        public Dictionary<string, EquipmentBase> EquipmentList { get; private set; }

        public EquipmentManager()
        {
            EquipmentList = new Dictionary<string, EquipmentBase>();
        }

        public void Initialize()
        {
            CreateEquipments();
            InitializeEquipments();
        }

        public void LoadBackup()
        {
            foreach (var item in EquipmentList)
            {
                item.Value.LoadBackup(
                    System.IO.Path.Combine(ConfigClasses.GlobalConst.PREVIOUS_BAKCUP_CONFIG_PATH,
                    item.Key));
            }
        }

        public void Save()
        {
            SaveEquipments();
        }

        public void SaveModules(params Module.FAModule[] modules)
        {
            foreach (var item in EquipmentList)
            {
                item.Value.SaveModules(modules);
            }
        }

        private void CreateEquipments()
        {
            PropertyInfo[] propList = this.GetType().GetProperties();
            foreach (PropertyInfo info in propList)
            {
                if (info.PropertyType.IsSubclassOf(typeof(EquipmentBase)) == false) continue;

                var equipment = (EquipmentBase)Activator.CreateInstance(info.PropertyType);
                equipment.Name = info.Name;

                info.SetValue(this, equipment, null);

                EquipmentList.Add(info.Name, equipment);
            }
        }

        private void InitializeEquipments()
        {
            foreach (var item in EquipmentList)
            {
                item.Value.Initialize(ConfigClasses.GlobalConst.ROOT_PATH + "config\\" +
                    item.Key + "\\");
            }
        }

        private void SaveEquipments()
        {
            foreach (var item in EquipmentList)
            {
                item.Value.Save();
            }
        }
    }
}