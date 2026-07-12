using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Part;

namespace FAFramework.Utility
{
    public static class PartToPartDefineConvertor
    {
        public static PartDefineForManualOperation Convert(FAPart part)
        {
            if (part is FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric)
                return ConvertMemoryBaseGenericPart(part as FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric);
            else if (part is FALibrary.Part.MemoryBasePart.FAPartOneWayACMotor)
                return ConvertOneWayACMotorPart(part as FALibrary.Part.MemoryBasePart.FAPartOneWayACMotor);
            else if (part is FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor)
                return ConvertTwoWayACMotorPart(part as FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor);

            return null;
        }

        public static void ExtractPartAction(FAPart part, List<AliasPartAction> result)
        {
            if (part == null) return;

            var properties = part.GetType().GetProperties();

            foreach (var item in properties)
            {
                if (item.PropertyType != typeof(FAPartAction)) continue;

                var propertyValue = item.GetValue(part, null);
                if (propertyValue == null) continue;

                result.Add(propertyValue as AliasPartAction);
            }
        }

        public static PartDefineForManualOperation ConvertMemoryBaseGenericPart(FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric part)
        {
            if ((part is FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric) == false) return null;

            PartDefineForManualOperation result = new PartDefineForManualOperation();

            result.Name = part.Name;

            var onActon = new AliasPartAction(part.TurnOnAction.ActionName,
                part.TurnOnAction.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        if (part.IsTurnOn() == true) return true;

                        return false;
                    }));

            var offActon = new AliasPartAction(part.TurnOffAction.ActionName,
                part.TurnOffAction.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        if (part.IsTurnOff() == true) return true;

                        return false;
                    }));

            result.AddAction(onActon);
            result.AddAction(offActon);

            result.StatusObjectList = ObjectElementExtractor.ExtractElement(part, string.Empty, false, null, null, new string[] { "Status", "InputIO", "OutputIO" });

            return result;
        }

        public static PartDefineForManualOperation ConvertOneWayACMotorPart(FALibrary.Part.MemoryBasePart.FAPartOneWayACMotor part)
        {
            if ((part is FALibrary.Part.MemoryBasePart.FAPartOneWayACMotor) == false) return null;

            PartDefineForManualOperation result = new PartDefineForManualOperation();

            result.Name = part.Name;

            var runActon = new AliasPartAction(part.Run.ActionName,
                part.Run.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        return true;
                    }));

            var stopActon = new AliasPartAction(part.Stop.ActionName,
                part.Stop.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        return true;
                    }));

            result.AddAction(runActon);
            result.AddAction(stopActon);

            result.StatusObjectList = ObjectElementExtractor.ExtractElement(part, string.Empty, false, null, null, new string[] { "Status", "InputIO", "OutputIO" });

            return result;
        }

        public static PartDefineForManualOperation ConvertTwoWayACMotorPart(FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor part)
        {
            if ((part is FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor) == false) return null;

            PartDefineForManualOperation result = new PartDefineForManualOperation();

            result.Name = part.Name;

            var runActon = new AliasPartAction(part.RunAction.ActionName,
                part.RunAction.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        return true;
                    }));

            var reverseRunActon = new AliasPartAction(part.ReverseRunAction.ActionName,
                part.ReverseRunAction.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        return true;
                    }));

            var stopActon = new AliasPartAction(part.StopAction.ActionName,
                part.StopAction.Execute,
                new Func<bool>(
                    delegate ()
                    {
                        return true;
                    }));

            result.AddAction(runActon);
            result.AddAction(reverseRunActon);
            result.AddAction(stopActon);

            result.StatusObjectList = ObjectElementExtractor.ExtractElement(part, string.Empty, false, null, null, new string[] { "Status", "InputIO", "OutputIO" });

            return result;
        }
    }
}