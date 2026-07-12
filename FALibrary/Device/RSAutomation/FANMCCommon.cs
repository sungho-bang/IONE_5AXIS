using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.RSAutomation
{
    public static class FANMCCommon
    {        
        private static object _thisObject = new object();
        private static bool _initialized = false;
        //private static bool _stoped = false;

        public static bool IsInitialized()
        {
            return _initialized;
        }

        public static void Initialize(object sender, ushort boardID)
        {
            lock (_thisObject)
            {
                if (_initialized == false)
                {
                    Init(boardID);
                    Run(boardID);
                    _initialized = true;
                }
            }
        }

        public static void Stop(object sender, ushort boardID)
        {
            //lock (_thisObject)
            //{
            //    if (_stoped == false)
            //    {
            //        Stop(boardID);
            //        _stoped = true;
            //    }
            //}
        }

        private static void Init(ushort boardID)
        {
            char[] array = new char[NMCSDK.NMCDEF.MAX_ERR_LEN];
            StringBuilder errorMsg = new StringBuilder(128);
            string msg;

            //NMCSDK.NMCSDKLib.MC_STATUS ms = NMCSDK.NMCSDKLib.MC_MasterInit(boardID);
            //NMCSDK.NMCSDKLib.MC_STATUS ms = NMCSDK.NMCSDKLib.MC_Init();
            NMCSDK.NMCSDKLib.MC_STATUS ms = NMCSDK.NMCSDKLib.MC_Init();

            if (ms != NMCSDK.NMCSDKLib.MC_STATUS.MC_OK)
            {
                NMCSDK.NMCSDKLib.MC_GetErrorMessage((uint)ms, (uint)128, errorMsg);
                msg = string.Format("initializing fail. Error : 0x{0:x}, {1}", ms, errorMsg);
                throw new Exception(msg);
            }
        }

        private static void Run(ushort boardID)
        {
            NMCSDK.NMCSDKLib.MC_STATUS ms;
            char[] array = new char[NMCSDK.NMCDEF.MAX_ERR_LEN];
            StringBuilder cstrErrorMsg = new StringBuilder(128);
            string msg;
            ushort BoardID = 0;
            byte MstMode = 0;

            ms = NMCSDK.NMCSDKLib.MC_MasterRUN(BoardID);

            if (ms != NMCSDK.NMCSDKLib.MC_STATUS.MC_OK)
            {
                NMCSDK.NMCSDKLib.MC_GetErrorMessage((uint)ms, (uint)128, cstrErrorMsg);
                msg = String.Format("Error :: 0x{0:x}, {1}", ms, cstrErrorMsg);
                throw new Exception(msg);
            }            

            while (true)
            {
                ms = NMCSDK.NMCSDKLib.MasterGetCurMode(BoardID, ref MstMode);

                if (ms != NMCSDK.NMCSDKLib.MC_STATUS.MC_OK)
                {
                    NMCSDK.NMCSDKLib.MC_GetErrorMessage((uint)ms, (uint)128, cstrErrorMsg);
                    msg = String.Format("Error :: 0x{0:x}, {1}", ms, cstrErrorMsg);
                    throw new Exception(msg);
                }

                if (MstMode == (byte)NMCSDK.NMCSDKLib.EcMstMode.eMM_RUN)
                {
                    break;
                }
                
                if ((MstMode == (byte)NMCSDK.NMCSDKLib.EcMstMode.eMM_ERR) || (MstMode == (byte)NMCSDK.NMCSDKLib.EcMstMode.eMM_LINKBROKEN))
                {
                    msg = "Master State is ERROR or LINKBROKEN  State";
                    throw new Exception(msg);
                }
            }
        }

        private static void Stop(ushort boardID)
        {
            NMCSDK.NMCSDKLib.MC_STATUS ms;
            char[] array = new char[NMCSDK.NMCDEF.MAX_ERR_LEN];
            StringBuilder cstrErrorMsg = new StringBuilder(128);
            string msg;
            ushort BoardID = 0;
            byte MstMode = 0;

            ms = NMCSDK.NMCSDKLib.MC_MasterSTOP(BoardID);

            if (ms != NMCSDK.NMCSDKLib.MC_STATUS.MC_OK)
            {
                NMCSDK.NMCSDKLib.MC_GetErrorMessage((uint)ms, (uint)128, cstrErrorMsg);
                msg = String.Format("Error :: 0x{0:x}, {1}", ms, cstrErrorMsg);
                throw new Exception(msg);
            }            

            while (true)
            {
                ms = NMCSDK.NMCSDKLib.MasterGetCurMode(BoardID, ref MstMode);

                if (ms != NMCSDK.NMCSDKLib.MC_STATUS.MC_OK)
                {
                    NMCSDK.NMCSDKLib.MC_GetErrorMessage((uint)ms, (uint)128, cstrErrorMsg);
                    msg = String.Format("Error :: 0x{0:x}, {1}", ms, cstrErrorMsg);
                    throw new Exception(msg);
                }

                if (MstMode == (byte)NMCSDK.NMCSDKLib.EcMstMode.eMM_IDLE)
                {
                    break;
                }
                
                if ((MstMode == (byte)NMCSDK.NMCSDKLib.EcMstMode.eMM_ERR) || (MstMode == (byte)NMCSDK.NMCSDKLib.EcMstMode.eMM_LINKBROKEN))
                {
                    msg = "Master State is ERROR or LINKBROKEN  State";
                    throw new Exception(msg);
                }
            }
        }
    }
}
