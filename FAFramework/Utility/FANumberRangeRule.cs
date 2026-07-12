using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace FAFramework.Utility
{
    public class FANumberRangeRule : ValidationRule
    {
        public enum TryParseResult
        {
            ParseOK,
            NoNumeric,
            SignError,
            PointError,
            RangeOver
        }

        public FALibrary.FARange Range { get; set; }

        public Type NumberType { get; set; }

        public FANumberRangeRule(Type type)
        {
            NumberType = type;
            Range = new FALibrary.FARange();
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            dynamic number;
            TryParseResult result = TryParse((string)value, out number);
            if (result == TryParseResult.NoNumeric)
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "NoNumber", "No number");
                return new ValidationResult(false, msg);
            }
            else if (result == TryParseResult.PointError)
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "DoesNotAllowARealNumber", "Does not allow real number");
                return new ValidationResult(false, msg);
            }
            else if (result == TryParseResult.SignError)
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "DoesNotAllowNegativeNumber", "Does not allow negative number");
                return new ValidationResult(false, msg);
            }
            else if (result == TryParseResult.RangeOver)
            {
                var msg = Utility.UtilityClass.GetStringResource(this, "ExceededTheAllowedRange", "Exceeded the allowed range");
                return new ValidationResult(false,
                    msg +
                    " [" + Range.Min + " ~ " + Range.Max + "]");
            }
            else
            {
                if (Range != null)
                {
                    if (number < Range.Min || number > Range.Max)
                    {
                        var msg = Utility.UtilityClass.GetStringResource(this, "ExceededTheAllowedRange", "Exceeded the allowed range");
                        return new ValidationResult(false, msg
                             + " [" + Range.Min + " ~ " + Range.Max + "]");
                    }
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
