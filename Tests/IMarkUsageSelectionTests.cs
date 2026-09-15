using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Linq;
using FAFramework.Equipment;
using FAFramework.VT3500;
using FAFramework.VT3500.ExtendedParts;
using FAFramework.VT3500.GUI;
using FAFramework.VT3500.JobInfo;
using FAFramework.VT3500.Modules;
using FALibrary.Sequence;

internal static class IMarkUsageSelectionTests
{
    private static T WithoutConstructor<T>() { return (T)FormatterServices.GetUninitializedObject(typeof(T)); }
    private static void SetState(SubEquipment equipment, EquipmentState state)
    {
        typeof(EquipmentBase).GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(equipment, state);
    }
    private static void Refresh(MainStatusControl control)
    {
        typeof(MainStatusControl).GetMethod("RefreshIMarkUsage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(control, null);
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
    }

    public static void Run(Action<bool, string> check)
    {
        var scheduler = new FASequenceManager();
        var equipment = WithoutConstructor<SubEquipment>();
        equipment.Name = "IMarkUsageTest";
        equipment.RearModule = WithoutConstructor<FARearLoadingModule>();
        equipment.RearModule.UseIMark = true;
        equipment.FrontModule = WithoutConstructor<FAFrontLoadingModule>();
        equipment.FrontModule.Equipment = equipment;
        equipment.FrontModule.Name = "FrontSelectionTest";
        equipment.FrontModule.TapeLoadingServo = new FATapeLoadingServo(scheduler)
        {
            SimulationMode = true, AxisNo = 1,
            TapeLoadingPos = new FALibrary.Part.MMCPart.FAMMCPosition(),
            TapeLoadingSlowPos = new FALibrary.Part.MMCPart.FAMMCPosition { DriveSpeed = 50 }
        };
        equipment.MainLoopModule = WithoutConstructor<FAMainLoopModule>();
        equipment.MainLoopModule.MoveJobInfo = new MoveJobInfo { FrontIMarkSensorOffset = 2.5, FeedingPitch = 150, FeedingSpeed = 500 };
        equipment.JobManagerInstance = new JobManager { JobFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UsageJobs") };
        Directory.CreateDirectory(equipment.JobManagerInstance.JobFolderPath);
        var job = new FALotJobInfo { Name = "10x15", MoveJobInfo = new MoveJobInfo { FrontIMarkSensorOffset = 2.5, FeedingPitch = 150, FeedingSpeed = 500 } };
        var otherJob = new FALotJobInfo { Name = "Other", MoveJobInfo = new MoveJobInfo
            { UseFrontIMark = true, FeedingPitch = 200, FeedingSpeed = 350, FrontIMarkSlowDistance = 20, FrontIMarkSensorOffset = 4 } };
        equipment.JobManagerInstance.LotJobInstance.LotJobInfoList.Add(job);
        equipment.JobManagerInstance.LotJobInstance.LotJobInfoList.Add(otherJob);
        equipment.MainLoopModule.SelectJob = job.Name;
        typeof(FAMainLoopModule).GetProperty("LoadedJobName").SetValue(equipment.MainLoopModule, job.Name, null);
        var stopState = new EquipmentState(equipment);
        typeof(EquipmentBase).GetProperty("StateStop").SetValue(equipment, stopState, null);
        SetState(equipment, stopState);
        string error;
        check(IMarkUsageSelection.GetBlockReason(null, null) != null, "Main usage selector fails closed before equipment loads");
        typeof(FAMainLoopModule).GetProperty("LoadedJobName").SetValue(equipment.MainLoopModule, null, null);
        check(IMarkUsageSelection.TryApply(equipment, job, true, out error) &&
            equipment.FrontModule.UseFrontIMark && equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 140,
            "A selected recipe can enable I-Mark at rest without initialization, applying a consistent FRONT snapshot");
        IMarkUsageSelection.TryApply(equipment, job, false, out error);
        check(IMarkUsageSelection.TryLoadRecipe(equipment, job, out error) &&
            IMarkUsageSelection.GetBlockReason(equipment, job) == null && equipment.MainLoopModule.LoadedJobName == null,
            "Recipe application enables FRONT I-Mark without initialization or pretending the full JOB was loaded");
        var recovery = typeof(FAFrontLoadingModule).GetField("_iMarkPositioningRecoveryRequired", BindingFlags.Instance | BindingFlags.NonPublic);
        var handled = typeof(FAFrontLoadingModule).GetField("_iMarkSensorOnHandled", BindingFlags.Instance | BindingFlags.NonPublic);
        recovery.SetValue(equipment.FrontModule, true);
        handled.SetValue(equipment.FrontModule, true);
        equipment.MainLoopModule.MoveJobInfo.PackingFeedPitch = 321;
        var idleInitialize = new FASequence(scheduler);
        equipment.MainLoopModule.InitializeMachine = idleInitialize;
        check(IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error) &&
            equipment.MainLoopModule.SelectJob == otherJob.Name && equipment.FrontModule.UseFrontIMark &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 180 &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.DriveSpeed == 350 &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingSlowPos.Position == 200 &&
            equipment.FrontModule.FrontIMarkSensorOffset == 4 &&
            equipment.MainLoopModule.MoveJobInfo.FeedingPitch == 200 &&
            equipment.MainLoopModule.MoveJobInfo.FrontIMarkSlowDistance == 20,
            "Recipe USE applies pitch, fast speed, slow distance and offset together to FRONT only");
        check(!equipment.IsInitializedOk && idleInitialize.State == SequenceState.Available &&
            (bool)recovery.GetValue(equipment.FrontModule) && (bool)handled.GetValue(equipment.FrontModule) &&
            equipment.FrontModule.TapeLoadingServo.CommandPos == 0 && equipment.FrontModule.TapeLoadingServo.ActualPos == 0 &&
            equipment.FrontModule.TapeLoadingServo.MoveToPos.Sequence.State == SequenceState.Available &&
            equipment.FrontModule.TapeLoadingServo.MoveTapeLoadingPos.Sequence.State == SequenceState.Available &&
            equipment.MainLoopModule.MoveJobInfo.PackingFeedPitch == 321 && equipment.RearModule.UseIMark,
            "Recipe apply starts no movement, preserves recovery/input latch and leaves initialization and REAR settings unchanged");
        recovery.SetValue(equipment.FrontModule, false);
        handled.SetValue(equipment.FrontModule, false);
        check(IMarkUsageSelection.TryLoadRecipe(equipment, job, out error) && !equipment.FrontModule.UseFrontIMark &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 150,
            "Recipe NOT USE restores normal full-pitch FRONT feed immediately");
        foreach (double invalidOffset in new[] { -1.0, double.NaN, double.PositiveInfinity })
        {
            typeof(MoveJobInfo).GetField("_frontIMarkSensorOffset", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(otherJob.MoveJobInfo, invalidOffset);
            check(!IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error) &&
                equipment.MainLoopModule.SelectJob == job.Name && equipment.FrontModule.AppliedFrontIMarkJobName == job.Name &&
                equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 150 &&
                equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.DriveSpeed == 500 &&
                equipment.FrontModule.FrontIMarkSensorOffset == 2.5 && !equipment.FrontModule.UseFrontIMark,
                "Invalid recipe offset is rejected before any setting changes: " + invalidOffset);
        }
        otherJob.MoveJobInfo.FrontIMarkSensorOffset = 4;
        otherJob.MoveJobInfo.FeedingSpeed = 0.1;
        check(!IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error) && equipment.MainLoopModule.SelectJob == job.Name,
            "Positive speed rounded to zero cannot enable I-Mark");
        otherJob.MoveJobInfo.FeedingSpeed = 350;
        job.MoveJobInfo.FrontIMarkSlowDistance = 20;
        check(IMarkUsageSelection.TryApply(equipment, job, true, out error) &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 130,
            "Usage selection captures matching FRONT distances together rather than mixing old and new targets");
        job.MoveJobInfo.FrontIMarkSlowDistance = 10;
        check(IMarkUsageSelection.TryApply(equipment, job, true, out error) &&
            equipment.FrontModule.UseFrontIMark && equipment.MainLoopModule.MoveJobInfo.UseFrontIMark && job.MoveJobInfo.UseFrontIMark,
            "Main USE synchronizes runtime, loaded JOB and selected JOB");
        check(IMarkUsageSelection.TryApply(equipment, job, false, out error) &&
            !equipment.FrontModule.UseFrontIMark && !equipment.MainLoopModule.MoveJobInfo.UseFrontIMark &&
            !job.MoveJobInfo.UseFrontIMark && otherJob.MoveJobInfo.UseFrontIMark && job.MoveJobInfo.FrontIMarkSensorOffset == 2.5,
            "Main NOT USE synchronizes only the selected usage option, preserving offset and other JOBs");
        check(equipment.RearModule.UseIMark, "Main FRONT mode selection preserves the runtime REAR BlackMark mode");
        check(!File.Exists(Path.Combine(equipment.JobManagerInstance.JobFolderPath, "jobfile.xml")),
            "Main selection does not silently save other pending configuration edits");
        equipment.JobManagerInstance.Save();
        var reload = new JobManager { JobFolderPath = equipment.JobManagerInstance.JobFolderPath };
        reload.Load();
        check(!reload.LotJobInstance.LotJobInfoList[0].MoveJobInfo.UseFrontIMark,
            "Existing explicit JOB Save/Load preserves the main-screen selection");
        SetState(equipment, new EquipmentState(equipment));
        check(IMarkUsageSelection.TryApply(equipment, job, true, out error) && job.MoveJobInfo.UseFrontIMark &&
            !equipment.FrontModule.UseFrontIMark && equipment.FrontModule.PendingFrontIMarkUse == true,
            "Run/initialize/non-stop state accepts the request while preserving current motion settings");
        check(!IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error) && equipment.MainLoopModule.SelectJob == job.Name,
            "Run/initialize/non-stop state rejects recipe application without changing selection");
        SetState(equipment, stopState);
        check(equipment.FrontModule.TryApplyPendingFrontIMarkUsage(false, out error) && equipment.FrontModule.UseFrontIMark,
            "A queued request applies when equipment becomes stopped and idle");
        IMarkUsageSelection.TryApply(equipment, job, false, out error);
        check(!IMarkUsageSelection.TryApply(equipment, null, true, out error) &&
            !IMarkUsageSelection.TryApply(equipment, otherJob, true, out error),
            "Missing or mismatched selected JOB cannot update runtime");
        var impostor = new FALotJobInfo { Name = job.Name };
        check(!IMarkUsageSelection.TryApply(equipment, impostor, true, out error), "A detached JOB object cannot update another recipe");
        check(!IMarkUsageSelection.TryLoadRecipe(equipment, null, out error) &&
            !IMarkUsageSelection.TryLoadRecipe(equipment, impostor, out error), "Recipe load rejects missing or detached JOBs");
        var busy = new FASequence(scheduler);
        busy.AddItem((actor, time) => { });
        equipment.FrontModule.TapeLoadingMoveWithIMark = busy;
        busy.Start();
        check(IMarkUsageSelection.TryApply(equipment, job, true, out error) && !equipment.FrontModule.UseFrontIMark &&
            equipment.FrontModule.PendingFrontIMarkUse == true, "Running manual feed accepts a pending selection without changing the active mode");
        check(!IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error), "Running manual feed blocks recipe load");
        busy.PreSuspend();
        busy.Suspend();
        for (int i = 0; i < 10; i++) scheduler.Run();
        check(busy.State == SequenceState.Suspended && IMarkUsageSelection.TryApply(equipment, job, false, out error) &&
            equipment.FrontModule.PendingFrontIMarkUse == false && !equipment.FrontModule.UseFrontIMark,
            "Suspended feed accepts a replacement request but Resume retains its current settings");
        check(!IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error), "Suspended feed blocks recipe load without changing resume settings");
        busy.Stop();
        check(IMarkUsageSelection.GetBlockReason(equipment, job) == null, "Selection becomes available after the active sequence ends");
        var servo = equipment.FrontModule.TapeLoadingServo;
        typeof(FALibrary.Part.MMCPart.FAMMCPart).GetProperty("RunFlag").SetValue(servo, true, null);
        check(IMarkUsageSelection.TryApply(equipment, job, true, out error) && !equipment.FrontModule.UseFrontIMark &&
            equipment.FrontModule.PendingFrontIMarkUse == true,
            "Moving FRONT feedback defers application without disabling mode selection");
        check(!IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error), "Moving FRONT feedback blocks recipe load");
        typeof(FALibrary.Part.MMCPart.FAMMCPart).GetProperty("RunFlag").SetValue(servo, false, null);
        IMarkUsageSelection.TryApply(equipment, job, false, out error);

        SetState(equipment, new EquipmentState(equipment));
        IMarkUsageSelection.TryApply(equipment, job, true, out error);
        var staleInitializeJob = (MoveJobInfo)job.MoveJobInfo.Clone();
        staleInitializeJob.UseFrontIMark = false;
        equipment.FrontModule.ApplyFrontIMarkJob(staleInitializeJob, job.Name);
        check(!equipment.FrontModule.UseFrontIMark && equipment.FrontModule.PendingFrontIMarkUse == true,
            "An older initialization snapshot cannot discard a newer usage request");
        SetState(equipment, stopState);
        check(IMarkUsageSelection.TryLoadRecipe(equipment, otherJob, out error) && !equipment.FrontModule.PendingFrontIMarkUse.HasValue,
            "Loading another recipe cancels pending settings from the previous recipe");
        IMarkUsageSelection.TryLoadRecipe(equipment, job, out error);
        IMarkUsageSelection.TryApply(equipment, job, false, out error);
        CheckFeedBoundary(equipment, job, stopState, check);
        SetState(equipment, new EquipmentState(equipment));
        IMarkUsageSelection.TryApply(equipment, job, true, out error);
        var originalLimit = servo.TapeLoadingSlowPos.UpLimit;
        servo.TapeLoadingSlowPos.UpLimit = 100;
        SetState(equipment, stopState);
        check(!equipment.FrontModule.TryApplyPendingFrontIMarkUsage(false, out error) && error != null &&
            equipment.FrontModule.PendingFrontIMarkError != null && !equipment.FrontModule.UseFrontIMark &&
            servo.TapeLoadingPos.Position == 150,
            "Pending settings are revalidated at application; changed limits fail without partial mutation");
        servo.TapeLoadingSlowPos.UpLimit = originalLimit;
        check(equipment.FrontModule.TryApplyPendingFrontIMarkUsage(false, out error) &&
            equipment.FrontModule.UseFrontIMark && equipment.FrontModule.PendingFrontIMarkError == null,
            "Pending settings can apply after validation succeeds without any motion command");
        IMarkUsageSelection.TryApply(equipment, job, false, out error);

        if (Application.Current == null) new Application();
        XNamespace p = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
        var app = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppStyles.xml"));
        var resources = new XElement(p + "ResourceDictionary", new XAttribute(XNamespace.Xmlns + "x", x.NamespaceName),
            app.Descendants(p + "Style").Where(s => (string)s.Attribute(x + "Key") == "OnOffSensorLabel2" || (string)s.Attribute(x + "Key") == "LEDButton"));
        Application.Current.Resources.MergedDictionaries.Add((ResourceDictionary)XamlReader.Parse(resources.ToString()));
        var control = new MainStatusControl { EquipmentInstance = equipment, JobManagerInstance = equipment.JobManagerInstance };
        var recipe = (ComboBox)control.FindName("receipe");
        control.Measure(new Size(1500, 1000));
        control.Arrange(new Rect(0, 0, 1500, 1000));
        control.UpdateLayout();
        recipe.SelectedItem = job;
        Refresh(control);
        var use = (RadioButton)control.FindName("IMarkUseRadio");
        var notUse = (RadioButton)control.FindName("IMarkNotUseRadio");
        var apply = (Button)control.FindName("RecipeApplyButton");
        check(notUse.IsChecked == true && use.IsChecked == false && use.IsEnabled,
            "Main radio buttons display actual runtime NOT USE and allow editing at rest");
        recipe.SelectedItem = otherJob;
        Refresh(control);
        check(use.IsChecked == true && use.IsEnabled && equipment.FrontModule.FrontIMarkSensorOffset == 4 &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 180 && equipment.MainLoopModule.LoadedJobName == null,
            "Real RECIPE selection immediately applies saved I-Mark USE without initialization");
        otherJob.MoveJobInfo.UseFrontIMark = false;
        otherJob.MoveJobInfo.FrontIMarkSensorOffset = 6;
        Refresh(control);
        check(use.IsEnabled && apply.IsEnabled && equipment.FrontModule.UseFrontIMark,
            "Pending JOB edits do not disable usage selection and do not silently change runtime");
        apply.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Refresh(control);
        check(notUse.IsChecked == true && use.IsEnabled && equipment.FrontModule.FrontIMarkSensorOffset == 6 &&
            equipment.FrontModule.TapeLoadingServo.TapeLoadingPos.Position == 200,
            "Real apply button reloads edits to the same recipe without reselection or motion");
        recipe.SelectedItem = job;
        Refresh(control);
        use.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Refresh(control);
        equipment.JobManagerInstance.Save();
        reload.Load();
        check(reload.LotJobInstance.LotJobInfoList[0].MoveJobInfo.UseFrontIMark,
            "USE selected by the real UI also survives explicit Save/Load");
        check(use.IsChecked == true && notUse.IsChecked == false && equipment.FrontModule.UseFrontIMark && job.MoveJobInfo.UseFrontIMark,
            "Actual main radio click selects USE and synchronizes JOB/runtime");
        notUse.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Refresh(control);
        check(!equipment.FrontModule.UseFrontIMark && notUse.IsChecked == true && use.IsChecked == false,
            "Actual main radio click selects NOT USE with exclusive selection");
        busy.Start();
        Refresh(control);
        check(use.IsEnabled && notUse.IsEnabled && !apply.IsEnabled, "Actual UI allows mode selection while feeding but keeps full recipe reapply blocked");
        SetState(equipment, new EquipmentState(equipment));
        use.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Refresh(control);
        check(use.IsChecked == true && !equipment.FrontModule.UseFrontIMark && control.IMarkUsageStatus == "사용 적용 대기",
            "Real UI during initialization/run displays requested USE and pending status separately from actual NOT USE");
        equipment.JobManagerInstance.Save();
        reload.Load();
        check(reload.LotJobInstance.LotJobInfoList[0].MoveJobInfo.UseFrontIMark && !equipment.FrontModule.UseFrontIMark,
            "Explicit Save preserves the requested option even while runtime application is pending");
        var pendingPanel = (Grid)control.FindName("IMarkUsagePanel");
        control.UpdateLayout();
        var pendingPreview = new DrawingVisual();
        using (var drawing = pendingPreview.RenderOpen())
            drawing.DrawRectangle(new VisualBrush(pendingPanel) { Stretch = Stretch.Fill }, null, new Rect(0, 0, 720, 228));
        SaveImage(pendingPreview, 720, 228, "IMark_Main_Pending.png");
        notUse.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        use.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Refresh(control);
        check(equipment.FrontModule.PendingFrontIMarkUse == true && !equipment.FrontModule.UseFrontIMark,
            "Repeated live selections retain only the latest request");
        busy.Stop();
        SetState(equipment, stopState);
        Refresh(control);
        check(equipment.FrontModule.UseFrontIMark && !equipment.FrontModule.PendingFrontIMarkUse.HasValue &&
            control.IMarkUsageStatus == "현재: 사용", "UI refresh applies pending settings after motion ends and clears pending status");
        recipe.SelectedItem = null;
        Refresh(control);
        check(!use.IsEnabled && equipment.MainLoopModule.SelectJob == null,
            "Clearing RECIPE does not throw and disables I-Mark selection");
        recipe.SelectedItem = job;
        Refresh(control);
        use.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Refresh(control);
        var panel = (Grid)control.FindName("IMarkUsagePanel");
        var useBounds = use.TransformToAncestor(panel).TransformBounds(new Rect(use.RenderSize));
        var notUseBounds = notUse.TransformToAncestor(panel).TransformBounds(new Rect(notUse.RenderSize));
        check(useBounds.Left >= 0 && notUseBounds.Right <= panel.ActualWidth && useBounds.Right <= notUseBounds.Left &&
            Math.Max(useBounds.Bottom, notUseBounds.Bottom) <= panel.ActualHeight,
            "Main selector text and radio controls fit their original equipment panel without overlap");
        var status = (TextBlock)control.FindName("IMarkUsageStatusText");
        var statusBounds = status.TransformToAncestor(panel).TransformBounds(new Rect(status.RenderSize));
        check(useBounds.Bottom <= statusBounds.Top && statusBounds.Bottom <= panel.ActualHeight,
            "Pending/current status fits beneath radio controls without overlap");
        var uiXml = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MainStatusSource.xml"));
        check(!uiXml.Descendants(p + "ToggleButton").Any(el => (string)el.Attribute("Checked") == "Check" || (string)el.Attribute("Unchecked") == "UnCheck"),
            "One-cycle headings no longer change I-Mark mode implicitly");
        var recipePanel = (Grid)control.FindName("RecipeSelectionPanel");
        var recipeBounds = recipe.TransformToAncestor(recipePanel).TransformBounds(new Rect(recipe.RenderSize));
        var applyBounds = apply.TransformToAncestor(recipePanel).TransformBounds(new Rect(apply.RenderSize));
        check(recipeBounds.Right <= applyBounds.Left && applyBounds.Right <= recipePanel.ActualWidth,
            "RECIPE dropdown and reapply icon fit the existing main header without overlap");
        SaveImage(control, 1500, 1000, "IMark_Main_Usage.png");
        var recipePreview = new DrawingVisual();
        using (var drawing = recipePreview.RenderOpen())
            drawing.DrawRectangle(new VisualBrush(recipePanel) { Stretch = Stretch.Fill }, null, new Rect(0, 0, 1000, 122));
        SaveImage(recipePreview, 1000, 122, "IMark_Recipe_Apply.png");
        var preview = new DrawingVisual();
        using (var drawing = preview.RenderOpen())
            drawing.DrawRectangle(new VisualBrush(panel) { Stretch = Stretch.Fill }, null, new Rect(0, 0, 720, 228));
        SaveImage(preview, 720, 228, "IMark_Main_Usage_Detail.png");
        control.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));
    }

    private static void CheckFeedBoundary(SubEquipment equipment, FALotJobInfo job, EquipmentState stopState, Action<bool, string> check)
    {
        var savedFront = equipment.FrontModule;
        var savedRuntime = equipment.MainLoopModule.MoveJobInfo;
        try
        {
            using (var f = new IMarkPositioningTests.Fixture(equipment, "LiveUsageBoundary"))
            {
                equipment.FrontModule = f.Module;
                equipment.MainLoopModule.MoveJobInfo = (MoveJobInfo)job.MoveJobInfo.Clone();
                job.MoveJobInfo.UseFrontIMark = true;
                f.Module.ApplyFrontIMarkJob(job.MoveJobInfo, job.Name);
                equipment.MainLoopModule.MoveJobInfo.UseFrontIMark = true;
                SetState(equipment, new EquipmentState(equipment));
                f.CompleteOffset = false;
                f.Module.TapeLoadingMoveWithIMark.Start();
                f.Pump(() => f.OffsetCalls == 1);
                string error;
                check(IMarkUsageSelection.TryApply(equipment, job, false, out error) && f.Module.UseFrontIMark &&
                    f.Target == 122.5 && f.Servo.TapeLoadingPos.Position == 140,
                    "NOT USE requested during real offset execution leaves active mode and target unchanged");
                f.Module.TapeLoadingMoveWithIMark.PreSuspend();
                f.Module.TapeLoadingMoveWithIMark.Suspend();
                f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Suspended);
                check(f.Module.PendingFrontIMarkUse == false && f.Module.UseFrontIMark && f.Target == 122.5,
                    "Pending NOT USE cannot alter the suspended offset or its resume target");
                f.Module.TapeLoadingMoveWithIMark.Resume();
                f.SetFeedback("ActualPos", f.Target);
                f.SetFeedback("MotionDone", true);
                f.Pump(() => f.Module.TapeLoadingMoveWithIMark.State == SequenceState.Terminated);
                check(f.CompletedCalls == 1 && f.Module.UseFrontIMark && f.OffsetCalls == 1,
                    "Active cycle completes with its original I-Mark setting");
                f.Run();
                check(f.CompletedCalls == 2 && !f.Module.UseFrontIMark && f.FastCalls == 2 && f.OffsetCalls == 1 &&
                    f.Servo.TapeLoadingPos.Position == 150 && !equipment.MainLoopModule.MoveJobInfo.UseFrontIMark,
                    "Next production FRONT feed consumes NOT USE before normal full-pitch movement");
                check(IMarkUsageSelection.TryApply(equipment, job, true, out error), "USE can be queued during continuous run");
                job.MoveJobInfo.FrontIMarkSensorOffset = 9;
                f.Sensor.Value = false;
                f.DetectDuringSlow = true;
                f.CompleteOffset = true;
                f.Run();
                check(f.CompletedCalls == 3 && f.Module.UseFrontIMark && f.OffsetCalls == 2 && f.Target == 122.5 &&
                    f.Module.FrontIMarkSensorOffset == 2.5,
                    "Next feed enables I-Mark with the captured settings, unaffected by later JOB edits");
                job.MoveJobInfo.FrontIMarkSensorOffset = 2.5;
                typeof(FAFrontLoadingModule).GetField("_iMarkPositioningRecoveryRequired", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(f.Module, true);
                IMarkUsageSelection.TryApply(equipment, job, false, out error);
                f.Run();
                check(f.AlarmId != 0 && f.CompletedCalls == 3 && f.Module.UseFrontIMark,
                    "Selecting NOT USE cannot bypass required recovery after a positioning fault");
            }
        }
        finally
        {
            equipment.FrontModule = savedFront;
            equipment.MainLoopModule.MoveJobInfo = savedRuntime;
            job.MoveJobInfo.UseFrontIMark = false;
            job.MoveJobInfo.FrontIMarkSensorOffset = 2.5;
            SetState(equipment, stopState);
        }
    }

    private static void SaveImage(Visual visual, int width, int height, string name)
    {
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(visual);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using (var stream = File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name))) encoder.Save(stream);
    }
}
