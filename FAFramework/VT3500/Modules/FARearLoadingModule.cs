using FAFramework.GUI;
using FAFramework.Utility;
using FAFramework.VT3500.ExtendedParts;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using FALibrary.Sequence;
using FALibrary.Utility;
using System;
using FAFramework.VT3500.ExtendedParts;
using FAFramework.VT3500.JobInfo;

namespace FAFramework.VT3500.Modules
{
    public class FARearLoadingModule : Module.FAPassModule
    {

        #region Sequences
        [FAAttribute("Sequences")]
        public FASequence MainLoop { get; set; }
        [FAAttribute("Sequences")]
        public FASequence Initialize { get; set; } 
        [FAAttribute("Sequences")]
        public FASequence WorkLoading { get; set; } // 용지 이동하는 서보모터 가동 
        [FAAttribute("Sequences")]
        public FASequence OnceCycleStart { get; set; }
        [FAAttribute("Sequences")]
        public FASequence OnceCycleEnd { get; set; }
        [FAAttribute("Sequences")]
        public FASequence ManualWorkLoading { get; set; } // 용지 이동하는 서보모터 가동 
        [FAAttribute("Sequences")]
        public FASequence WorkACMotorLoading { get; set; } // 마지막 AC 모터 가동 및 텐션
        [FAAttribute("Sequences")]
        public FASequence WorkPress { get; set; } // 프레스 찍는 행위
        [FAAttribute("Sequences")]
        public FASequence WorkBandCutting { get; set; } // 용지를 컷팅하는 행위(추가) 
        [FAAttribute("Sequences")]
        public FASequence MovingRoller { get; set; }

        [FAAttribute("Sequences")]
        public FASequence WorkManualLoading { get; set; } // Loading Manual 동작
        [FAAttribute("Sequences")]
        public FASequence WorkRearPullManual { get; set; } // Manual Pull 동작
        [FAAttribute("Sequences")]
        public FASequence SearchingImark { get; set; } // Manual Pull 동작







        //edge-lee 240617
        [FAAttribute("Sequences")]
        public FASequence WorkFourthPressServo { get; set; }
        [FAAttribute("Sequences")]
        public FASequence WorkFourthManualPressServo { get; set; }

        //





        #endregion

        #region Parts 
        public FAPartOneWayACMotor SealingTapeLoadingMotor { get; set; } // 사이드 용지를 감아주는 모터
        public FAPartOnOffSensor SealingTapeTensionUpSensor { get; set; } // 마지막 용지 텐션 유지용 위쪽 센서
        public FAPartOnOffSensor SealingTapeTensionDownSensor { get; set; } // 마지막 용지 텐션 유지용 아래쪽 센서

        public FAPartOnOffSensor BlackMarkCheckSensor { get; set; } // 바닥용지 감지 센서
        public FAPartUpDown SealingTopRoller { get; set; } // 롤러 동작 실린더
        public FAPartUpDown SealingBandCutting { get; set; } // 밴드 컷팅 실린더   
        public FABandRollerServo BandRollerServo { get; set; } // 밴드 이동 서보모터
        #endregion

        #region Time
        [FAAttribute("Time")]
        public FATime TimeLoadingTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime TimeLoadingInitialTimeout { get; set; }
        [FAAttribute("Time")]
        public FATime TimeManualDelay { get; set; } // Loading과 press사이에 시간지정
        [FAAttribute("Time")]
        public FATime TimeRollerDelay { get; set; }
        #endregion

        #region TackTime
        private DateTime _lastLoadingTime;
        [FAAttribute("TackTime")]
        public DateTime LastLoadingTime
        {
            get { return _lastLoadingTime; }
            set
            {
                if (_lastLoadingTime == value) return;
                _lastLoadingTime = value;
                NotifyPropertyChanged("LastLoadingTime");
            }
        }

        private TimeSpan _tact = TimeSpan.Zero;
        [FAAttribute("TackTime")]
        public TimeSpan Tact
        {
            get { return _tact; }
            set
            {
                if (_tact == value) return;
                _tact = value;
                NotifyPropertyChanged("Tact");
            }
        }
        #endregion

        #region Status
        [FAAttribute("Status")]
        public bool BandReceiveStandby { get; set; } // 동작 완료 확인
        [FAAttribute("Status")]
        public bool BandReceiveComplete { get; set; } // 동작 완료 확인
        [FAAttribute("Status")]
        public bool RearSeqTerminated { get; set; } // 동작 완료 확인
        [FAAttribute("Status")]
        public bool PlaceSkip { get; set; } // 동작 완료 확인
        [FAAttribute("Status")]
        public bool MachineResume { get; set; }

        #endregion

        #region Parameters
        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public int CurrentCount { get; set; }

        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public int ManualCurrentCount { get; set; }

        [FAPropertyAttribute]
        [FAAttribute("Parameters")]
        public int ManualCuttingCount { get; set; }

        [FAAttribute("Parameters")]
        public bool LoadingStep { get; set; }

        [FAAttribute("Parameters")]
        public bool PressStep { get; set; }
        [FAAttribute("Parameters")]
        public bool ReSet { get; set; }

        [FAAttribute("Parameters")]
        public bool RModuleServoOff { get; set; } 

        [FAAttribute("Parameters")]
        public bool OnceCycleStartEnd { get; set; }

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
        private bool _usePackingScrap;
        [FAPropertyAttribute]
        [FA("Jobs")]
        public bool UsePackingScrap
        {
            get { return _usePackingScrap; }
            set
            {
                if (_usePackingScrap == value) return;
                _usePackingScrap = value;
                NotifyPropertyChanged("UsePackingScrap");
            }
        }
        private int _jobCount;
        [FAPropertyAttribute]
        [FA("Jobs")]
        public int JobCount
        {
            get { return _jobCount; }
            set
            {
                if (_jobCount == value) return;
                _jobCount = value;
                NotifyPropertyChanged("JobCount");
            }
        }

        private bool _useIMark;
        [FAPropertyAttribute]
        [FA("Jobs")]
        public bool UseIMark
        {
            get { return _useIMark; }
            set
            {
                if (_useIMark == value) return;
                _useIMark = value;
                NotifyPropertyChanged("UseIMark");
            }
        }
        #endregion

        #region UI
        private int _uICuttingCount;
        [FAPropertyAttribute]
        [FA("UI")]
        public int UICuttingCount
        {
            get { return _uICuttingCount; }
            set
            {
                if (_uICuttingCount == value) return;
                _uICuttingCount = value;
                NotifyPropertyChanged("UICuttingCount");
            }
        }
        private int _uIGoalCount;
        [FAPropertyAttribute]
        [FA("UI")]
        public int UIGoalCount
        {
            get { return _uIGoalCount; }
            set
            {
                if (_uIGoalCount == value) return;
                _uIGoalCount = value;
                NotifyPropertyChanged("UIGoalCount");
            }
        }
        #endregion

        #region Alarm        
        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmPaperCheck { get; set; } // 바닥 용지 확인 알람

        [FAProperty]
        [FAAttribute("Alarm")]
        public int AlarmSealingTopRoller { get; set; } // 롤러의 up/down 확인

        [FAProperty]
        [FAAttribute("Alarm")]
        public int TourqeAlarmCheck { get; set; } // 토크 부족 확인



        [FAProperty]
        [FAAttribute("Alarm")]
        [AlarmInfo(ConfigClasses.GlobalConst.ALARM_TYPE_METHOD, ConfigClasses.GlobalConst.WARNING, "BlackMark Sensor Check Time Out")]
        public int AlarmBlackMarkCheckTimeOut { get; set; }

        //[DefaultAlarmInfo(1, Utility.Alarm.EAlarmType.MACHINE, Utility.Alarm.EAlarmStatus.ALARM)]
        //[AlarmDescription(KnownCulture.EnglishUS, "Achieve The Goal!", "Product Complete")]
        //[AlarmDescription(KnownCulture.Korean, "목표생산량 도달 했습니다!", "다음 생산을 해주십시오")]

        [FAProperty]
        [FAAttribute("Alarm")]
        public int AchieveTheGoal { get; set; }
        #endregion

        #region RetryInfo
        [FAAttribute("RetryInfo")]
        public FARetryInfo RetryInfoBlackMarkRetry { get; set; }
        #endregion

        #region Modules
        public FAFrontLoadingModule FModule { get; set; } // FrontModule 참조

        public FAPressModule FourthPressModule { get; set; } // FourthPressModule 참조

        public SubUnits.FAPressUnit FourthPressUnit { get; set; } // FourthPressUnit 참조

        public FAFourthPressServo FourthPressServo { get; set; }


        #endregion



        private void MakeWorkFourthManualPressServo()
        {
            var seq = WorkFourthManualPressServo;

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
              FourthPressServo.ServoOnAction.Execute(o);
              //ThirdPressServo.ServoOnAction.Execute(o);
              //BandTransferServo.ServoOnAction.Execute(o);
              //BandPickServo.ServoOnAction.Execute(o);
          });



            seq.AddItem(FourthPressServo.MovePickPos.Sequence);

            seq.AddItem(FourthPressModule.PressCheckDelay);


            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (FourthPressServo.ReadServoRatio >= Math.Truncate((FourthPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (FourthPressModule.TimeUpTimeOut.Time < time)
                {
                    FourthPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);


                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });




            seq.AddItem(FourthPressServo.Stop.Sequence);

            seq.AddItem(FourthPressModule.PressDelay);

            seq.AddItem(FourthPressServo.MoveHomePos.Sequence);


            seq.AddItem(FourthPressServo.Stop.Sequence);

            seq.AddTerminate();

        }


        //// edge-lee 240612
        public void MakeWorkFourthPressServo()
        {

            var seq = WorkFourthPressServo;

            #region Event
            seq.OnStart += delegate
            {
                FourthPressModule.PressTerminated = false;

                // Closing.Off.Execute(this); // closing 정지
                // Opening.Off.Execute(this); // opening 동작
                // MotorRun.Off.Execute(this); // motor 정지
                FourthPressModule.PressOpenRetry.ClearCount();
                FourthPressModule.PressCloseRetry.ClearCount();
                FourthPressModule.PressPressRetry.ClearCount();

            };
            seq.OnStop += delegate
            {
                //  PressSafeArea = false;
                FourthPressModule.PressTerminated = true;
            };
            #endregion


            seq.AddStep("PressStart").StepIndex = seq.AddItem(
                  (actor, time) =>
                  {
                      FourthPressModule.PressSafeArea = true;

                      if (FourthPressModule.UsePress)
                      {
                          FourthPressServo.ServoOnAction.Execute(actor);
                          WriteTraceLog($"module={this.Name}, UsePress={FourthPressModule.UsePress.ToString()}");
                          actor.NextStep();
                      }
                      else
                      {
                          //MotorRun.Off.Execute(actor);210823
                          WriteTraceLog($"module={this.Name}, UsePress={FourthPressModule.UsePress.ToString()}");
                          actor.NextTerminate();
                      }
                  });

            seq.AddItem(
(o) =>
{
              //ManualPressOnce = true;
              FourthPressServo.ServoOnAction.Execute(o);
              //ThirdPressServo.ServoOnAction.Execute(o);
              //BandTransferServo.ServoOnAction.Execute(o);
              //BandPickServo.ServoOnAction.Execute(o);
          });



            seq.AddItem(FourthPressServo.MovePickPos.Sequence);

            seq.AddItem(FourthPressModule.PressCheckDelay);

            seq.AddStep("CheckTorqueLimit").StepIndex = seq.AddItem((actor, time) => { actor.NextStep(); });

            seq.AddItem(
            (actor, time) =>
            {
                if (FourthPressServo.ReadServoRatio >= Math.Truncate((FourthPressServo.TorqMaxParamter / 2.94)))
                {

                    actor.NextStep();
                }
                else if (FourthPressModule.TimeUpTimeOut.Time < time)
                {
                    FourthPressServo.Stop.Execute(this);


                    RaiseAlarm(actor, TourqeAlarmCheck);


                    actor.AddTerminate();
                }
                //else
                // {
                //    actor.NextStep("CheckTorqueLimit");
                //}
            });


            // seq.AddItem(FourthPressServo.MovePlacePos.Sequence);


            seq.AddItem(FourthPressServo.Stop.Sequence);





            seq.AddItem(FourthPressModule.PressCheckDelay);


           // seq.AddItem(ThirdPressServo.MoveHomePos.Sequence);
            seq.AddItem(FourthPressServo.MoveHomePos.Sequence);


            // press up
            seq.AddItem(
                   (actor, time) =>
                   {
                       FourthPressModule.PressSafeArea = true;
                       actor.NextStep();


                   });

            seq.AddItem(FourthPressModule.OpenCheckDelay);
            seq.AddItem((o) => { FourthPressModule.ExistMaterial = true; });

            seq.AddTerminate();


        }





        public override void InitializeSequence() // 모든 함수 초기화
        {
            MakeMainLoop();
            MakeInitialize();
            MakeWorkLoading();
            MakeOnceCycleStart();
            MakeOnceCycleEnd();
            MakeManualWorkLoading();
            MakeWorkACMotorLoading();
            MakeWorkPress();
            MakeBandCutting();
            MakeMovingRoller();
            MakeWorkManualLoading();
            MakeWorkRearPullManual();
            MakeSearchingImark();



            MakeWorkFourthPressServo();
            MakeWorkFourthManualPressServo();
        }

        public override void ClearProductInfo()
        {
            base.ClearProductInfo();

            OnceCycleStartEnd = false;
            RearSeqTerminated = false;
            BandReceiveStandby = false;
            BandReceiveComplete = false;
            LoadingStep = false;
            PressStep = false;
            //PlaceSkip = false;
            ReSet = false;
            CurrentCount = 0;
            RetryInfoBlackMarkRetry.ClearCount();
        }

        public bool DownProcess()
        {
            return FModule.BandPlaceStandby && BandReceiveStandby;
        }

        private void MakeInitialize()
        {
            var seq = Initialize;
            
            seq.AddItem(SealingBandCutting.Up.Sequence); // 컷팅 업
                                                         //seq.AddItem(FourthPressModule.Initialize); // Press 초기화

            seq.AddItem(FourthPressServo.MoveHome.Sequence);

            seq.AddItem(
            (actor, time) =>
            {
                //if (UseIMark)
                //{
                //    actor.NextStep();
                //}
                //else
                //{
                    actor.NextStep("UnUseIMark");
                //}
            });
            seq.AddItem(
               (actor, time) =>
               {
                   if (SealingTopRoller.Status != SealingTopRoller.StatusList.Down)
                   {
                       RaiseAlarm(actor, AlarmSealingTopRoller);
                       //ShowMessage("AlarmSealingTopRoller", AlarmSealingTopRoller, "AlarmSealingTopRoller");
                   }
                   else
                   {
                       actor.NextStep();
                   }
               });
            seq.AddItem((o) => { BandRollerServo.ServoOnAction.Execute(this); });
            seq.AddStep("Again").StepIndex = seq.AddItem((o) => { BandRollerServo.SetHomeMarking(this); }); // 값 0으로
            seq.AddItem(BandRollerServo.MoveTapeLoadingSlowPos.Execute); // 감속
            seq.AddItem(
                (actor, time) =>
                {
                    if (BlackMarkCheckSensor.IsOn)
                    {
                        BandRollerServo.Stop.Execute(this);
                        actor.NextStep();
                    }
                    else if (TimeLoadingInitialTimeout.Time < time)
                    {
                        actor.NextStep("Again");
                    }
                });

            seq.AddStep("UnUseIMark").StepIndex = seq.AddItem(
               (actor, time) =>
               {
                   if (SealingTopRoller.Status != SealingTopRoller.StatusList.Down)
                   {
                       RaiseAlarm(actor, AlarmSealingTopRoller);
                       //ShowMessage("AlarmSealingTopRoller", AlarmSealingTopRoller, "AlarmSealingTopRoller");
                   }
                   else
                   {
                       actor.NextStep();
                   }
               });
            seq.AddItem((o) => { BandRollerServo.ServoOnAction.Execute(this); });
            seq.AddItem((o) => { BandRollerServo.SetHomeMarking(this); });
        }
        private void MakeMainLoop() // 반복적으로 동작
        {
            var seq = MainLoop;
            
            seq.AddItem(new FASequenceAtomicInfo(WorkLoading, true));
        }
        private void MakeWorkLoading()
        {
            //20200728
            var seq = WorkLoading;

            #region Event
            seq.OnStart += delegate
            {
                RearSeqTerminated = false;

                CloseMessage("AlarmSealingTopRoller");
            };
            seq.OnPreSuspend += delegate
            {
            };
            seq.OnTerminate += delegate
            {
                BandReceiveComplete = true;
                RearSeqTerminated = true;
                WriteTraceLog($"RearSeqTerminated ={RearSeqTerminated.ToString()},");
            };
            #endregion

            seq.AddItem(
               (actor, time) =>
               {
                   if (FourthPressModule.UsePress && FModule.ThirdPressModule.UsePress)
                   //if (FourthPressModule.UsePress && FModule.ThirdPressModule.UsePress && FModule.RearStop)
                   {
                       actor.NextStep();
                   }
                   else
                   {
                       WriteTraceLog($"ThirdPress={FModule.ThirdPressModule.UsePress.ToString()},$FourthPress={FourthPressModule.UsePress.ToString()}");
                       RearSeqTerminated = true;
                       BandReceiveComplete = true;
                       BandReceiveStandby = true;
                       actor.NextTerminate();
                   }
               });
            //seq.AddItem(
            //    (actor, time) =>
            //    {
            //        //if (!BandReceiveComplete)
            //        if (!BandReceiveComplete)
            //        {
            //            actor.NextStep();
            //        }
            //    });
            seq.AddItem((o) => { WriteTraceLog("LoadingStart"); });
            seq.AddItem(
                (actor, time) =>
                {
                    if (SealingTopRoller.Status != SealingTopRoller.StatusList.Down)
                    {
                        ShowMessage("AlarmSealingTopRoller", AlarmSealingTopRoller, "AlarmSealingTopRoller");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddStep("Start").StepIndex = seq.AddItem((o) => { BandReceiveStandby = true; }); //210202
            seq.AddItem(
                (actor, time) =>
                {
                    if (DownProcess())
                    {
                        WriteTraceLog("DownProcessOK");
                        actor.NextStep();
                    }
                    else if (ReSet)
                    {
                        WriteTraceLog("Reset OK");
                        ReSet = false;
                        actor.NextTerminate();
                    }
                    else if (FModule.WorkLoopBandMoveLoading.State == SequenceState.Terminated)
                    {
                        WriteTraceLog("FrontModule_Terminated");
                        ReSet = false;
                        actor.NextTerminate();
                    }
                });
            seq.AddItem(WorkPress);
            seq.AddStep("UnderSetTime").StepIndex = seq.AddItem(new FASequenceAtomicInfo( MovingRoller,true));
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       actor.NextStep("WorkPress");
                   }
                   else
                   {
                       actor.NextStep();
                   }
               });
            seq.AddItem(WorkBandCutting); // Press 동봉
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       var now = DateTime.Now; // 현재의 시간을 계산

                       if (LastLoadingTime != DateTime.MinValue)
                       {
                           Tact = now - LastLoadingTime;
                           string path = System.IO.Path.Combine(Manager.LogManager.LOG_ROOT_PATH,
                               Equipment.Name, "TactTime");
                           Manager.LogManager.Instance.WriteCSVLog(path, string.Empty,
                               Tact, "\t");
                       }
                       CurrentCount = 0;
                       WriteTraceLog($"Tact = {Tact}");
                       LastLoadingTime = now;
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnderSetTime");
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (UIGoalCount <= UICuttingCount)
                    {
                        UICuttingCount = 0; //Reset
                        //RaiseAlarm(actor, AchieveTheGoal); // 목표달성
                        ShowMessage("AchieveTheGoal", AchieveTheGoal, "AchieveTheGoal");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"현재 컷팅/목표 = {UICuttingCount}/{UIGoalCount}");
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();
           
            seq.AddStep("WorkPress").StepIndex = 
            seq.AddItem(WorkBandCutting); // Press 동봉
            seq.AddItem(
               (actor, time) =>
               {
                   //if (UICuttingCount % 4 == 0)
                   if (CurrentCount >= JobCount)
                   {
                       var now = DateTime.Now; // 현재의 시간을 계산

                       if (LastLoadingTime != DateTime.MinValue)
                       {
                           Tact = now - LastLoadingTime;
                           string path = System.IO.Path.Combine(Manager.LogManager.LOG_ROOT_PATH,
                               Equipment.Name, "TactTime");
                           Manager.LogManager.Instance.WriteCSVLog(path, string.Empty,
                               Tact, "\t");
                       }
                       CurrentCount = 0;
                       WriteTraceLog($"Tact = {Tact}");
                       LastLoadingTime = now;
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnderSetTime");
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (UIGoalCount <= UICuttingCount)
                    {
                        UICuttingCount = 0; //Reset
                        //RaiseAlarm(actor, AchieveTheGoal); // 목표달성
                        ShowMessage("AchieveTheGoal", AchieveTheGoal, "AchieveTheGoal");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"현재 컷팅/목표 = {UICuttingCount}/{UIGoalCount}");
                        actor.NextStep();
                    }
                });
            seq.AddItem(
                (actor, time) =>
                {
                    if (FModule.FeedingAfterLock)
                    {
                        
                        actor.NextStep("Start");
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();
        }
        private void MakeWorkACMotorLoading()
        {
            var seq = WorkACMotorLoading;

            #region Event
            seq.OnResume += delegate
            {
                SealingTapeLoadingMotor.Run.Execute(this);
            };
            seq.OnSuspended += delegate
            {
                SealingTapeLoadingMotor.Stop.Execute(this);
            };
            #endregion

            seq.AddStep("MotorLoop").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    if (UsePackingScrap)
                    {
                        actor.NextStep();
                    }
                });
            seq.AddItem(
               (actor, time) =>
               {
                   if (SealingTapeTensionDownSensor.IsOn)  // down 센서가 on 일때
                   {
                       SealingTapeLoadingMotor.Run.Execute(actor); // Packing motor stop
                       actor.NextStep();
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (SealingTapeTensionUpSensor.IsOn) // up 센서가 on 일때
                    {
                        SealingTapeLoadingMotor.Stop.Execute(actor); // Packing motor run
                        actor.NextStep();
                    }
                });
            seq.AddItem("MotorLoop");
        }
        private void MakeMovingRoller()
        {
            var seq = MovingRoller;

            #region Event
            seq.OnStart += delegate
            {
                RetryInfoBlackMarkRetry.ClearCount();
            };
            seq.OnSuspended += delegate
            {
                //210716
                //BandRollerServo.Stop.Execute(this);
                //BandRollerServo.SetHomeMarking(this);
            };
            seq.OnSuspending += delegate
            {
                //210716
                //BandRollerServo.Stop.Execute(this);
            };
            seq.OnTerminate += delegate
            {
                //BandReceiveStandby = true;
            };
            #endregion

            seq.AddItem(
                (actor, time) =>
                {
                    if (UseIMark)
                    {
                        actor.NextStep();
                    }
                    else
                    {
                        actor.NextStep("UnUseIMark");
                    }
                });
            seq.AddItem(BandRollerServo.MoveTapeLoadingPos.Sequence); // 가속
            seq.AddStep("Retry").StepIndex = seq.AddItem(BandRollerServo.MoveTapeLoadingSlowPos.Execute); // 감속
            seq.AddItem(
                (actor, time) =>
                {
                    if (BlackMarkCheckSensor.IsOn)
                    {
                        WriteTraceLog($"BandRollerServo 실제값={BandRollerServo.ActualPos}");
                        BandRollerServo.Stop.Execute(this);
                        actor.NextStep("UseIMark");
                    }
                    else if (TimeLoadingTimeout.Time < time)
                    {
                        if (RetryInfoBlackMarkRetry.IncreaseCount())
                        {
                            RModuleServoOff = true;
                            WriteTraceLog($"BandRollerServo 실제값={BandRollerServo.ActualPos},BlackMarkSensor={BlackMarkCheckSensor.Status}");
                            BandRollerServo.SettingHomeMarking(actor);
                            actor.NextStep("Retry");
                        }
                        else
                        {
                            RaiseAlarm(actor, AlarmBlackMarkCheckTimeOut);
                        }
                    }
                });
            seq.AddStep("UnUseIMark").StepIndex = seq.AddItem(BandRollerServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(BandRollerServo.MoveTapeLoadingSlowPos.Sequence);
            //seq.AddItem(BandRollerServo.MoveTapeUnUseImarkPos.Sequence);

            seq.AddStep("UseIMark").StepIndex = seq.AddItem((o) =>
            {
                RModuleServoOff = false;
                CurrentCount++;
                UICuttingCount += 2;
                WriteTraceLog($"BandRollerServo 실제값={BandRollerServo.ActualPos}");
                BandRollerServo.Stop.Execute(this);
            });
            seq.AddStep("Terminate").StepIndex = seq.AddItem(BandRollerServo.SetHomeMarking); // 값 0으로
            seq.AddItem(TimeRollerDelay);
        }
        private void MakeOnceCycleStart()
        {
            var seq = OnceCycleStart;
            
            seq.OnStart += delegate
            {
                OnceCycleStartEnd = true;
            };
            seq.OnTerminate += delegate
            {
                OnceCycleStartEnd = false ;
            };
            
            //seq.AddItem(FourthPressModule.WorkAnotherPress);
            seq.AddItem(WorkFourthPressServo);


            seq.AddStep("UnderSetTime").StepIndex = seq.AddItem(MovingRoller);
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       actor.NextStep("WorkPress");
                   }
                   else
                   {
                       actor.NextStep();
                   }
               });
            seq.AddItem(WorkBandCutting); // Press 동봉
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       var now = DateTime.Now; // 현재의 시간을 계산

                       if (LastLoadingTime != DateTime.MinValue)
                       {
                           Tact = now - LastLoadingTime;
                           string path = System.IO.Path.Combine(Manager.LogManager.LOG_ROOT_PATH,
                               Equipment.Name, "TactTime");
                           Manager.LogManager.Instance.WriteCSVLog(path, string.Empty,
                               Tact, "\t");
                       }
                       CurrentCount = 0;
                       WriteTraceLog($"Tact = {Tact}");
                       LastLoadingTime = now;
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnderSetTime");
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (UIGoalCount <= UICuttingCount)
                    {
                        UICuttingCount = 0;
                        //RaiseAlarm(actor, AchieveTheGoal);
                        ShowMessage("AchieveTheGoal", AchieveTheGoal, "AchieveTheGoal");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"현재 컷팅/목표 = {UICuttingCount}/{UIGoalCount}");
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

            seq.AddStep("WorkPress").StepIndex =
            seq.AddItem(WorkBandCutting);
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       var now = DateTime.Now; // 현재의 시간을 계산

                       if (LastLoadingTime != DateTime.MinValue)
                       {
                           Tact = now - LastLoadingTime;
                           string path = System.IO.Path.Combine(Manager.LogManager.LOG_ROOT_PATH,
                               Equipment.Name, "TactTime");
                           Manager.LogManager.Instance.WriteCSVLog(path, string.Empty,
                               Tact, "\t");
                       }
                       CurrentCount = 0;
                       WriteTraceLog($"Tact = {Tact}");
                       LastLoadingTime = now;
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnderSetTime");
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (UIGoalCount <= UICuttingCount)
                    {
                        UICuttingCount = 0; //Reset
                        //RaiseAlarm(actor, AchieveTheGoal); // 목표달성
                        ShowMessage("AchieveTheGoal", AchieveTheGoal, "AchieveTheGoal");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"현재 컷팅/목표 = {UICuttingCount}/{UIGoalCount}");
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

        }
        private void MakeOnceCycleEnd()
        {
            var seq = OnceCycleEnd;
           
            seq.AddStep("UnderSetTime").StepIndex = seq.AddItem(MovingRoller);
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       actor.NextStep("WorkPress");
                   }
                   else
                   {
                       actor.NextStep();
                   }
               });
            seq.AddItem(WorkBandCutting); // Press 동봉
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       var now = DateTime.Now; // 현재의 시간을 계산

                       if (LastLoadingTime != DateTime.MinValue)
                       {
                           Tact = now - LastLoadingTime;
                           string path = System.IO.Path.Combine(Manager.LogManager.LOG_ROOT_PATH,
                               Equipment.Name, "TactTime");
                           Manager.LogManager.Instance.WriteCSVLog(path, string.Empty,
                               Tact, "\t");
                       }
                       CurrentCount = 0;
                       WriteTraceLog($"Tact = {Tact}");
                       LastLoadingTime = now;
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnderSetTime");
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (UIGoalCount <= UICuttingCount)
                    {
                        UICuttingCount = 0;
                        //RaiseAlarm(actor, AchieveTheGoal);
                        ShowMessage("AchieveTheGoal", AchieveTheGoal, "AchieveTheGoal");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"현재 컷팅/목표 = {UICuttingCount}/{UIGoalCount}");
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

            seq.AddStep("WorkPress").StepIndex = 
            seq.AddItem(WorkBandCutting); 
            seq.AddItem(
               (actor, time) =>
               {
                   if (CurrentCount >= JobCount)
                   {
                       var now = DateTime.Now; // 현재의 시간을 계산

                       if (LastLoadingTime != DateTime.MinValue)
                       {
                           Tact = now - LastLoadingTime;
                           string path = System.IO.Path.Combine(Manager.LogManager.LOG_ROOT_PATH,
                               Equipment.Name, "TactTime");
                           Manager.LogManager.Instance.WriteCSVLog(path, string.Empty,
                               Tact, "\t");
                       }
                       CurrentCount = 0;
                       WriteTraceLog($"Tact = {Tact}");
                       LastLoadingTime = now;
                       actor.NextStep();
                   }
                   else
                   {
                       actor.NextStep("UnderSetTime");
                   }
               });
            seq.AddItem(
                (actor, time) =>
                {
                    if (UIGoalCount <= UICuttingCount)
                    {
                        UICuttingCount = 0; //Reset
                        //RaiseAlarm(actor, AchieveTheGoal); // 목표달성
                        ShowMessage("AchieveTheGoal", AchieveTheGoal, "AchieveTheGoal");
                        FAFramework.Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500.ModuleStateSignal.SignalPhoneMelodie1.DoTurnOn(actor);
                        Equipment.RequestStop();
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog($"현재 컷팅/목표 = {UICuttingCount}/{UIGoalCount}");
                        actor.NextStep();
                    }
                });
            seq.AddTerminate();

        }
        private void MakeWorkPress() // 프레스 동작
        {
            var seq = WorkPress;

            seq.OnStart += delegate
            {
                //BandReceiveComplete = true;
            };

            seq.OnTerminate += delegate
            {
                BandReceiveStandby = false;
            };
            //
            seq.AddItem((o) => { WriteTraceLog("PressStart"); });
            seq.AddStep("PressStart").StepIndex =
            //seq.AddItem(FourthPressModule.WorkAnotherPress); // press 동작
            //seq.AddItem(FourthPressModule.WorkFourthPress); // press 동작

            seq.AddItem(WorkFourthPressServo); // press 동작


            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();
        }
        private void MakeBandCutting() // 밴드 커팅(추가)
        {
            var seq = WorkBandCutting;

            seq.OnStart += delegate
            {

            };
            seq.AddItem(SealingBandCutting.Up.Sequence);
            seq.AddItem((o) => { WriteTraceLog("OnlyBandCuttingStart"); });
            seq.AddItem(SealingBandCutting.Down.Sequence);
            seq.AddStep("OnlyBandCuttingStart").StepIndex = seq.AddItem(
                (actor, time) =>
                {
                    actor.NextStep();
                });
            seq.AddItem(SealingBandCutting.Up.Sequence); // 컷팅기 위로
            seq.AddTerminate();
        }
        private void MakeManualWorkLoading()
        {
            //20200728
            var seq = ManualWorkLoading;
            
            seq.AddItem(
                (actor, time) =>
                {
                    if (SealingTopRoller.Status != SealingTopRoller.StatusList.Down)
                    {
                        ShowMessage("AlarmSealingTopRoller", AlarmSealingTopRoller, "AlarmSealingTopRoller");
                        actor.NextTerminate();
                    }
                    else
                    {
                        WriteTraceLog("Manual Lading Start");
                        actor.NextStep();
                    }
                });
            seq.AddItem(
              (actor, time) =>
              {
                  if (UseIMark)
                  {
                      WriteTraceLog("Use I-Mark");
                      actor.NextStep("BlackMarkCheck");
                  }
                  else
                  {
                      WriteTraceLog("UnUse I-Mark");
                      actor.NextStep();
                  }
              });
            //seq.AddItem(BandRollerServo.MoveTapeUnUseImarkPos.Sequence);
            seq.AddItem(BandRollerServo.MoveTapeLoadingPos.Sequence);
            seq.AddItem(BandRollerServo.MoveTapeLoadingSlowPos.Sequence);
            seq.AddItem("SetHomeMarking");

            seq.AddStep("BlackMarkCheck").StepIndex = seq.AddItem(BandRollerServo.MoveTapeLoadingPos.Sequence);
            seq.AddStep("BlackMarkCheckRetry").StepIndex = seq.AddItem(BandRollerServo.MoveTapeLoadingSlowPos.Execute);
            seq.AddItem(
                (actor, time) =>
                {
                    if (BlackMarkCheckSensor.IsOn)
                    {
                        BandRollerServo.Stop.Execute(actor);
                        WriteTraceLog($"BlackCheckSensor={BlackMarkCheckSensor.Status}");
                        actor.NextStep();
                    }
                    else
                    {
                        WriteTraceLog("Retry Sensing Black Mark");
                        BandRollerServo.SettingHomeMarking(actor);
                        BandRollerServo.MoveTapeLoadingSlowPos.Execute(actor);
                        actor.NextStep("BlackMarkCheckRetry");
                    }
                });
            seq.AddStep("SetHomeMarking").StepIndex = seq.AddItem((o) => { BandRollerServo.SetHomeMarking(this); ManualCurrentCount++; }); // 값 0으로
            seq.AddTerminate();
        }
        private void MakeWorkManualLoading()
        {
            var seq = WorkManualLoading;

            seq.OnStart += delegate
            {
                LoadingStep = false;
                PressStep = false;
            };
          
            LoadingQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new LoadingQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", " MANUAL LOOP SEQUENCE를 기동 하시겠습니까? [연속 동작] [단계 동작] [취소]");
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
                          if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.ContinueSequence)
                          {
                              actor.NextStep("LoadingContinue");
                          }
                          else if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.StepSequence)
                          {
                              LoadingStep = true; // 버튼 누를때 마다 true로 변경을 해줘야함(GUI 버튼 클릭시 변경으로 설정)
                              actor.NextStep("LoadingStep");
                          }
                          else if (questionWindow.Result == LoadingQuestionMessageBoxWindow.QuestionResult.Cancel)
                          {
                              actor.NextStep("Terminate");
                          }
                      }
                      else
                          actor.NextStep();
                  });

            // 단계 동작 ----------------------------------------------------------------------------------------------------------------------------------------------
            seq.AddStep("LoadingStep").StepIndex = seq.AddItem(
                (o) =>
                {
                    BandRollerServo.SetHomeMarking(o);
                }); // 서보모터 홈포지션

            seq.AddItem(
                (actor, time) =>
                {
                    if (SealingTopRoller.Status != SealingTopRoller.StatusList.Down)
                    {
                        ShowMessage("AlarmSealingTopRoller", AlarmSealingTopRoller, "AlarmSealingTopRoller");

                        actor.NextTerminate();
                    }
                    else
                    {
                        actor.NextStep();
                    }
                });

            seq.AddStep("LoadingStart").StepIndex = seq.AddItem(BandRollerServo.MoveTapeLoadingPos.Sequence);

            seq.AddItem(
                (actor, time) =>
                {
                    //if (BlackMarkCheckSensor.IsOn)
                    //{
                    actor.NextStep();
                    //}
                    //else if (BlackMarkCheckSensor.IsOff)
                    //{
                    //    BandRollerServo.Stop.Execute(actor);
                    //    SealingTapeLoadingMotor.Stop.Execute(actor);

                    //    BandRollerServo.SetHomeMarking(this); // 값 0으로
                    //    actor.NextStep("LoadingStart");
                    //}
                });

            //seq.AddItem(SealingTapeLoadingMotor.Stop.Sequence); //20210104
            seq.AddItem((o) => { BandRollerServo.SetHomeMarking(this); }); // 값 0으로

            seq.AddItem(
                (actor, time) =>
                {
                    if (LoadingStep)
                    {
                        LoadingStep = false;

                        actor.NextStep();
                    }
                });

            seq.AddItem(SealingBandCutting.Down.Sequence); // 컷팅이 내려

            seq.AddItem(
                (actor, time) =>
                {
                    if (LoadingStep)
                    {
                        LoadingStep = false;

                        actor.NextStep();
                    }
                });

            seq.AddItem(SealingBandCutting.Up.Sequence); // 컷팅기 위로
            seq.AddItem("Terminate");

            // 연속동작 ----------------------------------------------------------------------------------------------------------------------------------------------
            seq.AddStep("LoadingContinue").StepIndex = seq.AddItem(BandRollerServo.MoveHome); // 서보모터 홈포지션
            seq.AddItem(SealingBandCutting.Up.Sequence); // 컷팅 업

            seq.AddStep("LoadingContinueLoop").StepIndex = seq.AddItem(BandRollerServo.MoveVelocity.Sequence);
            seq.AddItem(
                (actor, time) =>
                {
                    if (BlackMarkCheckSensor.IsOn)
                    {
                        BandRollerServo.Stop.Execute(actor);
                        //SealingTapeLoadingMotor.Stop.Execute(actor);

                        BandRollerServo.SetHomeMarking(this); // 값 0으로
                        actor.NextStep();
                    }
                    else if (BlackMarkCheckSensor.IsOff)
                    {
                        BandRollerServo.Stop.Execute(actor);
                        //SealingTapeLoadingMotor.Stop.Execute(actor);

                        BandRollerServo.SetHomeMarking(this); // 값 0으로
                        actor.NextStep("LoadingContinueLoop");
                    }
                });

            seq.AddItem(SealingBandCutting.Down.Sequence); // 컷팅이 내려
            seq.AddItem(SealingBandCutting.Up.Sequence); // 컷팅기 위로
            seq.AddItem("LoadingContinueLoop"); // Loop 

            seq.AddStep("Terminate").StepIndex = seq.AddTerminate();
        }
        private void MakeWorkRearPullManual()
        {
            var seq = WorkRearPullManual;

            seq.OnStart += delegate
            {
                ManualCurrentCount = 0;
                BandRollerServo.SetHomeMarking(this);
                BandRollerServo.ServoOnAction.Execute(this);
            };

            PullQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new PullQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "포장 1사이클을 기동 하시겠습니까? [동작] [취소]");
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
                      //else
                      //    ManualCurrentCount = 0;
                      //    actor.NextTerminate();
                  });

            //seq.AddItem(FourthPressModule.ManualWorkPress);
            seq.AddItem(WorkFourthPressServo);


            seq.AddStep("UnderCuttingCount").StepIndex = seq.AddItem(ManualWorkLoading);
            seq.AddItem(TimeManualDelay);
            //seq.AddItem(FourthPressModule.ManualWorkPress, WorkBandCutting);
            seq.AddItem(WorkBandCutting);
            seq.AddItem(
              (actor, time) =>
              {
                  //if (UICuttingCount % 4 == 0)
                  if (ManualCurrentCount >= ManualCuttingCount)
                  {
                      actor.NextStep();
                  }
                  else
                  {
                      actor.NextStep("UnderCuttingCount");
                  }
              });
            seq.AddStep("Terminate").StepIndex = 
               // seq.AddItem((o) => { UseIMark = false; });
            seq.AddTerminate();
        }
        private void MakeSearchingImark()
        {
            var seq = SearchingImark;
            seq.OnStart += delegate
            {
                BandRollerServo.SetHomeMarking(this);
            };
            seq.OnTerminate += delegate
            {
                BandRollerServo.SetHomeMarking(this);
            };

            PullQuestionMessageBoxWindow questionWindow = null;
            seq.AddItem(
                (object obj) =>
                {
                    App.Current.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                questionWindow = new PullQuestionMessageBoxWindow();
                                questionWindow.Message = Utility.UtilityClass.GetStringResource(this, "QuestionMsgBox", "아이마크를 찾으시겠습니까? [동작] [취소]");
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
                      //else
                      //    ManualCurrentCount = 0;
                      //    actor.NextTerminate();
                  });

            seq.AddStep("Searching").StepIndex = seq.AddItem(BandRollerServo.MoveTapeLoadingSlowPos.Execute);
            seq.AddItem(
              (actor, time) =>
              {
                  if (BlackMarkCheckSensor.IsOn)
                  {
                      BandRollerServo.Stop.Execute(actor);
                      actor.NextStep();
                  }
                  else
                  {
                      actor.NextStep("Searching");
                  }
              });
            seq.AddTerminate();
        }
    }

}