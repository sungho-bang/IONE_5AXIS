using System.Collections.Generic;

namespace FAFramework.GEM
{
    public class CEID
    {
        public int ID { get; set; }

        private List<(int svid, string value)> SVIDs = new List<(int svid, string value)>();

        public (int svid, string value)[] GetSVIDs()
        {
            return SVIDs.ToArray();
        }

        public void AddSVID(int svid, string value)
        {
            SVIDs.Add((svid, value));
        }

        public void AddSVID(int svid, bool value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, long value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, int value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, short value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, sbyte value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, ulong value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, uint value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, ushort value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, byte value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, double value)
        {
            AddSVID(svid, value.ToString());
        }

        public void AddSVID(int svid, float value)
        {
            AddSVID(svid, value.ToString());
        }
    }
}
