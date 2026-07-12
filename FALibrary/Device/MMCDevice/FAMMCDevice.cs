using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.MMCDevice
{
    public abstract class FAMMCDevice : FADevice
    {
        public class FAMultiMoveInfo
        {
            public ushort AxisNo { get; set; }
            public double Positions { get; set; }
            public double MaxVelocity { get; set; }
            public double MaxAccel { get; set; }
            public double MaxDecel { get; set; }
        }



        public abstract int MovePos(ushort axisNo, double position, ushort speedMode, double startSpeed, double moveSpeed, double accelTime, double DecTime);
        public abstract void MoveIncPos(ushort axisNo, int position, ushort speedMode, double startSpeed, double moveSpeed, double accelTime, double DecTime);
        public abstract void MoveVelocity(ushort axisNo, double startSpeed, double moveSpeed, double accelTime, double decTime);
        public abstract void MoveSpeedOverrideVelocity(ushort axisNo, double dOverrideVelocity);
        public abstract bool IsMotionDone(ushort axisNo);
        public abstract void MoveHome(ushort axisNo, ushort homeDir, ushort homeMode, double startSpeed, double moveSpeed, double accelTime, double offset);
        public abstract void Stop(ushort axisNo);
        public abstract void JogPositive(ushort axisNo, double startSpeed, double moveSpeed, double accelTime);
        public abstract void JogNegative(ushort axisNo, double startSpeed, double moveSpeed, double accelTime);
        public abstract bool IsOrigin(ushort axisNo);
        public abstract bool IsEncoderZ(ushort axisNo);
        public abstract bool IsEmergency(ushort axisNo);
        public abstract bool IsInposition(ushort axisNo);
        public abstract bool IsServoAlarm(ushort axisNo);
        public abstract bool IsPositiveLimit(ushort axisNo);
        public abstract bool IsNegativeLimit(ushort axisNo);
        public abstract bool IsRunFlag(ushort axisNo);
        public abstract bool IsErrorFlag(ushort axisNo);
        public abstract bool IsHomeFlag(ushort axisNo);
        public abstract bool IsServoOn(ushort axisNo);
        public abstract void ResetAlarm(ushort axisNo);
        public abstract void ServoOn(ushort axisNo);
        public abstract void ServoOff(ushort axisNo);
        public abstract void MIO_OnOff(ushort axisNo, ushort nBitNo, bool bOn);
        public abstract double GetCommandPos(ushort axisNo);
        public abstract double GetActualPos(ushort axisNo);
        public abstract int GetCommandSpeed(ushort axisNo);
        public abstract void ResetCommandPos(ushort axisNo);
        public abstract void ResetActualPos(ushort axisNo);
        public abstract void SetLinkMode(ushort masterAxis, ushort slaveAxis, double ratio);
        public abstract void ResetLinkMode(ushort masterAxis);
        public abstract bool IsHomeOk(ushort axisNo);
        public abstract void MoveMultiPos(FAMultiMoveInfo[] info);

        //edgetechnology 2024-06-04


        public abstract void MoveStartTorque(int lAxisNo, double dTorque, double dVel, uint uAccFilterSel, uint uGainSel, uint uSpdLoopSel);
        public abstract void MoveTorqueStop(int lAxisNo, uint uMethod);
        public abstract void MotSetTorqueLimit(int lAxisNo, double dbPluseDirTorqueLimit, double dbMinusDirTorqueLimit);

        public abstract void MotGetTorqueLimit(int lAxisNo, ref double dbpPluseDirTorqueLimit, ref double dbpMinusDirTorqueLimit);
        public abstract void M3ServoSetTorqProfile(int lCoord, int lAxisNo, int TorqueSign, uint dwVLIM, uint dwProfileMode, uint dwStdTorq, uint dwStopTorq);
        public abstract void M3ServoGetTorqProfile(int lCoord, int lAxisNo, ref int lpTorqueSign, ref uint updwVLIM, ref uint upProfileMode, ref uint upStdTorq, ref uint upStopTorq);
        public abstract void StatusSetServoMonitor(int nAxisNo, uint uSelMon, double dActionValue, uint uAction);
        public abstract void StatusGetServoMonitor(int nAxisNo, uint uSelMon, ref double dpActionValue, ref uint upAction);
        public abstract void StatusSetServoMonitorEnable(int nAxisNo, uint uEnable);
        public abstract void StatusGetServoMonitorEnable(int nAxisNo, ref uint upEnable);
        public abstract void StatusReadServoMonitorFlag(int nAxisNo, uint uSelMon, ref uint upMonitorFlag, ref double dpMonitorValue);
        public abstract void StatusReadServoMonitorValue(int nAxisNo, uint uSelMon, ref double dpMonitorValue);
        public abstract void StatusSetReadServoLoadRatio(int lAxisNo, uint dwSelMon);
        public abstract void StatusReadServoLoadRatio(int lAxisNo, ref double dpMonitorValue);
        //->add
        public abstract void M3ServoCoordinatesSet(int lAxisNo, uint dwPosData, uint dwPos_sel, uint dwRefe);
        public abstract void M3ServoBreakOn(int lAxisNo);
        public abstract void M3ServoBreakOff(int lAxisNo);
        public abstract void M3ServoConfig(int lAxisNo, uint dwCfMode);
        public abstract void M3ServoSensOn(int lAxisNo);
        public abstract void M3ServoSensOff(int lAxisNo);
        public abstract void M3ServoSmon(int lAxisNo);
        public abstract void M3ServoGetSmon(int lAxisNo, ref uint pbParam);
        public abstract void M3ServoSvOn(int lAxisNo);
        public abstract void M3ServoSvOff(int lAxisNo);
        public abstract void M3ServoInterpolate(int lAxisNo, uint dwTPOS, uint dwVFF, uint dwTFF, uint dwTLIM);
        public abstract void M3ServoPosing(int lAxisNo, uint dwTPOS, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM);
        public abstract void M3ServoFeed(int lAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM);
        public abstract void M3ServoExFeed(int lAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2);
        public abstract void M3ServoExPosing(int lAxisNo, uint dwTPOS, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2);
        public abstract void M3ServoZret(int lAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2, uint bHomeDir, uint bHomeType);
        public abstract void M3ServoVelctrl(int lAxisNo, uint dwTFF, uint dwVREF, uint dwACCR, uint dwDECR, uint dwTLIM);
        public abstract void M3ServoTrqctrl(int lAxisNo, uint dwVLIM, int lTQREF);
        public abstract void M3ServoGetParameter(int lAxisNo, uint wNo, uint bSize, uint bMode, ref uint pbParam);
        public abstract void M3ServoSetParameter(int lAxisNo, uint wNo, uint bSize, uint bMode, ref uint pbParam);
        //

    }
}
