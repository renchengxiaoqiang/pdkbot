using PdkBot.BotLib.Common;
using PdkBot.BotLib.Extensions;
using PdkBot.DbEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PdkBot.Controls.Shop.CtlShops;
using static PdkBot.Params;

namespace PdkBot.Controls.Shop
{

    /// <summary>
    /// Interaction logic for CtlAccounts.xaml
    /// </summary>
    public partial class CtlShops : System.Windows.Controls.UserControl
    {

        private Action<SelectedShopArgs> selectedCallbackAction = null;

        private DateTime _preOpenShopTime;

        public CtlShops()
        {
            InitializeComponent();
        }

        public void InitUI(List<PlatformShop> shops,Action<SelectedShopArgs> cb = null)
        {
            selectedCallbackAction = cb;
            var ctlOneShop = new CtlOneShop();
            ctlOneShop.iconAvatar.Source = AssetImageHelper.GetImageFromWpfCache("newTab");
            ctlOneShop.txtUserName.Visibility = Visibility.Collapsed;
            ctlOneShop.Margin = new Thickness(5.0);
            ctlOneShop.PreviewMouseUp += (s, e) =>
            {
                if (_preOpenShopTime.xIsTimeElapseMoreThanMs(800))
                {
                    _preOpenShopTime = DateTime.Now;
                    if (selectedCallbackAction != null)
                    {
                        selectedCallbackAction(new SelectedShopArgs(null, true));
                    }
                }
            };
            panel.Children.Insert(0, ctlOneShop);

            foreach (var shop in shops)
            {
                ShowCtlShop(shop);
            }
        }

        public void ReloadShops(List<PlatformShop> shops)
        {
            panel.Children.Clear();
            var ctlOneShop = new CtlOneShop();
            ctlOneShop.iconAvatar.Source = AssetImageHelper.GetImageFromWpfCache("newTab");
            ctlOneShop.txtUserName.Visibility = Visibility.Collapsed;
            ctlOneShop.Margin = new Thickness(5.0);
            ctlOneShop.PreviewMouseUp += (s, e) =>
            {
                if (_preOpenShopTime.xIsTimeElapseMoreThanMs(800))
                {
                    _preOpenShopTime = DateTime.Now;
                    if (selectedCallbackAction != null)
                    {
                        selectedCallbackAction(new SelectedShopArgs(null, true));
                    }
                }
            };
            panel.Children.Insert(0, ctlOneShop);

            foreach (var shop in shops)
            {
                ShowCtlShop(shop);
            }
        }

        private void ShowCtlShop(PlatformShop shop)
        {
            var ctlOneShop = new CtlOneShop();
            ctlOneShop.Avatar = shop.Avatar;
            ctlOneShop.UserName = shop.UserName;
            ctlOneShop.BorderBrush = Brushes.Red;
            ctlOneShop.Margin = new Thickness(5.0);
            ctlOneShop.PreviewMouseUp += (s, e) =>
            {
                if (_preOpenShopTime.xIsTimeElapseMoreThanMs(800))
                {
                    _preOpenShopTime = DateTime.Now;
                    (s as CtlOneShop).IsOnline = true;
                    if (selectedCallbackAction != null) {
                        selectedCallbackAction(new SelectedShopArgs(shop));
                    }
                }
            };
            panel.Children.Add(ctlOneShop);

        }

        public class SelectedShopArgs
        {
            public SelectedShopArgs(PlatformShop shop,bool isAddNew = false, bool isAddAll = false)
            {
                IsAddNew = isAddNew;
                IsAddAll = isAddAll;
                Shop = shop;
            }
            public bool IsAddNew { get; set; }
            public bool IsAddAll { get; set; }
            public PlatformShop Shop { get; set; }
        }
    }
}
