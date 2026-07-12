using FAFramework.GUI;
using FAFramework.Utility;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FALibrary.Utility;
using System;
using FAFramework.VT3500.ExtendedParts;
using FAFramework.VT3500.JobInfo;
using FALibrary.Part.Inverter;
namespace FAFramework.VT3500.Modules
{
    public class FAFrontLoadingModule : Module.FAPassModule
    {

        #region Sequences
        //Auto Sequence 
        [FAAttribute("Sequences")]
        public FASequence MainLoop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence OnceCycleEnd { get; set; }
        [FAAttribute("Sequences")]
        public FASequence FrontOnceCycleEnd { get; set; }
        //Auto Sequence MainAutomicLoop
        [FAAttribute("Sequences")]
        public FASequence MainAutomicLoop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Initialize { get; set; }
        [FAAttribute("Sequences")]
        public FASequence FrontACMotorLoop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkInverterMotorLoading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkACMotorLoading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkFirstBandMoveLoading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkLoopBandMoveLoading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkBandPickMove { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkBandPlaceMove { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkFirstPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkTomsonPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkManualPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkManualWithOutTomsonPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence TapeMovePlaceCylinder { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkBandPlaceMoveCylinder { get; set; }
        [FAAttribute("Sequences")]
        public FASequence TapeMovePickCylinder { get; set; }

        [FAAttribute("Sequences")]
        public FASequence WorkTapeMovePickCylinder { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkTapeMovePick { get; set; }
       
        //Manual Sequence
        [FAAttribute("Sequences")]
        public FASequence WorkManualLoading { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkManualMoving { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkManualPicking { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkManualOnceOneCycle { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkMoveWithOutTomson { get; set; }


        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkFirstPressServo { get; set; }
        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkSecondPressServo { get; set; }
        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkThridPressServo { get; set; }




        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkFirstManualPressServo { get; set; }
        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkSecondManualPressServo { get; set; }
        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkThridManualPressServo { get; set; }


        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkOptionPressServo { get; set; }

        //edge-lee 241118
        [FAAttribute("Sequences")]
        public FASequence WorkOptionManualPressServo { get; set; }





        #endregion

        #region Parts 
        // Front Module --------------------------------------------------------------------------------------------------------------------
        public FAPartOnOffSensor ShapeTapeTensionUpSensor { get; set; }
        public FAPartOnOffSensor ShapeTapeTensionDownSensor { get; set; }
        public FAPartOnOffSensor ShapeTapeTensionSlowSensor { get; set; }
        public FAPartOnOffSensor PackingTapeTensionUpSensor { get; set; }
        public FAPartOnOffSensor PackingTapeTensionDownSensor { get; set; }
        public FAPartOnOffSensor TapeCoverServoOffSignal { get; set; }
        public FAPartOnOffSensor PackingMotorRunCheck { get; set; }
        public FAPartOneWayACMotor PackingTapeLoadingMotor { get; set; }
        public FAPartGripRelease TapeHoldGrip { get; set; }
        public FAPartGripRelease TapeLoadGrip { get; set; }
        public FAPartOnOff BandVaccum { get; set; }
        public FAPartOnOff BandVaccumEject { get; set; }
        public FAPartOnOffSensor VacuumCheck_Front { get; set; }
        public FAPartOnOffSensor VacuumCheck_Rear { get; set; }
        public FAPartPushHome BandPitchChangeCylinder { get; set; }
        public FAPartOneWayACMotor BypassCoveyorMotor { get; set; }
        public FAPartOnOffSensor BypassCoveyorExistCheck { get; set; }
        public FAPartOnOffSensor ReelPowerBrakeOnCheck { get; set; }
        public FAPartOnOffSensor ReelPowerServoOnCheck { get; set; }

        //public FAPartOneWayACMotor TapeCoverMotor { get; set; }
        public FATapeCoverServo TapeCoverServo { get; set; }
        public FAOptinPressServo OptionPressServo { get; set; }



        public FAOptionServo OptionServo { get; set; }


        public FAFirstPressServo FirstPressServo { get; set; }
        public FASecondPressServo SecondPressServo { get; set; }
        public FAThirdPressServo ThirdPressServo { get; set; }
     


        public FATapeLoadingServo TapeLoadingServo { get; set; }
        public FABandTransferServo BandTransferServo { get; set; }
        public FABandPickServo BandPickServo { get; set; }
        public FAInverter InverterMotor { get; set; }
        #endregion

        #region Alarm
        [DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        [AlarmDescription(KnownCulture.EnglishUS, "Can Not Detected Sensor", "Can Not Detected Sensor")]
        [AlarmDescription(KnownCulture.Korean, "센서 미감지", "센서가 감지되지 않고 있습니다. 제품을 확인해주세요.")]
        public int AlarmCanNotDetectedSensor { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.WARNING, "Vacuum Time Out")]
        public int AlarmVacuumTimeOut { get; set; }

        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmVacuumRetry { get; set; } // 베큠 재시도 알람


        [FAProperty]
        [FAAttribute("Alarm")]
        public int TourqeAlarmCheck { get; set; } // 토크 부족 확인




        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmRunningMessage { get; set; }
        //[DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        //[AlarmDescription(KnownCulture.EnglishUS, "Running Machine!", "Machine Is Running")]
        //[AlarmDescription(KnownCulture.Korean, "이창이 사라질 때 까지 설비조작 금지!", "이창이 사라질 때 까지 설비조작 금지!!")]
        //public int AlarmRunningMessage { get; set; }

        //[DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        //[AlarmDescription(KnownCulture.EnglishUS, "Materials Empty!", "Materials Empty")]
        //[AlarmDescription(KnownCulture.Korean, "현재 원단을 다 사용하였습니다!", "원단을 새로 넣어주시고, 초기화 후 재기동 해주세요")]
        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmShapeMoldingMaterialEmpty { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime VacuumDelay { get; set; }
        [FAAttribute("Time")]
        public FATime TapeLoadingGripSensorDelay { get; set; }
        [FAAttribute("Time")]
        public FATime TimeToRearTerminated { get; set; }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoVacuumRetry { get; set; }
        #endregion

        #region Status
        [FAAttribute("Parameters")]
        public bool PlaceComplete { get; set; }
        #endregion

        #region Parameters
        [FAAttribute("Parameters")]
        public bool FrontModuleTerminated { get; set; }
        [FAAttribute("Parameters")]
        public bool LoadingStep { get; set; }
        [FAAttribute("Parameters")]
        public bool ManualPressOnce { get; set; }
        [FAAttribute("Parameters")]
        public bool BandPlaceStandby { get; set; }

        [FAAttribute("Parameters")]
        public bool FeedingBeforeLock { get; set; }

        [FAAttribute("Parameters")]
        public bool FeedingAfterLock { get; set; }

        [FAAttribute("Parameters")]
        public bool FeedingError { get; set; }
        [FAAttribute("Parameters")]
        public bool OneCycleTerminate { get; set; }

        [FAAttribute("Parameters")]
        public bool BandPlaceInitLock { get; set; }
        [FAAttribute("Parameters")]
        public bool BandLoadingInitLock { get; set; }
        [FAAttribute("Parameters")] 
        public bool RearStop { get; set; }
        [FAAttribute("Parameters")]
        public bool PlaceOn { get; set; }
        [FAAttribute("Parameters")]
        public bool MachineResume { get; set; }
        [FAAttribute("Parameters")]
        public bool FirstStart { get; set; }
        //[FAAttribute("Parameters")]
        //public bool BandPlaceFinish { get; set; } // 동작 완료 확인

        private int _highSpeedVelocity;
        [FAPropertyAttribute]
        [FA("Parameters")]
        public int HighSpeedVelocity
        {
            get { return _highSpeedVelocity; }
            set
            {
                if (_highSpeedVelocity == value) return;
                _highSpeedVelocity = value;
                NotifyPropertyChanged("HighSpeedVelocity");
            }
        }

        private int _lowSpeedVelocity;
        [FAPropertyAttribute]
        [FA("Parameters")]
        public int LowSpeedVelocity
        {
            get { return _lowSpeedVelocity; }
            set
            {
                if (_lowSpeedVelocity == value) return;
                _lowSpeedVelocity = value;
                NotifyPropertyChanged("LowSpeedVelocity");
            }
        }

        private int _inverter_WriteSpeed_Fast;
        [FAPropertyAttribute]
        [FA("Parameters")]
        public int Inverter_WriteSpeed_Fast
        {
            get { return _inverter_WriteSpeed_Fast; }
            set
            {
                if (_inverter_WriteSpeed_Fast == value) return;
                _inverter_WriteSpeed_Fast = value;
                NotifyPropertyChanged("Inverter_WriteSpeed_Fast");
            }
        }
        private int _inverter_WriteSpeed_Slow;
        [FAPropertyAttribute]
        [FA("Parameters")]
        public int Inverter_WriteSpeed_Slow
        {
            get { return _inverter_WriteSpeed_Slow; }
            set
            {
                if (_inverter_WriteSpeed_Slow == value) return;
                _inverter_WriteSpeed_Slow = value;
                NotifyPropertyChanged("Inverter_WriteSpeed_Slow");
            }
        }
        private int _speedScale;
        [FAPropertyAttribute]
        [FA("Parameters")]
        public int SpeedScale
        {
            get { return _speedScale; }
            set
            {
                if (_speedScale == value) return;
                _speedScale = value;
                NotifyPropertyChanged("SpeedScale");
            }
        }
        #endregion

        #region Jobs
        private bool _useTopPeeling;
        [FAPropertyAttribute]
        [FA("Jobs")]
        public bool UseTopPeeling
        {
            get { return _useTopPeeling; }
            set
            {
                if (_useTopPeeling == value) return;
                _useTopPeeling = value;
                NotifyPropertyChanged("UseTopPeeling");
            }
        }
        private bool _useBottomPeeling;
        [FAPropertyAttribute]
        [FA("Jobs")]
        public bool UseBottomPeeling
        {
            get { return _useBottomPeeling; }
            set
            {
                if (_useBottomPeeling == value) return;
                _useBottomPeeling = value;
                NotifyPropertyChanged("UseBottomPeeling");
            }
        }
        #endregion

        #region UI
        private double _tapeLoadingServoSettingLength;
        [FAPropertyAttribute]
        [FA("UI")]
        public double TapeLoadingServoSettingLength
        {
            get { return _tapeLoadingServoSettingLength; }
            set
            {
                if (_tapeLoadingServoSettingLength == value) return;
                _tapeLoadingServoSettingLength = value;
                NotifyPropertyChanged("TapeLoadingServoSettingLength");
            }
        }

        private double _tapeLoadingServoUsedLength;
        [FAPropertyAttribute]
        [FA("UI")]
        public double TapeLoadingServoUsedLength
        {
            get { return _tapeLoadingServoUsedLength; }
            set
            {
                if (_tapeLoadingServoUsedLength == value) return;
                _tapeLoadingServoUsedLength = value;
                NotifyPropertyChanged("TapeLoadingServoUsedLength");
            }
        }
        #endregion
        //
        #region Modules
        public FARearLoadingModule RModule { get; set; } // RearModule 참조
        public FAPressModule FirstPressModule { get; set; } // FirstPressModule 참조
        public FAPressModule SecondPressModule { get; set; } // SecondPressModule 참조
        public FAPressModule OptionPressModule { get; set; } // 신규
        public FAPressModule ThirdPressModule { get; set; } // ThirdPressModule 참조
        public ExtensionOperationModule ModuleOperation { get; set; }
        public SubUnits.FAPressUnit FirstPressUnit { get; set; } // FirstPressUnit 참조
        public SubUnits.FAPressUnit SecondPressUnit { get; set; } // SecondPressUnit 참조
        public SubUnits.FAPressUnit OptionPressUnit { get; set; } // 신규
        public SubUnits.FAPressUnit ThirdPressUnit { get; set; } // ThirdPressUnit 참조
        #endregion
        
        #region InterLock
        public override void SetInterlock()
        {
            base.SetInterlock();

            //AddServoCanIMoveInterlock(BandTransferServo,
            //   (actualPos, targetPos) => ThirdPressModule.Closing.IsOn,
            //   $"톰슨프레스가 하강중입니다, 하강 출력 = {ThirdPressModule.Closing.Status}");

            //AddPartInterlock(ThirdPressModule.Closing.On,
            //    () => BandTransferServo.ActualPos >= BandTransferServo.StandbyPos.Position + BandTransferServo.Tolerance,
            //    $"톰슨프레스가 하강중, TransferServo를 확인해주세요.");
        }
        #endregion

        public override void InitializeSequence()
        {
            MakeMainLoop();
            MakeOnceCycleEnd();
            MakeInitialize();
            MakeFrontACMotorLoop();
            MakeWorkInverterMotorLoading();
            MakeWorkACMotorLoading();
            MakeWorkPress();
            MakeWorkTomsonPress();
            MakeWorkBandPickMove();
            MakeWorkBandPlaceMove();
            MakeWorkManualOnceOneCycle();
            //변경

            MakeWorkFirstPressServo();
            MakeWorkSecondPressServo();
            MakeWorkThridPressServo();
            MakeWorkOptionPressServo();


            MakeWorkFirstManualPressServo();
            MakeWorkSecondManualPressServo();
            MakeWorkThridManualPressServo();
            MakeWorkOptionManualPressServo();


            MakeWorkFirstBandMoveLoading();
            MakeWorkLoopBandMoveLoading();
            MakeWorkManualMoving();
            MakeWorkManualLoading();
            MakeMainAutomicLoop();
            MakeTapeMovePlaceCylinder();
            MakeWorkBandPlaceMoveCylinder();
            MakeTapeMovePickCylinder();
            MakeWorkTapeMovePickCylinder();
            MakeWorkTapeMovePick();
            MakeWorkManualPress();
            MakeFrontOnceCycleEnd();
            MakeWorkManualPicking();
            MakeWorkFirstPress();
            MakeWorkMoveWithOutTomson();
            MakeWorkManualWithOutTomsonPress();
        }

        private void MakeWorkOptionManualPressServo()
        {
            var seq = WorkOptionManualPressServo;

            #region Event
            seq.OnStart += delegate
            {
                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
            };
            #endregion

            PullQuestionMessageBoxWindow questionWindow = null;

            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new PullQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "서보를 동작 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (questionWindow != null)
                    {
                        if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.ContinueSequence)
                        {
                            actor.NextStep();
                        }
                        else if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.Cancel)
                        {
                            actor.NextTerminate();
                        }
                    }
                    else
                        actor.NextTerminate();
                });

            seq.AddItem(
          (o) =>
          {
              //ManualPressOnce = true;
              OptionPressServo.ServoOnAction.Execute(o);
              //ThirdPressServo.ServoOnAction.Execute(o);
              //BandTransferServo.ServoOnAction.Execute(o);
              //BandPickServo.ServoOnAction.Execute(o);
          });



            // seq.AddItem(FirstPressServo.MoveHomePos.Sequence);

            // edge-lee 240618
            seq.AddItem(OptionPressServo.MovePickPos.Sequence);




            seq.AddItem(OptionPressModule.PressCheckDelay);



            ///seq.AddItem((o) => OptionPressServo.MoveJogPositive.Execute(o));


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (OptionPressServo.ReadServoRatio >= Math.Truncate((OptionPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (OptionPressModule.TimeUpTimeOut.Time < time)
                {

                    OptionPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);

                    seq.AddItem(OptionPressServo.Stop.Sequence);
                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            seq.AddItem(OptionPressServo.Stop.Sequence);


            seq.AddItem(OptionPressModule.PressDelay);


            seq.AddItem(OptionPressServo.MoveHomePos.Sequence);


            seq.AddItem(OptionPressServo.Stop.Sequence);

            seq.AddTerminate();



        }



        private void MakeWorkThridManualPressServo()
        {
            var seq = WorkThridManualPressServo;

            #region Event
            seq.OnStart += delegate
            {
                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
            };
            #endregion

            PullQuestionMessageBoxWindow questionWindow = null;

            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new PullQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "서보를 동작 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (questionWindow != null)
                    {
                        if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.ContinueSequence)
                        {
                            actor.NextStep();
                        }
                        else if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.Cancel)
                        {
                            actor.NextTerminate();
                        }
                    }
                    else
                        actor.NextTerminate();
                });

            seq.AddItem(
          (o) =>
          {
              //ManualPressOnce = true;
              ThirdPressServo.ServoOnAction.Execute(o);
              //ThirdPressServo.ServoOnAction.Execute(o);
              //BandTransferServo.ServoOnAction.Execute(o);
              //BandPickServo.ServoOnAction.Execute(o);
          });



            // seq.AddItem(FirstPressServo.MoveHomePos.Sequence);

            // edge-lee 240618
            seq.AddItem(ThirdPressServo.MovePickPos.Sequence);




            seq.AddItem(ThirdPressModule.PressCheckDelay);



            //seq.AddItem((o) => ThirdPressServo.MoveJogPositive.Execute(o));


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (ThirdPressServo.ReadServoRatio >= Math.Truncate((ThirdPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (ThirdPressModule.TimeUpTimeOut.Time < time)
                {
                    ThirdPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);
                    seq.AddItem(ThirdPressServo.Stop.Sequence);
                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            seq.AddItem(ThirdPressServo.Stop.Sequence);


            seq.AddItem(ThirdPressModule.PressDelay);


            seq.AddItem(ThirdPressServo.MoveHomePos.Sequence);


            seq.AddItem(ThirdPressServo.Stop.Sequence);

            seq.AddTerminate();



        }

        private void MakeWorkSecondManualPressServo()
        {

            var seq = WorkSecondManualPressServo;

            #region Event
            seq.OnStart += delegate
            {
                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
            };
            #endregion

            PullQuestionMessageBoxWindow questionWindow = null;

            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new PullQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "서보를 동작 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (questionWindow != null)
                    {
                        if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.ContinueSequence)
                        {
                            actor.NextStep();
                        }
                        else if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.Cancel)
                        {
                            actor.NextTerminate();
                        }
                    }
                    else
                        actor.NextTerminate();
                });

            seq.AddItem(
          (o) =>
          {
              //ManualPressOnce = true;
              SecondPressServo.ServoOnAction.Execute(o);
              //ThirdPressServo.ServoOnAction.Execute(o);
              //BandTransferServo.ServoOnAction.Execute(o);
              //BandPickServo.ServoOnAction.Execute(o);
          });


            seq.AddItem(SecondPressServo.MovePickPos.Sequence);


            seq.AddItem(SecondPressModule.PressCheckDelay);

            //seq.AddItem((o) => SecondPressServo.MoveJogPositive.Execute(o));


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (SecondPressServo.ReadServoRatio >= Math.Truncate((SecondPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (SecondPressModule.TimeUpTimeOut.Time < time)
                {

                    SecondPressServo.Stop.Execute(this);

                    RaiseAlarm(actor, TourqeAlarmCheck);
                    seq.AddItem(SecondPressServo.Stop.Sequence);

                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            seq.AddItem(SecondPressServo.Stop.Sequence);



            seq.AddItem(SecondPressModule.PressDelay);


            seq.AddItem(SecondPressServo.MoveHomePos.Sequence);


            seq.AddItem(SecondPressServo.Stop.Sequence);

            seq.AddTerminate();


        }

        private void MakeWorkFirstManualPressServo()
        {

            var seq = WorkFirstManualPressServo;

            #region Event
            seq.OnStart += delegate
            {
                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
            };
            #endregion

            PullQuestionMessageBoxWindow questionWindow = null;

            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new PullQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "서보를 동작 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (questionWindow != null)
                    {
                        if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.ContinueSequence)
                        {
                            actor.NextStep();
                        }
                        else if (questionWindow.Result == PullQuestionMessageBoxWindow.QuestionResult.Cancel)
                        {
                            actor.NextTerminate();
                        }
                    }
                    else
                        actor.NextTerminate();
                });

            seq.AddItem(
          (o) =>
          {
              //ManualPressOnce = true;
              FirstPressServo.ServoOnAction.Execute(o);
              //FirstPressServo.SetTorqueLimitParams.Execute(o);
              //ThirdPressServo.ServoOnAction.Execute(o);
              //BandTransferServo.ServoOnAction.Execute(o);
              //BandPickServo.ServoOnAction.Execute(o);
          });


            seq.AddItem(FirstPressServo.MovePickPos.Sequence);





            seq.AddItem(FirstPressModule.PressCheckDelay);

           //seq.AddItem((o) => FirstPressServo.MoveJogPositive.Execute(o));



            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (FirstPressServo.ReadServoRatio >= Math.Truncate((FirstPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (FirstPressModule.TimeUpTimeOut.Time < time)
                {
                    FirstPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);

                   
                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });





            seq.AddItem(FirstPressServo.Stop.Sequence);


            // 추가
            seq.AddItem(FirstPressModule.PressDelay);



            seq.AddItem(FirstPressServo.MoveHomePos.Sequence);


            seq.AddItem(FirstPressServo.Stop.Sequence);

            seq.AddTerminate();



        }





        private void MakeInitialize()
        {
            var seq = Initialize;

            seq.OnStart += delegate
            {
                FeedingBeforeLock = false;
                BandPlaceInitLock = false;
                BandLoadingInitLock = false;
                RearStop = false;
                PlaceOn = false;
            };
            seq.AddItem((o) => { OptionServo.ServoOnAction.Execute(this); TapeCoverServo.ServoOnAction.Execute(this); }); 
            seq.AddItem(TapeHoldGrip.Grip.Sequence);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddItem(BandPickServo.MoveHome.Sequence);
            seq.AddItem(TapeLoadingServo.MoveHome.Sequence, BandTransferServo.MoveHome.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence); //빠른 이동

            //edge -lee 24 06 25
            seq.AddItem((o) => { FirstPressServo.ServoOnAction.Execute(this); SecondPressServo.ServoOnAction.Execute(this); OptionPressServo.ServoOnAction.Execute(this); ThirdPressServo.ServoOnAction.Execute(this); });
            seq.AddItem(FirstPressServo.MoveHome.Sequence, SecondPressServo.MoveHome.Sequence,OptionPressServo.MoveHome.Sequence, ThirdPressServo.MoveHome.Sequence);


            seq.AddItem(FirstPressModule.Initialize, SecondPressModule.Initialize, ThirdPressModule.Initialize);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            LoadingStep = false;
            ManualPressOnce = false;
            BandPlaceStandby = false;
            FrontModuleTerminated = false;
            FirstStart = true;
            FeedingBeforeLock = false;
            OneCycleTerminate = false;
            FeedingError = false;
            PlaceComplete = false;
        }

        private void MakeMainLoop()
        {
            var seq = MainLoop;

            seq.OnStart += delegate
            {
                FrontModuleTerminated = false;
            };
            seq.OnTerminate += delegate
            {
                MachineResume = false;
                FrontModuleTerminated = true;
                WriteTraceLog($"FrontModuleTerminated ={FrontModuleTerminated.ToString()},");
            };

            seq.AddItem(new FASequenceAtomicInfo(MainAutomicLoop, true));
        }


        public void MakeWorkOptionPressServo()
        {

            var seq = WorkOptionPressServo;

            #region Event
            seq.OnStart += delegate
            {
                OptionPressModule.PressTerminated = false;

                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
                OptionPressModule.PressOpenRetry.ClearCount();
                OptionPressModule.PressCloseRetry.ClearCount();
                OptionPressModule.PressPressRetry.ClearCount();

            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
                OptionPressModule.PressTerminated = true;
            };
            #endregion


            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                  (actor, time) =>
                  {
                      OptionPressModule.PressSafeArea = true;

                      if (OptionPressModule.UsePress)
                      {
                          OptionPressServo.ServoOnAction.Execute(actor);
                          WriteTraceLog($"module={this.Name}, UsePress={OptionPressModule.UsePress.ToString()}");
                          actor.NextStep();
                      }
                      else
                      {
                          //MotorRun.Off.Execute(actor);210823
                          WriteTraceLog($"module={this.Name}, UsePress={OptionPressModule.UsePress.ToString()}");
                          actor.NextTerminate();
                      }
                  });


            seq.AddItem(OptionPressServo.MoveHomePos.Sequence);
            seq.AddItem(OptionPressServo.MovePickPos.Sequence);


            // edge-lee 240618

            seq.AddItem(OptionPressModule.PressCheckDelay);




            //seq.AddItem((o) => OptionPressServo.MoveJogPositive.Execute(o));


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (OptionPressServo.ReadServoRatio >= Math.Truncate((OptionPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (OptionPressModule.TimeUpTimeOut.Time < time)
                {
                    OptionPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);


                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            seq.AddItem(OptionPressServo.Stop.Sequence);





            seq.AddItem(OptionPressModule.PressCheckDelay);


            seq.AddItem(OptionPressServo.MoveHomePos.Sequence);


            // press up
            seq.AddItem(
                   (actor, time) =>
                   {
                       OptionPressModule.PressSafeArea = true;
                       actor.NextStep();


                   });

            seq.AddItem(OptionPressModule.OpenCheckDelay);
            seq.AddItem((o) => { OptionPressModule.ExistMaterial = true; });

            seq.AddTerminate();


        }


        // edge-lee 240617
        public void MakeWorkThridPressServo()
        {

            var seq = WorkThridPressServo;

            #region Event
            seq.OnStart += delegate
            {
                ThirdPressModule.PressTerminated = false;

                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
                ThirdPressModule.PressOpenRetry.ClearCount();
                ThirdPressModule.PressCloseRetry.ClearCount();
                ThirdPressModule.PressPressRetry.ClearCount();

            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
                ThirdPressModule.PressTerminated = true;
            };
            #endregion


            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                  (actor, time) =>
                  {
                      ThirdPressModule.PressSafeArea = true;

                      if (ThirdPressModule.UsePress)
                      {
                          ThirdPressServo.ServoOnAction.Execute(actor);
                          WriteTraceLog($"module={this.Name}, UsePress={FirstPressModule.UsePress.ToString()}");
                          actor.NextStep();
                      }
                      else
                      {
                          //MotorRun.Off.Execute(actor);210823
                          WriteTraceLog($"module={this.Name}, UsePress={FirstPressModule.UsePress.ToString()}");
                          actor.NextTerminate();
                      }
                  });


            seq.AddItem(ThirdPressServo.MoveHomePos.Sequence);
            seq.AddItem(ThirdPressServo.MovePickPos.Sequence);


            // edge-lee 240618

            seq.AddItem(ThirdPressModule.PressCheckDelay);




           // seq.AddItem((o) => ThirdPressServo.MoveJogPositive.Execute(o));


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (ThirdPressServo.ReadServoRatio >= Math.Truncate((ThirdPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (ThirdPressModule.TimeUpTimeOut.Time < time)
                {
                    ThirdPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);


                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });



            seq.AddItem(ThirdPressServo.Stop.Sequence);





            seq.AddItem(ThirdPressModule.PressCheckDelay);


            seq.AddItem(ThirdPressServo.MoveHomePos.Sequence);


            // press up
            seq.AddItem(
                   (actor, time) =>
                   {
                       ThirdPressModule.PressSafeArea = true;
                       actor.NextStep();


                   });

            seq.AddItem(ThirdPressModule.OpenCheckDelay);
            seq.AddItem((o) => { ThirdPressModule.ExistMaterial = true; });

            seq.AddTerminate();


        }

        //// edge-lee 240612
        public void MakeWorkSecondPressServo()
        {

            var seq = WorkSecondPressServo;

            #region Event
            seq.OnStart += delegate
            {
                SecondPressModule.PressTerminated = false;

                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
                //SecondPressModule.PressOpenRetry.ClearCount();
               // SecondPressModule.PressCloseRetry.ClearCount();
               // SecondPressModule.PressPressRetry.ClearCount();

            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
                SecondPressModule.PressTerminated = true;
            };
            #endregion


            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                  (actor, time) =>
                  {
                      SecondPressModule.PressSafeArea = true;

                      if (SecondPressModule.UsePress)
                      {
                          SecondPressServo.ServoOnAction.Execute(actor);
                          WriteTraceLog($"module={this.Name}, UsePress={FirstPressModule.UsePress.ToString()}");
                          actor.NextStep();
                      }
                      else
                      {
                          //MotorRun.Off.Execute(actor);210823
                          WriteTraceLog($"module={this.Name}, UsePress={FirstPressModule.UsePress.ToString()}");
                          actor.NextTerminate();
                      }
                  });


            seq.AddItem(SecondPressServo.MoveHomePos.Sequence);
            seq.AddItem(SecondPressServo.MovePickPos.Sequence);


            // edge-lee 240618

            seq.AddItem(SecondPressModule.PressCheckDelay);



            //seq.AddItem((o) => SecondPressServo.MoveJogPositive.Execute(o));

            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (SecondPressServo.ReadServoRatio >= Math.Truncate((SecondPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (SecondPressModule.TimeUpTimeOut.Time < time)
                {
                    SecondPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);


                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            seq.AddItem(SecondPressServo.Stop.Sequence);

            seq.AddItem(SecondPressModule.PressCheckDelay);

            seq.AddItem(SecondPressServo.MoveHomePos.Sequence);


            // press up
            seq.AddItem(
                   (actor, time) =>
                   {
                       SecondPressModule.PressSafeArea = true;
                       actor.NextStep();


                   });

            seq.AddItem(SecondPressModule.OpenCheckDelay);
            seq.AddItem((o) => { SecondPressModule.ExistMaterial = true; });

            seq.AddTerminate();


        }



        //// edge-lee 240612
        public void MakeWorkFirstPressServo()
        {

            var seq = WorkFirstPressServo;

            #region Event
            seq.OnStart += delegate
            {
                FirstPressModule.PressTerminated = false;

                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
                FirstPressModule.PressOpenRetry.ClearCount();
                FirstPressModule.PressCloseRetry.ClearCount();
                FirstPressModule.PressPressRetry.ClearCount();

            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
                FirstPressModule.PressTerminated = true;
            };
            #endregion


            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                  (actor, time) =>
                  {
                      FirstPressModule.PressSafeArea = true;

                      if (FirstPressModule.UsePress)
                      {
                          FirstPressServo.ServoOnAction.Execute(actor);
                          WriteTraceLog($"module={this.Name}, UsePress={FirstPressModule.UsePress.ToString()}");
                          actor.NextStep();
                      }
                      else
                      {
                          //MotorRun.Off.Execute(actor);210823
                          WriteTraceLog($"module={this.Name}, UsePress={FirstPressModule.UsePress.ToString()}");
                          actor.NextTerminate();
                      }
                  });


            seq.AddItem(FirstPressServo.MoveHomePos.Sequence);
            seq.AddItem(FirstPressServo.MovePickPos.Sequence);

            seq.AddItem(FirstPressModule.PressCheckDelay);


            // edge-lee 240618
            //seq.AddItem((o) => FirstPressServo.MoveJogPositive.Execute(o));


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (FirstPressServo.ReadServoRatio >= Math.Truncate((FirstPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (FirstPressModule.TimeUpTimeOut.Time < time)
                {
                    FirstPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);


                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            seq.AddItem(FirstPressServo.Stop.Sequence);



            seq.AddItem(FirstPressModule.PressDelay);


            seq.AddItem(FirstPressServo.MoveHomePos.Sequence);


            // press up
            seq.AddItem(
                   (actor, time) =>
                   {
                       FirstPressModule.PressSafeArea = true;
                       actor.NextStep();


                   });

            //seq.AddItem(FirstPressModule.OpenCheckDelay);
            seq.AddItem((o) => { FirstPressModule.ExistMaterial = true; });





            seq.AddTerminate();


        }



        private void MakeMainAutomicLoop()
        {
            var seq = MainAutomicLoop;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("MainAutomicLoop_Start");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("MainAutomicLoop_Terminated");
            };
            #endregion

            seq.AddItem((o) => { ModuleOperation.OnceCycleStartSignal = false; });
            seq.AddItem(
                (actor, time) =>
                {
                    if (FirstStart)
                    {
                        WriteTraceLog("First_Start");
                        FeedingBeforeLock = false;
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog("Loop_Start");
                        actor.NextStep("LoopBandMoveLoading");
                    }
                });
            seq.AddItem(WorkFirstBandMoveLoading);
            seq.AddItem((o) => { FirstStart = false; });

            seq.AddItem("ConfirmRearSeqTerminated");
            seq.AddStep("LoopBandMoveLoading").StepIndex = seq.AddItem(WorkLoopBandMoveLoading);
            seq.AddStep("ConfirmRearSeqTerminated").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (RModule.RearSeqTerminated)
                    {
                        if (ModuleOperation.OnceCycleStartSignal)
                        {
                            WriteTraceLog($"RearSeqTerminated ={RModule.RearSeqTerminated.ToString()}," +
                                     $"OnceCycleStartSignal={ModuleOperation.OnceCycleStartSignal.ToString()}");

                            actor.NextStep("OnceCycle");
                        }
                        else
                        {
                            WriteTraceLog($"RearSeqTerminated ={RModule.RearSeqTerminated.ToString()}," +
                                     $"OnceCycleStartSignal={ModuleOperation.OnceCycleStartSignal.ToString()}");

                            actor.NextStep();
                        }
                    }
                    else if (BandPlaceInitLock)
                    {
                        WriteTraceLog("MainAutomicLoop_BandPlaceInitLock");
                        BandPlaceInitLock = false;
                        RModule.ReSet = true;
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

            seq.AddStep("OnceCycle").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (ThirdPressModule.UsePress && RModule.FourthPressModule.UsePress || BypassCoveyorExistCheck.IsOff)
                    //|| OptionPressModule.UsePress)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextTerminate();
                    }
                });
            //210716
            seq.AddItem(new FASequenceAtomicInfo(OnceCycleEnd, true));
            seq.AddTerminate();


        }
        private void MakeFrontOnceCycleEnd()
        {
            var seq = FrontOnceCycleEnd;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("FrontOnceCycleEnd_Start");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("FrontOnceCycleEnd_Terminated");
            };
            #endregion

            //210726
            seq.AddItem(
                (actor, time) =>
                {
                    //if (OptionPressModule.UsePress)
                    if(BypassCoveyorExistCheck.IsOn)
                    {
                        actor.NextStep("Option");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(BandTransferServo.MoveHomePos.Sequence, BandPitchChangeCylinder.Home.Sequence);
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddTerminate();

            seq.AddStep("Option").StepIndex = seq.AddItem(BandTransferServo.MoveHomePos.Sequence, BypassCoveyorMotor.Stop.Sequence);
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BypassCoveyorMotor.Run.Sequence);
            seq.AddTerminate();
        }
        //OnceCycleStartEnd
        private void MakeOnceCycleEnd()
        {
            var seq = OnceCycleEnd;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("OnceCycleEnd_Start");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("OnceCycleEnd_Terminated");
            };
            #endregion
            
            seq.AddItem(
                (actor, time) =>
                {
                    if (RModule.RearSeqTerminated && !RModule.OnceCycleStartEnd )
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem((o) => { ShowMessage("AlarmRunningMessage", AlarmRunningMessage, "AlarmRunningMessage"); });
            seq.AddItem(
                (actor,time) => 
                {
                    //if (OptionPressModule.UsePress || !RModule.FourthPressModule.UsePress)
                    if(BypassCoveyorExistCheck.IsOn || !RModule.FourthPressModule.UsePress)
                    {
                        actor.NextStep("PlaceDown");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(FrontOnceCycleEnd, RModule.OnceCycleStart);
            seq.AddItem((o) => { WriteTraceLog("OnceCycleEndEnd"); });
            seq.AddItem((o) => { CloseMessage("AlarmRunningMessage"); });
            seq.AddItem((o) => { ModuleOperation.OnceCycleStartSignal = false; RModule.OnceCycleStartEnd = false; });
            seq.AddTerminate();

            seq.AddStep("PlaceDown").StepIndex = seq.AddItem(FrontOnceCycleEnd);
            seq.AddItem((o) => { WriteTraceLog("OnceCycleEndEnd"); });
            seq.AddItem((o) => { CloseMessage("AlarmRunningMessage"); });
            seq.AddItem((o) => { ModuleOperation.OnceCycleStartSignal = false; RModule.OnceCycleStartEnd = false; });
            seq.AddTerminate();
        }
        private void MakeFrontACMotorLoop()
        {
            var seq = FrontACMotorLoop;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("FrontACMotorLoop_Start");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("FrontACMotorLoop_Terminated");
            };
            #endregion

            seq.AddItem(WorkACMotorLoading, WorkInverterMotorLoading);
        }
        private void MakeWorkInverterMotorLoading()
        {
            var seq = WorkInverterMotorLoading;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkInverterMotorLoading_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("WorkInverterMotorLoading_Terminated");
            };
            seq.OnSuspended += delegate
            {
                InverterMotor.Stop();
            };
            seq.OnSuspending += delegate
            {
                InverterMotor.Stop();
            };
            seq.OnStop += delegate
            {
               InverterMotor.Stop();
            };
            #endregion

            seq.AddStep("MotorLoop").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (ShapeTapeTensionSlowSensor.IsOn)
                    {
                        if (ShapeTapeTensionUpSensor.IsOn)
                        {
                            InverterMotor.Write_SetSpeed = Inverter_WriteSpeed_Fast;
                            InverterMotor.Run();
                            actor.NextStep();
                        }
                        else
                        {
                            if (ShapeTapeTensionDownSensor.IsOn)
                            {
                                InverterMotor.Stop();
                                actor.NextStep();
                            }
                            else
                            {
                                actor.NextStep();
                            }
                        }
                    }
                    else
                    {
                        RaiseAlarm(actor, AlarmShapeMoldingMaterialEmpty); //Alarm 추가할것
                    }
                });
            seq.AddItem("MotorLoop");
        }
     
        private void MakeWorkACMotorLoading()
        {
            var seq = WorkACMotorLoading;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkACMotorLoading_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("WorkACMotorLoading_Terminated");
            };
            #endregion

            seq.AddStep("MotorLoop").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (UseBottomPeeling)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {

                    if (PackingTapeTensionUpSensor.IsOn)
                    {
                        PackingTapeLoadingMotor.Stop.Execute(actor);
                        actor.NextStep();
                    }
                    else
                    {
                        if (PackingTapeTensionDownSensor.IsOn)
                        {
                            PackingTapeLoadingMotor.Run.Execute(actor);
                            actor.NextStep();
                        }
                    }
                });
            seq.AddItem("MotorLoop");
            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();
        }

        private void MakeWorkFirstBandMoveLoading()
        {
            var seq = WorkFirstBandMoveLoading;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("WorkFirstBandMoveLoading_Start");
            };
            seq.OnTerminate += delegate
            {
                RModule.ReSet = true;
                WriteTraceLog("WorkFirstBandMoveLoading_Terminated");
            };
            #endregion
            
            seq.AddItem(WorkTapeMovePickCylinder);
             //Transfer Home && LoadingServo Pull
            seq.AddStep("InitEnd").StepIndex = seq.AddItem((o) => { TapeLoadingServoUsedLength += TapeLoadingServo.TapeLoadingPos.Position / 1000; });
            seq.AddItem(
                (actor, time) =>
                {
                    if (TapeLoadingServoSettingLength <= TapeLoadingServoUsedLength)
                    {
                        if (TapeLoadingServoSettingLength == 0)
                        {
                            WriteTraceLog($"Setting o : TapeLoadingServoSettingLength = {TapeLoadingServoSettingLength}");
                            actor.NextStep();
                        }

                        WriteTraceLog($"TapeEmpty => SettingPosition={TapeLoadingServoSettingLength.ToString()}," +
                                      $"TapeLoadingSVUsedPosition => UsedPosition={TapeLoadingServoUsedLength.ToString()}");
                        TapeLoadingServoUsedLength = 0;
                        //RaiseAlarm(actor, AlarmShapeMoldingMaterialEmpty);
                        ShowMessage("AlarmShapeMoldingMaterialEmpty", AlarmShapeMoldingMaterialEmpty, "AlarmShapeMoldingMaterialEmpty");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"TapeUsed => SettingPosition={TapeLoadingServoSettingLength.ToString()}," +
                                      $"TapeLoadingSVUsedPosition => UsedPosition={TapeLoadingServoUsedLength.ToString()}");
                        actor.NextStep();
                    }
                });
            seq.AddItem(
               (actor, time) =>
               {
                   if (ThirdPressModule.UsePress)
                   {
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnUseTomson1");
                   }
               });
            seq.AddItem(WorkFirstPress); //Press && Trasfer Material Place
            seq.AddItem((o) => { WriteTraceLog("WorkFirstBandMoveLoading_WorkPressTerminated"); });
            seq.AddStep("BandPickMoveStart1").StepIndex = seq.AddItem(BandPickServo.MoveHomePos.Sequence, BandTransferServo.MoveStandbyPos.Sequence); // 20200923
            seq.AddStep("BandMoveLoadingStart1").StepIndex = seq.AddItem(TapeLoadGrip.Grip.Sequence, WorkBandPickMove); //TransferServo Pick Material
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(WorkTapeMovePickCylinder); //Transfer Home && LoadingServo Pull
            seq.AddStep("InitEnd1").StepIndex = seq.AddItem((o) => { TapeLoadingServoUsedLength += TapeLoadingServo.TapeLoadingPos.Position / 1000; });
            seq.AddItem(
                (actor, time) =>
                {
                    if (TapeLoadingServoSettingLength <= TapeLoadingServoUsedLength)
                    {
                        if (TapeLoadingServoSettingLength == 0)
                        {
                            WriteTraceLog($"Setting o : TapeLoadingServoSettingLength = {TapeLoadingServoSettingLength}");
                            actor.NextStep();
                        }

                        WriteTraceLog($"TapeEmpty => SettingPosition={TapeLoadingServoSettingLength.ToString()}," +
                                      $"TapeLoadingSVUsedPosition => UsedPosition={TapeLoadingServoUsedLength.ToString()}");
                        TapeLoadingServoUsedLength = 0;
                        // RaiseAlarm(actor, AlarmShapeMoldingMaterialEmpty);
                        ShowMessage("AlarmShapeMoldingMaterialEmpty", AlarmShapeMoldingMaterialEmpty, "AlarmShapeMoldingMaterialEmpty");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"TapeUsed => SettingPosition={TapeLoadingServoSettingLength.ToString()}," +
                                      $"TapeLoadingSVUsedPosition => UsedPosition={TapeLoadingServoUsedLength.ToString()}");
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

            seq.AddStep("Init1").StepIndex = 
            seq.AddItem(
                (actor, time) =>
                {
                    //if (OptionPressModule.UsePress)
                    if(BypassCoveyorExistCheck.IsOn)
                    {
                        actor.NextStep("Option");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence, BandTransferServo.MoveStandbyPos.Sequence, BandPitchChangeCylinder.Home.Sequence);
            seq.AddStep("OptionEnd").StepIndex = seq.AddItem(TapeHoldGrip.Grip.Sequence);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddItem("InitEnd1");
            //210726
            seq.AddStep("Option").StepIndex = seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence, BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem("OptionEnd");

            seq.AddStep("UnUseTomson1").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    //if (ThirdPressModule.ExistMaterial)
                    //{
                    //    ThirdPressModule.ExistMaterial = false;
                    //    PlaceOn = true;
                    //    actor.NextStep("Init1");
                    //}
                    //else
                    //{
                        actor.NextStep();
                    //}
                });
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(TapeHoldGrip.Grip.Sequence);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddItem("InitEnd1");

        }

        private void MakeWorkLoopBandMoveLoading()
        {
            var seq = WorkLoopBandMoveLoading;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkLoopBandMoveLoading_Start");
            };
            seq.OnTerminate += delegate
            {
                //RModule.ReSet = true;
                //WriteTraceLog("WorkLoopBandMoveLoading_Terminated");
            };
            #endregion

            seq.AddItem(
             (actor, time) =>
             {
                 if (FeedingBeforeLock)
                 {
                     FeedingBeforeLock = false;
                     actor.NextStep();
                 }
                 else
                 {
                     //if (ThirdPressModule.UsePress)
                     //{
                     //    actor.NextStep("PressProcess");
                     //}
                     //else
                     //{
                     //    actor.NextStep("UnUseTomson");
                     //}
                     actor.NextStep();
                 }
             });
            seq.AddItem(TapeMovePickCylinder); 
            seq.AddStep("PressProcess").StepIndex = seq.AddItem(WorkPress); //Press && Trasfer Material Place
            seq.AddItem(
               (actor, time) =>
               {
                   if (ThirdPressModule.UsePress)
                   {
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnUseTomson");
                   }
               });
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(WorkTapeMovePickCylinder); //Transfer Home && LoadingServo Pull
            seq.AddStep("InitEnd").StepIndex = seq.AddItem((o) => { TapeLoadingServoUsedLength += TapeLoadingServo.TapeLoadingPos.Position / 1000; });
            seq.AddItem(
                (actor, time) =>
                {
                    if (TapeLoadingServoSettingLength <= TapeLoadingServoUsedLength)
                    {
                        if (TapeLoadingServoSettingLength == 0)
                        {
                            WriteTraceLog($"Setting o : TapeLoadingServoSettingLength = {TapeLoadingServoSettingLength}");
                            actor.NextStep();
                        }

                        WriteTraceLog($"TapeEmpty => SettingPosition={TapeLoadingServoSettingLength.ToString()}," +
                                      $"TapeLoadingSVUsedPosition => UsedPosition={TapeLoadingServoUsedLength.ToString()}");
                        TapeLoadingServoUsedLength = 0;
                        // RaiseAlarm(actor, AlarmShapeMoldingMaterialEmpty);
                        ShowMessage("AlarmShapeMoldingMaterialEmpty", AlarmShapeMoldingMaterialEmpty, "AlarmShapeMoldingMaterialEmpty");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"TapeUsed => SettingPosition={TapeLoadingServoSettingLength.ToString()}," +
                                      $"TapeLoadingSVUsedPosition => UsedPosition={TapeLoadingServoUsedLength.ToString()}");
                        actor.NextStep();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (FeedingAfterLock)
                    {
                        FeedingAfterLock = false;
                        OneCycleTerminate = true;
                        FeedingBeforeLock = true;
                        actor.NextStep("PressProcess");
                    }
                    else
                    {
                        OneCycleTerminate = false;
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

            seq.AddStep("Init").StepIndex = seq.AddItem(
                (actor, time) => 
                {
                    //if (OptionPressModule.UsePress)
                    if(BypassCoveyorExistCheck.IsOn)
                    {
                        actor.NextStep("UseOptionPress");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence, BandTransferServo.MoveStandbyPos.Sequence, BandPitchChangeCylinder.Home.Sequence);
            seq.AddStep("EndOptionPress").StepIndex = seq.AddItem(TapeHoldGrip.Grip.Sequence);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddItem("InitEnd");

            seq.AddStep("UseOptionPress").StepIndex = seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence, BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem("EndOptionPress");

            seq.AddStep("UnUseTomson").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (ThirdPressModule.ExistMaterial)
                    {
                        
                        PlaceOn = true;
                        actor.NextStep("Init");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(TapeHoldGrip.Grip.Sequence);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddItem("InitEnd");

        }
        //Auto Sequence
        private void MakeWorkBandPickMove()
        {
            var seq = WorkBandPickMove;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkBandPickMove_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("WorkBandPickMove_Terminated");
            };
            #endregion

            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(
                (actor, time) =>
                {
                    if (ThirdPressModule.ExistMaterial)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    //if (OptionPressModule.UsePress)
                    if(BypassCoveyorExistCheck.IsOn)
                    {
                        if (ThirdPressModule.UsePress)
                        {
                            WriteTraceLog("3&O_Press_Use");
                            actor.NextStep("Option");

                        }
                        else
                        {
                            //TapeHoldGrip.Grip.Execute(this);
                            WriteTraceLog("O_Press_Use");
                            actor.NextStep("Option");
                        }
                    }
                    else
                    {
                        if (ThirdPressModule.UsePress)
                        {
                            WriteTraceLog("WorkBandPickMove_Use");
                            actor.NextStep();

                        }
                        else
                        {
                            //TapeHoldGrip.Grip.Execute(this);
                            WriteTraceLog("WorkBandPickMove_UnUse");
                            actor.NextStep();
                        }
                    }
                });
            // press 안으로 진입
            seq.AddItem(BandTransferServo.MoveTapeLoadingPos.Sequence, BandPitchChangeCylinder.Push.Sequence);
            seq.AddItem(BandPickServo.MovePickPos.Sequence);
            seq.AddItem((o) => { BandVaccum.On.Execute(o); BandVaccumEject.Off.Execute(o); });
            seq.AddItem(VacuumDelay);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();

            seq.AddStep("Option").StepIndex = seq.AddItem(BandTransferServo.MoveTapeLoadingPos.Sequence, BypassCoveyorMotor.Stop.Sequence);
            seq.AddItem(BandPickServo.MovePickPos.Sequence);
            seq.AddItem((o) => { BandVaccum.On.Execute(o); BandVaccumEject.Off.Execute(o); });
            seq.AddItem(VacuumDelay);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BypassCoveyorMotor.Run.Sequence);
            seq.AddTerminate();
        }

        private void MakeWorkTapeMovePickCylinder()
        {
            var seq = WorkTapeMovePickCylinder;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkTapeMovePickCylinder_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("WorkTapeMovePickCylinder_Terminated");
            };
            #endregion

            seq.AddItem(TapeMovePickCylinder, WorkTapeMovePick);
            seq.AddTerminate();

        }

        private void MakeWorkTapeMovePick()
        {
            var seq = WorkTapeMovePick;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkTapeMovePick_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("WorkTapeMovePick_Terminated");
            };
            #endregion

            seq.AddItem(
                (actor, time) =>
                {
                   // if (OptionPressModule.UsePress)
                   if(BypassCoveyorExistCheck.IsOn)
                    {
                        actor.NextStep("Option");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(BandTransferServo.MoveHomePos.Sequence, BandPitchChangeCylinder.Home.Sequence);
            seq.AddTerminate();

            seq.AddStep("Option").StepIndex = seq.AddItem(BandTransferServo.MoveHomePos.Sequence);
            seq.AddTerminate();
        }

        private void MakeTapeMovePickCylinder()
        {
            var seq = TapeMovePickCylinder;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("TapeMovePickCylinder_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("TapeMovePickCylinder_Terminated");
            };
            #endregion

            seq.AddItem(
                (actor, time) =>
                {
                    if (FeedingBeforeLock)
                    {
                        WriteTraceLog("MoveTapeLoadingPos Skip");
                        actor.NextStep("Terminate");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(TapeHoldGrip.Grip.Sequence);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();
        }

        private void MakeWorkBandPlaceMoveCylinder()
        {
            var seq = WorkBandPlaceMoveCylinder;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("WorkBandPlaceMoveCylinder_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("WorkBandPlaceMoveCylinder_Terminated");
            };
            #endregion

            seq.AddItem(WorkBandPlaceMove, TapeMovePlaceCylinder);
            seq.AddTerminate();
        }

        private void MakeTapeMovePlaceCylinder()
        {
            var seq = TapeMovePlaceCylinder;

            #region Event
            seq.OnStart += delegate
            {
                //WriteTraceLog("TapeMovePlaceCylinder_Start");
            };
            seq.OnTerminate += delegate
            {
                //WriteTraceLog("TapeMovePlaceCylinder_Terminated");
            };
            #endregion

            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddTerminate();
        }

        private void MakeWorkBandPlaceMove()
        {
            var seq = WorkBandPlaceMove;

            #region Event
            seq.OnStart += delegate
            {
                PlaceComplete = false;
                //WriteTraceLog("WorkBandPlaceMove_Start");
            };
            seq.OnTerminate += delegate
            {
                PlaceComplete = true;
                //WriteTraceLog("WorkBandPlaceMove_Terminated");
            };
            #endregion
            
            seq.AddItem(
                (actor, time) =>
                {
                    if (FirstStart)
                    {
                        WriteTraceLog("WorkBandPlaceMove_FirstStart");
                        actor.NextTerminate();
                    }
                    else
                    {
                        WriteTraceLog(string.Format("000 PlaceSkip {0}, FirstStart {1}", RModule.PlaceSkip, FirstStart));
                        WriteTraceLog("WorkBandPlaceMove_NoFirstStart");
                        actor.NextStep();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (ThirdPressModule.UsePress)
                    {
                        WriteTraceLog("ThirdPressModule.UsePress = true");

                        if (RModule.MachineResume)
                        {
                            RModule.PlaceSkip = false;
                            RModule.MachineResume = false;
                            actor.NextStep("BandPlaceMoveStart");
                        }

                        actor.NextStep();
                    }
                    else
                    {
                       
                        if (ThirdPressModule.ExistMaterial)
                        {
                            WriteTraceLog(string.Format("ExistMaterial {0}", ThirdPressModule.ExistMaterial));
                            ThirdPressModule.ExistMaterial = false;
                            actor.NextStep();
                        }
                        else
                        {
                            WriteTraceLog(string.Format("ExistMaterial {0}", ThirdPressModule.ExistMaterial));
                            actor.NextStep("UnUseTomson");
                        }
                        
                    }
                });
            seq.AddStep("BandPlaceMoveStart").StepIndex = seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(
               (actor, time) =>
               {
                   if (BandPlaceInitLock)
                   {
                       BandPlaceInitLock = false;
                       WriteTraceLog("BandPlaceInitLock = true");
                       actor.NextStep("UnUseTomson"); //init
                   }
                   else
                   {
                       BandPlaceStandby = true;
                       WriteTraceLog("BandPlaceInitLock = false");
                       actor.NextStep();
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (RModule.DownProcess())
                    {
                        WriteTraceLog("DownProcess()");
                        actor.NextStep();
                    }
                    else if (FeedingBeforeLock == true && RModule.WorkLoading.State == SequenceState.Terminated)
                    {
                        FeedingError = true;
                        WriteTraceLog("Error");
                        //actor.NextTerminate();
                        actor.NextStep();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (RModule.PlaceSkip)
                    {
                        if (RModule.MachineResume)
                        {
                            RModule.PlaceSkip = false;
                            RModule.MachineResume = false;
                            actor.NextStep();
                        }
                        else
                        {
                            RModule.PlaceSkip = false;
                            WriteTraceLog(string.Format("111 PlaceSkip {0},MachineResume {1}, FirstStart {2}", RModule.PlaceSkip, MachineResume, FirstStart));
                            actor.NextStep("UnUseTomson");
                        }
                    }
                    else
                    {
                        WriteTraceLog(string.Format("222 PlaceSkip {0},MachineResume {1}, FirstStart {2}", RModule.PlaceSkip, MachineResume, FirstStart));
                        //if (OptionPressModule.UsePress)
                        if(BypassCoveyorExistCheck.IsOn)
                        { 
                        BypassCoveyorMotor.Stop.Execute(actor);
                        }
                        actor.NextStep();
                    }
                });
            seq.AddStep("Init").StepIndex = seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((O) => { BandVaccumEject.Off.Execute(O); });
            seq.AddStep("UnUseTomson").StepIndex =
            seq.AddItem(
                (actor, time) =>
                {
                    BandPlaceStandby = false;

                    if (PlaceOn)
                    {
                        PlaceOn = false;
                        WriteTraceLog("PlaceOn");
                        actor.NextStep("Place");
                    }
                    else
                    {
                        WriteTraceLog("NOPlaceOn");
                        actor.NextStep();
                    }
                });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(
                (actor, time) =>
                {
                    if (FeedingError)
                    {
                        FeedingError = false;
                        WriteTraceLog("FeedingError");
                        actor.NextStep("FeedingError");
                    }
                    else
                    {
                        WriteTraceLog("NOFeedingError");
                        //if (OptionPressModule.UsePress)
                        if(BypassCoveyorExistCheck.IsOn)
                        {
                            BypassCoveyorMotor.Run.Execute(actor);
                        }
                        actor.NextStep();
                    }
                });
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence); // 빠른이동\
            seq.AddItem((o) => { WriteTraceLog("UnUseTomson_Terminated"); });
            seq.AddTerminate();

            seq.AddStep("FeedingError").StepIndex =
            seq.AddItem((o) => { RModule.OnceCycleStart.Start(); });
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem((o) => { WriteTraceLog("FeedingError_Terminated"); });
            seq.AddTerminate();

            seq.AddStep("Place").StepIndex = seq.AddItem(
             (actor, time) =>
             {
                 if (RModule.RearSeqTerminated)
                 {
                     WriteTraceLog("Place");
                     actor.NextStep();
                 }
             });
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence); // 빠른이동
            seq.AddItem((o) => { ThirdPressModule.ExistMaterial = false; });
            seq.AddItem((o) => { WriteTraceLog("Place_Terminated"); });
            seq.AddTerminate();
        }

        private void MakeWorkPress()
        {
            var seq = WorkPress;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("WorkPress_Start");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("WorkPress_Terminated");
            };
            #endregion

            //seq.AddStep("FrontPressStart").StepIndex = seq.AddItem(FirstPressModule.WorkAnotherPress,
            //                                                       SecondPressModule.WorkAnotherPress,
            //                                                       WorkTomsonPress,
            //                                                       OptionPressModule.WorkAnotherPress,
            //                                                       WorkBandPlaceMoveCylinder);

            //seq.AddStep("FrontPressStart").StepIndex = seq.AddItem(WorkFirstPressServo,
            //                                           WorkSecondPressServo,
            //                                           WorkTomsonPress,
            //                                           WorkOptionPressServo,
            //                                           WorkBandPlaceMoveCylinder);
            seq.AddStep("FrontPressStart").StepIndex = seq.AddItem(WorkFirstPressServo, WorkSecondPressServo, WorkTomsonPress, WorkOptionPressServo
                                                                                         ,WorkBandPlaceMoveCylinder);

            seq.AddTerminate();
        }

        private void MakeWorkFirstPress()
        {
            var seq = WorkFirstPress;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("WorkFirstPress_Start");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("WorkFirstPress_Terminated");
            };
            #endregion

           //seq.AddItem(FirstPressModule.WorkAnotherPress,SecondPressModule.WorkAnotherPress,
           //            OptionPressModule.WorkAnotherPress,ThirdPressModule.WorkAnotherPress,
           //            TapeMovePlaceCylinder);

            seq.AddItem(WorkFirstPressServo, WorkSecondPressServo, WorkOptionPressServo,
                                                    WorkThridPressServo,TapeMovePlaceCylinder);
        }

        private void MakeWorkTomsonPress()
        {
            var seq = WorkTomsonPress;

            #region Event
            seq.OnStart += delegate
            {
                WriteTraceLog("WorkTomsonPress");
            };
            seq.OnTerminate += delegate
            {
                WriteTraceLog("WorkTomsonPress_terminated");
            };
            #endregion

            //seq.AddItem(ThirdPressModule.WorkAnotherPress);
            seq.AddItem(WorkThridPressServo);

            seq.AddItem(
               (actor, time) =>
               {
                   if (PlaceComplete)
                   {
                       PlaceComplete = false;
                       actor.NextStep();
                   }
               });
            seq.AddItem(WorkBandPickMove);
        }

        private void MakeWorkManualPicking()
        {
            var seq = WorkManualPicking;

            seq.AddItem(
                (actor, time) =>
                {
                    if (ThirdPressModule.ManualUsePress)
                    {
                        if (BypassCoveyorExistCheck.IsOn)
                        {
                            actor.NextStep("Option");
                        }
                        else
                        {
                            actor.NextStep();
                        }
                        //if (OptionPressModule.UsePress)
                        //{
                        //    actor.NextStep("Option");
                        //}
                        //else
                        //{
                        //    actor.NextStep();
                        //}
                    }
                    else
                    {
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem(BandTransferServo.MoveTapeLoadingPos.Sequence, BandPitchChangeCylinder.Push.Sequence);
            seq.AddItem(BandPickServo.MovePickPos.Sequence);
            seq.AddItem((o) => { BandVaccum.On.Execute(o); BandVaccumEject.Off.Execute(o); }); //210719
            seq.AddItem(VacuumDelay);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveHomePos.Sequence, BandPitchChangeCylinder.Home.Sequence);
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddTerminate();

            seq.AddStep("Option").StepIndex = seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem(BandTransferServo.MoveTapeLoadingPos.Sequence, BypassCoveyorMotor.Stop.Sequence);
            seq.AddItem(BandPickServo.MovePickPos.Sequence);
            seq.AddItem((o) => { BandVaccum.On.Execute(o); BandVaccumEject.Off.Execute(o); }); //210719
            seq.AddItem(VacuumDelay);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BypassCoveyorMotor.Run.Sequence);
            seq.AddItem(BandTransferServo.MoveHomePos.Sequence, BypassCoveyorMotor.Stop.Sequence);
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BypassCoveyorMotor.Run.Sequence);
            seq.AddTerminate();
        }

        private void MakeWorkManualLoading()
        {
            var seq = WorkManualLoading;

            #region Event
            seq.OnStart += delegate
            {
                //WorkInverterMotorLoading.Start();
            };
            #endregion

            LoadingQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new LoadingQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", " 원단 1피치 운전을 기동 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                  (actor, time) =>
                  {
                      if (questionWindow != null)
                      {
                          if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.StepSequence)
                          {
                              LoadingStep = true;
                              actor.NextStep();
                          }
                          else if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.Cancel)
                          {
                              actor.NextStep("Terminate");
                          }
                      }
                      else
                          actor.NextStep("Terminate");
                  });
            seq.AddItem(
                (actor, time) =>
                {
                    if (!ManualPressOnce)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(
                (o) =>
                {
                    ManualPressOnce = true;
                    TapeLoadingServo.ServoOnAction.Execute(o);
                    BandTransferServo.ServoOnAction.Execute(o);
                    BandPickServo.ServoOnAction.Execute(o);
                });
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            //seq.AddItem(FirstPressModule.WorkManualUpPress, SecondPressModule.WorkManualUpPress, OptionPressModule.WorkManualUpPress, ThirdPressModule.WorkManualUpPress);
            seq.AddItem(WorkFirstPressServo, WorkSecondPressServo,WorkOptionPressServo, WorkThridPressServo);
            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeLoadingGripSensorDelay);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(TapeHoldGrip.Grip.Sequence, WorkManualPress);
            seq.AddItem(TapeLoadGrip.Release.Sequence, WorkManualPicking);
            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddStep("Terminate").StepIndex = seq.AddItem((o) => { ManualPressOnce = false; });
            seq.AddTerminate();
        }

        private void MakeWorkMoveWithOutTomson()
        {
            var seq = WorkMoveWithOutTomson;

            #region Event
            seq.OnStart += delegate
            {
                //WorkInverterMotorLoading.Start();
            };
            #endregion

            LoadingQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new LoadingQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", " 톰슨 없이 이동시키시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                  (actor, time) =>
                  {
                      if (questionWindow != null)
                      {
                          if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.StepSequence)
                          {
                              LoadingStep = true;
                              actor.NextStep();
                          }
                          else if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.Cancel)
                          {
                              actor.NextStep("Terminate");
                          }
                      }
                      else
                          actor.NextStep("Terminate");
                  });
            seq.AddItem(
                (actor, time) =>
                {
                    if (!ManualPressOnce)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(
                (o) =>
                {
                    ManualPressOnce = true;
                    TapeLoadingServo.ServoOnAction.Execute(o);
                    BandTransferServo.ServoOnAction.Execute(o);
                    BandPickServo.ServoOnAction.Execute(o);
                });
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            //seq.AddItem(FirstPressModule.WorkUpPress, SecondPressModule.WorkUpPress, OptionPressModule.WorkUpPress, ThirdPressModule.WorkUpPress);
            seq.AddItem(WorkFirstPressServo, WorkSecondPressServo, WorkOptionPressServo, WorkThridPressServo);



            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeLoadingGripSensorDelay);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(TapeHoldGrip.Grip.Sequence, WorkManualWithOutTomsonPress);
            seq.AddItem(TapeLoadGrip.Release.Sequence);
            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddStep("Terminate").StepIndex = seq.AddItem((o) => { ManualPressOnce = false;  });
            seq.AddTerminate();
        }

        private void MakeWorkManualOnceOneCycle()
        {
            var seq = WorkManualOnceOneCycle;

            #region Event
            seq.OnStart += delegate
            {
                //WorkInverterMotorLoading.Start();
            };
            #endregion

            LoadingQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new LoadingQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", " 전체 1싸이클 운전을 기동 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                  (actor, time) =>
                  {
                      if (questionWindow != null)
                      {
                          if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.StepSequence)
                          {
                              LoadingStep = true;
                              actor.NextStep();
                          }
                          else if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.Cancel)
                          {
                              actor.NextStep("Terminate");
                          }
                      }
                      else
                          actor.NextTerminate();
                  });
            seq.AddItem(
                (actor, time) =>
                {
                    if (!ManualPressOnce)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(
                (o) =>
            {
                //RearModule
                ManualPressOnce = true;
                RModule.ManualCurrentCount = 0;
                RModule.BandRollerServo.SetHomeMarking(this);
                RModule.BandRollerServo.ServoOnAction.Execute(this);
                //FrontModule
                TapeLoadingServo.ServoOnAction.Execute(o);
                BandTransferServo.ServoOnAction.Execute(o);
                BandPickServo.ServoOnAction.Execute(o);
            });

            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            //seq.AddItem(FirstPressModule.WorkManualUpPress, SecondPressModule.WorkManualUpPress, OptionPressModule.WorkManualUpPress, ThirdPressModule.WorkManualUpPress);
            seq.AddItem(WorkFirstPressServo, WorkSecondPressServo,WorkOptionPressServo, WorkThridPressServo);

            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeLoadingGripSensorDelay);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(TapeLoadingServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(TapeHoldGrip.Grip.Sequence, WorkManualPress);
            seq.AddItem(TapeLoadGrip.Release.Sequence, WorkManualPicking);
            seq.AddItem(TapeLoadingServo.MoveHomePos.Sequence);
            seq.AddItem(TapeLoadGrip.Grip.Sequence);
            seq.AddItem(TapeHoldGrip.Release.Sequence);
            seq.AddItem(
                (actor, time) =>
                {
                    if (ThirdPressModule.ManualUsePress && BypassCoveyorExistCheck.IsOff)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(RModule.WorkFourthManualPressServo);
            seq.AddStep("UnderCuttingCount").StepIndex = seq.AddItem(RModule.ManualWorkLoading);
            seq.AddItem(RModule.TimeManualDelay);
            seq.AddItem(RModule.WorkBandCutting);
            seq.AddItem(
              (actor, time) =>
              {
                  if (RModule.ManualCurrentCount >= RModule.ManualCuttingCount)
                  {
                      actor.NextStep();
                  }
                  else
                  {
                      actor.NextStep("UnderCuttingCount");
                  }
              });
            seq.AddStep("Terminate").StepIndex = seq.AddItem((o) => { ManualPressOnce = false; });
            seq.AddTerminate();
        }

        private void MakeWorkManualPress()
        {
            var seq = WorkManualPress;

            seq.AddItem(WorkFirstPressServo,
                WorkSecondPressServo,
                WorkOptionPressServo,
                WorkThridPressServo);
                        //SecondPressModule.ManualWorkPress,
                        //OptionPressModule.ManualWorkPress,
                        //ThirdPressModule.ManualWorkPress);

            

        }
        private void MakeWorkManualWithOutTomsonPress()
        {
            var seq = WorkManualWithOutTomsonPress;

            //seq.AddItem(FirstPressModule.ManualWorkPress,
            //            SecondPressModule.ManualWorkPress,
            //            OptionPressModule.ManualWorkPress);


            seq.AddItem(WorkFirstPressServo,
             WorkSecondPressServo,
             WorkOptionPressServo );
        }
        private void MakeWorkManualMoving()
        {
            var seq = WorkManualMoving;

            LoadingQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new LoadingQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", " 이재기 1사이클 운전을 기동 하시겠습니까? [동작] [취소]");
                                questionWindow.Cancelable = true;
                                questionWindow.Topmost = true;
                                questionWindow.Show();
                            }), null);
                });
            seq.AddItem(
                  (actor, time) =>
                  {
                      if (questionWindow != null)
                      {
                          if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.StepSequence)
                          {
                              actor.NextStep();
                          }
                          else if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.Cancel)
                          {
                              actor.NextStep("Terminate");
                          }
                      }
                      else
                          actor.NextStep("Terminate");
                  });
            seq.AddItem(
                (actor, time) =>
                {
                    if (!ManualPressOnce)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("Terminate");
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (BypassCoveyorExistCheck.IsOn)
                    {
                        actor.NextStep("Option");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                    //if (OptionPressModule.UsePress)
                    //{
                    //    actor.NextStep("Option");
                    //}
                    //else
                    //{
                    //    actor.NextStep();
                    //}
                });
            seq.AddItem((o) => { ManualPressOnce = true; });
            //seq.AddItem(/*ThirdPressModule.WorkUpPress*/, BandPitchChangeCylinder.Push.Sequence);
            seq.AddItem(WorkThridPressServo, BandPitchChangeCylinder.Push.Sequence);

            seq.AddItem(BandPickServo.MoveHomePos.Sequence, BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem(BandTransferServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(BandPickServo.MovePickPos.Sequence);
            seq.AddItem((o) => { BandVaccum.On.Execute(o); BandVaccumEject.Off.Execute(o); });
            seq.AddItem(VacuumDelay);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveHomePos.Sequence, BandPitchChangeCylinder.Home.Sequence);
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem((o) => { ManualPressOnce = false; });
            seq.AddTerminate();

            seq.AddStep("Option").StepIndex = seq.AddItem((o) => { ManualPressOnce = true; });
            //seq.AddItem(/*ThirdPressModule.WorkUpPress*/, BypassCoveyorMotor.Run.Sequence);
            seq.AddItem(WorkThridPressServo, BypassCoveyorMotor.Run.Sequence);

            
            seq.AddItem(BandPickServo.MoveHomePos.Sequence, BandTransferServo.MoveStandbyPos.Sequence);
            seq.AddItem(BandTransferServo.MoveTapeLoadingPos.Sequence, BypassCoveyorMotor.Stop.Sequence);
            seq.AddItem(BandPickServo.MovePickPos.Sequence);
            seq.AddItem((o) => { BandVaccum.On.Execute(o); BandVaccumEject.Off.Execute(o); });
            seq.AddItem(VacuumDelay);
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveHomePos.Sequence);
            seq.AddItem(BandPickServo.MovePlacePos.Sequence);
            seq.AddItem((o) => { BandVaccum.Off.Execute(o); BandVaccumEject.On.Execute(o); });
            seq.AddItem((o) => { BandVaccumEject.Off.Execute(o); });
            seq.AddItem(BandPickServo.MoveHomePos.Sequence);
            seq.AddItem(BandTransferServo.MoveStandbyPos.Sequence,BypassCoveyorMotor.Run.Sequence);
            seq.AddItem((o) => { ManualPressOnce = false; });
            seq.AddTerminate();
        }
    }
}