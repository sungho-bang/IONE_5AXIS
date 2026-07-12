using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.Utility
{
    public static class Assign
    {
        public static void AssignPartToModule(Module.FAModule module, Equipment.SubUnitBase unit)
        {
            var propertiesInfo = UtilityClass.GetAllFAAttributePropertiesNames(unit, false);
            var partsOfUnit = propertiesInfo.Properties.Where(x => x.Value != null && x.Value is FALibrary.Part.FAPart);
            var moduleType = module.GetType();
            foreach (var item in partsOfUnit)
            {
                var propertyInfo = moduleType.GetProperty(item.PropertyName);
                if (propertyInfo != null &&
                    propertyInfo.PropertyType.IsAssignableFrom(item.Value.GetType()))
                {
                    propertyInfo.SetValue(module, item.Value);
                }
            }
        }
    }
}
