using FAFramework.Utility;
using FAFramework.VT3500.JobInfo;
using FALibrary;
using FALibrary.Sequence;
using FALibrary.Utility;
using System;

namespace FAFramework.VT3500.Modules
{
    public class FAMainLoopModule : Module.FAModule
    {
        private VT3500.SubEquipment VT3500Equipment
        {
            get { return Equipment as VT3500.SubEquipment; }
        }

        #region Sequences

        [FAAttribute("Sequences")]
        public FASequence MainLoop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence OilTempFan { get; set; }
        [FAAttribute("Sequences")]
        public FASequence InitializeMachine { get; set; }
        [FAAttribute("Sequences")]
        public FASequence LoadRecipe { get; set; }

        #endregion

        #region Status
        [FAAttribute("Status")]
        public bool InitializeSelect { get; set; }

        [FAAttribute("Status")]
        public bool StatusCheckOk { get; set; }

        [FAAttribute("Status")]
        public bool Initializing { get; set; }

        //Recipe
        [FAAttribute("Status")]
        public string OldJobName { get; set; }
        #endregion

        #region Parameters
        [FAAttribute("Parameters")]
        [FAProperty]
        public string SelectJob { get; set; }
        [FAAttribute("Parameters")]
        public JobInfo.MoveJobInfo MoveJobInfo { get; set; } = new MoveJobInfo();
        [FAAttribute("Parameters")]
        public JobInfo.FALotJobInfo SelectedJobInfo { get; set; }
        #endregion

        #region Part
        public ExtensionOperationModule OperationModule { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Initialize is Completed.", "Initialize is Completed")]
        [AlarmDescription(KnownCulture.Korean, "초기화가 끝났습니다.", "초기화가 끝났습니다.")]
        public int InitializeCompleted { get; set; }
        
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Recipe Is Null", "Plese Select Recipe")]
        [AlarmDescription(KnownCulture.Korean, "레시피 가 선택되지 않았습니다", "레시피를 선택해 주세요")]
        public int AlarmJobLoadFail { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "성형 프레스 오일 온도가 너무 높습니다! 프레스 오일을 식혀주세요")]
        public int AlarmFirstPressOilTempCheck { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "피넛클 프레스 오일 온도가 너무 높습니다! 프레스 오일을 식혀주세요")]
        public int AlarmSecondPressOilTempCheck { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "옵션 프레스 오일 온도가 너무 높습니다! 프레스 오일을 식혀주세요")]
        public int AlarmOptionPressOilTempCheck { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "톰슨 프레스 오일 온도가 너무 높습니다! 프레스 오일을 식혀주세요")]
        public int AlarmThirdPressOilTempCheck { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "실링 프레스 오일 온도가 너무 높습니다! 프레스 오일을 식혀주세요")]
        public int AlarmFourthPressOilTempCheck { get; set; }
        #endregion

        public override void InitializeSequence()
        {
            var equip = Equipment as VT3500.SubEquipment;
            try
            {
                SelectedJobInfo = equip.JobManagerInstance.GetJob(SelectJob, out var msg);
            }
            catch
            {
            }
            MakeMainLoop();
            MakeInitializeMachine();
            MakeLoadRecipe();
            MakeOilTempFan();
        }

        private void MakeMainLoop()
        {

            var seq = MainLoop;
            var eqp = VT3500Equipment;

            #region Event
            seq.OnSuspended +=
              delegate
              {
              };
            seq.AddWatcher(
                 () =>
                 {
                     if (VT3500Equipment.State == VT3500Equipment.StateRun)
                     {
                         OperationModule.CheckDoorStatus.Start();
                         OperationModule.CheckAreaStatus.Start();
                     }

                     //if (SubEquipment.Instance.OperationUnit.StopButtonSwitch.IsOn)
                     //{
                     //    OperationModule.OnceCycleStartSignal = true;
                     //}
                 });
            #endregion
            //210205
            seq.AddStep("Loop").StepIndex =
            seq.AddItem(OilTempFan);// Fan On/Off 
            seq.AddItem(
                (actor, time) =>
                {
                    if (eqp.FrontModule.FrontModuleTerminated) //재사용
                    {
                        if (eqp.ThirdPressModule.UsePressStopSequence)
                        {
                            eqp.ThirdPressModule.UsePressStopSequence = false;
                            eqp.RearModule.BandReceiveComplete = false;
                            eqp.ThirdPressModule.UsePress = true;
                            //eqp.RearModule.PlaceSkip = true;

                            actor.NextStep();
                        }
                        else if (eqp.ThirdPressModule.UsePressResumeSequence)
                        {
                            eqp.ThirdPressModule.UsePressResumeSequence = false;
                            eqp.ThirdPressModule.UsePress = false;
                            //eqp.ThirdPressModule.MotorRun.Off.Execute(actor); 210823
                            //eqp.RearModule.PlaceSkip = false;
                            actor.NextStep();
                        }
                        else
                        {
                            actor.NextStep();
                        }
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(eqp.FrontModule.MainLoop, eqp.RearModule.MainLoop);
            seq.AddItem("Loop");
        }
        private void MakeOilTempFan()
        {
            var seq = OilTempFan;
            var eqp = VT3500Equipment;

            seq.AddItem(
                (actor, time) =>
                {
                    if (eqp.FirstPressModule.PressOilTempCheck.IsOn)
                    {
                        eqp.FirstPressModule.PressFanMotor.DoTurnOn(actor);
                        //RaiseAlarm(actor, AlarmFirstPressOilTempCheck);
                    }
                    else
                    {
                        eqp.FirstPressUnit.PressFanMotor.DoTurnOff(actor);
                    }

                    if (eqp.SecondPressModule.PressOilTempCheck.IsOn)
                    {
                        eqp.SecondPressModule.PressFanMotor.DoTurnOn(actor);
                        // RaiseAlarm(actor, AlarmSecondPressOilTempCheck);
                    }
                    else
                    {
                        eqp.SecondPressModule.PressFanMotor.DoTurnOff(actor);
                    }

                    if (eqp.OptionPressModule.PressOilTempCheck.IsOn)
                    {
                        eqp.OptionPressModule.PressFanMotor.DoTurnOn(actor);
                        // RaiseAlarm(actor, AlarmOptionPressOilTempCheck);
                    }
                    else
                    {
                        eqp.OptionPressModule.PressFanMotor.DoTurnOff(actor);
                    }

                    if (eqp.ThirdPressModule.PressOilTempCheck.IsOn)
                    {
                        eqp.ThirdPressModule.PressFanMotor.DoTurnOn(actor);
                        //RaiseAlarm(actor, AlarmThirdPressOilTempCheck);
                    }
                    else
                    {
                        eqp.ThirdPressModule.PressFanMotor.DoTurnOff(actor); 
                    }
                    if (eqp.FourthPressModule.PressOilTempCheck.IsOn)
                    {
                        eqp.FourthPressModule.PressFanMotor.DoTurnOn(actor);
                        //RaiseAlarm(actor, AlarmFourthPressOilTempCheck);
                    }
                    else
                    {
                        eqp.FourthPressModule.PressFanMotor.DoTurnOff(actor);
                    }
                    actor.NextTerminate();
                });

        }

        private void MakeInitializeMachine()
        {
            var seq = InitializeMachine;

            var eqp = VT3500Equipment;

            #region Event Handler

            seq.OnStart +=
               delegate
               {
                   //eqp.IsInitializedOk = true;
                   StatusCheckOk = false;
                   Initializing = false;
                   eqp.OperationUnit.InitializeButtonLamp.DoTurnOn(this);
                   eqp.OperationUnit.StopButtonLamp.DoTurnOff(this);
               };

            seq.OnSuspended +=
                delegate
                {
                    eqp.ClearSubSequencesState();
                };

            seq.OnTerminate +=
                delegate
                {
                    eqp.MainLoopModule.MainLoop.ClearState();
                    eqp.ClearSubSequencesState();
                    eqp.IsInitializedOk = true;
                    eqp.RequestStop();
                    Initializing = false;
                    ShowMessage("Initialize Completed", InitializeCompleted, "InitializeCompleted");

                    eqp.OperationUnit.InitializeButtonLamp.DoTurnOff(this);
                    eqp.OperationUnit.StopButtonLamp.DoTurnOn(this);
                };
            seq.AddWatcher(
                () =>
                {
                    if (VT3500Equipment.State == VT3500Equipment.StateInitialize)
                    {
                        OperationModule.CheckDoorStatus.Start();
                        OperationModule.CheckAreaStatus.Start();
                    }
                });
            #endregion

            seq.AddItem((object obj) => WriteTraceLog("START INITIALIZING"));
            seq.AddItem(LoadJob);
            seq.AddItem(eqp.FrontModule.Initialize, eqp.RearModule.Initialize);
            seq.AddItem(
                (o) =>
                {
                    foreach (var module in Equipment.ModuleList)
                    {
                        module.ClearProductInfo();
                        module.ClearRetryInfo();
                        module.SetDefaultValueAtProperty();
                    }
                });
            seq.AddTerminate();
        }

        private void MakeLoadRecipe()
        {
            var seq = LoadRecipe;

            seq.AddItem(
               (actor, time) =>
               {
                   if (SelectJob == null)
                   {
                       RaiseAlarm(actor, AlarmJobLoadFail, "Job is null\n");
                   }
                   else if (OldJobName == SelectJob)
                   {
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep();
                   }
               });
            seq.AddItem(LoadJob);
            seq.AddItem(
                delegate (FASequence actor, TimeSpan time)
                {
                    OldJobName = SelectJob;
                    WriteTraceLog(string.Format("SelectJob ={0}", SelectJob));
                    //원단이송
                    WriteTraceLog(string.Format("JobCopy => FeedingSpeed={0} , FeedingPitch={1}, MoldingTime={2}",
                        MoveJobInfo.FeedingSpeed,
                        MoveJobInfo.FeedingPitch,
                        MoveJobInfo.MoldingTime.Time));
                    //포장이송
                    WriteTraceLog(string.Format("JobCopy => PackingFeedSpeed={0} , PackingFeedPitch={1}, SealingTime={2}",
                        MoveJobInfo.PackingFeedSpeed,
                        MoveJobInfo.PackingFeedPitch,
                        MoveJobInfo.SealingTime.Time));
                    //Count Setting
                    WriteTraceLog(string.Format("JobCopy => PackageCount={0} , CuttingCount={1}",
                        MoveJobInfo.PackageCount,
                        MoveJobInfo.CuttingCount));
                    WriteTraceLog(string.Format("JobOption => UseFirstPress={0}, UseSecondPress={1}, UseOptionPress={2},UseThirdPress={3} ,UseFourthPress={4}, " +
                                                               "UseTopPeelingg={5}, UseBottomPeeling={6}, UsePackingScrap={7}, UseIMark={8}, TapeLoadingPos ={9}, TapeLoadingSlowPos={10}, TapeUnUseImarkPos={11}",
                       MoveJobInfo.UseFirstPress,
                       MoveJobInfo.UseSecondPress,
                       MoveJobInfo.UseOptionPress,
                       MoveJobInfo.UseThirdPress,
                       MoveJobInfo.UseFourthPress,
                       MoveJobInfo.UseTopPeeling,
                       MoveJobInfo.UseBottomPeeling,
                       MoveJobInfo.UsePackingScrap,
                       MoveJobInfo.UseIMark,
                       //210705
                       MoveJobInfo.TapeLoadingPos,
                       MoveJobInfo.TapeLoadingSlowPos,
                       MoveJobInfo.TapeUnUseImarkPos));
                    actor.NextStep();
                });
          
        }

        private void LoadJob(FASequence actor, TimeSpan time)
        {
            var equip = Equipment as VT3500.SubEquipment;
            string jobname = SelectJob;
            string msg = "";

            if (jobname == null)
            {
                RaiseAlarm(actor, AlarmJobLoadFail, "jobname is null\n" + msg);
            }
            else
            {
                var job = equip.JobManagerInstance.GetJob(jobname, out msg);

                if (job == null)
                {
                    RaiseAlarm(actor, AlarmJobLoadFail, "Job is null\n" + msg);
                }
                else
                {
                    if (job.MoveJobInfo == null)
                    {
                        RaiseAlarm(actor, AlarmJobLoadFail, "MoveJobInfo is null\n" + msg);
                    }
                    else
                    {
                        try
                        {
                            job.MoveJobInfo.CopyTo(MoveJobInfo);

                            var Feedspeed = Convert.ToUInt32(MoveJobInfo.FeedingSpeed);
                            var PackingSpeed = Convert.ToUInt32(MoveJobInfo.PackingFeedSpeed);

                            //var FinalSpeed = Convert.ToUInt32(VT3500Equipment.FrontModule.SpeedScale);
                            
                            VT3500Equipment.FrontLoadingUnit.TapeLoadingServo.TapeLoadingPos.DriveSpeed = Feedspeed ; 
                            VT3500Equipment.FrontLoadingUnit.TapeLoadingServo.TapeLoadingPos.Position = MoveJobInfo.FeedingPitch;
                            VT3500Equipment.FirstPressModule.PressDelay = MoveJobInfo.MoldingTime;
                            VT3500Equipment.FrontLoadingUnit.BandTransferServo.TapeLoadingPos.DriveSpeed = PackingSpeed ;
                            VT3500Equipment.FrontLoadingUnit.BandTransferServo.TapeLoadingPos.Position = MoveJobInfo.PackingFeedPitch;
                            VT3500Equipment.FourthPressModule.PressDelay = MoveJobInfo.SealingTime;
                            VT3500Equipment.RearModule.JobCount = MoveJobInfo.CuttingCount;
                            VT3500Equipment.RearLoadingUnit.BandRollerServo.TapeLoadingPos.Position = MoveJobInfo.TapeLoadingPos;
                            VT3500Equipment.RearLoadingUnit.BandRollerServo.TapeLoadingSlowPos.Position = MoveJobInfo.TapeLoadingSlowPos;
                            VT3500Equipment.RearLoadingUnit.BandRollerServo.TapeUnUseImarkPos.Position = MoveJobInfo.TapeUnUseImarkPos;
                            //Job_Option 
                            VT3500Equipment.FirstPressModule.UsePress = MoveJobInfo.UseFirstPress;
                            VT3500Equipment.SecondPressModule.UsePress = MoveJobInfo.UseSecondPress;
                            VT3500Equipment.OptionPressModule.UsePress = MoveJobInfo.UseOptionPress;
                            VT3500Equipment.ThirdPressModule.UsePress = MoveJobInfo.UseThirdPress;

                            if (!VT3500Equipment.ThirdPressModule.UsePress)
                            {
                                VT3500Equipment.FourthPressModule.UsePress = false;
                                MoveJobInfo.UseFourthPress = false;
                            }
                            else
                            {
                                VT3500Equipment.FourthPressModule.UsePress = MoveJobInfo.UseFourthPress;
                            }

                            if (VT3500Equipment.OptionPressModule.UsePress)
                            {
                                VT3500Equipment.FourthPressModule.UsePress = false;
                            }

                            VT3500Equipment.FrontModule.UseTopPeeling = MoveJobInfo.UseTopPeeling;
                            VT3500Equipment.FrontModule.UseBottomPeeling = MoveJobInfo.UseBottomPeeling;
                            VT3500Equipment.RearModule.UsePackingScrap = MoveJobInfo.UsePackingScrap;
                            VT3500Equipment.RearModule.UseIMark = MoveJobInfo.UseIMark;
                            actor.NextStep();

                        }
                        catch (Exception e)
                        {
                            RaiseAlarm(actor, AlarmJobLoadFail, e.ToString());
                        }

                    }
                }
            }
        }

    }
}
