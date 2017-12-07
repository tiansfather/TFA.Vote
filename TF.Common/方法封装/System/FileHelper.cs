using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{
    public static class FileHelper
    {
        public static string GetTempFolder()
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", ""));
            Directory.CreateDirectory(temp);
            return temp;
        }
        public static string GetRelativePath(string path)
        {
            if (path.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)) return path;
            if (path.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)) return path;
            if (path.StartsWith("ftp://", StringComparison.CurrentCultureIgnoreCase)) return path;
            path = GetFullPath(path);

            if (HttpContext.Current != null)
            {
                return path.Substring(0, HttpContext.Current.Server.MapPath("/").Length);
            }
            return path.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length);
        }

        public static string GetFullPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("路径为空");
            if (Regex.IsMatch(@"^\S+:", filePath)) return filePath;

            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(filePath);
            }
            return Path.GetFullPath(filePath);
        }
        public static string GetDirectoryName(string path)
        {
            path = GetFullPath(path);
            return Path.GetDirectoryName(path);
        }
        public static string GetFileName(string path)
        {
            path = GetFullPath(path);
            return Path.GetFileName(path);
        }
        public static string GetFileNameWithoutExtension(string path)
        {
            path = GetFullPath(path);
            return Path.GetFileNameWithoutExtension(path);
        }
        public static string GetExtension(string path)
        {
            path = GetFullPath(path);
            return Path.GetExtension(path);
        }
        public static string ChangeExtension(string path, string extension)
        {
            path = GetFullPath(path);
            return ChangeExtension(path, extension);
        }

        public static FileInfo GetFileInfo(string path)
        {
            path = GetFullPath(path);
            return new FileInfo(path);
        }
        public static bool Exists(string path)
        {
            path = GetFullPath(path);
            return File.Exists(path);
        }

        public static void Rename(string path, string newFileName)
        {
            path = GetFullPath(path);
            if (Path.HasExtension(newFileName)==false)
            {
                newFileName = newFileName + Path.GetExtension(path);
            }
            FileSystem.RenameFile(path, newFileName);
        }

        public static FileStream Create(string filePath)
        {
            filePath = GetFullPath(filePath);
            var dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            return File.Create(filePath);
        }
        public static FileStream Create(string filePath, int bufferSize)
        {
            filePath = GetFullPath(filePath);
            var dir = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            return File.Create(filePath, bufferSize);
        }
        public static void Copy(string sourceFileName, string destFileName, bool overwrite = true)
        {
            sourceFileName = GetFullPath(sourceFileName);
            destFileName = GetFullPath(destFileName);
            if (File.Exists(sourceFileName) == false) throw new FileNotFoundException("未找到文件" + sourceFileName);

            var dir = Path.GetDirectoryName(destFileName);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            File.Copy(sourceFileName, destFileName, overwrite);
        }
        public static void AppendAllText(string path, string contents)
        {
            AppendAllText(path, contents, Encoding.UTF8);
        }
        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            path = GetFullPath(path);
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(path) == false)
            {
                File.Create(path).Close();
            }
            File.AppendAllText(path, contents, encoding);
        }
        public static FileStream Open(string path, FileMode mode)
        {
            path = GetFullPath(path);
            //var dir = Path.GetDirectoryName(path);
            //if (Directory.Exists(dir) == false)
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //if (File.Exists(path) == false)
            //{
            //    File.Create(path).Close();
            //}
            return File.Open(path, mode);
        }
        public static FileStream Open(string path, FileMode mode, FileAccess access)
        {
            path = GetFullPath(path);
            //var dir = Path.GetDirectoryName(path);
            //if (Directory.Exists(dir) == false)
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //if (File.Exists(path) == false)
            //{
            //    File.Create(path).Close();
            //}
            return File.Open(path, mode, access);
        }
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            path = GetFullPath(path);
            //var dir = Path.GetDirectoryName(path);
            //if (Directory.Exists(dir) == false)
            //{
            //    Directory.CreateDirectory(dir);
            //}
            //if (File.Exists(path) == false)
            //{
            //    File.Create(path).Close();
            //}
            return File.Open(path, mode, access, share);
        }
        public static byte[] ReadAllBytes(string path)
        {
            path = GetFullPath(path);
            return File.ReadAllBytes(path);
        }
        public static string[] ReadAllLines(string path)
        {
            path = GetFullPath(path);
            return File.ReadAllLines(path, Encoding.UTF8);
        }
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            path = GetFullPath(path);
            return File.ReadAllLines(path, encoding);
        }
        public static string ReadAllText(string path)
        {
            path = GetFullPath(path);
            return File.ReadAllText(path, Encoding.UTF8);
        }
        public static string ReadAllText(string path, Encoding encoding)
        {
            path = GetFullPath(path);
            return File.ReadAllText(path, encoding);
        }
        public static void WriteAllBytes(string path, byte[] bytes)
        {
            path = GetFullPath(path);
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(path) == false)
            {
                File.Create(path).Close();
            }
            File.WriteAllBytes(path, bytes);
        }
        public static void WriteAllLines(string path, string[] contents)
        {
            WriteAllLines(path, contents, Encoding.UTF8);
        }
        public static void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            path = GetFullPath(path);
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(path) == false)
            {
                File.Create(path).Close();
            }
            File.WriteAllLines(path, contents, encoding);
        }
        public static void WriteAllText(string path, string contents)
        {
            WriteAllText(path, contents, Encoding.UTF8);
        }
        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            path = GetFullPath(path);
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(path) == false)
            {
                File.Create(path).Close();
            }
            File.WriteAllText(path, contents, encoding);
        }
        public static void Move(string sourceFileName, string destFileName, bool overwrite = true)
        {
            sourceFileName = GetFullPath(sourceFileName);
            destFileName = GetFullPath(destFileName);
            if (File.Exists(sourceFileName) == false) throw new FileNotFoundException("未找到文件" + sourceFileName);

            var dir = Path.GetDirectoryName(destFileName);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (overwrite)
            {
                if (File.Exists(destFileName))
                {
                    File.Delete(destFileName);
                }
            }
            File.Move(sourceFileName, destFileName);
        }
        public static void Delete(string path)
        {
            path = GetFullPath(path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public static void DeleteToRecycleBin(string path)
        {
            path = GetFullPath(path);
            if (!System.IO.File.Exists(path)) return;
            FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }


        public static void CreateFolder(string path)
        {
            path = GetFullPath(path);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        public static void DeleteFolder(string path)
        {
            GC.Collect();
            path = GetFullPath(path);
            DeleteFolder2(path);
        }
        private static void DeleteFolder2(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string childName in Directory.GetFileSystemEntries(path))//获取子文件和子文件夹
                {
                    if (File.Exists(childName)) //如果是文件
                    {
                        File.SetAttributes(childName, FileAttributes.Normal);
                        File.Delete(childName); //直接删除其中的文件    
                    }
                    else
                    {
                        DeleteFolder2(childName); //递归删除子文件夹   
                    }
                }
                try
                {
                    Directory.Delete(path, true);
                }
                catch (IOException)
                {
                    Directory.Delete(path, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(path, true);
                }

            }
        }
        public static void DeleteFolderToRecycleBin(string dirpath)
        {
            dirpath = GetFullPath(dirpath);
            if (!System.IO.Directory.Exists(dirpath)) return;
            FileSystem.DeleteDirectory(dirpath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }
        public static void CopyFolder(string OldDirectoryPath, string NewDirectoryPath)
        {
            OldDirectoryPath = GetFullPath(OldDirectoryPath);
            NewDirectoryPath = GetFullPath(NewDirectoryPath);
            CopyDirectory2(OldDirectoryPath, NewDirectoryPath);
        }
        private static void CopyDirectory2(string OldDirectoryPath, string NewDirectoryPath)
        {
            DirectoryInfo OldDirectory = new DirectoryInfo(OldDirectoryPath);
            DirectoryInfo NewDirectory = new DirectoryInfo(NewDirectoryPath);
            if (!NewDirectory.Exists)
            {
                NewDirectory.Create();
            }
            foreach (FileInfo file in OldDirectory.GetFiles())
            {
                file.CopyTo(Path.Combine(NewDirectory.FullName, file.Name), true);
            }
            foreach (DirectoryInfo subDirectory in OldDirectory.GetDirectories())
            {
                CopyDirectory2(subDirectory.FullName, Path.Combine(NewDirectory.FullName, subDirectory.Name));
            }
        }
        public static void MoveFolder(string oldFolder, string newFolder)
        {
            oldFolder = GetFullPath(oldFolder);
            newFolder = GetFullPath(newFolder);
            MoveFolder2(oldFolder, newFolder);
        }
        private static void MoveFolder2(string oldFolder, string newFolder)
        {
            DirectoryInfo OldDirectory = new DirectoryInfo(oldFolder);
            DirectoryInfo NewDirectory = new DirectoryInfo(newFolder);
            if (!NewDirectory.Exists)
            {
                NewDirectory.Create();
            }
            foreach (FileInfo file in OldDirectory.GetFiles())
            {
                var path = Path.Combine(NewDirectory.FullName, file.Name);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                file.MoveTo(Path.Combine(NewDirectory.FullName, file.Name));
            }
            foreach (DirectoryInfo subDirectory in OldDirectory.GetDirectories())
            {
                MoveFolder2(subDirectory.FullName, Path.Combine(NewDirectory.FullName, subDirectory.Name));
            }
            try
            {
                Directory.Delete(oldFolder, false);
            }
            catch (IOException)
            {
                Directory.Delete(oldFolder, false);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(oldFolder, false);
            }
        }

        public static string[] GetFiles(string dirpath)
        {
            dirpath = GetFullPath(dirpath);
            return Directory.GetFiles(dirpath);
        }
        public static string[] GetFolders(string dirpath)
        {
            dirpath = GetFullPath(dirpath);
            return Directory.GetDirectories(dirpath);
        }

        public static void RenameFolder(string path,string newName)
        {
            path = GetFullPath(path);
            FileSystem.RenameDirectory(path, newName);
        }

    }
}

namespace System.Windows.Forms
{
    public static class FileExt
    {
        #region 02 打开文件 文件夹

        public static void OpenFile(string filePath)
        {
            if (!File.Exists(filePath)) throw new Exception("文件不存在");
            System.Diagnostics.Process.Start(filePath);
        }

        public static void OpenFile(string filePath, string startExe)
        {
            if (!File.Exists(filePath)) throw new Exception("文件不存在");
            System.Diagnostics.Process.Start(startExe, filePath);
        }

        public static void OpenFolder(string folderPath)
        {
            var ext = Path.GetExtension(folderPath);

            if (!File.Exists(folderPath)) return;
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                psi.Arguments = " /select," + folderPath;
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception)
            {
                System.Diagnostics.Process.Start(Path.GetDirectoryName(folderPath));
                throw;
            }
        }

        public static void ExecCommand(string exePath, string cmd)
        {
            System.Diagnostics.Process.Start(exePath, cmd);
        }

        public static void ExecCommand(string p_Command)
        {
            System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(p_Command);
            p.UseShellExecute = true;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = p;
            process.Start();
        }

        #endregion 02 打开文件 文件夹

        public static string AppendFileName(string filePath, string appendText)
        {
            var dir = Path.GetDirectoryName(filePath);
            var filename = Path.GetFileNameWithoutExtension(filePath);
            var ex = Path.GetExtension(filePath);
            return Path.Combine(dir, filename + appendText + ex);
        }

        public static string GetDesktopPath()
        {
            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        }

        public static string GetDllPath(Type type)
        {
            return type.Assembly.Location;
        }

        #region 08 文件图标

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        private class Win32
        {
            public const uint SHGFI_ICON = 0x000000100;// get icon
            public const uint SHGFI_LARGEICON = 0x000000000;// get large icon
            public const uint SHGFI_SMALLICON = 0x000000001;// get small icon
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;// use passed dwFileAttribute
            public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll")]
            public static extern int DestroyIcon(System.IntPtr hIcon);
        }

        public static Bitmap GetLargeIconAsBitmap(string fileName)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            if (fileName.Contains("\\"))
            {
                Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
            }
            else
            {
                Win32.SHGetFileInfo(fileName, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_USEFILEATTRIBUTES);
            }
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Bitmap btp = (Bitmap)Bitmap.FromHicon(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return btp;
        }

        public static Bitmap GetSmallIconAsBitmap(string fileName)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            if (fileName.Contains("\\"))
            {
                Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            }
            else
            {
                Win32.SHGetFileInfo(fileName, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES);
            }
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Bitmap btp = (Bitmap)Bitmap.FromHicon(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return btp;
        }

        public static Icon GetLargeIcon(string fileName)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            if (fileName.Contains("\\"))
            {
                Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);
            }
            else
            {
                Win32.SHGetFileInfo(fileName, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON | Win32.SHGFI_USEFILEATTRIBUTES);
            }
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Icon shellIcon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return shellIcon;
        }

        public static Icon GetSmallIcon(string fileName)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            if (fileName.Contains("\\"))
            {
                Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
            }
            else
            {
                Win32.SHGetFileInfo(fileName, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON | Win32.SHGFI_USEFILEATTRIBUTES);
            }
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Icon shellIcon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return shellIcon;
        }

        #endregion 08 文件图标
    }
}