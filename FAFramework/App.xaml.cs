using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FAFramework
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#if !DEBUG
            if (CheckAliveProcess()) this.Shutdown();
#endif
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            TextWriterTraceListener textWriterTraceListener =
    new TextWriterTraceListener("SystemLog.log");
            Trace.Listeners.Add(textWriterTraceListener);
            Debug.Listeners.Add(textWriterTraceListener);

            if (e.Args.Length > 0)
            {
                if (e.Args[0].ToLower() == "ExtractLanguageFiles".ToLower())
                {
                    Trace.WriteLine(e.Args.Length.ToString());
                    ExtractLanguageFiles(e.Args);
                }
                else if (e.Args[0].ToLower() == "ExtractConfig".ToLower())
                {
                    ExtractConfig();
                }

                Environment.Exit(0);
            }
            else
                base.OnStartup(e);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            string msg = "Unhandled Expcetion. " + ex.ToString() + "\n" + ex.Message;
            Manager.LogManager.Instance.WriteSystemLog(msg);
            MessageBox.Show(msg);
            //Environment.Exit(0);
        }

        private static bool CheckAliveProcess()
        {
            int processcount = 0;
            Process[] procs;
            procs = Process.GetProcesses();
            string appName = Application.ResourceAssembly.GetName().Name;
            foreach (Process aProc in procs)
            {
                if (aProc.ProcessName.ToString().Equals(appName))
                {
                    processcount++;
                }
            }

            if (processcount == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ExtractFontStyleFiles(string[] paths)
        {
            foreach (var item in paths)
            {
                string dir;
                try
                {
                    dir = System.IO.Path.GetDirectoryName(item);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    Trace.Flush();
                    continue;
                }

                if (string.IsNullOrEmpty(dir) == false)
                {
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                }

                try
                {
                    CopyResource("ResourceDictionary/FontStyleResource.xaml", item);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    Trace.Flush();
                    continue;
                }
            }
        }

        private void ExtractLanguageFiles(string[] paths)
        {
            foreach (var item in paths)
            {
                string dir;
                try
                {
                    dir = System.IO.Path.GetDirectoryName(item);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    Trace.Flush();
                    continue;
                }

                if (string.IsNullOrEmpty(dir) == false)
                {
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                }

                try
                {
                    CopyResource("ResourceDictionary/StringResource_ko-kr.xaml", item);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    Trace.Flush();
                    continue;
                }
            }
        }

        private void CopyResource(string resourceName, string file)
        {
            try
            {
                var rd = new ResourceDictionary()
                {
                    Source = new Uri(resourceName, UriKind.Relative)
                };

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(file, settings);
                XamlWriter.Save(rd, writer);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void ExtractConfig()
        {
            foreach (var prop in typeof(Equipment.EquipmentManager).GetProperties())
            {
                if (typeof(Equipment.EquipmentBase).IsAssignableFrom(prop.PropertyType))
                {
                    Utility.ConfigCreator.CreateConfig(prop.PropertyType, prop.Name);
                }
            }
        }
    }
}