using System;
using System.Windows;
using System.Globalization;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;

namespace FAFramework.Manager
{
    public class StringResourceManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static volatile StringResourceManager _instance = null;
        private static object syncRoot = new Object();

        private StringResourceManager()
        {
            CultureInfoList = new FAFramework.Utility.ThreadSafeObservableCollection<CultureInfo>();
            LoadCultureInfoList();
            LoadSetting();
        }

        public static StringResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new StringResourceManager();
                    }
                }

                return _instance;
            }
        }

        private readonly string SETTING_FILE_NAME = "languageSetting.xml";

        public event EventHandler<FALibrary.FAGenericEventArgs<string>> OnChangedCulture;

        CultureInfo[] _systemCultureInfoList = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

        private string LANGUAGE_FOLDER
        {
            get
            {
                try
                {
                    return ConfigClasses.GlobalConst.ROOT_PATH + @"\config\language\";
                }
                catch
                {
                    return "";
                }
            }
        }

        private FAFramework.Utility.ThreadSafeObservableCollection<CultureInfo> _cultureInfoList;
        public FAFramework.Utility.ThreadSafeObservableCollection<CultureInfo> CultureInfoList
        {
            get { return _cultureInfoList; }
            private set
            {
                _cultureInfoList = value;
                NotifyPropertyChanged("CultureInfoList");
            }
        }

        private CultureInfo _currentCultureInstance;
        public CultureInfo CurrentCultureInstance
        {
            get { return _currentCultureInstance; }
            set
            {
                if (value == null) return;
                if (_currentCultureInstance == value) return;
                _currentCultureInstance = value;
                NotifyPropertyChanged("CurrentCultureInstance");

                ChangeLanguage(value.Name);
                if (OnChangedCulture != null)
                    OnChangedCulture(this, new FALibrary.FAGenericEventArgs<string>(value.Name));
            }
        }

        private string[] _systemCultureList = { "en-US" };

        public void LoadCultureInfoList()
        {
            Regex reg = new Regex(@"StringResource_[^\.]*.xaml");

            CultureInfoList = new FAFramework.Utility.ThreadSafeObservableCollection<CultureInfo>();

            if (Directory.Exists(LANGUAGE_FOLDER) == true)
            {
                var files = Directory.GetFiles(LANGUAGE_FOLDER);
                foreach (var item in files)
                {
                    var regResult = reg.Match(item);
                    if (regResult == null) continue;

                    string culture = regResult.Value.Replace("StringResource_", "");
                    culture = culture.Replace(".xaml", "");
                    CultureInfoList.Add(CultureInfo.GetCultureInfo(culture));
                }
            }

            AddDefaultCultureInCultureInfoList("ko-kr");
        }

        public void ChangeLanguage(string type, bool systemResource = false)
        {
            UriKind uriKind = UriKind.Absolute;

            if (IsValidCulture(type) == false)
            {
                MessageBox.Show("유효하지 않은 언어입니다.");
                return;
            }

            CultureInfo ci = new CultureInfo(type);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            string filename = String.Format("StringResource_{0}.xaml", type);
            string path = "";

            if (systemResource == false)
            {
                path = LANGUAGE_FOLDER + filename;
                if (File.Exists(path) == false)
                {
                    if (ExistInSystemCultureArray(type) == true)
                    {
                        systemResource = true;
                        path = "ResourceDictionary/" + filename;
                    }
                    else
                    {
                        MessageBox.Show("언어파일이 존재하지 않습니다.\n" +
                            path + filename);
                        return;
                    }
                }
            }

            if (systemResource) uriKind = UriKind.Relative;

            try
            {
                Application.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary()
                    {
                        Source = new Uri(path, uriKind)
                    });

                SaveSetting();
            }
            catch (Exception e)
            {
                MessageBox.Show("언어파일을 읽어오는데 실패하였습니다.\n" +
                    path + filename + "\n" +
                    e.ToString());
            }
        }

        private bool IsValidCulture(string type)
        {
            foreach (var item in _systemCultureInfoList)
            {
                if (string.Equals(item.Name, type, StringComparison.CurrentCultureIgnoreCase) == true)
                    return true;
            }

            return false;
        }

        private bool ExistCultureInfo(string type)
        {
            foreach (var cultureInfo in CultureInfoList)
            {
                if (string.Equals(type, cultureInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ExistInSystemCultureArray(string type)
        {
            foreach (var item in _systemCultureList)
            {
                if (string.Equals(type, item, StringComparison.CurrentCultureIgnoreCase) == true)
                    return true;
            }

            return false;
        }

        private void AddDefaultCultureInCultureInfoList(params string[] types)
        {
            foreach (var type in types)
            {
                bool exist = ExistCultureInfo(type);

                if (exist == false)
                    CultureInfoList.Add(CultureInfo.GetCultureInfo(type));
            }
        }

        private CultureInfo GetCulture(string culture)
        {
            culture = culture.ToLower();

            foreach (var item in CultureInfoList)
            {
                if (item.Name.ToLower() == culture)
                    return item;
            }

            return null;
        }

        private void LoadSetting()
        {
            try
            {
                string path = Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, SETTING_FILE_NAME);
                if (File.Exists(path) == false) return;

                XElement xml = XElement.Load(path);

                var currentCultureElement = xml.Element("CurrentCulture");
                if (currentCultureElement == null) return;

                string currentCulture = currentCultureElement.Value;
                CurrentCultureInstance = GetCulture(currentCulture);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                Trace.Flush();
            }
        }

        private void SaveSetting()
        {
            try
            {
                string path = Path.Combine(ConfigClasses.GlobalConst.CONFIG_PATH, SETTING_FILE_NAME);
                if (File.Exists(path) == false) return;

                XElement xml = XElement.Load(path);

                if (CurrentCultureInstance == null) return;

                var currentCultureElement = xml.Element("CurrentCulture");
                if (currentCultureElement == null)
                {
                    currentCultureElement = new XElement("CurrentCulture");
                    xml.Add(currentCultureElement);
                }

                currentCultureElement.Value = CurrentCultureInstance.Name;

                xml.Save(path);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                Trace.Flush();
            }
        }
    }
}
