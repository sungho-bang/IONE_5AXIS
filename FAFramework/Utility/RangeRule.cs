using System;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace FAFramework.Utility
{
    public class NumberRangeRule : ValidationRule
    {
        public enum TryParseResult
        {
            ParseOK,
            NoNumeric,
            SignError,
            PointError,
            RangeOver
        }

        public Type NumberType { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }

        public NumberRangeRule()
        {
        }

        public NumberRangeRule(Type type)
        {
            NumberType = type;
        }

        public NumberRangeRule(Type type, double min, double max)
        {
            NumberType = type;
            Min = min;
            Max = max;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            dynamic number;
            TryParseResult result = TryParse((string)value, out number);
            if (result == TryParseResult.NoNumeric)
                return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "NoNumber", "No Number"));
            else if (result == TryParseResult.PointError)
                return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "DoesNotAllowARealNumber", "Does Not Allow A Real Number"));
            else if (result == TryParseResult.SignError)
                return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "DoesNotAllowANegativeNumber", "Does Not Allow A Negative Number"));
            else if (result == TryParseResult.RangeOver)
                return new ValidationResult(false,
                    UtilityClass.GetStringResource(App.Current.MainWindow, "ExceededTheAllowedRange", "Exceeded The Allowed Range") +
                    " [" + Min + " ~ " + Max + "]");
            else
            {
                if (number < Min || number > Max)
                {
                    return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "ExceededTheAllowedRange", "Exceeded The Allowed Range")
                            + " [" + Min + " ~ " + Max + "]");
                }
            }

            return ValidationResult.ValidResult;
        }

        public TryParseResult TryParse(string value, out dynamic result)
        {
            result = 0;
            if (NumberType == typeof(sbyte))
            {
                sbyte number;
                if (!sbyte.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsIntegerValue(value))
                            return TryParseResult.RangeOver;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(byte))
            {
                byte number = 0;
                if (!byte.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (IsUnsignedIntegerValue(value))
                        {
                            return TryParseResult.RangeOver;
                        }
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(Int16))
            {
                Int16 number;
                if (!Int16.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsIntegerValue(value))
                            return TryParseResult.RangeOver;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(UInt16))
            {
                UInt16 number;
                if (!UInt16.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (IsUnsignedIntegerValue(value))
                        {
                            return TryParseResult.RangeOver;
                        }
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(int))
            {
                int number;
                if (!int.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsIntegerValue(value))
                            return TryParseResult.RangeOver;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(uint))
            {
                uint number;
                if (!uint.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (IsUnsignedIntegerValue(value))
                        {
                            return TryParseResult.RangeOver;
                        }
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(long))
            {
                long number;
                if (!long.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(ulong))
            {
                ulong number;
                if (!ulong.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(float))
            {
                float number;
                if (!float.TryParse((string)value, out number))
                {
                    if (IsRealValue(value)) return TryParseResult.RangeOver;
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(double))
            {
                double number;
                if (!double.TryParse((string)value, out number))
                {
                    return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }

            return TryParseResult.NoNumeric;
        }

        public bool IsRealValue(string value)
        {
            double number;
            return double.TryParse(value, out number);
        }

        public bool IsIntegerValue(string value)
        {
            Int64 number;
            return Int64.TryParse(value, out number);
        }

        public bool IsUnsignedIntegerValue(string value)
        {
            UInt64 number;
            return UInt64.TryParse(value, out number);
        }

        public bool IsLessThanZero(string value)
        {
            double number;
            if (double.TryParse(value, out number))
            {
                if (number < 0) return true;
                else return false;
            }

            return false;
        }
    }

    public class WrappedFARange : ValidationRule
    {
        public FALibrary.FARange Range { get; set; }

        public Type NumberType { get; set; }

        public enum TryParseResult
        {
            ParseOK,
            NoNumeric,
            SignError,
            PointError,
            RangeOver
        }

        public double Min
        {
            get { return Range.Min; }
            set
            {
                Range.Min = value;
            }
        }

        public double Max
        {
            get { return Range.Max; }
            set
            {
                Range.Max = value;
            }
        }

        public WrappedFARange()
        {
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            dynamic number;
            TryParseResult result = TryParse((string)value, out number);
            if (result == TryParseResult.NoNumeric)
                return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "NoNumber", "No Number"));
            else if (result == TryParseResult.PointError)
                return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "DoesNotAllowARealNumber", "Does Not Allow A Real Number"));
            else if (result == TryParseResult.SignError)
                return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "DoesNotAllowANegativeNumber", "Does Not Allow A Negative Number"));
            else if (result == TryParseResult.RangeOver)
                return new ValidationResult(false,
                    UtilityClass.GetStringResource(App.Current.MainWindow, "ExceededTheAllowedRange", "Exceeded The Allowed Range") +
                    " [" + Min + " ~ " + Max + "]");
            else
            {
                if (number < Min || number > Max)
                {
                    return new ValidationResult(false, UtilityClass.GetStringResource(App.Current.MainWindow, "ExceededTheAllowedRange", "Exceeded The Allowed Range")
                            + " [" + Min + " ~ " + Max + "]");
                }
            }

            return ValidationResult.ValidResult;
        }

        public TryParseResult TryParse(string value, out dynamic result)
        {
            result = 0;
            if (NumberType == typeof(sbyte))
            {
                sbyte number;
                if (!sbyte.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsIntegerValue(value))
                            return TryParseResult.RangeOver;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(byte))
            {
                byte number = 0;
                if (!byte.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (IsUnsignedIntegerValue(value))
                        {
                            return TryParseResult.RangeOver;
                        }
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(Int16))
            {
                Int16 number;
                if (!Int16.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsIntegerValue(value))
                            return TryParseResult.RangeOver;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(UInt16))
            {
                UInt16 number;
                if (!UInt16.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (IsUnsignedIntegerValue(value))
                        {
                            return TryParseResult.RangeOver;
                        }
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(int))
            {
                int number;
                if (!int.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsIntegerValue(value))
                            return TryParseResult.RangeOver;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(uint))
            {
                uint number;
                if (!uint.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (IsUnsignedIntegerValue(value))
                        {
                            return TryParseResult.RangeOver;
                        }
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(long))
            {
                long number;
                if (!long.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(ulong))
            {
                ulong number;
                if (!ulong.TryParse((string)value, out number))
                {
                    if (IsRealValue(value))
                    {
                        if (IsLessThanZero(value)) return TryParseResult.SignError;
                        else if (value.IndexOf('.') >= 0) return TryParseResult.PointError;
                        return TryParseResult.RangeOver;
                    }
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(float))
            {
                float number;
                if (!float.TryParse((string)value, out number))
                {
                    if (IsRealValue(value)) return TryParseResult.RangeOver;
                    else return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }
            else if (NumberType == typeof(double))
            {
                double number;
                if (!double.TryParse((string)value, out number))
                {
                    return TryParseResult.NoNumeric;
                }

                result = number;
                return TryParseResult.ParseOK;
            }

            return TryParseResult.NoNumeric;
        }

        public bool IsRealValue(string value)
        {
            double number;
            return double.TryParse(value, out number);
        }

        public bool IsIntegerValue(string value)
        {
            Int64 number;
            return Int64.TryParse(value, out number);
        }

        public bool IsUnsignedIntegerValue(string value)
        {
            UInt64 number;
            return UInt64.TryParse(value, out number);
        }

        public bool IsLessThanZero(string value)
        {
            double number;
            if (double.TryParse(value, out number))
            {
                if (number < 0) return true;
                else return false;
            }

            return false;
        }
    }
}
