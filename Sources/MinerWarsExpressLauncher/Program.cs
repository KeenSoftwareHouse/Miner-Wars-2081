using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Text;

namespace MinerWarsExpressLauncher
{
    static class Program
    {
        static readonly string ExeName = "MinerWars.exe";
        static readonly string DefaultPath = @"C:\Program Files (x86)\Steam\steamapps\common\MinerWars\MinerWars.exe";

        static string FromRegistryPath
        {
            get
            {
                string steamPath = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam").GetValue("SteamPath").ToString();
                return Path.Combine(steamPath, "steamapps", "common", "MinerWars", ExeName);
            }
        }

        static string InstallPath64
        {
            get
            {
                string installPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 223430").GetValue("InstallLocation").ToString();
                return Path.Combine(installPath, ExeName);
            }
        }

        static string InstallPath
        {
            get
            {
                string installPath = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 223430").GetValue("InstallLocation").ToString();
                return Path.Combine(installPath, ExeName);
            }
        }

        static string TestPaths(params string[] paths)
        {
            foreach (var p in paths)
            {
                try
                {
                    FileInfo fi = new FileInfo(p);
                    if (fi.Exists && fi.Name == ExeName)
                        return p;
                }
                catch { }
            }
            return null;
        }

        static bool VerifySteamAppIdTxt(string path)
        {
            const string steam_appid = "steam_appid.txt";
            string dir = Path.GetDirectoryName(path);
            string appIdPath = Path.Combine(dir, steam_appid);

            if (!File.Exists(appIdPath))
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.FileName = "cmd";
                    p.StartInfo.Arguments = string.Format("/C \"echo 223430> \"{0}\"\"", appIdPath);
                    p.StartInfo.UseShellExecute = true;
                    if (System.Environment.OSVersion.Version.Major >= 6)
                    {
                        p.StartInfo.Verb = "runas";
                    }
                    if (!p.Start())
                    {
                        throw new InvalidOperationException();
                    }
                    p.WaitForExit();
                }
                catch
                {
                    StringBuilder text = new StringBuilder();
                    text.AppendFormat("Cannot create file '{0}'", appIdPath);
                    text.AppendLine();
                    text.Append("Please create this file and write '223430' (without quotes) into this file");
                    MessageBox.Show(text.ToString(), String.Format("Cannot create {0}", steam_appid));
                    return false;
                }
            }
            return true;
        }

        [STAThread]
        static void Main(string[] args)
        {
            string path = TestPaths(args.FirstOrDefault(), InstallPath, FromRegistryPath, DefaultPath);

            while (path == null)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Title = "Please locate MinerWars.exe";
                dlg.Filter = "MinerWars.exe |MinerWars.exe";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                dlg.Multiselect = false;
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                path = TestPaths(dlg.FileName);
            }
            if (VerifySteamAppIdTxt(path))
            {
                AppDomain.CurrentDomain.ExecuteAssembly(path, new string[] { "-FromLauncher", "-modd", "-restarted" });
            }
        }
    }
}
