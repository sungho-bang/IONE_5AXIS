using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FAFramework;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Part.MemoryBasePart;
using FAFramework.GUI;
using FAFramework.Utility;
using FALibrary.Utility;
using FAFramework.VT3500.ExtendedParts;
using FAFramework.VT3500.JobInfo;
using FALibrary.Part.Inverter;

namespace FAFramework.VT3500.Modules
{
    public class FASafetyModule : Module.FAModule
    {
        private VT3500.SubEquipment VT3500Equipment
        {
            get { return Equipment as VT3500.SubEquipment; }
        }

        public SubUnits.FADoorUnit DoorUnit { get; set; }

        public SubUnits.FAHeaterUnit PressHeaterUnit { get; set; }

        [FAAttribute("Sequence")]
        public FASequence SafetyHeater { get; set; }
        [FAAttribute("Sequence")]
        public FASequence SafetyOverHeater { get; set; }
        [FAAttribute("Sequence")]
        public FASequence SafetyAlertHeater { get; set; }

        #region Alarm        
        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmSSR80HeatAlarmCheck { get; set; } // 80도이상
        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmSSR60HeatAlertCheck { get; set; } // 60도이상
        #endregion

        #region Part
        public FAPartOnOffSensor HeaterPowerOnCheck { get; set; }
        public FAPartOnOffSensor HeaterOverHeatingCheck { get; set; }
        public FAPartOnOffSensor SSR80HeatAlarmCheck { get; set; }
        public FAPartOnOffSensor SSR60HeatAlertCheck { get; set; }
        public FAPartOnOff HeaterPowerOn { get; set; }
        #endregion

        #region Parameters
        [FAProperty]
        [FA("Parameters")]
        public bool MessageOnce { get; set; }
        #endregion

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();
            MessageOnce = false;
        }

        public override void InitializeSequence()
        {
            MakeSafetyHeater();
            //MakeSafetyHeater(PressHeaterUnit.FirstPressBottomHeaterPowerOnCheck);
            //MakeSafetyHeater(PressHeaterUnit.FourthPressBottomHeaterPowerOnCheck);
            //MakeSafetyOverHeater(PressHeaterUnit.FirstPressTopHeaterOverHeatingCheck, PressHeaterUnit.FirstPressTopHeaterPowerOn);
            //MakeSafetyOverHeater(PressHeaterUnit.FirstPressBottomHeaterOverHeatingCheck, PressHeaterUnit.FirstPressBottomHeaterPowerOn);
            //MakeSafetyOverHeater();
            MakeSafetyAlertHeater();
        }


        public void MakeSafetyHeater()
        {
            var seq = SafetyHeater;

            string msg = string.Empty;

            seq.AddStep("Start").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (HeaterPowerOnCheck.IsOff)
                    {
                        msg = HeaterPowerOnCheck.Name + "HeaterPower off 감지.  Heater 전원 확인";
                        Manager.MessageWindowManager.Instance.Show(HeaterPowerOnCheck.Name, msg);
                        WriteTraceLog($"{msg}");
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (HeaterPowerOnCheck.IsOn)
                    {
                        msg = HeaterPowerOnCheck.Name + "HeaterPower on 감지.  Heater 전원 확인";
                        Manager.MessageWindowManager.Instance.CloseWindow(HeaterPowerOnCheck.Name);
                        WriteTraceLog($"{msg}");
                        actor.NextStep("Start");
                    }
                });
        }
        public void MakeSafetyAlertHeater() //60도 이상임!
        {
            var seq = SafetyAlertHeater;

            string msg = string.Empty;

            seq.AddStep("60Check").StepIndex = seq.AddItem(
             (actor, time) =>
             {
                 if (SSR60HeatAlertCheck.IsOn)
                 {
                     if(!MessageOnce)
                     {
                         MessageOnce = true;
                         ShowMessage("SSR60HeatAlertCheck", AlarmSSR60HeatAlertCheck, "SSR60HeatAlertCheck");
                     }
                     actor.NextStep("80Check");
                 }
                 else
                 {
                     MessageOnce = false;
                     CloseMessage("SSR60HeatAlertCheck");
                     actor.NextStep("60Check");
                 }
             });
            seq.AddStep("80Check").StepIndex = seq.AddItem(
            (actor, time) =>
            {
                if (SSR80HeatAlarmCheck.IsOn)
                {
                    msg = SSR80HeatAlarmCheck.Name + "방열판의 온도가 80도 이상입니다.SSR80HeatAlarmCheck신호가 꺼질때 까지 설비를 잠깐 멈춰주세요";
                    ShowMessage("SSR80HeatAlarmCheck", AlarmSSR80HeatAlarmCheck, "SSR80HeatAlarmCheck"); //창 사라질때까지 Start 하지 말아요. Alarm 추가
                    WriteTraceLog($"{msg}");
                    Equipment.RequestStop();
                    actor.NextStep();
                }
                else
                {
                    CloseMessage("SSR80HeatAlarmCheck");
                    actor.NextStep("60Check");
                }
            });
            seq.AddItem("60Check");
        }

        //public void MakeSafetyOverHeater()
        //{
        //    var seq = SafetyOverHeater;

        //    seq.OnTerminate += delegate
        //    {
        //        //SafetyHeater.Start();
        //    };

        //    string msg = string.Empty;

        //    seq.AddStep("Start").StepIndex = seq.AddItem(
        //      (actor, time) =>
        //      {
                  
        //          if (SSR80HeatAlarmCheck.IsOn)
        //          {
        //              //HeaterPowerOn.DoTurnOff(this);
        //              msg = SSR80HeatAlarmCheck.Name + "방열판의 온도가 80도 이상입니다.SSR80HeatAlarmCheck신호가 꺼질때 까지 설비를 잠깐 멈춰주세요";
        //              ShowMessage("SSR80HeatAlarmCheck", AlarmSSR80HeatAlarmCheck, "SSR80HeatAlarmCheck"); //창 사라질때까지 Start 하지 말아요. Alarm 추가
        //              WriteTraceLog($"{msg}");
        //              Equipment.RequestStop();
        //              actor.NextStep();
        //          }
        //          else
        //          {
        //              CloseMessage("SSR80HeatAlarmCheck");
        //              actor.NextStep();
        //          }
        //      });
        //    seq.AddItem(
        //       (actor, time) =>
        //       {
                   
        //           if (SSR80HeatAlarmCheck.IsOff)
        //           {
        //               CloseMessage("SSR80HeatAlarmCheck");
        //               actor.NextStep("Start");
        //               actor.NextTerminate();
        //           }
        //           else
        //           {
        //               actor.NextStep("Start");
        //           }
        //       });
            
        //}
    }
}
