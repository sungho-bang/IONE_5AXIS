using System;
using System.Globalization;
using System.Linq;
using FAFramework.VT3500.JobInfo;
using FALibrary.Sequence;

namespace FAFramework.VT3500.Modules
{
    internal static class IMarkUsageSelection
    {
        public static string GetBlockReason(SubEquipment equipment, FALotJobInfo selectedJob)
        {
            if (equipment?.FrontModule?.TapeLoadingServo == null || equipment.MainLoopModule?.MoveJobInfo == null)
                return "설비 설정을 불러온 후 변경할 수 있습니다.";
            if (selectedJob?.MoveJobInfo == null || string.IsNullOrWhiteSpace(selectedJob.Name) ||
                equipment.JobManagerInstance?.LotJobInstance?.LotJobInfoList.Contains(selectedJob) != true ||
                selectedJob.Name != equipment.MainLoopModule.SelectJob)
                return "메인 RECIPE에서 적용할 JOB을 선택하세요.";
            return null;
        }

        public static string GetRecipeBlockReason(SubEquipment equipment, FALotJobInfo selectedJob)
        {
            if (equipment?.FrontModule?.TapeLoadingServo == null || equipment.MainLoopModule?.MoveJobInfo == null)
                return "설비 설정을 불러온 후 변경할 수 있습니다.";
            if (selectedJob?.MoveJobInfo == null || string.IsNullOrWhiteSpace(selectedJob.Name) ||
                equipment.JobManagerInstance?.LotJobInstance?.LotJobInfoList.Contains(selectedJob) != true)
                return "메인 RECIPE에서 적용할 JOB을 선택하세요.";
            if (equipment.StateStop == null || equipment.State != equipment.StateStop)
                return "설비 정지 상태에서만 I-Mark 사용 여부를 변경할 수 있습니다.";

            var rear = equipment.RearModule;
            var front = equipment.FrontModule;
            var servo = front.TapeLoadingServo;
            var main = equipment.MainLoopModule;
            var sequences = new[]
            {
                main.MainLoop, main.InitializeMachine, main.LoadRecipe,
                front.MainLoop, front.Initialize, front.MainAutomicLoop, front.WorkFirstBandMoveLoading,
                front.WorkLoopBandMoveLoading, front.TapeLoadingMoveWithIMark, front.TapeMovePickCylinder,
                front.TapeMovePlaceCylinder, front.WorkManualLoading, front.WorkMoveWithOutTomson, front.WorkManualOnceOneCycle,
                front.TapeLoadGrip?.Grip?.Sequence, front.TapeLoadGrip?.Release?.Sequence,
                front.TapeHoldGrip?.Grip?.Sequence, front.TapeHoldGrip?.Release?.Sequence,
                rear?.MainLoop, rear?.Initialize, rear?.WorkLoading, rear?.OnceCycleStart, rear?.OnceCycleEnd,
                rear?.MovingRoller, rear?.ManualWorkLoading, rear?.WorkManualLoading,
                rear?.SearchingImark, rear?.WorkRearPullManual, rear?.WorkBandCutting,
                servo.MoveHomePos?.Sequence,
                servo.MoveHome?.Sequence, servo.MoveToPos?.Sequence, servo.MoveVelocity?.Sequence,
                servo.MoveTapeLoadingPos?.Sequence, servo.MoveTapeLoadingSlowPos?.Sequence, servo.Stop?.Sequence
            };
            if (servo.RunFlag || sequences.Any(IsActive))
                return "진행 중이거나 일시정지된 동작을 종료한 후 변경하세요.";
            return null;
        }

        private static bool IsActive(FASequence sequence)
        {
            return sequence != null && sequence.State != SequenceState.Available &&
                sequence.State != SequenceState.Terminated && sequence.State != SequenceState.Aborted;
        }

        public static bool TryLoadRecipe(SubEquipment equipment, FALotJobInfo selectedJob, out string error)
        {
            error = GetRecipeBlockReason(equipment, selectedJob);
            if (error != null)
            {
                WriteLog(equipment, "RECIPE_FRONT_IMARK_REJECTED", "Job=" + selectedJob?.Name + ";Reason=" + error);
                return false;
            }
            MoveJobInfo job;
            var previousJob = equipment.FrontModule.AppliedFrontIMarkJobName;
            var previousUse = equipment.FrontModule.UseFrontIMark;
            try
            {
                var source = selectedJob.MoveJobInfo;
                job = new MoveJobInfo
                {
                    UseFrontIMark = source.UseFrontIMark,
                    FeedingPitch = source.FeedingPitch,
                    FeedingSpeed = source.FeedingSpeed,
                    FrontIMarkSlowDistance = source.FrontIMarkSlowDistance,
                    FrontIMarkSensorOffset = source.FrontIMarkSensorOffset
                };
                equipment.FrontModule.ApplyFrontIMarkJob(job, selectedJob.Name);
            }
            catch (Exception ex)
            {
                error = "FRONT I-Mark 설정을 확인하세요: " + ex.Message;
                WriteLog(equipment, "RECIPE_FRONT_IMARK_REJECTED", "Job=" + selectedJob.Name + ";Reason=" + error);
                return false;
            }

            // Apply only the FRONT feed settings; do not mark the whole machine initialized or loaded.
            var runtime = equipment.MainLoopModule.MoveJobInfo;
            runtime.UseFrontIMark = job.UseFrontIMark;
            runtime.FeedingPitch = job.FeedingPitch;
            runtime.FeedingSpeed = job.FeedingSpeed;
            runtime.FrontIMarkSlowDistance = job.FrontIMarkSlowDistance;
            runtime.FrontIMarkSensorOffset = job.FrontIMarkSensorOffset;
            equipment.MainLoopModule.SelectJob = selectedJob.Name;
            WriteLog(equipment, "RECIPE_FRONT_IMARK_APPLIED", string.Format(CultureInfo.InvariantCulture,
                "Job={0};PreviousJob={1};PreviousUse={2};UseFrontIMark={3};Pitch={4};FeedingSpeed={5};SlowDistance={6};Offset={7};Axis=FRONT.TapeLoadingServo;Input=X1135;MotionCommand=None",
                selectedJob.Name, previousJob, previousUse, job.UseFrontIMark, job.FeedingPitch,
                job.FeedingSpeed, job.FrontIMarkSlowDistance, job.FrontIMarkSensorOffset));
            return true;
        }

        public static bool TryApply(SubEquipment equipment, FALotJobInfo selectedJob, bool useIMark, out string error)
        {
            error = GetBlockReason(equipment, selectedJob);
            if (error != null)
            {
                WriteLog(equipment, "MAIN_USAGE_CHANGE_REJECTED", "Reason=" + error);
                return false;
            }

            try
            {
                var source = selectedJob.MoveJobInfo;
                var settings = new MoveJobInfo
                {
                    UseFrontIMark = useIMark, FeedingPitch = source.FeedingPitch, FeedingSpeed = source.FeedingSpeed,
                    FrontIMarkSlowDistance = source.FrontIMarkSlowDistance, FrontIMarkSensorOffset = source.FrontIMarkSensorOffset
                };
                equipment.FrontModule.QueueFrontIMarkUsage(settings, selectedJob.Name);
            }
            catch (Exception ex)
            {
                error = "FRONT I-Mark 설정을 확인하세요: " + ex.Message;
                WriteLog(equipment, "MAIN_USAGE_CHANGE_REJECTED", "Reason=" + error);
                return false;
            }
            selectedJob.MoveJobInfo.UseFrontIMark = useIMark;
            string applyError;
            equipment.FrontModule.TryApplyPendingFrontIMarkUsage(false, out applyError);
            return true;
        }

        private static void WriteLog(SubEquipment equipment, string eventName, string detail)
        {
            if (equipment == null) return;
            try
            {
                Manager.LogManager.Instance.WriteIMarkLog(equipment,
                    "Schema=IMarkV2;Source=MainUI;Event=" + eventName + ";" + detail);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }
    }
}
