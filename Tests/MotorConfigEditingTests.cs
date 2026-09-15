using System;
using System.Collections.Generic;
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
using FAFramework.GUI.Standard;
using FAFramework.VT3500.ExtendedParts;
using FAFramework.VT3500.GUI;
using FALibrary.Part.MMCPart;
using FALibrary.Sequence;

internal static class MotorConfigEditingTests
{
    private static int passed;
    private static void Check(bool result, string message)
    {
        if (!result) throw new Exception("FAIL: " + message);
        Console.WriteLine("PASS: " + message);
        passed++;
    }

    private static IEnumerable<T> Children<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T) yield return (T)child;
            foreach (var descendant in Children<T>(child)) yield return descendant;
        }
    }

    private static XElement PartXml(string name)
    {
        return XDocument.Load("InputPartList.xml").Root.Elements("Part")
            .Single(p => (string)p.Element("Name") == name);
    }

    private static FASecondPressServo Servo()
    {
        var servo = new FASecondPressServo(new FASequenceManager());
        servo.LoadParameters(PartXml("SecondPressServo").Element("Parameters"));
        servo.Name = "SecondPressServo";
        servo.SimulationMode = true;
        return servo;
    }

    private static void Layout(MotorConfigControl control)
    {
        control.Measure(new Size(1500, 1000));
        control.Arrange(new Rect(0, 0, 1500, 1000));
        control.UpdateLayout();
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
    }

    private static MotorConfigControl Control(FAMMCPart servo)
    {
        var control = new MotorConfigControl { Part = servo, Background = Brushes.White };
        Layout(control);
        var list = Children<ListView>(control).First(l => ReferenceEquals(l.ItemsSource, control.Positions));
        list.SelectedItem = control.Positions.First(position => position.Name == "PickPos");
        Layout(control);
        return control;
    }

    private static TextBox Right(MotorConfigControl control, FAMMCPosition position, string property = "Position")
    {
        return Children<TextBox>(control).Single(b =>
        {
            var binding = b.GetBindingExpression(TextBox.TextProperty);
            return !b.IsReadOnly && binding != null && ReferenceEquals(binding.ResolvedSource, position) &&
                binding.ParentBinding.Path.Path == property;
        });
    }

    [STAThread]
    private static int Main()
    {
        try
        {
            new Application();
            XNamespace p = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
            var app = XDocument.Load("AppStyles.xml");
            var resources = new XElement(p + "ResourceDictionary", new XAttribute(XNamespace.Xmlns + "x", x.NamespaceName),
                app.Descendants(p + "Style").Where(s => (string)s.Attribute(x + "Key") == "OnOffSensorLabel" ||
                    (string)s.Attribute(x + "Key") == "defaultListViewItemStyle"));
            Application.Current.Resources.MergedDictionaries.Add((ResourceDictionary)XamlReader.Parse(resources.ToString()));
            Application.Current.Resources.MergedDictionaries.Add((ResourceDictionary)XamlReader.Parse(File.ReadAllText("KoreanResources.xaml")));
            Application.Current.Resources["masterPermission"] = FAFramework.Equipment.UserPermissionTypes.MASTER;

            string error;
            var servo = Servo();
            var control = Control(servo);
            Check(servo.PickPos.Position == 0 && servo.AxisNo == 9, "Customer XML reproduces Pinnacle PickPos=0 on axis 9");
            Right(control, servo.PickPos).Text = "45";
            Check(servo.PickPos.Position == 0 && ((TextBox)control.FindName("textBoxPosition")).Text == "0",
                "Before commit: right editor 45, left editor/model 0 reproduces supplied screenshot");
            Check(control.TryPrepareManualMove(out error) && servo.TargetPosition.Position == 45 && servo.PickPos.Position == 45,
                "Move preparation uses right-hand edited 45 rather than stale left-hand 0");
            Layout(control);
            Check(((TextBox)control.FindName("textBoxPosition")).Text == "45", "Left/right position editors synchronize after commit");
            Check(servo.ActualPos == 0 && servo.CommandPos == 0, "Preparation alone never commands a motor");
            ((Button)control.FindName("buttonMove")).RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            Check(servo.ActualPos == 45 && servo.CommandPos == 45, "Actual WPF Move button reaches 45 using simulation only");

            var saved = new XElement(PartXml("SecondPressServo"));
            servo.SaveParameters(saved.Element("Parameters"));
            saved.Save("SavedSecondPress.xml");
            var reloaded = new FASecondPressServo(new FASequenceManager());
            reloaded.LoadParameters(XElement.Load("SavedSecondPress.xml").Element("Parameters"));
            Check(reloaded.PickPos.Position == 45, "Production XML SaveParameters/LoadParameters preserves PickPos=45");
            Check(reloaded.AxisNo == 9 && reloaded.Scale == 0.001 && reloaded.SpeedRate == 30,
                "Motor number, scale and speed ratio remain unchanged");

            Layout(control);
            foreach (var list in Children<ListView>(control))
            {
                var view = list.View as GridView;
                if (view != null && view.Columns.Count > 0) view.Columns[0].Width = 145;
            }
            Layout(control);
            var png = new RenderTargetBitmap(1500, 1000, 96, 96, PixelFormats.Pbgra32);
            png.Render(control);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(png));
            using (var stream = File.Create("MotorConfig_45_simulated.png")) encoder.Save(stream);

            servo = Servo(); control = Control(servo);
            ((TextBox)control.FindName("textBoxPosition")).Text = "40";
            Check(control.TryPrepareManualMove(out error) && servo.TargetPosition.Position == 40,
                "Left-hand manual position edits are also applied");

            foreach (var invalid in new[] { "", "abc", "NaN", "Infinity", "1000" })
            {
                servo = Servo(); control = Control(servo);
                Right(control, servo.PickPos).Text = invalid;
                Check(!control.TryPrepareManualMove(out error) && servo.TargetPosition.Position == 0 && servo.PickPos.Position == 0,
                    "Invalid/out-of-range position rejected without mutation: " + invalid);
            }
            servo = Servo(); control = Control(servo);
            Right(control, servo.PickPos).Text = "45";
            Right(control, servo.PickPos, "DriveSpeed").Text = "0";
            Check(!control.TryPrepareManualMove(out error) && servo.PickPos.Position == 0,
                "Zero move speed blocks the whole request before applying the position");

            servo = Servo(); control = Control(servo);
            Right(control, servo.PickPos).Text = "45";
            ((TextBox)control.FindName("textBoxPosition")).Text = "40";
            Check(!control.TryPrepareManualMove(out error) && servo.PickPos.Position == 0,
                "Conflicting left/right edits are rejected, not silently overwritten");

            servo = Servo(); control = Control(servo);
            Right(control, servo.HomePos).Text = "abc";
            Right(control, servo.PickPos).Text = "45";
            Check(control.TryPrepareManualMove(out error) && servo.TargetPosition.Position == 45,
                "Move commits only the selected position, not unrelated pending edits");
            Check(!control.TryApplyEdits(out error), "Save validates other pending position edits");

            servo = Servo(); control = Control(servo); control.ReadOnly = true;
            Check(!control.TryPrepareManualMove(out error), "Read-only motor UI cannot prepare motion");
            control.ReadOnly = false; control.SelectedPosition = servo.TorqueLimitParams;
            Check(!control.TryPrepareManualMove(out error), "Torque rows cannot be sent as position moves");

            var xml = XDocument.Load("ConfigBaseSource.xml");
            var motors = xml.Descendants().Where(e => e.Name.LocalName == "MotorConfigControl").ToList();
            Check(motors.Count == 11 && motors.All(e => (string)e.Attribute("Loaded") == "MotorConfigControl_Loaded"),
                "All 11 motor tabs have the saving registration event, including Pinnacle");

            var host = (ConfigBaseControl)FormatterServices.GetUninitializedObject(typeof(ConfigBaseControl));
            typeof(ConfigBaseControl).GetField("_motorControls", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(host, new List<MotorConfigControl>());
            var register = typeof(ConfigBaseControl).GetMethod("MotorConfigControl_Loaded", BindingFlags.Instance | BindingFlags.NonPublic);
            servo = Servo(); control = Control(servo);
            var servo2 = Servo(); var control2 = Control(servo2);
            register.Invoke(host, new object[] { control, null });
            register.Invoke(host, new object[] { control2, null });
            Right(control, servo.PickPos).Text = "45";
            Right(control2, servo2.PickPos).Text = "bad";
            Check(!host.TryApplyMotorEdits(out error) && servo.PickPos.Position == 0,
                "Save validates every registered tab before mutating any tab");
            Right(control2, servo2.PickPos).Text = "25";
            Check(host.TryApplyMotorEdits(out error) && servo.PickPos.Position == 45 && servo2.PickPos.Position == 25,
                "Save applies all registered motor tabs without commanding motion");
            Check(servo.ActualPos == 0 && servo2.ActualPos == 0, "Multi-tab Save never moves the axes");

            var first = new FAFirstPressServo(new FASequenceManager());
            first.LoadParameters(PartXml("FirstPressServo").Element("Parameters"));
            first.SimulationMode = true;
            var firstControl = Control(first);
            Check(firstControl.TryPrepareManualMove(out error) && first.TargetPosition.Position == 15 && first.AxisNo == 8,
                "Existing forming-press PickPos=15 still prepares on original axis 8");

            servo = Servo(); control = Control(servo);
            Right(control, servo.PickPos).Text = "45";
            Right(control, servo.PickPos, "DriveSpeed").Text = "250";
            ((TextBox)control.FindName("textBoxAccTime")).Text = "120";
            Check(control.TryPrepareManualMove(out error) && servo.TargetPosition.Position == 45 &&
                servo.TargetPosition.DriveSpeed == 250 && servo.TargetPosition.AccelTime == 120,
                "Position, speed and acceleration drafts survive shared-editor notifications together");
            servo = Servo(); control = Control(servo);
            ((TextBox)control.FindName("textBoxStartSpeed")).Text = "-1";
            Check(!control.TryPrepareManualMove(out error), "Negative unsigned speed is rejected");

            string logPath = Path.Combine("Log", "SystemLog", DateTime.Now.ToString("yyyy"),
                DateTime.Now.ToString("MM"), DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            for (int i = 0; i < 100 && (!File.Exists(logPath) || !File.ReadAllText(logPath).Contains("Result=RETURNED")); i++)
                System.Threading.Thread.Sleep(20);
            Check(File.ReadAllText(logPath).Contains("Result=REQUEST;Part=SecondPressServo;Axis=9;Selected=PickPos;Target=45"),
                "SystemLog records selected motor, axis and the committed 45 target");

            Console.WriteLine("PASS TOTAL: " + passed);
            return 0;
        }
        catch (Exception ex) { Console.WriteLine(ex); return 1; }
        finally
        {
            var manager = FAFramework.Manager.LogManager.Instance;
            manager.Run = false;
            var thread = (System.Threading.Thread)typeof(FAFramework.Manager.LogManager)
                .GetField("_thread", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(manager);
            thread.Join(5000);
        }
    }
}
