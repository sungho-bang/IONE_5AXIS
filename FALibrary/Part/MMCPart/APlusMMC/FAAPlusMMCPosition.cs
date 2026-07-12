using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.MMCPart.APlusMMC
{
    [Serializable]
    public class FAAPlusMMCPosition : FAObject
    {
        private string _name;
        private double _position = 0;
        private uint _startSpeed = 0;
        private uint _driveSpeed = 0;
        private uint _accelTime = 0;
        private uint _deaccelTime = 0;

        [FAAttribute("")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        [FAAttribute("")]
        public double Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;

                _position = value;
                NotifyPropertyChanged("Position");
            }
        }

        [FAAttribute("")]
        public uint StartSpeed
        {
            get { return _startSpeed; }
            set
            {
                if (_startSpeed == value) return;

                _startSpeed = value;
                NotifyPropertyChanged("StartSpeed");
            }
        }

        [FAAttribute("")]
        public uint DriveSpeed
        {
            get { return _driveSpeed; }
            set
            {
                if (_driveSpeed == value) return;

                _driveSpeed = value;
                NotifyPropertyChanged("DriveSpeed");
            }
        }

        [FAAttribute("")]
        public uint AccelTime
        {
            get { return _accelTime; }
            set
            {
                if (_accelTime == value) return;

                _accelTime = value;
                NotifyPropertyChanged("AccelTime");
            }
        }

        [FAAttribute("")]
        public uint DeaccelTime
        {
            get { return _deaccelTime; }
            set
            {
                if (_deaccelTime == value) return;

                _deaccelTime = value;
                NotifyPropertyChanged("DeaccelTime");
            }
        }

        public void CopyTo(FAAPlusMMCPosition position)
        {
            position.AccelTime = this.AccelTime;
            position.DeaccelTime = this.DeaccelTime;
            position.DriveSpeed = this.DriveSpeed;
            position.Position = this.Position;
            position.StartSpeed = this.StartSpeed;            
        }

        public override string ToString()
        {
            string result = "";
            result += "Name = " + Name + ", ";
            result += "Position = " + Position + ", ";
            result += "StartSpeed = " + StartSpeed + ", ";
            result += "DriveSpeed = " + DriveSpeed + ", ";
            result += "AccelTime = " + AccelTime + ", ";
            result += "DeaccelTime = " + DeaccelTime;
            return result;
        }
    }
}
