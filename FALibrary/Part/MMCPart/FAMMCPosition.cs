using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Part.MMCPart
{
    [Serializable]
    public class FAMMCPosition : FAObject
    {
        private string _name;
        private double _position = 0;
        private uint _startSpeed = 0;
        private double _driveSpeed = 0;
        private uint _accelTime = 0;
        private uint _decelTime = 0;

        // ★ 추가: 위치/토크에 대한 Low / Up Limit
        private double _lwLimit = -999.9;
        private double _upLimit = 999.9;

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
        public double DriveSpeed
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
        public uint DecelTime
        {
            get { return _decelTime; }
            set
            {
                if (_decelTime == value) return;

                _decelTime = value;
                NotifyPropertyChanged("DecelTime");
            }
        }

        // ★ 추가: Lw Limit
        [FAAttribute("")]
        public double LwLimit
        {
            get { return _lwLimit; }
            set
            {
                if (Math.Abs(_lwLimit - value) < double.Epsilon) return;

                _lwLimit = value;
                NotifyPropertyChanged("LwLimit");
            }
        }

        // ★ 추가: Up Limit
        [FAAttribute("")]
        public double UpLimit
        {
            get { return _upLimit; }
            set
            {
                if (Math.Abs(_upLimit - value) < double.Epsilon) return;

                _upLimit = value;
                NotifyPropertyChanged("UpLimit");
            }
        }

        public void CopyTo(FAMMCPosition position)
        {
            position.AccelTime = this.AccelTime;
            position.DecelTime = this.DecelTime;
            position.DriveSpeed = this.DriveSpeed;
            position.Position = this.Position;
            position.StartSpeed = this.StartSpeed;

            // ★ 추가: Limit 값도 같이 복사
            position.LwLimit = this.LwLimit;
            position.UpLimit = this.UpLimit;
        }

        public override string ToString()
        {
            string result = "";
            result += "Name = " + Name + ", ";
            result += "Position = " + Position + ", ";
            result += "StartSpeed = " + StartSpeed + ", ";
            result += "DriveSpeed = " + DriveSpeed + ", ";
            result += "AccelTime = " + AccelTime + ", ";
            result += "DecelTime = " + DecelTime + ", ";
            result += "LwLimit = " + LwLimit + ", ";
            result += "UpLimit = " + UpLimit;
            return result;
        }
    }
}
