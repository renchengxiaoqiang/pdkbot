using PdkBot.BotLib.Common;
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

namespace PdkBot.Controls.Shop
{
    public partial class CtlOneShop : UserControl
    {
        public bool IsOnline  { 
            get
            {

                return txtUserName.Foreground == Brushes.Green;
            }
            set {
                if (value)
                {
                    txtUserName.Foreground = Brushes.Green;
                }
                else {
                    txtUserName.Foreground = Brushes.Gray;
                }               
            }
        }

        public string Avatar
        {
            get
            {
                return iconAvatar.Tag?.ToString();
            }
            set
            {
                iconAvatar.Tag = value.Trim();
                WebImageHelper.GetImageFromUrl(value, iconAvatar);
            }
        }

        public string UserName
        {
            get
            {
                return txtUserName.Text.Trim();
            }
            set
            {
                txtUserName.Text = value.Trim();
            }
        }

        public CtlOneShop()
        {
            InitializeComponent();
        }
    }
}
