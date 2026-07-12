using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace FALibrary.Sequence
{
    public enum SequenceState
    {
        Available, Running, Idle, Starting, Terminated, Suspended, Suspending, Alarm, Aborted,
    }

    public class StepInfo
    {
        public int StepIndex { get; set; }
    }

    public class FASequenceAtomicInfo
    {
        public FASequence Sequence { get; set; }
        public bool Atomic { get; set; }
        
        public FASequenceAtomicInfo(FASequence seq, bool atomic)
        {
            Sequence = seq;
            Atomic = atomic;             
        }
    }

    public class FASequence : FAObject
    {
        public abstract class FASequenceItem : FAObject
        {
            protected bool IsStarted { get; set; }
            protected Stopwatch Watch { get; private set; }
            protected Action<FASequence, TimeSpan> Executor { get; set; }

            public SequenceState State { get; protected set; }
            public bool Atomic { get; set; }
            public FASequence Owner
            {
                get;
                protected set;
            }
            public bool TimerRunningState
            {
                get
                {
                    if (Watch != null)
                    {
                        return Watch.IsRunning;
                    }

                    return false;
                }
            }

            public TimeSpan Time
            {
                get
                {
                    if (Watch != null)
                    {
                        return Watch.Elapsed;
                    }

                    return new TimeSpan(0);
                }
            }

            public FASequenceItem(FASequence owner)
            {
                Owner = owner;
                Watch = new Stopwatch();
            }

            public void Execute(FASequence actor)
            {
                if (Watch.ElapsedTicks == 0)
                {
                    Watch = Stopwatch.StartNew();
                }

                if (IsStarted == false)
                {
                    IsStarted = true;
                    FirstExecute(actor);
                }

                Executor(actor, Watch.Elapsed);
            }

            public abstract bool IsSuspended();

            public void ClearState()
            {
                IsStarted = false;
                State = SequenceState.Available;
                Watch.Stop();
                Watch.Reset();
            }

            public void ClearTime()
            {
                Watch.Stop();
                Watch.Reset();
            }

            protected abstract void FirstExecute(FASequence actor);

            public virtual void Suspend()
            {
                if (Atomic == false)
                {
                    State = SequenceState.Suspending;
                    Watch.Stop();
                }
            }

            public virtual void Resume()
            {
                State = SequenceState.Running;
                ClearTime();
            }

            public virtual void Stop()
            {
                State = SequenceState.Available;
                Watch.Stop();
            }

            public virtual void SetAlarm()
            {
                State = SequenceState.Alarm;
                Watch.Stop();
            }
        }

        public class FAAutoNextSequenceItem : FASequenceItem
        {
            private Action<object> _autoNextExecutor = null;

            public FAAutoNextSequenceItem(FASequence owner, Action<object> actoin)
                : base(owner)
            {
                _autoNextExecutor = actoin;
                Executor = AutoNextExecute;
            }

            private void AutoNextExecute(FASequence actor, TimeSpan time)
            {
                _autoNextExecutor(actor);
                actor.NextStep();
            }

            protected override void FirstExecute(FASequence actor)
            {
                State = SequenceState.Running;
            }

            public override bool IsSuspended()
            {
                if (Atomic == true)
                    return false;
                else if (State == SequenceState.Suspending)
                {
                    State = SequenceState.Suspended;
                    return true;
                }
                else if (State == SequenceState.Alarm)
                    return true;
                else if (State == SequenceState.Suspended)
                    return true;
                else if (State == SequenceState.Available)
                    return true;
                else if (State == SequenceState.Terminated)
                    return true;

                return false;
            }
        }

        public class FAConditionalSequenceItem : FASequenceItem
        {
            private Action<FASequence, TimeSpan> _action = null;

            public FAConditionalSequenceItem(FASequence owner, Action<FASequence, TimeSpan> actoin)
                : base(owner)
            {
                Atomic = false;
                _action = actoin;
                Executor = ExecuteAction;
            }

            private void ExecuteAction(FASequence actor, TimeSpan time)
            {
                _action(actor, time);
            }

            protected override void FirstExecute(FASequence actor)
            {
                State = SequenceState.Running;
            }

            public override bool IsSuspended()
            {
                if (State == SequenceState.Alarm)
                    return true;
                else if (Atomic == true)
                    return false;
                else if (State == SequenceState.Suspending)
                {
                    State = SequenceState.Suspended;
                    return true;
                }

                else if (State == SequenceState.Suspended)
                    return true;
                else if (State == SequenceState.Available)
                    return true;
                else if (State == SequenceState.Terminated)
                    return true;

                return false;
            }
        }

        public class FASequenceCombinationSequenceItem : FASequenceItem
        {
            private List<FASequenceAtomicInfo> _sequenceList = new List<FASequenceAtomicInfo>();

            public FASequenceCombinationSequenceItem(FASequence owner, params FASequenceAtomicInfo[] sequences)
                : base(owner)
            {
                _sequenceList.AddRange(sequences);
                Executor = ConfirmTerminatedSequences;
            }

            protected override void FirstExecute(FASequence actor)
            {
                State = SequenceState.Running;
                foreach (FASequenceAtomicInfo item in _sequenceList)
                {
                    item.Sequence.Atomic = item.Atomic;
                    item.Sequence.Start(Owner);
                }
            }

            private void ConfirmTerminatedSequences(FASequence actor, TimeSpan time)
            {
                foreach (FASequenceAtomicInfo item in _sequenceList)
                {
                    if (item.Sequence.State != SequenceState.Terminated)
                        return;
                }

                actor.NextStep();
            }

            public override bool IsSuspended()
            {
                if (State == SequenceState.Suspending)
                {
                    foreach (FASequenceAtomicInfo item in _sequenceList)
                    {
                        if (item.Sequence.IsBackground)
                            continue;
                        else if (item.Sequence.State != SequenceState.Suspended &&
                            item.Sequence.State != SequenceState.Available &&
                            item.Sequence.State != SequenceState.Terminated)
                            return false;
                    }
                    
                    State = SequenceState.Suspended;
                    
                    return true;
                }
                else if (State == SequenceState.Suspended)
                    return true;
                else if (State == SequenceState.Available)
                    return true;
                else if (State == SequenceState.Terminated)
                    return true;

                return false;
            }

            public override void Suspend()
            {
                State = SequenceState.Suspending;
                foreach (FASequenceAtomicInfo item in _sequenceList)
                {
                    if (item.Atomic == false)
                        item.Sequence.Suspend();
                }

                Watch.Stop();
            }

            public override void SetAlarm()
            {
                Suspend();
            }

            public override void Resume()
            {
                State = SequenceState.Running;

                foreach (FASequenceAtomicInfo item in _sequenceList)
                {
                    item.Sequence.Resume();
                }

                ClearTime();
            }

            public override void Stop()
            {
                State = SequenceState.Available;
                foreach (FASequenceAtomicInfo item in _sequenceList)
                {
                    item.Sequence.Stop();
                }

                Watch.Stop();
            }
        }

        private Queue<Action> _standbyQueue = new Queue<Action>();
        private SequenceState _state = SequenceState.Available;
        private FASequenceItem _currentItem = null;
        private Dictionary<string, StepInfo> _steps = new Dictionary<string, StepInfo>();
        private List<Func<FASequence, bool>> _runningInterlockList = new List<Func<FASequence, bool>>();
        private List<Action> _watcherList = new List<Action>();
        private List<FASequenceItem> _sequenceList = new List<FASequenceItem>();
        private bool _atomic;
        private System.Diagnostics.Stopwatch _stepStopWatch = new Stopwatch();

        public event EventHandler OnStart = null;
        public event EventHandler OnStop = null;
        public event EventHandler OnPreSuspend = delegate { };
        public event EventHandler OnSuspending = null;
        public event EventHandler OnSuspended = null;
        public event EventHandler OnResume = null;
        public event EventHandler OnTerminate = null;
        public event EventHandler OnChangeStep = delegate { };
        public FASequenceManager SequenceManager { get; private set; }
        public FASequenceItem CurrentItem
        {
            get { return _currentItem; }
            protected set
            {
                if (_currentItem != value)
                {
                    _currentItem = value;
                    NotifyPropertyChanged("CurrentIndex");
                    NotifyPropertyChanged("CurrentStepName");

                    try
                    {
                        OnChangeStep(this, EventArgs.Empty);
                    }
                    catch (Exception e)
                    {
                        Utility.Trace.WriteLine(this, "Sequence", e.ToString());
                    }
                }
            }
        }
        public FASequence Caller
        {
            get;
            protected set;
        }

        private bool _locked;
        [FAAttribute("")]
        public bool Locked
        {
            get { return _locked; }
            set
            {
                if (_locked == value) return;
                _locked = value;
                NotifyPropertyChanged("Locked");
            }
        }

        [FAAttribute("")]
        public bool Atomic
        {
            get { return _atomic; }
            set
            {
                if (_atomic == value) return;
                _atomic = value;
                NotifyPropertyChanged("Atomic");
            }
        }

        private bool _isBackground;
        [FAAttribute("")]
        public bool IsBackground
        {
            get { return _isBackground; }
            set
            {
                if (_isBackground == value) return;
                _isBackground = value;
                NotifyPropertyChanged("IsBackground");
            }
        }

        private TimeSpan _stepElapsed;
        [FAAttribute("")]
        public TimeSpan StepElapsed
        {
            get
            {
                return _stepElapsed;
            }

            private set
            {
                if (_stepElapsed == value) return;
                _stepElapsed = value;
                NotifyPropertyChanged("StepElapsed");
            }
        }

        private TimeSpan _currentItemTime;
        [FAAttribute("")]
        public TimeSpan CurrentItemTime
        {
            get
            {
                return _currentItemTime;
            }

            private set
            {
                if (_currentItemTime == value) return;

                _currentItemTime = value;
                NotifyPropertyChanged("CurrentItemTime");
            }
        }

        private bool _currentItemTimerRunningState;
        [FAAttribute("")]
        public bool CurrentItemTimerRunningState
        {
            get
            {
                return _currentItemTimerRunningState;
            }

            private set
            {
                if (_currentItemTimerRunningState == value) return;

                _currentItemTimerRunningState = value;
                NotifyPropertyChanged("CurrentItemTimerRunningState");
            }
        }

        [FAAttribute("", true)]
        public string Name
        {
            get;
            set;
        }

        private DateTime _startedTime;
        [FAAttribute("")]
        public DateTime StartedTime
        {
            get { return _startedTime; }
            set
            {
                _startedTime = value;
                NotifyPropertyChanged("StartedTime");
            }
        }

        private DateTime _terminatedTime;
        [FAAttribute("")]
        public DateTime TerminatedTime
        {
            get { return _terminatedTime; }
            set
            {
                _terminatedTime = value;
                NotifyPropertyChanged("TerminatedTime");
            }
        }

        private string _lastErrorMessage;
        [FAAttribute("")]
        public string LastErrorMessage
        {
            get { return _lastErrorMessage; }
            set
            {
                _lastErrorMessage = value;
                NotifyPropertyChanged("LastErrorMessage");
            }
        }

        private bool _result;
        /// <summary>
        /// 결과가 성공인지 실패인지 기록할 수 있는 Flag.
        /// AddItem에서 등록된 메소드에서 사용할 수 있다.
        /// Sequence가 시작될 때 true로 초기화 된다.
        /// 초기화 외에는 FASequence 내부에서 사용하는 곳은 없다.
        /// </summary>
        [FAAttribute("")]
        public bool Result
        {
            get { return _result; }
            set
            {
                if (_result == value) return;
                _result = value;
                NotifyPropertyChanged("Result");
            }
        }

        public object Tag { get; set; }

        public FASequence(FASequenceManager aSequenceManager)
        {
            FAConditionalSequenceItem startItem = new FAConditionalSequenceItem(this, StartSequence);
            FAConditionalSequenceItem terminateItem = new FAConditionalSequenceItem(this, TerminateSequence);
            _sequenceList.Add(startItem);
            _sequenceList.Add(terminateItem);
            CurrentItem = _sequenceList[0];
            aSequenceManager.Add(this);
            SequenceManager = aSequenceManager;
        }

        [FAAttribute("")]
        public SequenceState State
        {
            get
            {
                return _state;
            }

            private set
            {
                if (_state == value) return;

                _state = value;
                NotifyPropertyChanged("State");         
            }
        }        

        [FAAttribute("")]
        public int CurrentIndex
        {
            get
            {
                return _sequenceList.IndexOf(CurrentItem);
            }
        }

        [FAAttribute("")]
        public string CurrentStepName
        {
            get
            {
                foreach (KeyValuePair<string, StepInfo> item in _steps)
                {
                    if (item.Value.StepIndex == CurrentIndex)
                        return item.Key;
                }
               
                return "";
            }
        }

        [FAAttribute("")]
        public int Length
        {
            get
            {
                return _sequenceList.Count();
            }
        }

        public Dictionary<string, StepInfo> Steps
        {
            get
            {
                return _steps;
            }
        }        

        public void AddRunningInterlock(Func<FASequence, bool> method)
        {
            _runningInterlockList.Add(method);
        }

        public void AddWatcher(Action method)
        {
            _watcherList.Add(method);
        }

        public int AddAtomicItem(Action<FASequence, TimeSpan> actor)
        {
            FAConditionalSequenceItem item = new FAConditionalSequenceItem(this, actor);
            item.Atomic = true;
            _sequenceList.Insert(_sequenceList.Count - 1, item);
            return _sequenceList.Count - 2;
        }

        public int AddItem(Action<FASequence, TimeSpan> actor)
        {
            FAConditionalSequenceItem item = new FAConditionalSequenceItem(this, actor);
            _sequenceList.Insert(_sequenceList.Count - 1, item);
            return _sequenceList.Count - 2;
        }

        public int AddItem(Action<object> actor)
        {
            FAAutoNextSequenceItem item = new FAAutoNextSequenceItem(this, actor);
            _sequenceList.Insert(_sequenceList.Count - 1, item);     
            return _sequenceList.Count - 2;
        }

        public int AddItem(params FASequence[] sequences)
        {
            List<FASequenceAtomicInfo> list = new List<FASequenceAtomicInfo>();
            foreach (var seq in sequences)
            {
                FASequenceAtomicInfo seqAtomicInfo = new FASequenceAtomicInfo(seq, false);
                list.Add(seqAtomicInfo);
            }

            FASequenceCombinationSequenceItem item = new FASequenceCombinationSequenceItem(this, list.ToArray());
            _sequenceList.Insert(_sequenceList.Count - 1, item);
            return _sequenceList.Count - 2;
        }

        public int AddItem(params FASequenceAtomicInfo[] sequences)
        {
            FASequenceCombinationSequenceItem item = new FASequenceCombinationSequenceItem(this, sequences);
            _sequenceList.Insert(_sequenceList.Count - 1, item);
            return _sequenceList.Count - 2;
        }

        public int AddItem(Utility.FATime time)
        {
            Utility.FATime timeRef = time;

            FAConditionalSequenceItem item =
                new FAConditionalSequenceItem(this, 
                    delegate(FASequence actor, TimeSpan t)
                    {
                        if (time.Time < t)
                            actor.NextStep();
                    });
            _sequenceList.Insert(_sequenceList.Count - 1, item);
            return _sequenceList.Count - 2;
        }

        public int AddItem(string stepName)
        {
            string StepNameClone = stepName;
            FAConditionalSequenceItem item =
                new FAConditionalSequenceItem(this, 
                    delegate(FASequence actor, TimeSpan t)
                    {
                        actor.NextStep(StepNameClone);
                    });
            _sequenceList.Insert(_sequenceList.Count - 1, item);
            return _sequenceList.Count - 2;
        }

        public int AddTerminate()
        {
            return AddItem(TerminateSequence);
        }

        [FAAttribute("")]
        public void Start()
        {
            Atomic = false;
            Start(null);
        }

        [FAAttribute("")]
        public void AtomicStart()
        {
            Atomic = true;
            Start(this);
        }

        private bool Start(FASequence sender)
        {
            Caller = sender;

            if (State == SequenceState.Terminated || State == SequenceState.Aborted)
                State = SequenceState.Available;

            if (State == SequenceState.Available)
            {
                if (OnStart != null)
                    OnStart(this, EventArgs.Empty);

                Result = true;
                State = SequenceState.Running;
                CurrentItem = _sequenceList[0];
                CurrentItem.ClearState();
                _stepStopWatch.Restart();
                return true;
            }

            return false;
        }

        public void Execute()
        {
            foreach (var method in _watcherList)
            {
                if (method != null)
                {
                    try
                    {
                        method();
                    }
                    catch (Exception e)
                    {
                        LastErrorMessage = e.Message;
                    }
                }
            }

            if (State == SequenceState.Running)
            {
                if (IsInterlock(this) == false)
                {
                    CurrentItem.Execute(this);
                }
            }
            else if (State == SequenceState.Suspending)
            {
                if (CurrentItem.Atomic == true)
                    CurrentItem.Execute(this);
                else if (CurrentItem.IsSuspended())
                {
                    State = SequenceState.Suspended;
                    if (OnSuspended != null)
                        OnSuspended(this, EventArgs.Empty);
                }
            }
            else if (State == SequenceState.Alarm)
            {
                if (CurrentItem.IsSuspended())
                {
                    State = SequenceState.Suspended;
                    if (OnSuspended != null)
                        OnSuspended(this, EventArgs.Empty);
                }
            }
            else if (IsStartable())
            {
                if (_standbyQueue.Count > 0)
                {
                    Action obj = _standbyQueue.Dequeue();
                    if (obj != null)
                    {
                        try
                        {
                            obj();
                        }
                        catch
                        {
                        }
                    }

                    Start();
                }
            }

            if (CurrentItem != null)
            {
                CurrentItemTime = CurrentItem.Time;
                CurrentItemTimerRunningState = CurrentItem.TimerRunningState;
            }

            StepElapsed = _stepStopWatch.Elapsed;
        }

        [FAAttribute("")]
        public void Stop()
        {
            if (OnStop != null)
                OnStop(this, EventArgs.Empty);

            State = SequenceState.Aborted;
            CurrentItem.Stop();
            CurrentItem.ClearState();
            CurrentItem = _sequenceList[0];
            _stepStopWatch.Stop();
        }

        public void SetAlarm()
        {
            if (State == SequenceState.Running)
            {
                if (OnSuspending != null)
                    OnSuspending(this, EventArgs.Empty);

                var caller = GetAtomicCaller(this);
                if (caller != null)
                {
                    caller.ForceSuspend();
                }
                else
                {

//jbpark_2019.10.10 Atomic알람발생시 스택오버플로우 발생
                    var backgrondCaller = GetBackgroundCaller(this);
                    if (backgrondCaller != null)
                    {
                        backgrondCaller.CurrentItem.Suspend();
                    }
                }

                CurrentItem.SetAlarm();
            }

            State = SequenceState.Alarm;
        }

        [FAAttribute("")]
        public void Suspend()
        {
            if (IsBackground)
                return;

            if (Atomic)
                return;

            if (State == SequenceState.Running)
            {
                if (OnSuspending != null)
                    OnSuspending(this, EventArgs.Empty);

                State = SequenceState.Suspending;
                CurrentItem.Suspend();
            }
        }

        [FAAttribute("")]
        public void ForceSuspend()
        {
            if (State == SequenceState.Running)
            {
                if (OnSuspending != null)
                    OnSuspending(this, EventArgs.Empty);

                State = SequenceState.Suspending;
                CurrentItem.Suspend();
            }
        }

        [FAAttribute("")]
        public void PreSuspend()
        {
            OnPreSuspend(this, EventArgs.Empty);
        }

        [FAAttribute("")]
        public void Resume()
        {
            if (State == SequenceState.Suspended)
            {
                if (OnResume != null)
                    OnResume(this, EventArgs.Empty);

                CurrentItem.Resume();
                State = SequenceState.Running;
            }
        }

        public void ClearState()
        {
            State = SequenceState.Available;
            if (CurrentItem != null)
            {
                CurrentItem.Stop();
                CurrentItem.ClearState();
                CurrentItem = _sequenceList[0];
                _stepStopWatch.Reset();
            }

            _standbyQueue.Clear();            
        }

        public void Retry()
        {
            CurrentItem = _sequenceList[0];
        }

        public void NextStep()
        {            
            CurrentItem.ClearState();
            CurrentItem = _sequenceList[CurrentIndex + 1];
            CurrentItem.ClearState();
        }

        public void NextStep(string stepName)
        {
            if (Steps.ContainsKey(stepName))
            {
                var nextItem = _sequenceList[Steps[stepName].StepIndex];

                if (nextItem != CurrentItem)
                    _stepStopWatch.Restart();

                CurrentItem.ClearState();
                CurrentItem = nextItem;
                CurrentItem.ClearState();
            }
            else
            {
                LastErrorMessage = string.Format("Not exist step. {0}", stepName);
            }
        }

        public void NextTerminate()
        {
            CurrentItem.ClearState();
            CurrentItem = _sequenceList[_sequenceList.Count - 1];
            CurrentItem.ClearState();

            _stepStopWatch.Stop();
        }

        public bool IsRestartable()
        {
            if (this.State == SequenceState.Running ||
                this.State == SequenceState.Suspending ||
                this.State == SequenceState.Starting ||
                this.State == SequenceState.Alarm) return false;
            else return true;
        }

        public bool IsStartable()
        {
            if (this.State == SequenceState.Terminated ||
                this.State == SequenceState.Available)
                return true;
            else
                return false;
        }

        private void StartSequence(FASequence actor, TimeSpan time)
        {
            StartedTime = DateTime.Now;
            actor.NextStep();
        }

        private void TerminateSequence(FASequence actor, TimeSpan time)
        {
            TerminatedTime = DateTime.Now;

            if (OnTerminate != null)
                OnTerminate(actor, EventArgs.Empty);

            CurrentItem.ClearState();
            State = SequenceState.Terminated;
            actor.NextTerminate();
        }

        public void AddStandby(Action obj)
        {
            _standbyQueue.Enqueue(obj);
        }

        private bool IsInterlock(FASequence actor)
        {
            foreach (Func<FASequence, bool> item in _runningInterlockList)
            {
                if (item(actor) == true)
                    return true;
            }

            return false;
        }

        public FASequence GetAtomicCaller(FASequence aSeq)
        {
            if (aSeq.Caller is FASequence)
            {
                if (aSeq.Caller.Atomic == true)
                    return aSeq.Caller;
                else
                    return GetAtomicCaller(aSeq.Caller);
            }
            else
                return null;
        }

        public FASequence GetBackgroundCaller(FASequence aSeq)
        {
            if (aSeq.Caller is FASequence)
            {
                if (aSeq.Caller.IsBackground == true)
                    return aSeq.Caller;
                else
                    return GetBackgroundCaller(aSeq.Caller);
            }
            else
                return null;
        }
    }
}
