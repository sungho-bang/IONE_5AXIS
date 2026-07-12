using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAFramework.Manager
{
    public class LotManager
    {
        private List<string> _lotList = new List<string>();

        public bool IsEmpty
        {
            get
            {
                if (_lotList.Count == 0) return true;

                return false;
            }
        }

        public void AddLot(string lotID)
        {
            if (_lotList.Contains(lotID) == false)
                _lotList.Add(lotID);
        }

        public void RemoveLot(string lotID)
        {
            if (_lotList.Contains(lotID))
                _lotList.Remove(lotID);
        }

        public void AllClear()
        {
            _lotList.Clear();
        }
    }
}
