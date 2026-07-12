using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.RSAutomation
{
    public class FANMCMMCDevice : MMCDevice.FAMMCDevice
    {
        class GearedInfo
        {
            public ushort SlaveAxis { get; set; }
            public double Ratio { get; set; } // master : slave

            public static bool TrayParse(string str, out GearedInfo result)
            {
                result = null;
                if (string.IsNullOrEmpty(str)) return false;

                var splitResult = str.Split(':');
                if (splitResult == null || splitResult.Length != 2) return false;

                ushort axisNo = 0;
                if (ushort.TryParse(splitResult[0], out axisNo) == false) return false;

                double ratio = 0.0;
                if (double.TryParse(splitResult[1], out ratio) == false) return false;

                result = new GearedInfo { SlaveAxis = axisNo, Ratio = ratio };
                return true;
            }
        }

        Dictionary<ushort, GearedInfo[]> _axisGearedInfos = new Dictionary<ushort, GearedInfo[]>();

        public ushort BoardID { get; set; }

        private uint _axisCount;

        Dictionary<int, uint> _axisStatus = new Dictionary<int, uint>();

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            if (xml.Element("AxisGearedInfo") != null)
                LoadAxisGearedInfo(xml.Element("AxisGearedInfo"));
        }

        public override void Open()
        {
            FANMCCommon.Initialize(this, BoardID);

            _axisCount = GetAxisCount();
        }

        public override void Close()
        {
            FANMCCommon.Stop(this, BoardID);
        }

        public override int MovePos(ushort axisNo, double position, ushort speedMode, double startSpeed, double moveSpeed, double accelTime, double decTime)
        {
            int iResult = 0;
            double accel = moveSpeed / (accelTime * 0.001);
            double decel = moveSpeed / (decTime * 0.001);
            double jerk = accel * 10;

            if (SetGearIn(axisNo, accel, decel, jerk) == false) return iResult;

            //jbpark_2020.02.04
            iResult = (int)NMCSDK.NMCSDKLib.MC_MoveAbsolute(BoardID,
                axisNo,
                position,
                moveSpeed,
                accel,
                decel,
                jerk,
                NMCSDK.NMCSDKLib.MC_DIRECTION.mcPositiveDirection,//NMCSDK.NMCSDKLib.MC_DIRECTION.mcShortestWay,
                NMCSDK.NMCSDKLib.MC_BUFFER_MODE.mcAborting);

            return iResult;
        }

        public override void MoveIncPos(ushort axisNo, int position, ushort speedMode, double startSpeed, double moveSpeed, double accelTime, double decTime)
        {
            double accel = moveSpeed / (accelTime * 0.001);
            double decel = moveSpeed / (decTime * 0.001);
            double jerk = accel * 10;

            if (SetGearIn(axisNo, accel, decel, jerk) == false) return;

            NMCSDK.NMCSDKLib.MC_MoveRelative(BoardID,
                axisNo,
                position,
                moveSpeed,
                accel,
                decel,
                jerk,
                NMCSDK.NMCSDKLib.MC_BUFFER_MODE.mcAborting);
        }

        public override void MoveVelocity(ushort axisNo, double startSpeed, double moveSpeed, double accelTime, double decTime)
        {
            double absSpeed = Math.Abs(moveSpeed);
            double accel = absSpeed / (accelTime * 0.001);
            double decel = absSpeed / (decTime * 0.001);
            double jerk = accel * 10;

            if (SetGearIn(axisNo, accel, decel, jerk) == false) return;

            NMCSDK.NMCSDKLib.MC_DIRECTION dir = NMCSDK.NMCSDKLib.MC_DIRECTION.mcPositiveDirection;

            if (moveSpeed < 0)
                dir = NMCSDK.NMCSDKLib.MC_DIRECTION.mcNegativeDirection;

            NMCSDK.NMCSDKLib.MC_MoveVelocity(BoardID,
                axisNo,
                absSpeed,
                accel,
                decel,
                jerk,
                dir,
                NMCSDK.NMCSDKLib.MC_BUFFER_MODE.mcAborting);
        }

        public override bool IsMotionDone(ushort axisNo)
        {
            uint axisInfo = 0;
            NMCSDK.NMCSDKLib.MC_ReadAxisInfo(BoardID, axisNo, ref axisInfo);
            if ((axisInfo & (uint)NMCSDK.NMCSDKLib.MC_AXISINFO.mcAIMotionComplete) != 0)
                return true;
            else
                return false;
        }

        public override void MoveHome(ushort axisNo, ushort homeDir, ushort homeMode, double startSpeed, double moveSpeed, double accelTime, double offset)
        {
            SetGearOut(axisNo);

            NMCSDK.NMCSDKLib.MC_Home(BoardID,
                axisNo,
                0,
                NMCSDK.NMCSDKLib.MC_BUFFER_MODE.mcAborting);
        }

        public override void Stop(ushort axisNo)
        {
            double actualVelocity = 0;

            //NMCSDK.NMCSDKLib.MC_ReadActualVelocity(BoardID,
            //axisNo,
            //ref actualVelocity);
            NMCSDK.NMCSDKLib.MC_ReadCommandedVelocity(BoardID,
                axisNo,
                ref actualVelocity);

            double decel = Math.Abs(actualVelocity) * 1000; //20 : 50msec, 10 : 100msec
            if (decel <= 0)
            {
                decel = 1000;
            }

            double jerk = decel * 10;

            NMCSDK.NMCSDKLib.MC_Halt(BoardID,
                axisNo,
                decel,
                jerk,
                NMCSDK.NMCSDKLib.MC_BUFFER_MODE.mcAborting);
        }
        public override void MoveSpeedOverrideVelocity(ushort axisNo, double dOverrideVelocity)
        {
        }
        public override void JogPositive(ushort axisNo, double startSpeed, double moveSpeed, double accelTime)
        {
            MoveVelocity(axisNo, (int)startSpeed, (int)moveSpeed, accelTime, accelTime);
        }

        public override void JogNegative(ushort axisNo, double startSpeed, double moveSpeed, double accelTime)
        {
            MoveVelocity(axisNo, -(int)startSpeed, -(int)moveSpeed, accelTime, accelTime);
        }

        public override bool IsOrigin(ushort axisNo)
        {
            return GetStatus(axisNo, 17);
        }

        public override bool IsEncoderZ(ushort axisNo)
        {
            return false; // RS Automation 제품에서 Encoder Z를 지원하지 않음.
        }

        public override bool IsEmergency(ushort axisNo)
        {
            return false; // RS Automation 제품에서 Emergency Signal을 지원하지 않음.
        }

        public override bool IsInposition(ushort axisNo)
        {
            return GetStatus(axisNo, 26);
        }

        public override bool IsServoAlarm(ushort axisNo)
        {
            return GetStatus(axisNo, 0);
        }

        public override bool IsPositiveLimit(ushort axisNo)
        {
            return GetStatus(axisNo, 16);
        }

        public override bool IsNegativeLimit(ushort axisNo)
        {
            return GetStatus(axisNo, 15);
        }

        public override bool IsRunFlag(ushort axisNo)
        {
            return !GetStatus(axisNo, 3);
        }

        public override bool IsErrorFlag(ushort axisNo)
        {
            return GetStatus(axisNo, 0);
        }

        public override bool IsHomeFlag(ushort axisNo)
        {
            return GetStatus(axisNo, 7);
        }

        public override bool IsServoOn(ushort axisNo)
        {
            return GetStatus(axisNo, 23);
        }

        public override void ResetAlarm(ushort axisNo)
        {
            NMCSDK.NMCSDKLib.MC_Reset(BoardID,
                axisNo);
        }

        public override void ServoOn(ushort axisNo)
        {
            NMCSDK.NMCSDKLib.MC_Power(BoardID,
                axisNo,
                true);
        }

        public override void ServoOff(ushort axisNo)
        {
            NMCSDK.NMCSDKLib.MC_Power(BoardID,
                axisNo,
                false);
        }

        public override void MIO_OnOff(ushort axisNo, ushort nBitNo, bool bOn)
        {
            NMCSDK.NMCSDKLib.MC_WriteDigitalOutput(BoardID,
                axisNo,
                nBitNo,
                bOn);
        }
        public override double GetCommandPos(ushort axisNo)
        {
            return ReadParameger(axisNo, 1);
        }
        //gg
        public override double GetActualPos(ushort axisNo)
        {
            double actualPosition = 0;
            NMCSDK.NMCSDKLib.MC_ReadActualPosition(BoardID,
                axisNo,
                ref actualPosition);
            return (double)actualPosition;
            //string.Format(actualPosition 0:F2);

        }

        public override int GetCommandSpeed(ushort axisNo)
        {
            double actualVelocity = 0;

            NMCSDK.NMCSDKLib.MC_ReadActualVelocity(BoardID,
                axisNo,
                ref actualVelocity);
            return (int)actualVelocity;
        }

        public override void ResetCommandPos(ushort axisNo)
        {
            NMCSDK.NMCSDKLib.MC_SetPosition(BoardID,
                axisNo,
                0,
                false,
                NMCSDK.NMCSDKLib.MC_EXECUTION_MODE.mcImmediately);
        }

        public override void ResetActualPos(ushort axisNo)
        {
            NMCSDK.NMCSDKLib.MC_SetPosition(BoardID,
                axisNo,
                0,
                false,
                NMCSDK.NMCSDKLib.MC_EXECUTION_MODE.mcImmediately);
        }

        public override void SetLinkMode(ushort masterAxis, ushort slaveAxis, double ratio)
        {

        }

        public override void ResetLinkMode(ushort slaveAxis)
        {
            NMCSDK.NMCSDKLib.MC_GearOut(BoardID, slaveAxis);
        }

        public override bool IsHomeOk(ushort axisNo)
        {
            return GetStatus(axisNo, 24);
        }

        public override void MoveMultiPos(FAMultiMoveInfo[] info)
        {
            // RS Automation 제품에서 해당 기능을 지원하지 않음.
        }

        private bool GetStatus(ushort axisNo, int bitNumber)
        {
            if (_axisStatus.Count <= 0)
                return false;

            int result = 1;
            uint axisStatus = _axisStatus[axisNo];

            var status = (int)axisStatus & (result << bitNumber);
            if (status == 0)
                return false;
            else
                return true;
        }

        private dynamic ReadParameger(ushort axisNo, uint paramNumber)
        {
            double axisStatus = 0;
            NMCSDK.NMCSDKLib.MC_ReadParameter(BoardID, axisNo, paramNumber, ref axisStatus);
            return axisStatus;
        }

        private void LoadAxisGearedInfo(System.Xml.Linq.XElement xml)
        {
            foreach (var item in xml.Elements())
            {
                ushort masterAxis = 0;

                if (item.Element("MasterAxis") != null)
                {
                    var value = item.Element("MasterAxis").Value.Trim();
                    if (ushort.TryParse(value, out masterAxis) == false)
                        continue;
                }
                else
                    continue;

                if (item.Element("SlaveAxis") != null)
                {
                    var value = item.Element("SlaveAxis").Value.Trim();
                    var geardInfos = ParseGearedInfo(value);
                    if (geardInfos.Count == 0) continue;

                    _axisGearedInfos.Add(masterAxis, geardInfos.ToArray());
                }
                else
                    continue;
            }
        }

        private List<GearedInfo> ParseGearedInfo(string str)
        {
            List<GearedInfo> slaveAxises = new List<GearedInfo>();
            foreach (var strAxisNo in str.Split(','))
            {
                GearedInfo geardInfo;
                if (GearedInfo.TrayParse(strAxisNo, out geardInfo) == false)
                    return new List<GearedInfo>();

                slaveAxises.Add(geardInfo);
            }

            return slaveAxises;
        }

        private bool SetGearIn(ushort masterAxis, double acc, double dec, double jerk)
        {
            if (_axisGearedInfos.ContainsKey(masterAxis))
            {
                foreach (var item in _axisGearedInfos[masterAxis])
                {
                    if (IsHomeOk(item.SlaveAxis) == false) return false;

                    NMCSDK.NMCSDKLib.MC_GearIn(BoardID,
                        masterAxis,
                        item.SlaveAxis,
                        10000,
                        (uint)(item.Ratio * 10000),
                        NMCSDK.NMCSDKLib.MC_SOURCE.mcActualValue,
                        acc,
                        dec,
                        jerk,
                        NMCSDK.NMCSDKLib.MC_BUFFER_MODE.mcAborting);

                    ServoOn(item.SlaveAxis);
                }
            }

            return true;
        }

        private uint GetAxisCount()
        {
            uint totalAxisCount = 0;

            NMCSDK.NMCSDKLib.MasterGetAxesCount(
                BoardID,
                ref totalAxisCount);

            return totalAxisCount;
        }

        private void SetGearOut(ushort masterAxis)
        {
            if (_axisGearedInfos.ContainsKey(masterAxis))
            {
                foreach (var item in _axisGearedInfos[masterAxis])
                {
                    ResetLinkMode(item.SlaveAxis);
                }
            }
        }

        public override void ReadWrite()
        {
            base.ReadWrite();

            uint axisStatus = 0;

            for (ushort i = 1; i <= _axisCount; i++)
            {
                NMCSDK.NMCSDKLib.MC_ReadAxisStatus(BoardID, i, ref axisStatus);

                if (_axisStatus.ContainsKey(i))
                    _axisStatus[i] = axisStatus;
                else
                    _axisStatus.Add(i, axisStatus);
            }
        }

        //edgetechnology 2024-06-04
        public override void MoveStartTorque(int lAxisNo, double dTorque, double dVel, uint uAccFilterSel, uint uGainSel, uint uSpdLoopSel)
        {

        }
        public override void MoveTorqueStop(int lAxisNo, uint uMethod)
        {

        }

        public override void MotSetTorqueLimit(int lAxisNo, double dbPluseDirTorqueLimit, double dbMinusDirTorqueLimit)
        {

        }
        public override void MotGetTorqueLimit(int lAxisNo, ref double dbpPluseDirTorqueLimit, ref double dbpMinusDirTorqueLimit)
        {

        }
        public override void M3ServoSetTorqProfile(int lCoord, int lAxisNo, int TorqueSign, uint dwVLIM, uint dwProfileMode, uint dwStdTorq, uint dwStopTorq)
        {

        }
        public override void M3ServoGetTorqProfile(int lCoord, int lAxisNo, ref int lpTorqueSign, ref uint updwVLIM, ref uint upProfileMode, ref uint upStdTorq, ref uint upStopTorq)
        {
        }

        public override void StatusSetServoMonitor(int nAxisNo, uint uSelMon, double dActionValue, uint uAction)
        {

        }
        public override void StatusGetServoMonitor(int nAxisNo, uint uSelMon, ref double dpActionValue, ref uint upAction)
        {

        }
        public override void StatusSetServoMonitorEnable(int nAxisNo, uint uEnable)
        {

        }
        public override void StatusGetServoMonitorEnable(int nAxisNo, ref uint upEnable)
        {

        }
        public override void StatusReadServoMonitorFlag(int nAxisNo, uint uSelMon, ref uint upMonitorFlag, ref double dpMonitorValue)
        {

        }
        public override void StatusReadServoMonitorValue(int nAxisNo, uint uSelMon, ref double dpMonitorValue)
        {

        }
        public override void StatusSetReadServoLoadRatio(int lAxisNo, uint dwSelMon)
        {

        }
        public override void StatusReadServoLoadRatio(int lAxisNo, ref double dpMonitorValue)
        {

        }
        //->add
        public override void M3ServoCoordinatesSet(int lAxisNo, uint dwPosData, uint dwPos_sel, uint dwRefe)
        {

        }

        public override void M3ServoBreakOn(int lAxisNo)
        {

        }
        public override void M3ServoBreakOff(int lAxisNo)
        {

        }
        public override void M3ServoConfig(int lAxisNo, uint dwCfMode)
        {

        }
        public override void M3ServoSensOn(int lAxisNo)
        {

        }
        public override void M3ServoSensOff(int lAxisNo)
        {

        }
        public override void M3ServoSmon(int lAxisNo)
        {

        }
        public override void M3ServoGetSmon(int lAxisNo, ref uint pbParam)
        {

        }
        public override void M3ServoSvOn(int lAxisNo)
        {

        }
        public override void M3ServoSvOff(int lAxisNo)
        {

        }
        public override void M3ServoInterpolate(int lAxisNo, uint dwTPOS, uint dwVFF, uint dwTFF, uint dwTLIM)
        {

        }
        public override void M3ServoPosing(int lAxisNo, uint dwTPOS, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM)
        {

        }
        public override void M3ServoFeed(int lAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM)
        {

        }
        public override void M3ServoExFeed(int lAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2)
        {

        }
        public override void M3ServoExPosing(int lAxisNo, uint dwTPOS, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2)
        {

        }
        public override void M3ServoZret(int lAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2, uint bHomeDir, uint bHomeType)
        {

        }
        public override void M3ServoVelctrl(int lAxisNo, uint dwTFF, uint dwVREF, uint dwACCR, uint dwDECR, uint dwTLIM)
        {

        }
        public override void M3ServoTrqctrl(int lAxisNo, uint dwVLIM, int lTQREF)
        {

        }
        public override void M3ServoGetParameter(int lAxisNo, uint wNo, uint bSize, uint bMode, ref uint pbParam)
        {

        }
        public override void M3ServoSetParameter(int lAxisNo, uint wNo, uint bSize, uint bMode, ref uint pbParam)
        {

        }
        ///
    }
}
