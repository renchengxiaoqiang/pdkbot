using iNKORE.UI.WPF.Modern.Common;
using iNKORE.UI.WPF.Modern.Controls;
using iNKORE.UI.WPF.Modern.Controls.Helpers;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using PdkBot.BotLib;
using PdkBot.BotLib.Common;
using PdkBot.BotLib.Wpf;
using PdkBot.DbEntity;
using PdkBot.DbEntity.Pdd;
using PdkBotLib.Db;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PdkBot.Params;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace PdkBot.Pages
{
    /// <summary>
    /// Interaction logic for PddPage.xaml
    /// </summary>
    public partial class PddPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        long _incrementCount = 0;
        private string chatUrl = "https://mms.pinduoduo.com/chat-merchant/index.html#/";
        private string injectJsPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Platform", "pdd.js");
        private string fileName = System.IO.Path.Combine(AppContext.BaseDirectory, "pdd_shops.json");
        private string defaultWebTitle = "拼多多商家后台";
        private ConcurrentDictionary<uint, string> webviewMap;
        private NavigationViewItem selfNavItem;
        public event EventHandler<ReceieveNewMessageEventArgs> EvReceieveNewMessage;
        public event EventHandler<PageSelectionChangedEventArgs> EvPageSelectionChanged;

        public ICommand AddTabButtonCommand { get; }
        public PddPage()
        {
            InitializeComponent();
            webviewMap = new ConcurrentDictionary<uint, string>();
            Init();
            DataContext = this;
            AddTabButtonCommand = new RelayCommand(async k => {
                HideTabs();
            });
            IsVisibleChanged += PddPage_IsVisibleChanged;
        }

        private void PddPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && EvPageSelectionChanged!=null)
            {
                EvPageSelectionChanged(sender, new PageSelectionChangedEventArgs(this, tabs.SelectedItem as TabItem));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.ExtraData != null)
            {
                var navItem = e.ExtraData as NavigationViewItem;
                if (selfNavItem == null)
                {
                    selfNavItem = navItem;
                }
                if (selfNavItem == navItem) return;

                ChangeTab(navItem.Content?.ToString());
            }
        }

        private void tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var tab = e.AddedItems[0] as TabItem;
                if (EvPageSelectionChanged != null)
                {
                    EvPageSelectionChanged(sender, new PageSelectionChangedEventArgs(this, tab));
                }
                ChangeNavItem(tab.Header?.ToString());
            }
        }
        private void RemoveNavItem(string userName)
        {
            NavigationViewItem removed = null;
            foreach (NavigationViewItem item in selfNavItem.MenuItems)
            {
                if (item.Content.ToString() == userName)
                {
                    removed = item;
                    break;
                }
            }
            if (removed != null && selfNavItem.MenuItems.Count > 0)
            {
                try
                {
                    if (selfNavItem.MenuItems.Count == 0)
                    {
                        selfNavItem.IsExpanded = false;
                    }
                    else
                    {
                        selfNavItem.MenuItems.Remove(removed);
                    }
                }
                catch { }
                
            }
        }

        private void ChangeNavItem(string userName)
        {
            try { 
                foreach (NavigationViewItem item in selfNavItem.MenuItems)
                {
                    if (item.Content.ToString() == userName)
                    {
                        item.IsSelected = true;
                    }
                    else
                    {
                        item.IsSelected = false;
                    }
                }
            }
            catch { }
        }

        private void ChangeTab(string userName)
        {
            ShowTabs();
            foreach (TabItem item in tabs.Items)
            {
                if(item.Header.ToString() == userName)
                {
                    tabs.SelectedItem = item;
                    break;
                }               
            }
        }

        private TabItem GetTab(string userName)
        {
            TabItem tab = null;
            foreach (TabItem item in tabs.Items)
            {
                if (item.Header.ToString() == userName)
                {
                    tab = item;
                    break;
                }
            }
            return tab;
        }

        private async void CreateNewShop()
        {
            var id = Interlocked.Increment(ref _incrementCount);
            var newTab = new TabItem();
            newTab.Header = defaultWebTitle+id;
            var icon = new ImageIcon();
            icon.Source = AssetImageHelper.GetImageFromWpfCache("pdd");
            TabItemHelper.SetIcon(newTab, icon);
            tabs.Items.Add(newTab);
            newTab.Loaded += Tab_Loaded;
            tabs.SelectedItem = newTab;
            await CreateNewWebView(newTab);
            AddNewSubNavItem(newTab.Header, selfNavItem.Tag, new ImageIcon {
                Source = AssetImageHelper.GetImageFromWpfCache("pdd")
            });
        }

        /// <summary>
        /// 这么麻烦的关闭Tab的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tab_Loaded(object sender, RoutedEventArgs e)
        {
            var tab = sender as TabItem;
            tab.SetValue(TabItemHelper.CloseTabButtonCommandProperty, 
                new RelayCommand(k => {
                    if (tabs.SelectedItem == tab)
                    {
                       tabs.SelectedIndex--;
                    }
                    tabs.Items.Remove(sender);
                    e.Handled = true;
                    RemoveNavItem(tab.Header?.ToString());
                    if(tabs.Items.Count < 1)
                    {
                        HideTabs();
                    }
                })
            );
        }

        private async void LoadOfflineShop(PlatformShop shop)
        {
            var tab = GetTab(shop.UserName);
            if (tab == null)
            {
                tab = new TabItem();
                tab.Header = shop.UserName;
                var icon = new ImageIcon();
                WebImageHelper.GetImageFromUrl(shop.Avatar, icon);
                TabItemHelper.SetIcon(tab, icon);
                tab.Tag = shop.WebViewId;
                tabs.Items.Add(tab);
                tab.Loaded += Tab_Loaded;
                tabs.SelectedItem = tab;
                await CreateNewWebView(tab);
                AddNewSubNavItem(tab.Header, selfNavItem.Tag, icon);
            }
            else
            {
                tabs.SelectedItem = tab;
            }
        }

        private void AddNewSubNavItem(object content,object tag,ImageIcon icon)
        {
            selfNavItem.IsSelected = false;
            foreach (NavigationViewItem item in selfNavItem.MenuItems)
            {
                item.IsSelected = false;
            }
            var subNavItem = new NavigationViewItem()
            {
                Content = content,
                Tag = tag,
                Icon = icon,
                IsSelected = true
            };
            try
            {
                selfNavItem.MenuItems.Add(subNavItem);
                if (!selfNavItem.IsExpanded)
                {
                    selfNavItem.IsExpanded = true;
                }
            }
            catch { }
        }

        private void Init()
        {
            try
            {
                var shops = Params.Shop.GetPlatformShops(Shop.ShopTypeEnum.Pinduodduo);
                ctlShops.InitUI(shops, selected => {
                    ShowTabs();
                    DispatcherEx.xInvoke(async  () => {                     
                        if (selected.IsAddNew)
                        {
                            CreateNewShop();
                        }
                        else if (selected.IsAddAll)
                        {
                            foreach (var shop in shops)
                            {
                                LoadOfflineShop(shop);
                                await Task.Delay(2000);
                            }
                        }
                        else {
                            LoadOfflineShop(selected.Shop);
                        }                    
                    });
                });              

            }
            catch (Exception ex)
            {
               Log.Exception(ex);
            }
        }

        private void ShowTabs()
        {
            ctlShops.Visibility = Visibility.Collapsed;
            tabs.Visibility = Visibility.Visible;
        }
        private void HideTabs()
        {
            var shops = Params.Shop.GetPlatformShops(Shop.ShopTypeEnum.Pinduodduo);
            ctlShops.ReloadShops(shops);
            ctlShops.Visibility = Visibility.Visible;
            tabs.Visibility = Visibility.Collapsed;
        }

        private async Task CreateNewWebView(TabItem tab)
        {
            try
            {
                var webviewId = tab.Tag?.ToString();
                webviewId = string.IsNullOrEmpty(webviewId) ? Guid.NewGuid().ToString().Replace("-", "") : webviewId;
                var userDataPath = System.IO.Path.Combine(AppContext.BaseDirectory, "webview_profiles", webviewId);
                if (!Directory.Exists(userDataPath))
                {
                    Directory.CreateDirectory(userDataPath);
                }
                var env = await CoreWebView2Environment.CreateAsync(null, userDataPath);
                var webview = new Microsoft.Web.WebView2.Wpf.WebView2();
                tab.Content = webview;
                tab.Tag = webviewId;
                await webview.EnsureCoreWebView2Async(env);
                webviewMap[webview.CoreWebView2.BrowserProcessId] = webviewId;
                webview.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
                webview.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                var browserProcessId = webview.CoreWebView2.BrowserProcessId;
                webview.CoreWebView2.Navigate(chatUrl);
            }
            catch { }
        }

        private void UpdateBadge(string webviewId, NewMessage msg)
        {
            try
            {
                TabItem tab = null;
                foreach (TabItem item in tabs.Items)
                {
                    if (item.Tag.ToString() == webviewId)
                    {
                        tab = item;
                        break;
                    }
                }

                foreach (NavigationViewItem item in selfNavItem.MenuItems)
                {
                    if (item.Content.ToString() == tab?.Header?.ToString())
                    {
                        if (msg.NewMessageCount > 0)
                        {
                            item.InfoBadge = new InfoBadge()
                            {
                                Background = System.Windows.Media.Brushes.Red,
                                Opacity = 1.0,
                                Value = msg.NewMessageCount,
                            };
                        }
                        else
                        {
                            item.InfoBadge = null;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
        }

        private void UpdateTabHeader(PlatformShop user)
        {
            try
            {
                var prevTabHeader = string.Empty;
                foreach (TabItem item in tabs.Items)
                {
                    if (item.Tag.ToString() == user.WebViewId)
                    {
                        prevTabHeader = item.Header.ToString();
                        item.Header = user.UserName;
                        var icon = new ImageIcon();
                        WebImageHelper.GetImageFromUrl(user.Avatar, icon);
                        TabItemHelper.SetIcon(item, icon);
                        break;
                    }
                }

                foreach (NavigationViewItem item in selfNavItem.MenuItems)
                {
                    if (item.Content.ToString() == prevTabHeader)
                    {
                        item.Content = user.UserName;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var coreWebView = sender as CoreWebView2;
            var response = e.WebMessageAsJson;
            var pres = JsonSerializer.Deserialize<PlatformResponse>(response);


            if (pres.Type == "currentuser")
            {
                var webviewId = webviewMap[coreWebView.BrowserProcessId];
                var user = JsonSerializer.Deserialize<PlatformShop>(pres.Response);
                user.WebViewId = webviewId;
                //保存已经登录的用户
                UpdateTabHeader(user);
                SaveToDb(user);
            }
            else if (pres.Type == "newmessage")
            {
                var newmsg = JsonSerializer.Deserialize<NewMessage>(pres.Response);

                var webviewId = webviewMap[coreWebView.BrowserProcessId];
                UpdateBadge(webviewId,newmsg);
            }
            else if (pres.Type == "receiveMessage")
            {
                var webviewId = webviewMap[coreWebView.BrowserProcessId];
                var selectedTabItem = tabs.SelectedItem as TabItem;
                if (EvReceieveNewMessage != null && this.IsVisible && webviewId == selectedTabItem?.Tag?.ToString())
                {
                    EvReceieveNewMessage(this,new ReceieveNewMessageEventArgs(pres.Response));
                }
                var res = JsonSerializer.Deserialize<ReceiveMessageResponse>(pres.Response);
                var recvMessage = res.pdd_message;
                if(recvMessage.actionId == 20008)
                {
                    //var msgs = res.payload.push_data.data;
                    //foreach(var msg in msgs)
                    //{

                        ////测试回复消息
                        //if (msg.message.from.role == "user") {
                        //    var uid = msg.message.from.uid;
                        //    var content = msg.message.rawContent + "$";
                        //    var cmd = $"window.___pdd_imsdk.sendMsg({JsonSerializer.Serialize(new { uid, content })})";
                        //    coreWebView.ExecuteScriptAsync(cmd);
                        //}                        
                    //}
                }
            }
        }

        private void SaveToDb(PlatformShop shop)
        {
            try
            {
                var shops = Params.Shop.GetPlatformShops(Shop.ShopTypeEnum.Pinduodduo);
                if (!shops.Any(k => k.WebViewId == shop.WebViewId))
                {
                    Params.Shop.AddNewShop(Shop.ShopTypeEnum.Pinduodduo, shop);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        private async void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            var coreWebView = sender as CoreWebView2;
            if (coreWebView.Source.StartsWith(chatUrl))
            {
                await Task.Delay(2000);
                var script = File.ReadAllText(injectJsPath);
                await coreWebView.ExecuteScriptAsync(script);
            }
        }

    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
