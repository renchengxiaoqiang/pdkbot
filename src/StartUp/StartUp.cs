using iNKORE.UI.WPF.Modern;
using PdkBot.BotLib;
using PdkBot.BotLib.Extensions;
using PdkBot.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Windows;
namespace PdkBot.StartUp
{
	public class StartUp
	{
		[STAThread]
		public static void Main(string[] args)
		{
            try
			{
                // 检查当前用户是否已经是管理员
                //bool isRuningAdmin = IsRuningForAdmin();

                //if (!isRuningAdmin)
                //{
                //    // 如果不是管理员，使用提升权限启动当前应用程序
                //    RestartElevated();
                //    return;
                //}

                bool createdNew;
                using (new Mutex(true, "PdkBot", out createdNew))
				{
					if (createdNew)
					{
                        KillProcess();
						App app = new App();

                        app.DispatcherUnhandledException += App_DispatcherUnhandledException;
                        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

						app.InitializeComponent();

                        var themeName = Params.Other.GetApplicationThemeName();
                        ThemeManager.Current.ApplicationTheme = themeName == "Dark" ? ApplicationTheme.Dark : ApplicationTheme.Light;
                        app.Run(MainWindow.Inst);
					}
					else
					{
						var app = Application.Current;
						if (app != null)
						{
							app.Shutdown();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
		}

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Exception(e.ExceptionObject as Exception);
        }

        private static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

            Log.Exception(e.Exception);
        }

        private static void KillProcess()
		{
            var processes = Process.GetProcessesByName("Bot");
			var curProcess = Process.GetCurrentProcess();
			foreach (var p in processes.xSafeForEach())
			{
                if (p.Id != curProcess.Id)
				{
					p.Kill();
				}
			}
		}

        private static bool IsRuningForAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RestartElevated()
        {
            // 获取当前执行的exe文件路径
            string exeName = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // 启动新的进程，以管理员权限运行
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "runas";
            processStartInfo.FileName = exeName;

            try
            {
                Process.Start(processStartInfo);
				Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("提升权限失败: " + ex.Message, "错误");
            }
        }

    }
}
