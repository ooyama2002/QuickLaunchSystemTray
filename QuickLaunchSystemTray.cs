using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Quick Launch System Tray")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Quick Launch System Tray")]
[assembly: AssemblyCopyright("Copyright ©  2025")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("3032e0a5-2a0a-4886-b0aa-99dea518609b")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace QuickLaunchSystemTray
{
    public class Program
    {
        private static System.Windows.Forms.NotifyIcon NotifyIcon1;
        private static readonly List<string> EXCLUSION_DIRECTORY = new List<string>() { "User Pinned" };
        private static readonly List<string> EXCLUSION_FILE = new List<string>() { "desktop.ini" };
        private static readonly string QUICK_LAUNCH_PATH = GetQuickLaunchPath();

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            NotifyIcon1 = new NotifyIcon();
            NotifyIcon1.Icon = GetSelfIcon();
            NotifyIcon1.Visible = true;
            NotifyIcon1.Text = "Quick Launch System Tray";
            NotifyIcon1.Click += NotifyIcon1_Click;

            CreateContextMenu();

            Application.Run();
        }

        private static void NotifyIcon1_Click(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(NotifyIcon).GetMethod(
                "ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(NotifyIcon1, null);
        }

        private static Icon GetSelfIcon()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                return Icon.ExtractAssociatedIcon(assembly.Location);
            }
            return null;
        }

        private static void CreateContextMenu()
        {
            ContextMenuStrip strip = new ContextMenuStrip();
            CreateQuickLaunchMenu(strip, null, QUICK_LAUNCH_PATH);
            CreateSystemMenu(strip);
            NotifyIcon1.ContextMenuStrip = strip;
        }

        private static void CreateSystemMenu(ContextMenuStrip paramCms)
        {
            ToolStripMenuItemEx tsiG = new ToolStripMenuItemEx();
            tsiG.Text = "System Menu";

            ToolStripMenuItemEx tsi;
            tsi = new ToolStripMenuItemEx();
            tsi.Text = "Open Quick Launch Folder";
            tsi.Click += (sender, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start(QUICK_LAUNCH_PATH);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            tsiG.DropDownItems.Add(tsi);

            tsi = new ToolStripMenuItemEx();
            tsi.Text = "Refresh";
            tsi.Click += (sender, e) => { CreateContextMenu(); };
            tsiG.DropDownItems.Add(tsi);

            tsi = new ToolStripMenuItemEx();
            tsi.Text = "Quit";
            tsi.Click += (sender, e) => { Application.Exit(); };
            tsiG.DropDownItems.Add(tsi);

            paramCms.Items.Add(new ToolStripSeparator());
            paramCms.Items.Add(tsiG);
            paramCms.Items.Add(new ToolStripSeparator());
        }

        private static void CreateQuickLaunchMenu(ContextMenuStrip paramCms, ToolStripMenuItemEx paramTsi, string paramPath)
        {

            foreach (string d in Directory.GetDirectories(paramPath))
            {
                DirectoryInfo di = new DirectoryInfo(d);
                if (EXCLUSION_DIRECTORY.Contains(di.Name)) continue;
                ToolStripMenuItemEx tsi = new ToolStripMenuItemEx();
                tsi.Text = di.Name;
                tsi.Image = GetIconBitMap(di.FullName);
                tsi.Click += (sender, e) => { ToolStripMenuItem_Click(sender, e, di.FullName); };
                CreateQuickLaunchMenu(null, tsi, d);
                if (paramCms != null)
                {
                    paramCms.Items.Add(tsi);
                }
                else
                {
                    paramTsi.DropDownItems.Add(tsi);
                }
            }

            foreach (string f in Directory.GetFiles(paramPath))
            {
                FileInfo fi = new FileInfo(f);
                if (EXCLUSION_FILE.Contains(fi.Name)) continue;
                ToolStripMenuItemEx tsi = new ToolStripMenuItemEx();
                string name = fi.Name;
                if (name.EndsWith(".lnk"))
                {
                    name = name.Substring(0, name.LastIndexOf("."));
                }
                tsi.Text = name;
                tsi.Image = GetIconBitMap(fi.FullName);
                tsi.Click += (sender, e) => { ToolStripMenuItem_Click(sender, e, fi.FullName); };
                if (paramCms != null)
                {
                    paramCms.Items.Add(tsi);
                }
                else
                {
                    paramTsi.DropDownItems.Add(tsi);
                }
            }
        }

        private static void ToolStripMenuItem_Click(object sender, EventArgs e, string fullName)
        {
            if (((ToolStripMenuItemEx)sender).IsRightClick)
            {
                if (Directory.Exists(fullName))
                {
                    ContextMenuStrip strip2 = new ContextMenuStrip();
                    ToolStripMenuItem tsi;
                    tsi = new ToolStripMenuItem();
                    tsi.Text = "Open Folder";
                    tsi.Click += (sender1, e1) =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(fullName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };
                    strip2.Items.Add(tsi);
                    System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
                    strip2.Show(sp);
                }
                else if (File.Exists(fullName))
                {
                    ContextMenuStrip strip2 = new ContextMenuStrip();
                    ToolStripMenuItem tsi;

                    tsi = new ToolStripMenuItem();
                    tsi.Text = "Open file location";
                    tsi.Click += (sender1, e1) =>
                    {
                        if (fullName.EndsWith(".lnk"))
                        {
                            //IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                            //IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(fullName);
                            Type type = Type.GetTypeFromProgID("WScript.Shell");
                            dynamic shell = Activator.CreateInstance(type);
                            dynamic shortcut = shell.CreateShortcut(fullName);
                            string targetPath = shortcut.TargetPath;
                            if (string.IsNullOrEmpty(targetPath)) return;
                            targetPath = targetPath.Substring(0, targetPath.LastIndexOf("\\"));
                            try
                            {
                                System.Diagnostics.Process.Start(targetPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        else
                        {
                            string targetPath = fullName.Substring(0, fullName.LastIndexOf("\\"));
                            try
                            {
                                System.Diagnostics.Process.Start(targetPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    };
                    strip2.Items.Add(tsi);

                    tsi = new ToolStripMenuItem();
                    tsi.Text = "Properties";
                    tsi.Click += (sender1, e1) =>
                    {
                        try
                        {
                            Win32.ShowFileProperties(fullName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };
                    strip2.Items.Add(tsi);

                    System.Drawing.Point sp = System.Windows.Forms.Cursor.Position;
                    strip2.Show(sp);
                }
            }
            else
            {
                if (File.Exists(fullName))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(fullName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private static string GetQuickLaunchPath()
        {
            string[] cmds = System.Environment.GetCommandLineArgs();
            if (cmds.Length > 1)
            {
                if (Directory.Exists(cmds[1]))
                {
                    return cmds[1];
                }
            }

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            path += (path.EndsWith("\\") ? "" : "\\") + @"Microsoft\Internet Explorer\Quick Launch";
            return path;
        }

        private static System.Drawing.Bitmap GetIconBitMap(string fileFullName)
        {
            try
            {
                return GetIcon(fileFullName).ToBitmap();
            }
            catch (Exception ex)
            {
                Console.WriteLine(fileFullName);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                System.Drawing.Bitmap img = new System.Drawing.Bitmap(16, 16);
                return img;
            }
        }

        private static System.Drawing.Icon GetIcon(string fileFullName)
        {
            Win32.SHFILEINFO shinfo = new Win32.SHFILEINFO();
            IntPtr hImgSmall = Win32.SHGetFileInfo(fileFullName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
            return icon;
        }

        private class Win32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public IntPtr iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon  
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon  

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct SHELLEXECUTEINFO
            {
                public int cbSize;
                public uint fMask;
                public IntPtr hwnd;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpVerb;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpFile;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpParameters;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpDirectory;
                public int nShow;
                public IntPtr hInstApp;
                public IntPtr lpIDList;
                [MarshalAs(UnmanagedType.LPTStr)]
                public string lpClass;
                public IntPtr hkeyClass;
                public uint dwHotKey;
                public IntPtr hIcon;
                public IntPtr hProcess;
            }

            private const int SW_SHOW = 5;
            private const uint SEE_MASK_INVOKEIDLIST = 12;
            public static bool ShowFileProperties(string Filename)
            {
                SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
                info.cbSize = Marshal.SizeOf(info);
                info.lpVerb = "properties";
                info.lpFile = Filename;
                info.nShow = SW_SHOW;
                info.fMask = SEE_MASK_INVOKEIDLIST;
                return ShellExecuteEx(ref info);
            }
        }

        class ToolStripMenuItemEx : ToolStripMenuItem
        {
            public bool IsRightClick { get; set; }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                IsRightClick = (e.Button == MouseButtons.Right);
                base.OnMouseDown(e);
            }
        }
    }
}
