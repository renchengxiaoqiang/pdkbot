using PdkBot.BotLib.Common;
using PdkBot.BotLib.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdkBot.Version
{
    public class InstalledVersionManager
    {
        private static string _startUpPathOfExe;

        public static string GetExeFileName()
        {
            var fn = GetExeFileNameFromConfigFile();
            if (string.IsNullOrEmpty(fn))
            {
                fn = GetMaxVersionExeFileName();
            }
            return fn;
        }

        private static string GetMaxVersionExeFileName()
        {
            var fn = string.Empty;
            var newestVersion = GetNewestVersion();
            if (newestVersion != null)
            {
                fn = newestVersion.Path + "\\Bot.exe";
            }
            if (!string.IsNullOrEmpty(fn) && !File.Exists(fn))
            {
                fn = string.Empty;
            }
            return fn;
        }

        public static void SaveVersionToConfigFile(int v)
        {
            var fn = PathEx.ParentOfExePath + "config.ini";
            File.WriteAllText(fn, ShareUtil.ConvertVersionToString(v));
        }

        private static string GetExeFileNameFromConfigFile()
        {
            var exeFn = string.Empty;
            try
            {
                var path = StartUpPathOfExe + "Config.ini";
                using (var reader = new StreamReader(path))
                {
                    string fullVersion = reader.ReadToEnd().Trim();
                    if (fullVersion.StartsWith("v"))
                    {
                        fullVersion = fullVersion.Substring(1);
                        string[] subVs = fullVersion.Split('.');
                        int version = Convert.ToInt32(subVs[0]) * 10000;
                        version += Convert.ToInt32(subVs[1]) * 100;
                        version += Convert.ToInt32(subVs[2]);
                        exeFn = GetExistPathByVersion(version) + "\\Bot.exe";
                    }
                }
                if (!string.IsNullOrEmpty(exeFn) && !File.Exists(exeFn))
                {
                    exeFn = null;
                }
            }
            catch
            {
                exeFn = null;
            }
            return exeFn;
        }

        public static List<InstalledVersion> GetAllInstalledVersionAndSortByVersionDesc()
        {
            var installedVersions = new List<InstalledVersion>();
            var dirts = Directory.GetDirectories(PathEx.ParentOfExePath, "v*");
            if (dirts != null && dirts.Length != 0)
            {
                foreach (string dir in dirts)
                {
                    var versionFromDir = GetVersionFromDir(dir);
                    if (versionFromDir > 0)
                    {
                        installedVersions.Add(new InstalledVersion
                        {
                            Path = dir,
                            Version = versionFromDir
                        });
                    }
                }
            }
            installedVersions = installedVersions.OrderByDescending(k => k.Version).ToList();
            return installedVersions;
        }

        private static string StartUpPathOfExe
        {
            get
            {
                if (_startUpPathOfExe == null)
                {
                    string fileName = Process.GetCurrentProcess().MainModule.FileName;
                    _startUpPathOfExe = Path.GetDirectoryName(fileName) + "\\";
                }
                return _startUpPathOfExe;
            }
        }

        public static InstalledVersion GetNewestVersion()
        {
            InstalledVersion newestVersion = null;
            var allInstalledVersions = GetAllInstalledVersionAndSortByVersionDesc();
            if (allInstalledVersions != null && allInstalledVersions.Count() > 0)
            {
                newestVersion = allInstalledVersions[0];
            }
            return newestVersion;
        }

        private static string GetExistPathByVersion(int version)
        {
            var path = string.Empty;
            var allInstalledVersions = GetAllInstalledVersionAndSortByVersionDesc();
            if (allInstalledVersions != null && allInstalledVersions.Count() > 0)
            {
                InstalledVersion installedVersion = allInstalledVersions.SingleOrDefault(k => k.Version == version);
                if (installedVersion != null)
                {
                    path = installedVersion.Path;
                }
            }
            return path;
        }

        private static int GetVersionFromDir(string dir)
        {
            var version = -1;
            try
            {
                var versionIdx = dir.LastIndexOf('\\');
                if (versionIdx > 0)
                {
                    var fullVersion = dir.Substring(versionIdx + 2);
                    var subVs = fullVersion.Split('.');
                    version = Convert.ToInt32(subVs[0]) * 10000;
                    version += Convert.ToInt32(subVs[1]) * 100;
                    version += Convert.ToInt32(subVs[2]);
                }
            }
            catch
            {
                version = -1;
            }
            return version;
        }

    }

    public class InstalledVersion
    {
        public string Path;
        public int Version;
    }
}
