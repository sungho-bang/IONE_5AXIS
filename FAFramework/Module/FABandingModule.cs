using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Part.HeaterPart;
using FALibrary.Utility;
using FAFramework.Utility;

namespace FAFramework.Module
{
    public class FABandingModule : FAModule
    {
        #region Sequences
        [FAAttribute("Sequence")]
        public FASequence Banding { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Strapping machine is power off", "Power on of strapping machine")]
        [AlarmDescription(KnownCulture.Korean, "밴딩 머신 전원이 꺼져 있습니다.", "밴딩 머신 전원을 켜세요.")]
        public int AlarmBandingMachinePowerOff { get; set; }

        [DefaultAlarmInfo(2, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Strapping complete signal did not received", "Check strapping machine")]
        [AlarmDescription(KnownCulture.Korean, "밴딩 완료 신호가 들어오지 않습니다.", "밴딩기를 확인하세요.")]
        public int AlarmBandingCompleteSignalNotReceived { get; set; }

        [DefaultAlarmInfo(3, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Strapping error", "Check strapping machine")]
        [AlarmDescription(KnownCulture.Korean, "밴딩 에러", "밴딩기를 확인하세요.")]
        public int AlarmBandingError { get; set; }

        [DefaultAlarmInfo(4, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Strap empty", "supply strap")]
        [AlarmDescription(KnownCulture.Korean, "밴딩끈 없음", "밴딩끈을 보충하세요.")]
        public int AlarmStrapEmpty { get; set; }

        [DefaultAlarmInfo(5, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Strap not checked", "Check strap status")]
        [AlarmDescription(KnownCulture.Korean, "밴딩끈이 감지 되지 않음", "밴딩끈을 확인하세요.")]
        public int AlarmBandingWireNotChecked { get; set; }

        [DefaultAlarmInfo(6, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "Low than set temperature", "Check heater")]
        [AlarmDescription(KnownCulture.Korean, "히터 온도가 설정온도보다 낮습니다.", "히터를 확인하세요.")]
        public int AlarmLowerThanTheSetTemperature { get; set; }

        [DefaultAlarmInfo(7, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.WARNING)]
        [AlarmDescription(KnownCulture.EnglishUS, "High than set temperature", "Check heater")]
        [AlarmDescription(KnownCulture.Korean, "히터 온도가 설정온도보다 높습니다.", "히터를 확인하세요.")]
        public int AlarmHigherThanTheSetTemperature { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeBandingTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime TimeDelayTimeBeforeConfirmBandingComplete { get; set; }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoBandingRetry { get; set; }
        #endregion

        #region Parts
        public FAPartOnOffSensor PowerOnCheck { get; set; }
        public FAPartOnOffSensor StrapEmptyCheck { get; set; }
        public FAPartOnOffSensor CompleteStrapping { get; set; }
        public FAPartOnOffSensor ErrorCheck { get; set; }
        public FAPartOnOffSensor OutOfStrapCheck { get; set; }
        public FAPartOnOffSensor BandCheck { get; set; }
        public FAPartOnOff OutputStop { get; set; }
        public FAPartOnOff OutputStart { get; set; }
        public FAPartOnOff OutputReverse { get; set; }
        public FAPartOnOff OutputForward { get; set; }
        public FAPartOnOff OutputReady { get; set; }
        public FAAutonicsTZHeater BandingHeater { get; set; }
        #endregion

        #region Parameters
        private double _heaterSetTemperature;
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public double HeaterSetTemperature
        {
            get { return _heaterSetTemperature; }
            set
            {
                if (_heaterSetTemperature == value) return;

                _heaterSetTemperature = value;
                NotifyPropertyChanged("HeaterSetTemperature");
            }
        }

        private double _temperatureLowerTolerance;
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public double TemperatureLowerTolerance
        {
            get { return _temperatureLowerTolerance; }
            set
            {
                if (_temperatureLowerTolerance == value) return;

                _temperatureLowerTolerance = value;
                NotifyPropertyChanged("TemperatureLowerTolerance");
            }
        }

        private double _temperatureUpperTolerance;
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public double TemperatureUpperTolerance
        {
            get { return _temperatureUpperTolerance; }
            set
            {
                if (_temperatureUpperTolerance == value) return;

                _temperatureUpperTolerance = value;
                NotifyPropertyChanged("TemperatureUpperTolerance");
            }
        }

        private bool _useBandCheckSensor;
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public bool UseBandCheckSensor
        {
            get { return _useBandCheckSensor; }
            set
            {
                if (_useBandCheckSensor == value) return;

                _useBandCheckSensor = value;
                NotifyPropertyChanged("UseBandCheckSensor");
            }
        }

        private bool _useOverHeatingDetection;
        [FAAttribute("Parameters")]
        [FAPropertyAttribute]
        public bool UseOverHeatingDetection
        {
            get { return _useOverHeatingDetection; }
            set
            {
                if (_useOverHeatingDetection == value) return;

                _useOverHeatingDetection = value;
                NotifyPropertyChanged("UseOverHeatingDetection");
            }
        }
        #endregion

        private string _bandingHeaterTemperatureWarning = string.Empty;

        public override void InitializeSequence()
        {
            MakeBanding();
        }

        private void MakeBanding()
        {
            string windowName = string.Empty;

            var seq = Banding;

            seq.AddWatcher(
                delegate
                {
                    if (UseOverHeatingDetection && Equipment.State != Equipment.StateStop)
                    {
                        if (BandingHeater.Temperature > HeaterSetTemperature + TemperatureUpperTolerance)
                        {
                            string msg = string.Format("Heater Temp.={0}, Heater Set Temp.={1}, Upper Tolerance={2}",
                                BandingHeater.Temperature, HeaterSetTemperature, TemperatureUpperTolerance);
                            var alarm = Utility.AlarmUtility.GetAlarm(AlarmHigherThanTheSetTemperature, "Higher than the set temperature.");
                            Manager.MessageWindowManager.Instance.Show(Equipment, "HeaterTempWarning", out windowName, alarm, msg);
                            Equipment.RequestStop();
                        }
                        else if (BandingHeater.Temperature < HeaterSetTemperature - TemperatureLowerTolerance)
                        {
                            string msg = string.Format("Heater Temp.={0}, Heater Set Temp.={1}, Upper Tolerance={2}",
                                BandingHeater.Temperature, HeaterSetTemperature, TemperatureLowerTolerance);
                            var alarm = Utility.AlarmUtility.GetAlarm(AlarmLowerThanTheSetTemperature, "Lower than the set temperature.");
                            Manager.MessageWindowManager.Instance.Show(Equipment, "HeaterTempWarning", out windowName, alarm, msg);
                            Equipment.RequestStop();
                        }
                        else
                        {
                            Manager.MessageWindowManager.Instance.CloseWindow("HeaterTempWarning");
                        }
                    }
                });
            seq.OnStop += OnStopBanding;
            seq.OnSuspending += OnStopBanding;
            seq.OnStart += OnStartBanding;

            seq.AddStep("ConfirmBandingMachinePowerOn").StepIndex =
                seq.AddItem(ConfirmBandingMachinePowerOn);
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (OutOfStrapCheck.IsOn)
                    {
                        string defaultName = "BandingStrapEmpty";
                        var alarm = Utility.AlarmUtility.GetAlarm(AlarmStrapEmpty, "Banding Strap Empty.");
                        Manager.MessageWindowManager.Instance.Show(Equipment, defaultName, out windowName, alarm, string.Empty, true);
                        actor.Suspend();
                        actor.NextStep("RetryBanding");
                    }
                    else
                        actor.NextStep();
                });
            seq.AddItem(ConfirmBandingTemperature);
            seq.AddItem(OutputStop.DoTurnOn);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem(OutputReady.DoTurnOn);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem(OutputReady.DoTurnOff);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem(OutputStart.TurnOnAction.ExecuteForSequence);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem(OutputStart.TurnOffAction.ExecuteForSequence);
            seq.AddItem(ConfirmBandingComplete);
            seq.AddItem(ConfirmDelayTimeBeforeConfirmBandingComplete);
            seq.AddItem(ConfirmBandingWire);
            seq.AddTerminate();

            seq.AddStep("RetryBanding").StepIndex =
                seq.AddItem(ConfirmBandingMachinePowerOn);
            seq.AddItem(OutputStop.DoTurnOn);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem(OutputReady.DoTurnOn);
            seq.AddItem(new FATime(FATimeType.millisecond, 500));
            seq.AddItem(OutputReady.DoTurnOff);
            seq.AddItem("ConfirmBandingMachinePowerOn");
        }

        private void OnStartBanding(object sender, EventArgs e)
        {
            RetryInfoBandingRetry.ClearCount();
        }

        private void ConfirmBandingMachinePowerOn(FASequence actor, TimeSpan time)
        {
            if (PowerOnCheck.IsOn)
                actor.NextStep();
            else
            {
                RaiseAlarm(actor, AlarmBandingMachinePowerOff);
            }
        }

        private void ConfirmWireEmpty(FASequence actor, TimeSpan time)
        {
            if (OutOfStrapCheck.IsOn)
            {
                RaiseAlarm(actor, AlarmStrapEmpty);
                actor.NextStep("RetryBanding");
            }
            else
                actor.NextStep();
        }

        private void ConfirmBandingTemperature(FASequence actor, TimeSpan time)
        {
            if (BandingHeater.Temperature < HeaterSetTemperature - TemperatureLowerTolerance)
            {
                string msg = string.Format("Heater Temp.={0}, Heater Set Temp.={1}, Upper Tolerance={2}",
                                BandingHeater.Temperature, HeaterSetTemperature, TemperatureLowerTolerance);
                var alarm = Utility.AlarmUtility.GetAlarm(AlarmLowerThanTheSetTemperature, "Low than the set temperature.");
                Manager.MessageWindowManager.Instance.Show(Equipment, "HeaterTempWarning", out _bandingHeaterTemperatureWarning, alarm, msg);

                actor.NextStep("ConfirmBandingMachinePowerOn");
            }
            else if (BandingHeater.Temperature > HeaterSetTemperature + TemperatureUpperTolerance)
            {
                string msg = string.Format("Heater Temp.={0}, Heater Set Temp.={1}, Upper Tolerance={2}",
                                BandingHeater.Temperature, HeaterSetTemperature, TemperatureUpperTolerance);
                var alarm = Utility.AlarmUtility.GetAlarm(AlarmHigherThanTheSetTemperature, "High than the set temperature.");
                Manager.MessageWindowManager.Instance.Show(Equipment, "HeaterTempWarning", out _bandingHeaterTemperatureWarning, alarm, msg);

                actor.NextStep("ConfirmBandingMachinePowerOn");
            }
            else
            {
                Manager.MessageWindowManager.Instance.CloseWindow(_bandingHeaterTemperatureWarning);
                actor.NextStep();
            }
        }

        private void ConfirmDelayTimeBeforeConfirmBandingComplete(FASequence actor, TimeSpan time)
        {
            if (TimeDelayTimeBeforeConfirmBandingComplete.Time < time)
            {
                actor.NextStep();
            }
        }

        private void ConfirmBandingComplete(FASequence actor, TimeSpan time)
        {
            if (UseBandCheckSensor == false)
                actor.NextStep();
            else if (BandCheck.IsOn)
            {
                actor.NextStep();
            }
            else if (TimeBandingTimeout.Time < time)
            {
                if (ErrorCheck.IsOn)
                {
                    RaiseAlarm(actor, AlarmBandingError);
                    actor.NextStep("RetryBanding");
                }
                else if (StrapEmptyCheck.IsOn)
                {
                    var alarm = Utility.AlarmUtility.GetAlarm(AlarmStrapEmpty, "Lack Strap.");
                    Manager.MessageWindowManager.Instance.Show(Equipment, "LackStrap", alarm.AlarmName);
                }
                else
                {
                    if (RetryInfoBandingRetry.IncreaseCount() == false)
                    {
                        RaiseAlarm(actor, AlarmBandingCompleteSignalNotReceived);
                    }

                    actor.NextStep("RetryBanding");
                }
            }
        }

        private void ConfirmBandingWire(FASequence actor, TimeSpan time)
        {
            if (UseBandCheckSensor == false)
                actor.NextStep();
            else if (BandCheck.IsOn)
                actor.NextStep();
            else if (TimeBandingTimeout.Time < time)
            {
                if (RetryInfoBandingRetry.IncreaseCount() == false)
                {
                    RaiseAlarm(actor, AlarmBandingWireNotChecked);
                }

                actor.NextStep("RetryBanding");
            }
        }

        private void OnStopBanding(object sender, EventArgs e)
        {
            OutputStart.DoTurnOff(sender);
            OutputReady.DoTurnOff(sender);
        }
    }
}
