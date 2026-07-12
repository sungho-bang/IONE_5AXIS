using System;
using System.Collections;
using System.Collections.Generic;

namespace FAFramework.Utility
{
    public static class StaticCommands
    {
        private static System.Collections.Generic.Dictionary<object, System.Windows.Threading.DispatcherTimer> OnOffRepeatThreadDic = new System.Collections.Generic.Dictionary<object, System.Windows.Threading.DispatcherTimer>();

        public static CommandHandler OnOffCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric;
                    if (part.IsTurnOn())
                        part.TurnOffAction.Execute(null);
                    else
                        part.TurnOnAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler OnCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric;
                    part.TurnOnAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler OffCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric;
                    part.TurnOffAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler OnOffRepeatCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    dynamic parameters = obj;
                    var checkBox = parameters[0] as System.Windows.Controls.CheckBox;
                    var part = parameters[1] as FALibrary.Part.MemoryBasePart.FAPartMemoryBaseGeneric;

                    System.Windows.Threading.DispatcherTimer timer;
                    if (OnOffRepeatThreadDic.ContainsKey(checkBox) == false)
                    {
                        bool actionFlag = true;
                        timer = new System.Windows.Threading.DispatcherTimer();
                        timer.Tick +=
                            delegate
                            {
                                if (actionFlag)
                                    part.TurnOnAction.Execute(null);
                                else
                                    part.TurnOffAction.Execute(null);

                                actionFlag = !actionFlag;

                                double repeatTime = 0;
                                if (double.TryParse(checkBox.Tag.ToString(), out repeatTime) == true)
                                {
                                    try
                                    {
                                        var intRepeatTime = Math.Round(repeatTime);
                                        var interval = new TimeSpan(0, 0, 0, (int)intRepeatTime, (int)((repeatTime - intRepeatTime) * 1000));
                                        if (interval != timer.Interval)
                                            timer.Interval = interval;
                                    }
                                    catch (Exception e)
                                    {
                                        System.Windows.MessageBox.Show(e.ToString());
                                        Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                                    }
                                }
                            };

                        OnOffRepeatThreadDic.Add(checkBox, timer);
                    }

                    timer = OnOffRepeatThreadDic[checkBox];

                    if (checkBox.IsChecked == false)
                    {
                        timer.Stop();
                    }
                    else
                    {
                        var repeatTime = double.Parse(parameters[2]);
                        var intRepeatTime = Math.Round(repeatTime);
                        var interval = new TimeSpan(0, 0, 0, (int)intRepeatTime, (int)((repeatTime - intRepeatTime) * 1000));
                        timer.Interval = interval;
                        timer.Start();
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler OneWayMotorRunCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartOneWayACMotor;
                    part.Run.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler OneWayMotorStopCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartOneWayACMotor;
                    part.Stop.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler TwoWayMotorRunCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor;
                    part.RunAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler TwoWayMotorReserveRunCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor;
                    part.ReverseRunAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }

            }, true);

        public static CommandHandler TwoWayMotorStopCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartTwoWayACMotor;
                    part.StopAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler PartActionExecuteCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var partAction = obj as FALibrary.Part.FAPartAction;
                    partAction.Execute(null);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler SequenceExecuteCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var sequence = obj as FALibrary.Sequence.FASequence;
                    sequence.Start();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler SequenceResumeCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var sequence = obj as FALibrary.Sequence.FASequence;
                    sequence.Resume();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler SequenceStopCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var sequence = obj as FALibrary.Sequence.FASequence;
                    sequence.Stop();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler SequenceStopAndClearCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var sequence = obj as FALibrary.Sequence.FASequence;
                    sequence.Stop();
                    sequence.ClearState();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler ActuatorSelectCommand = new CommandHandler(
            (obj) =>
            {
                try
                {
                    var button = obj as System.Windows.Controls.Button;
                    dynamic parent = button.Parent;
                    if (button.Tag != null &&
                        parent.Tag != null &&
                        button.Tag == parent.Tag)
                        parent.Tag = null;
                    else
                        parent.Tag = button.Tag;
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler ServoMovePosCommand = new CommandHandler(
            (obj) =>
            {
                try
                {
                    var arr = obj as object[];
                    var part = arr[0] as FALibrary.Part.MMCPart.FAMMCPart;
                    var position = arr[1] as FALibrary.Part.MMCPart.FAMMCPosition;
                    part.MovePositionNoCopyTargetPosition(position);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler DoorLockCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartDoor;
                    part.Lock();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler DoorUnlockCommand = new CommandHandler(
            delegate (object obj)
            {
                try
                {
                    var part = obj as FALibrary.Part.MemoryBasePart.FAPartDoor;
                    part.Unlock();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);

        public static CommandHandler ShowNumericKeyboard = new CommandHandler(
            (parameter) =>
            {
                try
                {
                    var arr = parameter as object[];
                    dynamic obj = arr[0] as FALibrary.FAObject;
                    string property = (string)arr[1];
                    if (obj.RangeList.ContainsKey(property))
                    {
                        var range = obj.RangeList[property];
                        GUI.NumericKeyboardWindow keyboard = new GUI.NumericKeyboardWindow();
                        keyboard.Min = range.Min;
                        keyboard.Max = range.Max;                       
                        keyboard.NumType = obj.GetType().GetProperty(property).GetType();

                        if ((bool)keyboard.ShowDialog())
                        {
                            obj.GetType().GetProperty(property).SetValue(obj, keyboard.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                    Manager.LogManager.Instance.WriteSystemLog(e.ToString());
                }
            }, true);
    }
}
