using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Part.MemoryBasePart;
using FAFramework.Utility;

namespace FAFramework.Module
{
    public class StateSignalModule : FAModule
    {
        [FAAttribute("Sequences")]
        public FASequence Loop { get; set; }

        #region Parts
        public FAPartOnOff SignalTowerRed { get; set; }
        public FAPartOnOff SignalTowerYellow { get; set; }
        public FAPartOnOff SignalTowerGreen { get; set; }
        public FAPartOnOff SignalTowerBuzzer { get; set; }
        public FAPartOnOff SignalPhoneMelodie1 { get; set; }
        public FAPartOnOff SignalPhoneMelodie2 { get; set; }
        public FAPartOnOff SignalPhoneMelodie3 { get; set; }
        public FAPartOnOff SignalPhoneMelodie4 { get; set; }
        #endregion

        #region Parameters
        private ConfigClasses.EquipmentStateConfigGroup _equipmentStateConfigs = new ConfigClasses.EquipmentStateConfigGroup();
        [FAAttribute("Parameters")]
        [FASerializable]
        public ConfigClasses.EquipmentStateConfigGroup EquipmentStateConfigs
        {
            get { return _equipmentStateConfigs; }
            set
            {
                _equipmentStateConfigs = value;
                NotifyPropertyChanged("EquipmentStateConfigs");
            }
        }
        #endregion

        #region Status
        private bool _offSound;
        [FAAttribute("Status")]
        public bool OffSound
        {
            get { return _offSound; }
            set
            {
                if (_offSound == value) return;
                _offSound = value;
                NotifyPropertyChanged("OffSound");
            }
        }

        private bool _customMode;
        [FAAttribute("Status")]
        public bool CustomMode
        {
            get { return _customMode; }
            set
            {
                if (_customMode == value) return;
                _customMode = value;
                NotifyPropertyChanged("CustomMode");
            }
        }

        private ConfigClasses.EquipmentStateConfig _customState = new ConfigClasses.EquipmentStateConfig();
        [FAAttribute("")]
        public ConfigClasses.EquipmentStateConfig CustomState
        {
            get { return _customState; }
            set
            {
                if (_customState == value) return;
                _customState = value;
                NotifyPropertyChanged("CustomState");
            }
        }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FALibrary.Utility.FATime TimeLampSwitchTime { get; set; }
        #endregion

        protected ConfigClasses.EquipmentStateConfig CurrentStateConfig { get; private set; }

        public override void InitializeSequence()
        {
            MakeLoop();

            Equipment.StatePreAlarm.OnChangedState +=
                delegate
                {
                    SetStateSignal(EquipmentStateConfigs.ErrorStateConfig);
                    StartLoop();
                };

            Equipment.StatePreWarning.OnChangedState +=
                delegate
                {
                    SetStateSignal(EquipmentStateConfigs.WarningStateConfig);
                    StartLoop();
                };

            Equipment.StateStop.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    SetStateSignal(EquipmentStateConfigs.AutoStopStateConfig);
                    StartLoop();
                };

            Equipment.StateRun.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    OffSound = false;
                    SetStateSignal(EquipmentStateConfigs.AutoRunStateConfig);
                    StartLoop();
                };

            Equipment.StateRundown.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    OffSound = false;
                    SetStateSignal(EquipmentStateConfigs.RunDownStateConfig);
                    StartLoop();
                };

            Equipment.StateInitialize.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    OffSound = false;
                    CustomMode = false;
                    SetStateSignal(EquipmentStateConfigs.InitializeStateConfig);
                    StartLoop();
                };

            Equipment.StateAlarm.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    SetStateSignal(EquipmentStateConfigs.ErrorStateConfig);
                    StartLoop();
                };

            Equipment.StateWarning.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    SetStateSignal(EquipmentStateConfigs.WarningStateConfig);
                    StartLoop();
                };

            Equipment.StateSuspend.OnChangedState +=
                delegate (object sender, FALibrary.FAGenericEventArgs<Equipment.EquipmentState> e)
                {
                    SetStateSignal(EquipmentStateConfigs.SuspendedConfig);
                };

            Equipment.StateRun.OnClearAlarm +=
                delegate
                {
                    SetStateSignal(EquipmentStateConfigs.AutoRunStateConfig);
                    StartLoop();
                };

            Equipment.StateRundown.OnClearAlarm +=
                delegate
                {
                    SetStateSignal(EquipmentStateConfigs.RunDownStateConfig);
                    StartLoop();
                };
        }

        private void MakeLoop()
        {
            var seq = Loop;

            seq.OnStart +=
                delegate
                {
                    OffSound = false;
                    CustomMode = false;
                };

            seq.AddStep("Loop").StepIndex = seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (CustomMode)
                    {
                        OffSound = false;
                        actor.NextStep("CustomMode");
                    }
                    else
                        actor.NextStep();
                });
            seq.AddItem((object obj) => SwitchOn(CurrentStateConfig));
            seq.AddItem(TimeLampSwitchTime);
            seq.AddItem((object obj) => SwitchOff(CurrentStateConfig));
            seq.AddItem(TimeLampSwitchTime);
            seq.AddItem("Loop");

            seq.AddStep("CustomMode").StepIndex = seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    if (CustomMode)
                        actor.NextStep();
                    else
                    {
                        OffSound = false;
                        actor.NextStep("Loop");
                    }
                });
            seq.AddItem((object obj) => SwitchOn(CustomState));
            seq.AddItem(TimeLampSwitchTime);
            seq.AddItem((object obj) => SwitchOff(CustomState));
            seq.AddItem(TimeLampSwitchTime);
            seq.AddItem("CustomMode");
        }

        private void StartLoop()
        {
            if (Loop == null) return;
            if (Loop.IsStartable() == true)
                Loop.Start();
        }

        private void SwitchOn(ConfigClasses.EquipmentStateConfig stateConfig)
        {
            if (stateConfig == null) return;

            try
            {
                SetLampState(SignalTowerGreen, stateConfig.SignalTower.GreenLamp, true);
                SetLampState(SignalTowerYellow, stateConfig.SignalTower.YellowLamp, true);
                SetLampState(SignalTowerRed, stateConfig.SignalTower.RedLamp, true);
                SetBuzzerState(stateConfig.SignalTower.Buzzer, true);
                SwitchOnButtonLamp(stateConfig);
            }
            catch
            {
            }
        }

        private void SwitchOff(ConfigClasses.EquipmentStateConfig stateConfig)
        {
            if (stateConfig == null) return;

            try
            {
                SetLampState(SignalTowerGreen, stateConfig.SignalTower.GreenLamp, false);
                SetLampState(SignalTowerYellow, stateConfig.SignalTower.YellowLamp, false);
                SetLampState(SignalTowerRed, stateConfig.SignalTower.RedLamp, false);
                SetBuzzerState(stateConfig.SignalTower.Buzzer, false);
                SwitchOffButtonLamp(stateConfig);
            }
            catch
            {
            }
        }

        private void SetStateSignal(object config)
        {
            if (config == null) return;

            if (config is ConfigClasses.EquipmentStateConfig)
            {
                CurrentStateConfig = config as ConfigClasses.EquipmentStateConfig;

                try
                {
                    SetLampState(SignalTowerGreen, CurrentStateConfig.SignalTower.GreenLamp);
                    SetLampState(SignalTowerYellow, CurrentStateConfig.SignalTower.YellowLamp);
                    SetLampState(SignalTowerRed, CurrentStateConfig.SignalTower.RedLamp);
                    SetBuzzerState(CurrentStateConfig.SignalTower.Buzzer);
                    SetButtonLampState(CurrentStateConfig);
                }
                catch
                {
                }
            }
        }

        protected void SetLampState(FAPartOnOff part, int state, bool on = false)
        {
            if (state == ConfigClasses.GlobalConst.LAMP_OFF)
                part.Off.Execute(this);
            else if (state == ConfigClasses.GlobalConst.LAMP_ON)
                part.On.Execute(this);
            else if (state == ConfigClasses.GlobalConst.LAMP_SWITCH)
            {
                if (on == true)
                    part.On.Execute(this);
                else
                    part.Off.Execute(this);
            }
        }

        private void SetBuzzerState(int state, bool on = false)
        {
            if (state == ConfigClasses.GlobalConst.BUZZER_ON && OffSound == false)
            {
                SignalTowerBuzzer.On.Execute(this);
                SignalPhoneMelodie1.Off.Execute(this);
                SignalPhoneMelodie2.Off.Execute(this);
                SignalPhoneMelodie3.Off.Execute(this);
                SignalPhoneMelodie4.Off.Execute(this);
            }
            else if (state == ConfigClasses.GlobalConst.BUZZER_OFF || OffSound)
            {
                SignalTowerBuzzer.Off.Execute(this);
                SignalPhoneMelodie1.Off.Execute(this);
                SignalPhoneMelodie2.Off.Execute(this);
                SignalPhoneMelodie3.Off.Execute(this);
                SignalPhoneMelodie4.Off.Execute(this);
            }
            else if (state == ConfigClasses.GlobalConst.BUZZER_SWITCH)
            {
                if (on == true)
                    SignalTowerBuzzer.On.Execute(this);
                else
                    SignalTowerBuzzer.Off.Execute(this);
            }
            else if (state == ConfigClasses.GlobalConst.BUZZER_MELODY_1)
            {
                SignalTowerBuzzer.Off.Execute(this);
                SignalPhoneMelodie1.On.Execute(this);
            }
            else if (state == ConfigClasses.GlobalConst.BUZZER_MELODY_2)
            {
                SignalTowerBuzzer.Off.Execute(this);
                SignalPhoneMelodie2.On.Execute(this);
            }
            else if (state == ConfigClasses.GlobalConst.BUZZER_MELODY_3)
            {
                SignalTowerBuzzer.Off.Execute(this);
                SignalPhoneMelodie3.On.Execute(this);
            }
            else if (state == ConfigClasses.GlobalConst.BUZZER_MELODY_4)
            {
                SignalTowerBuzzer.Off.Execute(this);
                SignalPhoneMelodie4.On.Execute(this);
            }
        }

        protected virtual void SwitchOnButtonLamp(ConfigClasses.EquipmentStateConfig stateConfig)
        {
        }

        protected virtual void SwitchOffButtonLamp(ConfigClasses.EquipmentStateConfig stateConfig)
        {
        }

        protected virtual void DoTurnOffSound(object sender, EventArgs e)
        {
            OffSound = true;
        }

        protected virtual void SetButtonLampState(ConfigClasses.EquipmentStateConfig stateConfig)
        {
        }
    }
}
