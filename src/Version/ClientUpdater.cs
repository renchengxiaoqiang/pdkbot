using Microsoft.VisualBasic.Logging;
using PdkBot.BotLib.Extensions;
using PdkBot.BotLib.Misc;
using PdkBot.BotLib.Wpf;
using PdkBot.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace PdkBot.Version
{
    public class ClientUpdater
    {
        private static bool _isUpdating;
		private static string _patchFn;
		private static string _baseFn;
		static ClientUpdater()
		{
            _patchFn = Path.Combine(PathEx.ParentOfExePath, "patch");
			_baseFn = Path.Combine(PathEx.ParentOfExePath, "base");
		}

        public static async Task UpdateAsync(int newVersion)
        {
            await Task.Factory.StartNew(() => Update(newVersion), TaskCreationOptions.LongRunning);
        }

        public static void Update(int newVersion)
		{
			DispatcherEx.xInvoke(async () => { 
			
				if (!_isUpdating)
				{
					_isUpdating = true;
					var newVerDir = string.Empty;
					try
					{
						newVerDir = Path.Combine(PathEx.ParentOfExePath, ConvertVersionToString(newVersion));
						if (await DownFileAsync("", _patchFn))
						{
							DirectoryEx.Delete(newVerDir, true);
							CopyBaseFile(newVerDir);
							Zip.UnZipFile(_patchFn, newVerDir);
							File.Delete(_patchFn);
							//DeleteOldVersion(ent.PatchVersion);
							InstalledVersionManager.SaveVersionToConfigFile(newVersion);
							var msg = string.Format("{0}已升级到版本{1},软件将于10秒以后自动重启。\r\n\r\n升级信息:{2}", "ComfyUIApp", ConvertVersionToString(newVersion), "修复已知的Bug，优化使用体验");
							WndTrayTip.ShowTrayTip(msg, "软件升级", 15, string.Empty, () => Reboot());
						}
					}
					catch (Exception ex)
					{
                        //Log.Exception(ex);
                        DispatcherEx.xInvoke(() => MessageBox.Show(string.Format("升级失败，原因={0}", ex.Message), "提示", MessageBoxButton.OK, MessageBoxImage.Hand));
						DirectoryEx.Delete(newVerDir, true);
					}
					_isUpdating = false;
				}
			});
		}

        public static void Reboot()
        {
	        var fn = Path.Combine(PathEx.ParentOfExePath, "Booter.exe");
	        Process.Start(fn, "reboot");
			DispatcherEx.xInvoke(() =>
			{
				if (Application.Current != null)
				{
					Application.Current.Shutdown();
				}
			});
		}

		public static void DeleteOldVersion(int updatedVersion)
		{
			try
			{
				//var installedVersions = InstalledVersionManager.GetAllInstalledVersionAndSortByVersionDesc();
				//var deleteVersions = installedVersions.Where(k=>k.Version < updatedVersion);
				//foreach (var delVer in deleteVersions)
				//{
				//	Log.Info("delete version=" + delVer.Version);
				//	DirectoryEx.Delete(delVer.Path, true);
				//}				
			}
			catch (Exception ex)
			{
				//Log.Exception(ex);
			}
		}

        public static async Task<bool> DownFileAsync(string url, string localfn)
        {
			var rt = false;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 发送GET请求获取文件内容
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    // 读取文件内容为字节数组
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                    // 写入本地文件
                    await File.WriteAllBytesAsync(localfn, fileBytes);
					rt = response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"下载失败: {ex.Message}");
                }
            }
			return rt;
        }


        private static void CopyBaseFile(string destDir)
		{
			var installedVersion = GetNewestVersion();
			if (installedVersion==null || string.IsNullOrEmpty(installedVersion.Path))
			{
                MessageBox.Show("无法升级到最新版....请手动安装!!");
			}
			else
			{
				DirectoryEx.Copy(installedVersion.Path, destDir);
			}
		}

        private static InstalledVersion GetNewestVersion()
		{
			var newestVer = InstalledVersionManager.GetNewestVersion();
			return newestVer;
		}

        public static string ConvertVersionToString(int v)
        {
            int v1 = v / 10000;
            int v2 = v % 10000 / 100;
            int v3 = v % 100;
            return string.Format("v{0}.{1}.{2}", v1, v2, v3);
        }

        public static int ConvertStringToVersion(string vstr)
        {
            vstr = vstr.Trim().ToLower();
            var vs = vstr.Split('.');
            int v1 = Convert.ToInt32(vs[0].Substring(1));
            int v2 = Convert.ToInt32(vs[1]);
            int v3 = Convert.ToInt32(vs[2]);
            return v1 * 10000 + v2 * 100 + v3;
        }
    }
}
