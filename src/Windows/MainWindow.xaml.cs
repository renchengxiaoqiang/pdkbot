using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using PdkBot.BotLib.Wpf.Extensions;
using PdkBot.Pages;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace PdkBot.Windows
{
    public partial class MainWindow : Window
    {
        public static MainWindow Inst { get; set; }

        private List<System.Windows.Controls.Page> pages;
        public event EventHandler<ReceieveNewMessageEventArgs> EvReceieveNewMessage;
        public event EventHandler<PageSelectionChangedEventArgs> EvPageSelectionChanged;

        static MainWindow()
        {
            Inst = new MainWindow();
        }

        private int WindowHandle;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //QNScanner.LoopScan();

            var hwndSoure = PresentationSource.FromVisual(this) as HwndSource;
            WindowHandle = (int)hwndSoure.Handle;
            pages = FindPages();

            foreach (var p in pages)
            {
                BindEvent(p, "EvReceieveNewMessage", new EventHandler<ReceieveNewMessageEventArgs>(OnReceieveNewMessage));
                BindEvent(p, "EvPageSelectionChanged", new EventHandler<PageSelectionChangedEventArgs>(OnPageSelectionChanged));
            }
            navItemHome.IsSelected = true;
        }

        /// <summary>
        /// 收消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceieveNewMessage(object sender, ReceieveNewMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
            if(EvReceieveNewMessage != null) { EvReceieveNewMessage(this, e); }
        }

        private void OnPageSelectionChanged(object sender, PageSelectionChangedEventArgs e)
        {
            Console.WriteLine(e.Page.Title);
            if (EvReceieveNewMessage != null) { EvPageSelectionChanged(this, e); }
        }

        private void navBar_SelectionChanged(iNKORE.UI.WPF.Modern.Controls.NavigationView sender, iNKORE.UI.WPF.Modern.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;
            if (item == null) return;
            var page = pages.FirstOrDefault(p => p.Title == item.Tag.ToString());
            contentFrame.Navigate(page,item);
        }

        public List<System.Windows.Controls.Page> FindPages()
        {
            var pageInstances = new List<System.Windows.Controls.Page>();
            var pageBaseType = typeof(System.Windows.Controls.Page);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = assemblies.FirstOrDefault(a => a.GetName().Name == "PdkBot");
            
            try
            {

                var pageTypes = assembly.GetTypes()
                    .Where(type =>
                        !type.IsAbstract &&       // 排除抽象类
                        !type.IsInterface &&      // 排除接口
                        pageBaseType.IsAssignableFrom(type) &&  // 继承自Page
                        type.GetConstructor(Type.EmptyTypes) != null  // 有无参构造函数
                    );

                foreach (var pageType in pageTypes)
                {
                    try
                    {
                        // 通过无参构造函数实例化
                        var pageInstance = (System.Windows.Controls.Page)Activator.CreateInstance(pageType);
                        pageInstances.Add(pageInstance);
                    }
                    catch (Exception ex)
                    {
                            
                    }
                }
            }
            catch { }

            return pageInstances;
        }

        public static bool BindEvent(object target, string eventName, Delegate handler)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // 获取事件信息
            Type targetType = target.GetType();
            EventInfo eventInfo = targetType.GetEvent(eventName);

            if (eventInfo == null)
                return false; // 事件不存在

            // 检查委托类型是否匹配
            if (handler.GetType() != eventInfo.EventHandlerType)
                throw new ArgumentException("委托类型与事件类型不匹配", nameof(handler));

            // 绑定事件
            eventInfo.AddEventHandler(target, handler);
            return true;
        }

    }
}