using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAFramework.VT3500.Modules
{
    public class ExtensionStateSignalModule : Module.StateSignalModule
    {
        private VT3500.SubEquipment VT3500SEquipment
        {
            get { return Equipment as VT3500.SubEquipment; }
        }

        protected override void SwitchOnButtonLamp(ConfigClasses.EquipmentStateConfig stateConfig)
        {
            SetLampState(VT3500SEquipment.OperationUnit.StartButtonLamp, CurrentStateConfig.ButtonLamp.StartLamp, true);
            SetLampState(VT3500SEquipment.OperationUnit.StopButtonLamp, CurrentStateConfig.ButtonLamp.StopLamp, true);
            SetLampState(VT3500SEquipment.OperationUnit.InitializeButtonLamp, CurrentStateConfig.ButtonLamp.InitialLamp, true);
            SetLampState(VT3500SEquipment.OperationUnit.AlarmClearButtonLamp, CurrentStateConfig.ButtonLamp.JamClearLamp, true);
            SetLampState(VT3500SEquipment.OperationUnit.SoundClearButtonLamp, CurrentStateConfig.ButtonLamp.SoundClearLamp, true);
        }

        protected override void SwitchOffButtonLamp(ConfigClasses.EquipmentStateConfig stateConfig)
        {
            SetLampState(VT3500SEquipment.OperationUnit.StartButtonLamp, CurrentStateConfig.ButtonLamp.StartLamp, false);
            SetLampState(VT3500SEquipment.OperationUnit.StopButtonLamp, CurrentStateConfig.ButtonLamp.StopLamp, false);
            SetLampState(VT3500SEquipment.OperationUnit.InitializeButtonLamp, CurrentStateConfig.ButtonLamp.InitialLamp, false);
            SetLampState(VT3500SEquipment.OperationUnit.AlarmClearButtonLamp, CurrentStateConfig.ButtonLamp.JamClearLamp, false);
            SetLampState(VT3500SEquipment.OperationUnit.SoundClearButtonLamp, CurrentStateConfig.ButtonLamp.SoundClearLamp, false);
        }

        protected override void SetButtonLampState(ConfigClasses.EquipmentStateConfig stateConfig)
        {
            SwitchOffButtonLamp(stateConfig);
        }
    }
}
