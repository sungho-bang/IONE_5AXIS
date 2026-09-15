using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using FALibrary.Sequence;

namespace FAFramework.VT3500.Modules
{
    // Observes existing steps; never advances a sequence or commands hardware.
    internal sealed class IMarkSequenceDiagnostics
    {
        private sealed class RepeatedEvent
        {
            public long Count;
            public long LastWritten = -5000;
        }

        private readonly FASequence _sequence;
        private readonly Func<string> _snapshot;
        private readonly Func<string> _configuration;
        private readonly Func<string> _input;
        private readonly Func<string> _motion;
        private readonly Action<string> _write;
        private readonly Stopwatch _elapsed = new Stopwatch();
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, RepeatedEvent> _repeated = new Dictionary<string, RepeatedEvent>();
        private readonly Dictionary<int, string> _steps = new Dictionary<int, string>();
        private string _runId;
        private string _lastInput;
        private string _lastMotion;
        private string _lastError;
        private SequenceState _lastState;
        private long _lastHeartbeat;
        private bool _active;

        public IMarkSequenceDiagnostics(FASequence sequence, Func<string> snapshot,
            Func<string> configuration, Func<string> input, Func<string> motion, Action<string> write)
        {
            _sequence = sequence;
            _snapshot = snapshot;
            _configuration = configuration;
            _input = input;
            _motion = motion;
            _write = write;
            sequence.OnStart += delegate { Guard(Start); };
            sequence.OnChangeStep += delegate
            {
                Repeat("STEP_ENTER", "StepIndex=" + sequence.CurrentIndex);
            };
            sequence.OnPreSuspend += delegate { Record("SUSPEND_REQUESTED"); };
            sequence.OnSuspending += delegate { Record("SUSPENDING"); };
            sequence.OnSuspended += delegate { Record("SUSPENDED"); };
            sequence.OnResume += delegate { Record("RESUMED"); };
            sequence.OnStop += delegate { Guard(() => End("ABORTED")); };
            sequence.OnTerminate += delegate { Guard(() => End("TERMINATED")); };
            sequence.AddWatcher(() => Guard(Observe));
        }

        public int TrackStep(int index, string action)
        {
            _steps[index] = action;
            return index;
        }

        private void Start()
        {
            _runId = Guid.NewGuid().ToString("N");
            _repeated.Clear();
            _elapsed.Restart();
            _lastHeartbeat = 0;
            _active = true;
            _lastInput = null;
            _lastMotion = null;
            _lastError = _sequence.LastErrorMessage;
            _lastState = _sequence.State;
            Record("START", _configuration());
        }

        private void End(string eventName)
        {
            if (!_active) return;
            foreach (var pair in _repeated)
                if (pair.Value.Count > 1)
                    Record("REPEAT_TOTAL", "Key=" + pair.Key + ";Count=" + pair.Value.Count);
            Record(eventName, "Result=" + _sequence.Result + ";LastError=" + _sequence.LastErrorMessage);
            _active = false;
            _elapsed.Stop();
        }

        private void Observe()
        {
            if (!_active) return;
            if (_sequence.State == SequenceState.Available || _sequence.State == SequenceState.Aborted ||
                _sequence.State == SequenceState.Terminated)
            {
                End("STATE_RESET_OR_ENDED");
                return;
            }
            var input = _input();
            if (input != _lastInput)
            {
                Record(_lastInput == null ? "INPUT_INITIAL" : "INPUT_CHANGED",
                    "Previous=" + (_lastInput ?? "UNOBSERVED") + ";Observed=" + input);
                _lastInput = input;
            }
            var motion = _motion();
            if (motion != _lastMotion)
            {
                Record(_lastMotion == null ? "MOTION_INITIAL" : "MOTION_CHANGED",
                    "PreviousMotion=" + (_lastMotion ?? "UNOBSERVED") + ";ObservedMotion=" + motion);
                _lastMotion = motion;
            }
            if (_lastState != _sequence.State)
            {
                Record("STATE_CHANGED", "Previous=" + _lastState + ";Observed=" + _sequence.State);
                _lastState = _sequence.State;
            }
            if (_lastError != _sequence.LastErrorMessage)
            {
                _lastError = _sequence.LastErrorMessage;
                Record("SEQUENCE_ERROR_CHANGED", "LastError=" + _lastError);
            }
            if (_elapsed.ElapsedMilliseconds - _lastHeartbeat >= 5000)
            {
                Record("HEARTBEAT", "StepElapsedMs=" + _sequence.StepElapsed.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture));
                _lastHeartbeat = _elapsed.ElapsedMilliseconds;
            }
        }

        // Tight manual search loops retain exact counts without logging every scan.
        public void Repeat(string eventName, string key, string detail = "")
        {
            Guard(() =>
            {
                if (!_active) return;
                var fullKey = eventName + ":" + key;
                RepeatedEvent entry;
                if (!_repeated.TryGetValue(fullKey, out entry))
                {
                    entry = new RepeatedEvent();
                    _repeated.Add(fullKey, entry);
                }
                entry.Count++;
                if (_elapsed.ElapsedMilliseconds - entry.LastWritten < 5000) return;
                entry.LastWritten = _elapsed.ElapsedMilliseconds;
                Record(eventName, "Key=" + key + ";Count=" + entry.Count + ";" + detail);
            });
        }

        public void Record(string eventName, string detail = "")
        {
            Guard(() =>
            {
                if (!_active) return;
                string action;
                _steps.TryGetValue(_sequence.CurrentIndex, out action);
                _write(string.Format(CultureInfo.InvariantCulture,
                    "Schema=IMarkV2;RunId={0};Seq={1};Caller={2};Event={3};ElapsedMs={4};Step={5};StepName={6};Action={7};SeqState={8};{9};{10}",
                    _runId, _sequence.Name, _sequence.Caller == null ? "NONE" : _sequence.Caller.Name,
                    eventName, _elapsed.ElapsedMilliseconds, _sequence.CurrentIndex,
                    _sequence.CurrentStepName, action ?? "", _sequence.State, _snapshot(), detail));
            });
        }

        private void Guard(Action action)
        {
            try { lock (_syncRoot) { action(); } }
            catch (Exception e) { Trace.WriteLine("I-Mark diagnostics failed: " + e); }
        }
    }
}
