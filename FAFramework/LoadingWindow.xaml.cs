using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FAFramework
{
    /// <summary>
    /// LoadingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            //MyFadingText.Text = "test";
        }

        private String ChangeText(String msg)
        {

            MyFadingText.Text = msg;

        

            return hello;
        }

        //private 필드
        private string hello;

        //private 필드 값을 외부에 공개하는 속성
        public string Hello
        {
            //get 접근자
            get { return hello; }

            //set 접근자 : value 키워드 사용
            set { ChangeText(value); }
        }
      

        
     }
}
