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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Messenger.Resouces.CustomControls
{
    public class RoundedButton : Button
    {

        //Điều chỉnh độ cong của góc
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RoundedButton), new PropertyMetadata(default(CornerRadius)));



        //Đổi màu background khi trỏ chuột vào
        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(RoundedButton), new PropertyMetadata(default(Brush)));


        //Đổi màu background khi click chuột
        public Brush OnClickBackground
        {
            get { return (Brush)GetValue(OnClickBackgroundProperty); }
            set { SetValue(OnClickBackgroundProperty, value); }
        }

        public static readonly DependencyProperty OnClickBackgroundProperty =
            DependencyProperty.Register("OnClickBackground", typeof(Brush), typeof(RoundedButton), new PropertyMetadata(default(Brush)));



        static RoundedButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoundedButton), new FrameworkPropertyMetadata(typeof(RoundedButton)));
        }
    }
}
