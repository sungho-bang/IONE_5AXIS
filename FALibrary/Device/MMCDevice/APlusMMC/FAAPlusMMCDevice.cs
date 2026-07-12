using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace FALibrary.Device.MMCDevice.APlusMMC
{
    public partial class FAAPlusMMCDevice
    {
        //TMC302A_GetCardStatus( )
        //[Software Check]는 논리 변경 시 반환 값이 반전됩니다.
        public readonly int CARD_STATUS_ORIGIN = 0;     //0x0001 1 ORG IN Origin(Home) Sensor (원점 센서) [Software Check]
        public readonly int CARD_STATUS_ENCODER_Z = 1;     //0x0002 2 EZ IN Encoder Z (엔코더 Z상) [Software Check]
        public readonly int CARD_STATUS_EMERGENCY = 2;     //0x0004 3 EMG IN Emergency (비상정지) [Hardware Check]
        public readonly int CARD_STATUS_INP = 3;     //0x0008 4 INP IN Servo Inposition (서보 위치 결정 완료) [Software Check]
        public readonly int CARD_STATUS_SERVO_ALARM = 4;    //0x0010 5 ALM IN Servo Alarm (서보 알람) [Software Check]
        public readonly int CARD_STATUS_LIMIT_PLUS = 5;    //0x0020 6 LMT+ IN +Limit Sensor (+리미트 센서) [Software Check]
        public readonly int CARD_STATUS_LIMIT_MINUS = 6;    //0x0040 7 LMT- IN -Limit Sensor (-리미트 센서) [Software Check]
        //0x0000 8 N.C N.C N.C
        public readonly int CARD_STATUS_RUN_FLAG = 8;   //0x0100 9 RUN FLAG Motion 수행 중
        public readonly int CARD_STATUS_ERR_FLAG = 9;   //0x0200 10 ERR FLAG Error 발생
        public readonly int CARD_STATUS_HOME_FLAG = 10;   //0x0400 11 HOME FLAG 원점 복귀 Motion 수행 중
        //0x0000 12 N.C N.C N.C
        //0x0000 13 N.C N.C N.C
        public readonly int CARD_STATUS_COUNTER_CLR = 13;  //0x2000 14 C.CLR OUT Servo Error Counter Clear (서보 편차 카운터 클리어)
        public readonly int CARD_STATUS_SERVO_ON = 14;  //0x4000 15 SON OUT Servo On (서보 온)
        public readonly int CARD_STATUS_ALARM_RESET = 15;  //0x8000 16 RST OUT Servo Alarm Reset (서보 알람 리셋)



        //TMC302A_GetMainStatus( )
        public readonly int MAIN_STATUS_RUN_AXIS0 = 0;     //0x0001 1 RUN N.C N.C N.C 0축(4축) RUN (Motion 수행 중)
        public readonly int MAIN_STATUS_RUN_AXIS1 = 1;     //0x0002 2 N.C RUN N.C N.C 1축(5축) RUN (Motion 수행 중)
        public readonly int MAIN_STATUS_RUN_AXIS2 = 2;     //0x0004 3 N.C N.C RUN N.C 2축(6축) RUN (Motion 수행 중)
        public readonly int MAIN_STATUS_RUN_AXIS3 = 3;     //0x0008 4 N.C N.C N.C RUN 3축(7축) RUN (Motion 수행 중)
        public readonly int MAIN_STATUS_ERR_AXIS0 = 4;    //0x0010 5 ERR N.C N.C N.C 0축(4축) ERROR (Error 발생)
        public readonly int MAIN_STATUS_ERR_AXIS1 = 5;    //0x0020 6 N.C ERR N.C N.C 1축(5축) ERROR (Error 발생)
        public readonly int MAIN_STATUS_ERR_AXIS2 = 6;    //0x0040 7 N.C N.C ERR N.C 2축(6축) ERROR (Error 발생)
        public readonly int MAIN_STATUS_ERR_AXIS3 = 7;    //0x0080 8 N.C N.C N.C ERR 3축(7축) ERROR (Error 발생)
        public readonly int MAIN_STATUS_HOME_AXIS0 = 8;   //0x0100 9 HOME N.C N.C N.C 0축(4축) HOME (원점 복귀 Motion 수행 중)
        public readonly int MAIN_STATUS_HOME_AXIS1 = 9;   //0x0200 10 N.C HOME N.C N.C 1축(5축) HOME (원점 복귀 Motion 수행 중)
        public readonly int MAIN_STATUS_HOME_AXIS2 = 10;   //0x0400 11 N.C N.C HOME N.C 2축(6축) HOME (원점 복귀 Motion 수행 중)
        public readonly int MAIN_STATUS_HOME_AXIS3 = 11;   //0x0800 12 N.C N.C N.C HOME 3축(7축) HOME (원점 복귀 Motion 수행 중)

        //TMC302A_GetDrvStatus( )
        public readonly int DRV_STATUS_CMP_PLUS = 0;     //0x0001 1 CMP+ Position 값이 COMP+ 값보다 크거나 같을 때
        public readonly int DRV_STATUS_CMP_MINUS = 1;     //0x0002 2 CMP- Position 값이 COMP- 값보다 작을 때
        public readonly int DRV_STATUS_ASND = 2;     //0x0004 3 ASND 직선 가감속에서 가속할 때
        public readonly int DRV_STATUS_CNST = 3;     //0x0008 4 CNST 직선 가감속에서 등속할 때
        public readonly int DRV_STATUS_DSND = 4;    //0x0010 5 DSND 직선 가감속에서 감속할 때
        public readonly int DRV_STATUS_AASND = 5;    //0x0020 6 AASND S자 가감속에서 가감속도가 증가할 때
        public readonly int DRV_STATUS_ACNST = 6;    //0x0040 7 ACNST S자 가감속에서 가감속도가 일정할 때
        public readonly int DRV_STATUS_ADSND = 7;    //0x0080 8 ADSND S자 가감속에서 가감속도가 감소할 때
        //0x0000 9 N.C N.C
        public readonly int DRV_STATUS_STOP_ORG = 9;   //0x0200 10 S-ORG ORG 신호에 의해 정지할 때
        public readonly int DRV_STATUS_STOP_EZ = 10;   //0x0400 11 S-EZ EZ 신호에 의해 정지할 때
        //0x0000 12 N.C N.C
        public readonly int DRV_STATUS_STOP_LIMIT_PLUS = 12;  //0x1000 13 S-LMT+ LMT+ 신호에 의해 정지할 때
        public readonly int DRV_STATUS_STOP_LIMIT_MINUS = 13;  //0x2000 14 S-LMT- LMT- 신호에 의해 정지할 때
        public readonly int DRV_STATUS_STOP_ALARM = 14;  //0x4000 15 S-ALM ALM 신호에 의해 정지할 때
        public readonly int DRV_STATUS_STOP_EMG = 15;  //0x8000 16 S-EMG EMG 신호에 의해 정지할 때

        //디바이스 시작 종료

        /* Loading/Unloading function */
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_LoadDevice();
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_UnloadDevice();



        /* 장치 초기화               */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Reset(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSystemDefault(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutSvOn(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetSvOn(ushort nBoardNo, ushort nAxisNo);



        /* 에러 처리               */
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetErrorCode();
        [DllImport("tmcMApiAap.dll")]
        public static extern string TMC302A_GetErrorString(int nErrorCode);
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetMotionErrCod(ushort nBoardNo, ushort nAxisNo);



        /* 시스템 I/O 환경 설정    */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSvAlm(ushort nBoardNo, ushort nAxisNo, ushort wEnable, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetSvAlm(ushort nBoardNo, ushort nAxisNo, ref ushort wpIsEnable, ref ushort wpLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSvInpos(ushort nBoardNo, ushort nAxisNo, ushort wEnable, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetSvInpos(ushort nBoardNo, ushort nAxisNo, ref ushort wpIsEnable, ref ushort wpLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetHlmt(ushort nBoardNo, ushort nAxisNo, ushort wStopMethod, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetHlmt(ushort nBoardNo, ushort nAxisNo, ref ushort wpStopMethod, ref ushort wpLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetOrg(ushort nBoardNo, ushort nAxisNo, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetOrg(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetEncoderZ(ushort nBoardNo, ushort nAxisNo, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetEncoderZ(ushort nBoardNo, ushort nAxisNo);

        //클리어 카운트는 항상 사용함 
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSvCClr(ushort nBoardNo, ushort nAxisNo, ushort wEnable, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetSvCClr(ushort nBoardNo, ushort nAxisNo, ref ushort wpIsEnable, ref ushort wpLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSvCClrTime(ushort nBoardNo, ushort nAxisNo, ushort wTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetSvCClrTime(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutSvCClrDO(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetSvCClrDO(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutSvCClrCmd(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutSvAlmRst(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetSvAlmRst(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetEmergency(ushort nBoardNo, ushort wLogic);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetEmergency(ushort nBoardNo);





        /* 모션 제어 환경 설정    */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetRangeMode(ushort nBoardNo, ushort nAxisNo, ushort wMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetRangeMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetPulseMode(ushort nBoardNo, ushort nAxisNo, ushort wOutMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetPulseMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetPulseDir(ushort nBoardNo, ushort nAxisNo, ushort wDir);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetPulseDir(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetEncoderMode(ushort nBoardNo, ushort nAxisNo, ushort wInMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetEncoderMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetEncoderDir(ushort nBoardNo, ushort nAxisNo, ushort wInDir);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetEncoderDir(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCompCountMode(ushort nBoardNo, ushort nAxisNo, ushort wCmpMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetCompCountMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSlmt(ushort nBoardNo, ushort nAxisNo, int lSlmtP, int lSlmtM, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetSlmt(ushort nBoardNo, ushort nAxisNo, ref int lpSlmtP, ref int lpSlmtM, ref ushort wpIsEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCounterRing(ushort nBoardNo, ushort nAxisNo, uint dwCommandPos, uint dwFeedbackPos, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetCounterRing(ushort nBoardNo, ushort nAxisNo, ref uint dwpCommandPos, ref uint dwpFeedbackPos, ref ushort wpIsEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFilterTime(ushort nBoardNo, ushort nAxisNo, ushort wTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetFilterTime(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFilterSensor(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetFilterSensor(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFilterEncoderZ(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetFilterEncoderZ(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFilterSvIF(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetFilterSvIF(ushort nBoardNo, ushort nAxisNo);



        /* 단축 모션 제어         */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetJogSpeed(ushort nBoardNo, ushort nAxisNo, uint dwStartSpeed, uint dwWorkSpeed, uint dwAccTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetJogSpeed(ushort nBoardNo, ushort nAxisNo, ref uint pdwStartSpeed, ref uint dwpWorkSpeed, ref uint dwpAccTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Jog_Move(ushort nBoardNo, ushort nAxisNo, ushort wDir);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetSpeedMode(ushort nBoardNo, ushort nAxisNo, ushort wSpeedMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetSpeedMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetPosSpeed(ushort nBoardNo, ushort nAxisNo, uint dwStartSpeed, uint dwWorkSpeed, uint dwAccTime, uint dwDecTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetPosSpeed(ushort nBoardNo, ushort nAxisNo, ref uint dwpStartSpeed, ref uint dwpWorkSpeed, ref uint dwpAccTime, ref uint dwpDecTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Inc_Move(ushort nBoardNo, ushort nAxisNo, int lDistance);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Abs_Move(ushort nBoardNo, ushort nAxisNo, int lPosition);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_Done(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Decel_Stop(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Sudden_Stop(ushort nBoardNo, ushort nAxisNo);



        /* 다축 동시 모션 제어         */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Jog_Move(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList, ushort[] wDirList);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Abs_Move(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList, int[] lDisList);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Inc_Move(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList, int[] lPosList);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_Multi_Done(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Decel_Stop(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Sudden_Stop(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList);



        /* 원점 복귀                 */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetHomeDir(ushort nBoardNo, ushort nAxisNo, ushort wHomDir);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetHomeDir(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetHomeMode(ushort nBoardNo, ushort nAxisNo, ushort wHomMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetHomeMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetHomeSpeed(ushort nBoardNo, ushort nAxisNo, uint dwStartSpeed, uint dwWorkSpeed, uint dwAccTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetHomeSpeed(ushort nBoardNo, ushort nAxisNo, ref uint dwpStartSpeed, ref uint dwpWorkSpeed, ref uint dwpAccTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetHomeOffset(ushort nBoardNo, ushort nAxisNo, int lHomOffset);
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetHomeOffset(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Home_Move(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Home_Move(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList);



        /* 속도 및 위치 오버라이딩    */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_OverrideSpeed(ushort nBoardNo, ushort nAxisNo, uint dwNewWorkSpeed);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Inc_OverrideMove(ushort nBoardNo, ushort nAxisNo, int lNewDistance);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Abs_OverrideMove(ushort nBoardNo, ushort nAxisNo, int lNewPosition);


        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_OverrideSpeed(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList, uint[] dwaNewWorkSpeed);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Inc_OverrideMove(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList, int[] laNewDistance);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_Multi_Abs_OverrideMove(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList, int[] laNewPosition);





        /* 모션 시스템 상태 모니터링 및 위치 및 속도 관리   */
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetCardStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetMainStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDrvStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetErrStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetInputStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetEvtStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCommandPos(ushort nBoardNo, ushort nAxisNo, int lCommandPos);
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetCommandPos(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetActualPos(ushort nBoardNo, ushort nAxisNo, int lFeedbackPos);
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetActualPos(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetCommandSpeed(ushort nBoardNo, ushort nAxisNo);



        /*  비교기    */
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCompTrgWidth(ushort nBoardNo, ushort nAxisNo, uint wCmpWidth);
        [DllImport("tmcMApiAap.dll")]
        public static extern uint TMC302A_GetCompTrgWidth(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCompTrgMode(ushort nBoardNo, ushort wLogic, ushort wCmpMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetCompTrgMode(ushort nBoardNo, ref ushort wpLogic, ref ushort wpCmpMode);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCompTrgOneData(ushort nBoardNo, ushort nAxisNo, int lStartData);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCompTrgTable(ushort nBoardNo, ushort nAxisNo, ushort nNumData, int[] lPositionList);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetCompTrgContTable(ushort nBoardNo, ushort nAxisNo, ushort nNumData, int lStartData, int lInterval);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetInitCompTrg(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFreeCompTrg(ushort nBoardNo);





        /* 범용 디지털 입출력 */


        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutDO(ushort nBoardNo, uint dwOutStatus);
        [DllImport("tmcMApiAap.dll")]
        public static extern uint TMC302A_GetDO(ushort nBoardNo);



        // nChannelNo : 각 비트값

        // 0 ~ 63:
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutDOBit(ushort nBoardNo, ushort nChannelNo, ushort wOutStatus);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDOBit(ushort nBoardNo, ushort nChannelNo);



        // wGroupNo : 입출력 설정 값 [0~7]

        // 0 : 0  ~ 7

        // 1 : 8  ~ 15

        // 2 : 16 ~ 23

        // 3 : 24 ~ 31

        // 4 : 32 ~ 39

        // 5 : 40 ~ 47

        // 6 : 48 ~ 55

        // 7 : 56 ~ 63
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutDOByte(ushort nBoardNo, ushort wGroupNo, byte bOutStatus);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetDOByte(ushort nBoardNo, ushort wGroupNo, ref byte bpOutStatus);

        // wGroupNo : 입출력 설정 값 [0~3]

        // 0 : 0  ~ 15

        // 1 : 16 ~ 31

        // 2 : 32 ~ 47

        // 3 : 48 ~ 63
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutDOWord(ushort nBoardNo, ushort wGroupNo, ushort wOutStatus);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetDOWord(ushort nBoardNo, ushort wGroupNo, ref ushort wpOutStatus);



        // wGroupNo : 입출력 설정 값 [0~1]

        // 0 : 0  ~ 15

        // 1 : 16 ~ 31
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutDODWord(ushort nBoardNo, ushort wGroupNo, uint dwOutStatus);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetDODWord(ushort nBoardNo, ushort wGroupNo, ref uint dwpOutStatus);


        [DllImport("tmcMApiAap.dll")]
        public static extern uint TMC302A_GetDI(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDIBit(ushort nBoardNo, ushort nChannelNo);

        // wGroupNo : 입출력 설정 값 [0~3]

        // 0 : 0  ~ 7

        // 1 : 8  ~ 15

        // 2 : 16 ~ 23

        // 3 : 24 ~ 31
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetDIByte(ushort nBoardNo, ushort wGroupNo, ref byte bpInStatus);

        // wGroupNo : 입출력 설정 값 [0~1]

        // 0 : 0  ~ 15

        // 1 : 16 ~ 31
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetDIWord(ushort nBoardNo, ushort wGroupNo, ref ushort wpInStatus);

        // wGroupNo : 입출력 설정 값 [0]

        // 0 : 0  ~ 31
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetDIDWord(ushort nBoardNo, ushort wGroupNo, ref uint dwpInStatus);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetDiFilter(ushort nBoardNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDiFilter(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetDiFilterTime(ushort nBoardNo, ushort wTime);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDiFilterTime(ushort nBoardNo);



        // 추가 (2008.01.21)
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_HomeIsBusy(ushort nBoardNo, ushort nAxisNo);

        // 추가 (2008.01.21)
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetHomeSuccess(ushort nBoardNo, ushort nAxisNo, ushort wEnable);

        // 추가 (2008.01.21)
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetHomeSuccess(ushort nBoardNo, ushort nAxisNo);

        // 추가 (2008.05.20)
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFixedRange(ushort nBoardNo, ushort nAxisNo, ushort wEnable);

        // 추가 (2008.05.20)
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetFixedRange(ushort nBoardNo, ushort nAxisNo);





        /* 외부 신호에 의한 모션 제어    */

        // 수동펄스 사용 여부 (0 : 사용 안함, 1 : 수동펄스 사용  )

        // 배율    wRate > 0 이상 
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetExtMode(ushort nBoardNo, ushort nAxisNo, ushort wMode, ushort wRate);

        // 추가 (2009.03.06)

        // 수동펄스 사용 여부 (0 : 사용 안함, 1 : 수동펄스 사용 )
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_GetExtMode(ushort nBoardNo, ushort nAxisNo, ref ushort wMode, ref ushort wRate);

        // 추가 (2009.04.08)

        // 수동펄스 필터  사용 여부(0 : 사용 안함, 1 : 사용  )
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetFilterExt(ushort nBoardNo, ushort nAxisNo, ushort wEnable);

        // 추가 (2009.08.08)

        // 수동펄스 필터 사용 여부 (0 : 사용 안함, 1 : 수동펄스 사용 )
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetFilterExt(ushort nBoardNo, ushort nAxisNo);




        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetBoardID(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetAxisNum(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDiNum(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetDoNum(ushort nBoardNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_LogCheck(ushort wLogCheck);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_PutSvRun(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetSvRun(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetBlockMode(ushort wBlocking);
        [DllImport("tmcMApiAap.dll")]
        public static extern ushort TMC302A_GetBlockMode();
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_SetAccOffset(ushort nBoardNo, ushort nAxisNo, int lOffset);
        [DllImport("tmcMApiAap.dll")]
        public static extern int TMC302A_GetAccOffset(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAap.dll")]
        public static extern void TMC302A_BoardInfo(ushort nBoardNo, ref uint dwpBoard, ref uint dwpComm, ref uint dwpAxis, ref uint dwpDiNum, ref uint dwpDoNum);
    }

    public partial class FAAPlusMMCDevice
    {
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_PutSvAlmRst(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetSvAlmRst(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetEmergency(ushort nBoardNo, ushort wLogic);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetEmergency(ushort nBoardNo);

        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Reset(ushort nBoardNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetSystemDefault(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_PutSvOn(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetSvOn(ushort nBoardNo, ushort nAxisNo);

        [DllImport("tmcMApiAcp.dll")]
        public static extern int TMC304A_LoadDevice();
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_UnloadDevice();

        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetRangeMode(ushort nBoardNo, ushort nAxisNo, ushort wMode);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetRangeMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetPulseMode(ushort nBoardNo, ushort nAxisNo, ushort wOutMode);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetPulseMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetPulseDir(ushort nBoardNo, ushort nAxisNo, ushort wDir);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetPulseDir(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetEncoderMode(ushort nBoardNo, ushort nAxisNo, ushort wInMode);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetEncoderMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetEncoderDir(ushort nBoardNo, ushort nAxisNo, ushort wInDir);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetEncoderDir(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetCompCountMode(ushort nBoardNo, ushort nAxisNo, ushort wCmpMode);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetCompCountMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetSlmt(ushort nBoardNo, ushort nAxisNo, int lSlmtP, int lSlmtM, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_GetSlmt(ushort nBoardNo, ushort nAxisNo, ref int lpSlmtP, ref int lpSlmtM, ref ushort wpIsEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetCounterRing(ushort nBoardNo, ushort nAxisNo, uint dwCommandPos, uint dwFeedbackPos, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_GetCounterRing(ushort nBoardNo, ushort nAxisNo, ref uint dwpCommandPos, ref uint dwpFeedbackPos, ref ushort wpIsEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetFilterTime(ushort nBoardNo, ushort nAxisNo, ushort wTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetFilterTime(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetFilterSensor(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetFilterSensor(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetFilterEncoderZ(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetFilterEncoderZ(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetFilterSvIF(ushort nBoardNo, ushort nAxisNo, ushort wEnable);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetFilterSvIF(ushort nBoardNo, ushort nAxisNo);

        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetJogSpeed(ushort nBoardNo, ushort nAxisNo, uint dwStartSpeed, uint dwWorkSpeed, uint dwAccTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_GetJogSpeed(ushort nBoardNo, ushort nAxisNo, ref uint pdwStartSpeed, ref uint dwpWorkSpeed, ref uint dwpAccTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Jog_Move(ushort nBoardNo, ushort nAxisNo, ushort wDir);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetSpeedMode(ushort nBoardNo, ushort nAxisNo, ushort wSpeedMode);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetSpeedMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetPosSpeed(ushort nBoardNo, ushort nAxisNo, uint dwStartSpeed, uint dwWorkSpeed, uint dwAccTime, uint dwDecTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_GetPosSpeed(ushort nBoardNo, ushort nAxisNo, ref uint dwpStartSpeed, ref uint dwpWorkSpeed, ref uint dwpAccTime, ref uint dwpDecTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Inc_Move(ushort nBoardNo, ushort nAxisNo, int lDistance);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Abs_Move(ushort nBoardNo, ushort nAxisNo, int lPosition);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_Done(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Decel_Stop(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Sudden_Stop(ushort nBoardNo, ushort nAxisNo);

        /* 원점 복귀                 */
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetHomeDir(ushort nBoardNo, ushort nAxisNo, ushort wHomDir);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetHomeDir(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetHomeMode(ushort nBoardNo, ushort nAxisNo, ushort wHomMode);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetHomeMode(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetHomeSpeed(ushort nBoardNo, ushort nAxisNo, uint dwStartSpeed, uint dwWorkSpeed, uint dwAccTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_GetHomeSpeed(ushort nBoardNo, ushort nAxisNo, ref uint dwpStartSpeed, ref uint dwpWorkSpeed, ref uint dwpAccTime);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetHomeOffset(ushort nBoardNo, ushort nAxisNo, int lHomOffset);
        [DllImport("tmcMApiAcp.dll")]
        public static extern int TMC304A_GetHomeOffset(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Home_Move(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_Multi_Home_Move(ushort nBoardNo, ushort nAxisNoNum, ushort[] nAxisNoList);

        /* 모션 시스템 상태 모니터링 및 위치 및 속도 관리   */
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetCardStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetMainStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetDrvStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetErrStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetInputStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern ushort TMC304A_GetEvtStatus(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetCommandPos(ushort nBoardNo, ushort nAxisNo, int lCommandPos);
        [DllImport("tmcMApiAcp.dll")]
        public static extern int TMC304A_GetCommandPos(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern void TMC304A_SetActualPos(ushort nBoardNo, ushort nAxisNo, int lFeedbackPos);
        [DllImport("tmcMApiAcp.dll")]
        public static extern int TMC304A_GetActualPos(ushort nBoardNo, ushort nAxisNo);
        [DllImport("tmcMApiAcp.dll")]
        public static extern int TMC304A_GetCommandSpeed(ushort nBoardNo, ushort nAxisNo);
    }

    public partial class FAAPlusMMCDevice : FAMMCDevice
    {
        protected struct AxisStatus
        {
            public bool Origin { get; set; }
            public bool EncoderZ { get; set; }
            public bool Emergency { get; set; }
            public bool Inposition { get; set; }
            public bool ServoAlarm { get; set; }
            public bool PositiveLimit { get; set; }
            public bool NegativeLimit { get; set; }
            public bool RunFlag { get; set; }
            public bool ErrFlag { get; set; }
            public bool HomeFlag { get; set; }
            public bool CounterClear { get; set; }
            public bool ServoOn { get; set; }
            public bool AlarmReset { get; set; }
        }

        protected struct AxisParameters
        {
            public ushort HomeDirection { get; set; }
            public ushort HomeMode { get; set; }
            public uint HomeStartSpeed { get; set; }
            public uint HomeMoveSpeed { get; set; }
            public uint HomeAccel { get; set; }
            public int HomeOffset { get; set; }
        }

        protected enum CardModel
        {
            Axis2Type, Axis8Type
        }

        protected CardModel ModelNo { get; set; }

        public ushort CardNo { get; set; }
        public bool IsSlave { get; set; }

        public FAAPlusMMCDevice()
        {
            IsSlave = false;
        }

        public override void LoadParameters(XElement xml)
        {
            base.LoadParameters(xml);
            if (xml.Element("Property") != null)
                LoadModelNo(xml.Element("Property"));
        }

        public override void Open()
        {
            if (IsSlave == false)
            {
                if (ModelNo == CardModel.Axis2Type)
                    TMC302A_LoadDevice();
                else if (ModelNo == CardModel.Axis8Type)
                    TMC304A_LoadDevice();
            }
        }

        public override void Close()
        {
            if (IsSlave == false)
            {
                if (ModelNo == CardModel.Axis2Type)
                    TMC302A_UnloadDevice();
                else if (ModelNo == CardModel.Axis8Type)
                    TMC304A_UnloadDevice();
            }
        }

        public override int MovePos(ushort axisNo, double position, ushort speedMode, double startSpeed, double moveSpeed, double accelTime, double DecTime)
        {
            if (ModelNo == CardModel.Axis2Type)
            {
                TMC302A_PutSvOn(CardNo, axisNo, 1);
                TMC302A_SetSpeedMode(CardNo, axisNo, speedMode);
                TMC302A_SetPosSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime, (uint)DecTime);
                TMC302A_Abs_Move(CardNo, axisNo, (int)position);
            }
            else if (ModelNo == CardModel.Axis8Type)
            {
                TMC304A_PutSvOn(CardNo, axisNo, 1);
                TMC304A_SetSpeedMode(CardNo, axisNo, speedMode);
                TMC304A_SetPosSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime, (uint)DecTime);
                TMC304A_Abs_Move(CardNo, axisNo, (int)position);
            }
            return 0;
        }

        public override void MoveIncPos(ushort axisNo, int position, ushort speedMode, double startSpeed, double moveSpeed, double accelTime, double DecTime)
        {
            if (ModelNo == CardModel.Axis2Type)
            {
                TMC302A_PutSvOn(CardNo, axisNo, 1);
                TMC302A_SetSpeedMode(CardNo, axisNo, speedMode);
                TMC302A_SetPosSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime, (uint)DecTime);
                TMC302A_Inc_Move(CardNo, axisNo, position);
            }
            else if (ModelNo == CardModel.Axis8Type)
            {
                TMC304A_PutSvOn(CardNo, axisNo, 1);
                TMC304A_SetSpeedMode(CardNo, axisNo, speedMode);
                TMC304A_SetPosSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime, (uint)DecTime);
                TMC304A_Inc_Move(CardNo, axisNo, position);
            }
        }

        public override void MoveVelocity(ushort axisNo, double startSpeed, double moveSpeed, double accelTime, double decTime)
        {
        }
        public override void MoveSpeedOverrideVelocity(ushort axisNo, double dOverrideVelocity)
        {
        }

        public override bool IsMotionDone(ushort axisNo)
        {
            int motionDone = 0;
            if (ModelNo == CardModel.Axis2Type)
                motionDone = TMC302A_Done(CardNo, axisNo);
            else if (ModelNo == CardModel.Axis8Type)
                motionDone = TMC304A_Done(CardNo, axisNo);

            bool b = IsRunFlag(axisNo);
            bool c = IsHomeFlag(axisNo);
            bool d = IsErrorFlag(axisNo);


            if (motionDone == 0 &&
                IsRunFlag(axisNo) == false &&
                IsHomeFlag(axisNo) == false &&
                IsErrorFlag(axisNo) == false) return true;
            else return false;
        }

        public override void MoveHome(ushort axisNo, ushort homeDir, ushort homeMode, double startSpeed, double moveSpeed, double accelTime, double offset)
        {
            if (ModelNo == CardModel.Axis2Type)
            {
                TMC302A_SetRangeMode(CardNo, axisNo, 500);
                TMC302A_PutSvOn(CardNo, axisNo, 1);
                TMC302A_SetHomeDir(CardNo, axisNo, homeDir);
                TMC302A_SetHomeMode(CardNo, axisNo, homeMode);
                TMC302A_SetHomeSpeed(CardNo, axisNo, (uint)startSpeed,
                     (uint)moveSpeed,
                     (uint)accelTime);
                TMC302A_SetHomeOffset(CardNo, axisNo, (int)offset);
                TMC302A_Home_Move(CardNo, axisNo);
            }
            else if (ModelNo == CardModel.Axis8Type)
            {
                TMC304A_SetRangeMode(CardNo, axisNo, 500);
                TMC304A_PutSvOn(CardNo, axisNo, 1);
                TMC304A_SetHomeDir(CardNo, axisNo, homeDir);
                TMC304A_SetHomeMode(CardNo, axisNo, homeMode);
                TMC304A_SetHomeSpeed(CardNo, axisNo, (uint)startSpeed,
                    (uint)moveSpeed,
                    (uint)accelTime);
                TMC304A_SetHomeOffset(CardNo, axisNo, (int)offset);
                TMC304A_Home_Move(CardNo, axisNo);
            }
        }

        public override void Stop(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                TMC302A_Decel_Stop(CardNo, axisNo);
            else if (ModelNo == CardModel.Axis8Type)
                TMC304A_Decel_Stop(CardNo, axisNo);
        }

        public override void JogPositive(ushort axisNo, double startSpeed, double moveSpeed, double accelTime)
        {
            if (ModelNo == CardModel.Axis2Type)
            {
                TMC302A_SetSpeedMode(CardNo, axisNo, 0);
                TMC302A_SetJogSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime);
                TMC302A_Jog_Move(CardNo, axisNo, 1);
            }
            else if (ModelNo == CardModel.Axis8Type)
            {
                TMC304A_SetSpeedMode(CardNo, axisNo, 0);
                TMC304A_SetJogSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime);
                TMC304A_Jog_Move(CardNo, axisNo, 1);
            }
        }

        public override void JogNegative(ushort axisNo, double startSpeed, double moveSpeed, double accelTime)
        {
            if (ModelNo == CardModel.Axis2Type)
            {
                TMC302A_SetSpeedMode(CardNo, axisNo, 0);
                TMC302A_SetJogSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime);
                TMC302A_Jog_Move(CardNo, axisNo, 0);
            }
            else if (ModelNo == CardModel.Axis8Type)
            {
                TMC304A_SetSpeedMode(CardNo, axisNo, 0);
                TMC304A_SetJogSpeed(CardNo, axisNo, (uint)startSpeed, (uint)moveSpeed, (uint)accelTime);
                TMC304A_Jog_Move(CardNo, axisNo, 0);
            }
        }

        public override bool IsOrigin(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_ORIGIN);
        }

        public override bool IsEncoderZ(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_ENCODER_Z);
        }

        public override bool IsEmergency(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_EMERGENCY);
        }

        public override bool IsInposition(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_INP);
        }

        public override bool IsServoAlarm(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_SERVO_ALARM);
        }

        public override bool IsPositiveLimit(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_LIMIT_PLUS);
        }

        public override bool IsNegativeLimit(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_LIMIT_MINUS);
        }

        public override bool IsRunFlag(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_RUN_FLAG);
        }

        public override bool IsErrorFlag(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_ERR_FLAG);
        }

        public override bool IsHomeFlag(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_HOME_FLAG);
        }

        public bool IsCounterClear(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_COUNTER_CLR);
        }

        public override bool IsServoOn(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_SERVO_ON);
        }

        public bool IsAlarmReset(ushort axisNo)
        {
            ushort status = GetStatus(axisNo);

            return Utility.FAUtility.CheckBit(status, CARD_STATUS_ALARM_RESET);
        }

        public override void ResetAlarm(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
            {
                TMC302A_PutSvAlmRst(CardNo, axisNo, 1);
                System.Threading.Thread.Sleep(10);
                TMC302A_PutSvAlmRst(CardNo, axisNo, 0);
            }
            else if (ModelNo == CardModel.Axis8Type)
            {
                TMC304A_PutSvAlmRst(CardNo, axisNo, 1);
                System.Threading.Thread.Sleep(10);
                TMC304A_PutSvAlmRst(CardNo, axisNo, 0);
            }
        }

        public override void ServoOn(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                TMC302A_PutSvOn(CardNo, axisNo, 1);
            else if (ModelNo == CardModel.Axis8Type)
                TMC304A_PutSvOn(CardNo, axisNo, 1);
        }

        public override void ServoOff(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                TMC302A_PutSvOn(CardNo, axisNo, 0);
            else if (ModelNo == CardModel.Axis8Type)
                TMC304A_PutSvOn(CardNo, axisNo, 0);
        }

        public override void MIO_OnOff(ushort axisNo, ushort nBitNo, bool bOn)
        {
            //TMC302A_PutDOBit(CardNo, nChannelNo, (ushort)(bOn ? 1 : 0));
        }
        public override double GetCommandPos(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                return TMC302A_GetCommandPos(CardNo, axisNo);
            else if (ModelNo == CardModel.Axis8Type)
                return TMC304A_GetCommandPos(CardNo, axisNo);

            return 0;
        }

        public override double GetActualPos(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                return TMC302A_GetActualPos(CardNo, axisNo);
            else if (ModelNo == CardModel.Axis8Type)
                return TMC304A_GetActualPos(CardNo, axisNo);

            return 0;
        }

        public override int GetCommandSpeed(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                return TMC302A_GetCommandSpeed(CardNo, axisNo);
            else if (ModelNo == CardModel.Axis8Type)
                return TMC302A_GetCommandSpeed(CardNo, axisNo);

            return 0;
        }

        public override void ResetCommandPos(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                TMC302A_SetCommandPos(CardNo, axisNo, 0);
            else if (ModelNo == CardModel.Axis8Type)
                TMC304A_SetCommandPos(CardNo, axisNo, 0);
        }

        public override void ResetActualPos(ushort axisNo)
        {
            if (ModelNo == CardModel.Axis2Type)
                TMC302A_SetActualPos(CardNo, axisNo, 0);
            else if (ModelNo == CardModel.Axis8Type)
                TMC304A_SetActualPos(CardNo, axisNo, 0);
        }

        public ushort GetStatus(ushort axisNo)
        {
            ushort status = 0;
            if (ModelNo == CardModel.Axis2Type)
                status = TMC302A_GetCardStatus(CardNo, axisNo);
            else if (ModelNo == CardModel.Axis8Type)
                status = TMC304A_GetCardStatus(CardNo, axisNo);

            return status;
        }

        private void LoadModelNo(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                string name = "";
                string strValue;

                if (item.Element("Name") != null)
                    name = item.Element("Name").Value.Trim();
                else
                    continue;

                if (item.Element("Value") != null)
                    strValue = item.Element("Value").Value.Trim();
                else
                    continue;

                if (name == "ModelNo")
                {
                    if (strValue == "Axis8Type")
                    {
                        ModelNo = CardModel.Axis8Type;
                    }
                }
            }
        }

        public override bool IsHomeOk(ushort axisNo)
        {
            return IsMotionDone(axisNo);
        }



        public override void ResetLinkMode(ushort masterAxis)
        {
        }

        public override void SetLinkMode(ushort masterAxis, ushort slaveAxis, double ratio)
        {
        }

        public override void MoveMultiPos(FAMMCDevice.FAMultiMoveInfo[] info)
        {
        }

        //edgetechnology 2024-06-24


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
