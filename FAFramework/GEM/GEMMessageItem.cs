using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.GEM
{
    public enum GEMItemType
    {
        Binary,
        Bool,
        Ascii,
        J8,
        SignedInt1,
        SignedInt2,
        SignedInt4,
        SignedInt8,
        UnsignedInt1,
        UnsignedInt2,
        UnsignedInt4,
        UnsignedInt8,
        Float4,
        Float8,
    }

    public class GEMMessageItem
    {
        private List<GEMMessageItem> _subItems = new List<GEMMessageItem>();
        private GEMItemType ItemType { get; set; }
        private object Value { get; set; }

        public GEMMessageItem(short[] value)
        {
            ItemType = GEMItemType.Binary;
            Value = value;
        }

        public GEMMessageItem(bool value)
        {
            ItemType = GEMItemType.Bool;
            if (value)
                Value = (short)1;
            else
                Value = (short)0;
        }

        public GEMMessageItem(string value)
        {
            ItemType = GEMItemType.Ascii;
            Value = value;
        }

        public GEMMessageItem(sbyte value)
        {
            
            ItemType = GEMItemType.SignedInt1;
            Value = value;
        }

        public GEMMessageItem(short value)
        {
            ItemType = GEMItemType.SignedInt2;
            Value = value;
        }

        public GEMMessageItem(int value)
        {
            ItemType = GEMItemType.SignedInt4;
            Value = value;
        }

        public GEMMessageItem(long value)
        {
            ItemType = GEMItemType.SignedInt8;
            Value = value;
        }

        public GEMMessageItem(byte value)
        {
            ItemType = GEMItemType.UnsignedInt1;
            Value = value;
        }

        public GEMMessageItem(ushort value)
        {
            ItemType = GEMItemType.UnsignedInt2;
            Value = value;
        }

        public GEMMessageItem(uint value)
        {
            ItemType = GEMItemType.UnsignedInt4;
            Value = value;
        }

        public GEMMessageItem(ulong value)
        {
            ItemType = GEMItemType.UnsignedInt8;
            Value = value;
        }

        public GEMMessageItem(float value)
        {
            ItemType = GEMItemType.Float4;
            Value = value;
        }

        public GEMMessageItem(double value)
        {
            ItemType = GEMItemType.Float8;
            Value = value;
        }

        public GEMMessageItem AddSubItem(short[] value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(bool value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(string value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(sbyte value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(short value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(int value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(long value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(byte value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(ushort value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(uint value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(ulong value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(float value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(double value)
        {
            var subItem = new GEMMessageItem(value);
            _subItems.Add(subItem);
            return subItem;
        }

        public GEMMessageItem AddSubItem(GEMMessageItem subItem)
        {
            _subItems.Add(subItem);
            return subItem;
        }

        public void AddToGEM(AxEZGEMLib.AxEZGEM gem, int id)
        {
            AddToGEM(gem, id, ItemType, Value);

            if (HasSubItems())
            {
                gem.OpenListItem(id);
                gem.OpenListItem(id);

                foreach (var item in _subItems)
                {
                    item.AddToGEM(gem, id);
                }

                gem.CloseListItem(id);
                gem.CloseListItem(id);
            }
        }

        public bool HasSubItems()
        {
            return _subItems.Count() > 0;
        }

        private void AddToGEM(AxEZGEMLib.AxEZGEM gem, int id, GEMItemType itemType, object value)
        {
            switch (itemType)
            {
                case GEMItemType.Binary:
                    {
                        var binary = (short[])value;
                        gem.AddBinaryItem(id, ref binary[0], binary.Length);
                        break;
                    }

                case GEMItemType.Bool:
                    {
                        var v = (short)value;
                        gem.AddBoolItem(id, ref v, 1);
                        break;
                    }

                case GEMItemType.Ascii:
                    {
                        var v = (string)value;
                        gem.AddAsciiItem(id, v, v.Length);
                        break;
                    }

                case GEMItemType.SignedInt1:
                    {
                        var v = (short)value;
                        gem.AddI1Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.SignedInt2:
                    {
                        var v = (short)value;
                        gem.AddI2Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.SignedInt4:
                    {
                        var v = (int)value;
                        gem.AddI4Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.SignedInt8:
                    {
                        var v = (int)value;
                        gem.AddI8Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.UnsignedInt1:
                    {
                        var v = (short)value;
                        gem.AddU1Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.UnsignedInt2:
                    {
                        var v = (int)value;
                        gem.AddU2Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.UnsignedInt4:
                    {
                        var v = (double)value;
                        gem.AddU4Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.UnsignedInt8:
                    {
                        var v = (double)value;
                        gem.AddU8Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.Float4:
                    {
                        var v = (float)value;
                        gem.AddF4Item(id, ref v, 1);
                        break;
                    }

                case GEMItemType.Float8:
                    {
                        var v = (double)value;
                        gem.AddF8Item(id, ref v, 1);
                        break;
                    }
            }
        }
    }
}
