using FAFramework.Utility;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FALibrary.Utility;
using FAFramework.VT3500.ExtendedParts;
using FAFramework.GUI;
using System;

namespace FAFramework.VT3500.Modules
{
    public class FAPressModule : Module.FAPassModule
    {
        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence MainLoop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Initialize { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkAnotherPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkFourthPress { get; set; }

        //Manual Sequence
        [FAAttribute("Sequences")]
        public FASequence WorkPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence ManualWorkPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkUpPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkManualUpPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkDownPress { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkPress2 { get; set; }
        #endregion

        #region Parts 
        // Input
        // Press 동작
        public FAPartOnOffSensor MotorRunCheck { get; set; }
        public FAPartOnOffSensor OpenCheck { get; set; }
        public FAPartOnOffSensor CloseCheck { get; set; }
        public FAPartOnOffSensor PressCheck { get; set; }
        public FAPartOnOffSensor PressOilTempCheck { get; set; }
        // Output 
        // Press 동작
        public FAPartOnOff MotorRun { get; set; }
        public FAPartOnOff Opening { get; set; }
        public FAPartOnOff Closing { get; set; }
        public FAPartOnOff PressFanMotor { get; set; } // 상시 On

        public FAPartOnOffSensor FAreaCheck { get; set; }
        public FAPartOnOffSensor RAreaCheck { get; set; }
        public SubUnits.FAHeaterUnit HeaterFirstTopUnit { get; set; }
        public SubUnits.FAHeaterUnit HeaterFirstBottomUnit { get; set; }
        public SubUnits.FAHeaterUnit HeaterFourthBottomUnit { get; set; }
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime OutputDelay { get; set; }
        [FAAttribute("Time")]
        public FATime ClosingDelay { get; set; }
        [FA("Time")]
        public FATime PressDelay { get; set; }

        [FAAttribute("Time")]
        public FATime PressCheckDelay { get; set; }
        [FAAttribute("Time")]
        public FATime OpenCheckDelay { get; set; }
        [FAAttribute("Time")]
        public FATime TimeUpTimeOut { get; set; }
        [FAAttribute("Time")]
        public FATime TimeOpenCheckTimeOut { get; set; }
        [FAAttribute("Time")]
        public FATime TimeSecondOpenCheckTimeOut { get; set; }
        [FAAttribute("Time")]
        public FATime TimeCloseCheckTimeOut { get; set; }
        [FAAttribute("Time")]
        public FATime TimePressCheckTimeOut { get; set; }
        #endregion

        #region Alarm
        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.WARNING, "Open Check Time Out")]
        public int AlarmOpenCheckTimeOut { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.WARNING, "Press Check Time Out")]
        public int AlarmPressCheckTimeOut { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.WARNING, "Close Check Time Out")]
        public int AlarmCloseCheckTimeOut { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "앞쪽 안전센서가 감지 되었습니다")]
        public int AlarmFAreaDetected { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "뒤쪽 안전센서가 감지 되었습니다")]
        public int AlarmRAreaDetected { get; set; }

        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.ALARM, "프레스 오일 온도가 너무 높습니다! 프레스 오일을 식혀주세요")]
        public int AlarmPressOilTempCheck { get; set; }
        #endregion

        #region Modules
        public FAFrontLoadingModule FModule { get; set; } // FrontModule 참조
        public FARearLoadingModule RModule { get; set; } // FrontModule 참조
        public FAFrontLoadingModule BandTransferServo { get; set; }
        public Modules.FASafetyModule SafetyFirstTopModule { get; set; }
        public Modules.FASafetyModule SafetyFirstBottomModule { get; set; }
        #endregion

        #region Jobs
        private bool _usePress;
        [FAPropertyAttribute]
        [FA("Jobs")]
        public bool UsePress
        {
            get { return _usePress; }
            set
            {
                if (_usePress == value) return;
                _usePress = value;
                NotifyPropertyChanged("UsePress");
            }
        }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo PressOpenRetry { get; set; }

        [FAAttribute("RetryInfo")]
        public FARetryInfo PressCloseRetry { get; set; }

        [FAAttribute("RetryInfo")]
        public FARetryInfo PressPressRetry { get; set; }
        #endregion

        #region Parameters

        private bool _manualUsePress;
        [FAPropertyAttribute]
        [FA("Parameters")]
        public bool ManualUsePress
        {
            get { return _manualUsePress; }
            set
            {
                if (_manualUsePress == value) return;
                _manualUsePress = value;
                NotifyPropertyChanged("ManualUsePress");
            }
        }

        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public bool UsePressCheckDelay { get; set; }

        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public bool UsePressStopSequence { get; set; }

        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public bool UsePressResumeSequence { get; set; }
        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public bool ShowOilMessageOnce { get; set; }
        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public bool ExistMaterial { get; set; } //Memory
        #endregion
        //검증필
        #region Status 
        [FAAttribute("Status")]
        public bool PressSafeArea { get; set; }
        [FAAttribute("Status")]
        public bool PressTerminated { get; set; }
        #endregion

        #region UI
        private string _upButtonColor = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string UpButtonColor
        {
            get { return _upButtonColor; }
            set
            {
                if (_upButtonColor == value) return;
                _upButtonColor = value;
                NotifyPropertyChanged("UpButtonColor");
            }
        }

        private string _downButtonColor = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string DownButtonColor
        {
            get { return _downButtonColor; }
            set
            {
                if (_downButtonColor == value) return;
                _downButtonColor = value;
                NotifyPropertyChanged("DownButtonColor");
            }
        }
        private bool _downButtonOutput;
        [FAPropertyAttribute]
        [FA("UI")]
        public bool DownButtonOutput
        {
            get { return _downButtonOutput; }
            set
            {
                if (_downButtonOutput == value) return;
                _downButtonOutput = value;
                NotifyPropertyChanged("DownButtonOutput");
            }
        }

        private string _firstTopHeater = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string FirstTopHeater
        {
            get { return _firstTopHeater; }
            set
            {
                if (_firstTopHeater == value) return;
                _firstTopHeater = value;
                NotifyPropertyChanged("FirstTopHeater");
            }
        }
        private string _firstTopHeater1 = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string FirstTopHeater1
        {
            get { return _firstTopHeater1; }
            set
            {
                if (_firstTopHeater1 == value) return;
                _firstTopHeater1 = value;
                NotifyPropertyChanged("FirstTopHeater1");
            }
        }
        private string _firstBottomHeater = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string FirstBottomHeater
        {
            get { return _firstBottomHeater; }
            set
            {
                if (_firstBottomHeater == value) return;
                _firstBottomHeater = value;
                NotifyPropertyChanged("FirstBottomHeater");
            }
        }
        private string _firstBottomHeater1 = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string FirstBottomHeater1
        {
            get { return _firstBottomHeater1; }
            set
            {
                if (_firstBottomHeater1 == value) return;
                _firstBottomHeater1 = value;
                NotifyPropertyChanged("FirstBottomHeater1");
            }
        }
        private string _fourthTopHeater = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string FourthTopHeater
        {
            get { return _fourthTopHeater; }
            set
            {
                if (_fourthTopHeater == value) return;
                _fourthTopHeater = value;
                NotifyPropertyChanged("FourthTopHeater");
            }
        }
        private string _fourthTopHeater1 = "White";
        [FAPropertyAttribute]
        [FA("UI")]
        public string FourthTopHeater1
        {
            get { return _fourthTopHeater1; }
            set
            {
                if (_fourthTopHeater1 == value) return;
                _fourthTopHeater1 = value;
                NotifyPropertyChanged("FourthTopHeater1");
            }
        }
        #endregion

        public override void InitializeSequence() // 모든 함수 초기화
        {
            MakeInitialize();
            MakeWorkUpPress();
            MakeWorkManualUpPress();
            MakeWorkDownPress();
            MakeManualWorkPress();
            MakeWorkAnotherPress();
            MakeWorkPress2();
            MakeWorkFourthPress();
        }
        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            UsePressStopSequence = false;
            UsePressResumeSequence = false;
            ShowOilMessageOnce = false;
            ExistMaterial = false;
            PressTerminated = false;
        }

        private void MakeInitialize()
        {
            var seq = Initialize;

            // Press Up
            seq.AddItem(WorkUpPress);
        }

        public void MakeWorkAnotherPress()//210612
        {
            var seq = WorkAnotherPress;

            #region Event
            seq.OnStart += delegate
            {
                PressTerminated = false;
                Closing.Off.Execute(this); // closing 정지
                Opening.Off.Execute(this); // opening 동작
                //ExistMaterial = false;

                PressOpenRetry.ClearCount();
                PressCloseRetry.ClearCount();
                PressPressRetry.ClearCount();
            };

            seq.OnTerminate +=
                delegate
                {
                    PressTerminated = true;
                };

            #endregion
          
            seq.AddItem(WorkUpPress);
            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    PressSafeArea = true;

                    if (UsePress)
                    {
                        MotorRun.On.Execute(actor);
                        WriteTraceLog($"module={this.Name}, UsePress={UsePress.ToString()}");
                        actor.NextStep();
                    }
                    else
                    {
                        //MotorRun.Off.Execute(actor);210823
                        WriteTraceLog($"module={this.Name}, UsePress={UsePress.ToString()}");
                        actor.NextTerminate();
                    }
                });

            // motor run
            seq.AddStep("MotorRun").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        MotorRun.On.Execute(actor);
                        Opening.Off.Execute(actor);
                        Closing.On.Execute(actor);
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Closing.Off.Execute(actor);
                        Opening.Off.Execute(actor);
                        WorkUpPress.Start();
                        RaiseAlarm(actor, AlarmCloseCheckTimeOut); //검증
                    }
                });
            seq.AddStep("CloseCheck").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (CloseCheck.IsOn && OpenCheck.IsOff && PressCheck.IsOff)
                    {
                        Closing.Off.Execute(actor);
                        PressSafeArea = false;
                        actor.NextStep();
                    }
                    else if (TimeCloseCheckTimeOut.Time < time)
                    {
                        Closing.Off.Execute(actor);
                        Opening.Off.Execute(actor);
                        WorkUpPress.Start();
                        RaiseAlarm(actor, AlarmCloseCheckTimeOut); //검증
                    }
                });
            seq.AddItem(PressDelay);
            seq.AddItem(
             (actor, time) =>
             {
                 if (UsePressCheckDelay)
                 {
                     actor.NextStep();
                 }
                 else
                 {
                     actor.NextStep("UnUsePressCheckDelay");
                 }
             });
            seq.AddItem(PressCheckDelay);
            // press up
            seq.AddStep("UnUsePressCheckDelay").StepIndex =
            seq.AddItem(Closing.Off.Sequence);
            seq.AddItem(Opening.On.Sequence);
            // press stop
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        PressSafeArea = true;
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Closing.Off.Execute(actor);
                        Opening.Off.Execute(actor);
                        WorkUpPress.Start();
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut); //검증
                    }
                });
            seq.AddItem(Opening.Off.Sequence);
            seq.AddItem(OpenCheckDelay);
            seq.AddItem((o) => { ExistMaterial = true; });
            seq.AddTerminate();
        }

        public void MakeWorkFourthPress()//210612
        {
            var seq = WorkFourthPress;

            #region Event
            seq.OnStart += delegate
            {
                Closing.Off.Execute(this); // closing 정지
                Opening.Off.Execute(this); // opening 동작

                PressOpenRetry.ClearCount();
                PressCloseRetry.ClearCount();
                PressPressRetry.ClearCount();
            };
            #endregion
           
            seq.AddItem(WorkUpPress);
            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    PressSafeArea = true;

                    if (UsePress)
                    {
                        WriteTraceLog($"module={this.Name}, UsePress={UsePress.ToString()}");
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"module={this.Name}, UsePress={UsePress.ToString()}");
                        actor.NextTerminate();
                    }
                });

            // motor run
            seq.AddStep("MotorRun").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        MotorRun.On.Execute(actor);
                        Opening.Off.Execute(actor);
                        Closing.On.Execute(actor);
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Closing.Off.Execute(actor);
                        Opening.Off.Execute(actor);
                        WorkUpPress.Start();
                        //actor.NextTerminate();
                        RaiseAlarm(actor, AlarmCloseCheckTimeOut); //검증
                    }
                    //else
                    //{
                    //    MotorRun.On.Execute(actor);
                    //    Closing.Off.Execute(actor);
                    //    Opening.On.Execute(actor);
                    //}
                });
            seq.AddStep("CloseCheck").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (CloseCheck.IsOn && OpenCheck.IsOff && PressCheck.IsOff)
                    {
                        Closing.Off.Execute(actor);
                        PressSafeArea = false;
                        actor.NextStep();
                    }
                    else if (TimeCloseCheckTimeOut.Time < time)
                    {
                        Closing.Off.Execute(actor);
                        Opening.Off.Execute(actor);
                        WorkUpPress.Start();
                        //actor.NextTerminate();
                        RaiseAlarm(actor, AlarmCloseCheckTimeOut); //검증
                    }
                    //else
                    //{
                    //    MotorRun.On.Execute(actor);
                    //    Opening.Off.Execute(actor);
                    //    Closing.On.Execute(actor);
                    //}
                });
            seq.AddItem(PressDelay);
            seq.AddItem(
             (actor, time) =>
             {
                 if (UsePressCheckDelay)
                 {
                     actor.NextStep();
                 }
                 else
                 {
                     actor.NextStep("UnUsePressCheckDelay");
                 }
             });
            seq.AddItem(PressCheckDelay);
            // press up
            seq.AddStep("UnUsePressCheckDelay").StepIndex =
            seq.AddItem(Closing.Off.Sequence);
            seq.AddItem(Opening.On.Sequence);
            // press stop
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        PressSafeArea = true;
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Closing.Off.Execute(actor);
                        Opening.Off.Execute(actor);
                        WorkUpPress.Start();
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut); //검증
                    }
                    //else
                    //{
                    //    MotorRun.On.Execute(actor);
                    //    Closing.Off.Execute(actor);
                    //    Opening.On.Execute(actor);
                    //}
                });
            seq.AddItem(Opening.Off.Sequence);
            seq.AddItem(OpenCheckDelay);
            seq.AddTerminate();
        }
        //Manual Sequence
        public void MakeManualWorkPress()
        {
            var seq = ManualWorkPress;

            #region Event
            seq.OnStart += delegate
            {
                Closing.Off.Execute(this); // closing 정지
                Opening.Off.Execute(this); // opening 동작
                //MotorRun.Off.Execute(this); // motor 정지
            };
            seq.OnStop += delegate
            {
                PressSafeArea = false;
            };
            #endregion

            seq.AddItem(
                (actor, time) =>
                {
                    if (ManualUsePress)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(WorkUpPress);
            seq.AddStep("MotorRun").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        MotorRun.On.Execute(actor);
                        Closing.On.Execute(actor);
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            //하사점
            seq.AddItem(
                (actor, time) =>
                {
                    if (CloseCheck.IsOn && PressCheck.IsOff && OpenCheck.IsOff)
                    {
                        Closing.Off.Execute(actor);
                        PressSafeArea = false;
                        actor.NextStep();
                    }
                    else if (TimeCloseCheckTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmCloseCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(PressDelay);
            seq.AddItem(
            (actor, time) =>
            {
                if (UsePressCheckDelay)
                {
                    actor.NextStep();
                }
                else
                {
                    actor.NextStep("UnUsePressCheckDelay");
                }
            });
            seq.AddItem(PressCheckDelay);
            // press up
            seq.AddStep("UnUsePressCheckDelay").StepIndex =
            seq.AddItem(Closing.Off.Sequence);
            seq.AddItem(Opening.On.Execute);
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        PressSafeArea = true;
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(Opening.Off.Sequence);
            //seq.AddItem(MotorRun.Off.Sequence);
            seq.AddItem(OpenCheckDelay);
            //seq.AddItem((o) => { MotorRun.Off.Execute(o); }); //210823
            seq.AddTerminate();
        }

        public void MakeWorkPress2()
        {
            var seq = WorkPress2;

            #region Event
            seq.OnStart += delegate
            {
                Closing.Off.Execute(this); // closing 정지
                Opening.Off.Execute(this); // opening 동작
                //MotorRun.Off.Execute(this); // motor 정지
            };
            seq.OnStop += delegate
            {
                PressSafeArea = false;
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
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "프레스를 동작 하시겠습니까? [동작] [취소]");
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
            seq.AddItem(WorkUpPress);
            seq.AddStep("MotorRun").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        MotorRun.On.Execute(actor);
                        Closing.On.Execute(actor);
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            //하사점
            seq.AddItem(
                (actor, time) =>
                {
                    if (CloseCheck.IsOn && PressCheck.IsOff && OpenCheck.IsOff)
                    {
                        Closing.Off.Execute(actor);
                        PressSafeArea = false;
                        actor.NextStep();
                    }
                    else if (TimeCloseCheckTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmCloseCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(PressDelay);
            seq.AddItem(
            (actor, time) =>
            {
                if (UsePressCheckDelay)
                {
                    actor.NextStep();
                }
                else
                {
                    actor.NextStep("UnUsePressCheckDelay");
                }
            });
            seq.AddItem(PressCheckDelay);
            // press up
            seq.AddStep("UnUsePressCheckDelay").StepIndex =
            seq.AddItem(Closing.Off.Sequence);
            seq.AddItem(Opening.On.Execute);
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        PressSafeArea = true;
                        actor.NextStep();
                    }
                    else if (TimeOpenCheckTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(Opening.Off.Sequence);
            //seq.AddItem(MotorRun.Off.Sequence);
            seq.AddItem(OpenCheckDelay);
            //seq.AddItem((o) => { MotorRun.Off.Execute(o); });210823
            seq.AddTerminate();
        }

        private void MakeWorkUpPress()
        {
            var seq = WorkUpPress;

            #region Event
            seq.OnStart += delegate
            {
                Closing.Off.Execute(this); // closing 정지
                Opening.Off.Execute(this); // opening 동작
                //MotorRun.Off.Execute(this); // motor 정지
            };
            #endregion

            seq.AddStep("UpPressStart").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (UsePress)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(MotorRun.On.Sequence);
           
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn)
                    {
                        //MotorRun.Off.Execute(actor); //210823
                        actor.NextTerminate();
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(Closing.Off.Sequence);
            seq.AddItem(Opening.On.Sequence);
            seq.AddStep("PressCheck").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (PressCheck.IsOn && OpenCheck.IsOff && CloseCheck.IsOff)
                    {
                        WriteTraceLog("PressON");
                        actor.NextStep("Down");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddStep("Stop").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        WriteTraceLog("OpenCheck");
                        actor.NextStep();
                    }
                    else if (TimeUpTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);

                        if (PressCheck.IsOn)
                        {
                            actor.NextStep("PressCheck");
                        }
                        else
                        {
                            RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        }
                    }
                });
            seq.AddItem(OpenCheckDelay);
            seq.AddItem(Opening.Off.Sequence);
            //seq.AddItem((o) => { MotorRun.Off.Execute(o); }); //210823
            seq.AddTerminate();

            seq.AddStep("Down").StepIndex = seq.AddItem(Opening.Off.Sequence);
            seq.AddItem(Closing.On.Sequence);
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        Closing.Off.Execute(actor);
                        actor.NextStep();
                    }
                    else if (TimeSecondOpenCheckTimeOut.Time < time)
                    {
                        MotorRun.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem("Stop");
        }

        private void MakeWorkManualUpPress()
        {
            var seq = WorkManualUpPress;

            #region Event
            seq.OnStart += delegate
            {
                Closing.Off.Execute(this); // closing 정지
                Opening.Off.Execute(this); // opening 동작
                //MotorRun.Off.Execute(this); // motor 정지
            };
            #endregion

            seq.AddStep("UpPressStart").StepIndex = seq.AddItem(
                (actor, time) => 
                {
                    if (ManualUsePress)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextTerminate(); 
                    }
                });
            seq.AddItem(MotorRun.On.Sequence);

            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn)
                    {
                        //MotorRun.Off.Execute(actor); //210823
                        actor.NextTerminate();
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(Closing.Off.Sequence);
            seq.AddItem(Opening.On.Sequence);
            seq.AddStep("PressCheck").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (PressCheck.IsOn && OpenCheck.IsOff && CloseCheck.IsOff)
                    {
                        WriteTraceLog("PressON");
                        actor.NextStep("Down");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddStep("Stop").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        WriteTraceLog("OpenCheck");
                        actor.NextStep();
                    }
                    else if (TimeUpTimeOut.Time < time)
                    {
                        Opening.Off.Execute(actor);

                        if (PressCheck.IsOn)
                        {
                            actor.NextStep("PressCheck");
                        }
                        else
                        {
                            RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        }
                    }
                });
            seq.AddItem(OpenCheckDelay);
            seq.AddItem(Opening.Off.Sequence);
            //seq.AddItem((o) => { MotorRun.Off.Execute(o); }); //210823
            seq.AddTerminate();

            seq.AddStep("Down").StepIndex = seq.AddItem(Opening.Off.Sequence);
            seq.AddItem(Closing.On.Sequence);
            seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn && CloseCheck.IsOff && PressCheck.IsOff)
                    {
                        Closing.Off.Execute(actor);
                        actor.NextStep();
                    }
                    else if (TimeSecondOpenCheckTimeOut.Time < time)
                    {
                        MotorRun.Off.Execute(actor);
                        Closing.Off.Execute(actor);
                        RaiseAlarm(actor, AlarmOpenCheckTimeOut);
                        actor.NextTerminate();
                    }
                });
            seq.AddItem("Stop");
        }

        private void MakeWorkDownPress()
        {
            var seq = WorkDownPress;

            #region Event
            seq.OnStart += delegate
            {
                Closing.Off.Execute(this);
                Opening.Off.Execute(this);
                //MotorRun.Off.Execute(this);
            };
            #endregion
            // motor run
            seq.AddStep("DownPressStart").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (OpenCheck.IsOn)
                    {
                        MotorRun.On.Execute(actor);
                        Closing.On.Execute(actor);
                        Opening.Off.Execute(actor);
                        actor.NextStep();
                    }
                    else
                    {
                        Opening.On.Execute(actor);
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (CloseCheck.IsOn)
                    {
                        actor.NextStep();
                    }
                    else if (TimeUpTimeOut.Time < time)
                    {
                        //    RaiseAlarm(actor, AlarmCloseCheckTimeOut);
                        //    여기에 확인이 안될 경우에는 다시 업 할 수 있게 조건식을 지정
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        Opening.On.Execute(actor);
                        actor.NextStep("DownPressStart");
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (PressCheck.IsOn)
                    {
                        actor.NextStep();
                    }
                    else if (TimeUpTimeOut.Time < time)
                    {
                        //    RaiseAlarm(actor, AlarmCloseCheckTimeOut);
                        //    여기에 확인이 안될 경우에는 다시 업 할 수 있게 조건식을 지정
                        Closing.Off.Execute(actor);
                        MotorRun.Off.Execute(actor);
                        Opening.On.Execute(actor);
                        actor.NextStep("DownPressStart");
                    }
                });
            seq.AddItem(ClosingDelay);
            seq.AddItem(
                (actor, time) =>
                {
                    Closing.Off.Execute(actor);
                    actor.NextStep();
                });
            seq.AddItem(Opening.Off.Sequence);
            //seq.AddItem(MotorRun.Off.Sequence);
            seq.AddTerminate();
        }
    }
}
