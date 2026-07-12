using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using FAFramework.Utility;
using FALibrary;
using FALibrary.Part.MemoryBasePart;
using System.Threading;
using System.Net.NetworkInformation;

namespace FAFramework
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Mutex _mutex = null;
        public MainWindow()
        {
            string mutexName = System.Diagnostics.Process.GetCurrentProcess().ProcessName; //"FAFramework"
            bool isCreatedNew = false;
            _mutex = new Mutex(true, mutexName, out isCreatedNew);
            if (isCreatedNew == false)
            {
                MessageBox.Show("프로그램이 이미 실행중입니다.");
                Application.Current.Shutdown();
                return;
            }
            InitializeComponent();


#if DEBUG // 시뮬레이션 선택 화면
            if (MessageBox.Show("시뮬레이션 모드로 사용하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Equipment.MainEquipment.SIMULATION_MODE = true;
            else
                Equipment.MainEquipment.SIMULATION_MODE = false;
#endif


            // initialize 화면 뿌리기
            FAFramework.LoadingWindow LdWindow = new FAFramework.LoadingWindow();
            LdWindow.Show();


            if (!BuildAdapterListing()) // dglee // 20241122  Mac 확인
            {
                MessageBox.Show("This License code is incorrect Please contact your software provider",
                                                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);                
            }
            


            SetCheckNotExistDeviceInDeviceList();
            UtilityClass.BlockAltF4(this);

            Equipment.MainEquipment.Instance.Initialize();
            Equipment.MainEquipment.Instance.Start();

            var eqp = Equipment.MainEquipment.Instance.EquipmentManagerInstance.VT3500;
            if (Equipment.MainEquipment.SIMULATION_MODE)
            {
                try
                {
                    eqp.CommonUnit.EmergencyOff.InputIO[0].Value = true;
                }
                catch
                {

                }
            }

            var win = new VT3500.GUI.EquipmentWindow();
            eqp.Window = win;
            if (GetResolution(out bool windowMode, out int width, out int height))
            {
                if (windowMode)
                {
                    //win.WindowStyle = WindowStyle.SingleBorderWindow;
                    //win.WindowState = WindowState.Maximized;
                    win.Width = width;
                    win.Height = height;
                }
            }
          

            LdWindow.Close();


          





            win.Show(); // 설비 화면 보여주기 
        }


        private bool BuildAdapterListing()
        {

            // MAcAdress 확인      
            var mac = NetworkInterface.GetAllNetworkInterfaces();
            int IPCount = 60;
            String[] GetIPAddress = new String[mac.Length];
            String[] szFixIP = new String[IPCount];

            for (int i = 0; i < mac.Length; i++)
            {
                GetIPAddress[i] = mac[i].GetPhysicalAddress().ToString();
            }


            for (int i = 0; i < IPCount; i++)
            {
                szFixIP[i] = null;
            }

            szFixIP[0] = "C87F542D396E"; // dglee 
            szFixIP[1] = "E0E1A91EF0D0";
            szFixIP[2] = "00155D3BDD92";
            szFixIP[3] = "70A8D317A95E";
            szFixIP[4] = "E89C250DDBF7";
            szFixIP[5] = "309C23922E1B";
            


            for (int i = 0; i < IPCount; i++)
            {
                for (int j = 0; j < mac.Length; j++)
                {

                    if (szFixIP[i] == GetIPAddress[j])
                    {
                        return true;
                    }
                }

            }
            return false;
        }



        private bool GetResolution(out bool windowMode, out int width, out int height)
        {
            var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resoulution.txt");
            
            windowMode = false;
            width = 1280;
            height = 1024;
            if (!System.IO.File.Exists(file)) return false;

            try
            {
                var text = System.IO.File.ReadAllText(file);
                var arr = text.Split(new char[] { ',' });
                if (arr[0].Trim().ToLower() == "window")
                    windowMode = true;
                width = int.Parse(arr[1]);
                height = int.Parse(arr[2]);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void SetCheckNotExistDeviceInDeviceList()
        {
            var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CheckNotExistDeviceInDeviceList");
            ConfigClasses.GlobalConst.CHECK_EXIST_DEVICE_IN_DEVICELIST = !System.IO.File.Exists(file);
        }
    }
}