using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;

namespace FileLocations
{
    public class Locations
    {
        public static string UserDirectory = string.Empty;
        public static string CommonDirectory = string.Empty;
        public static string ApplicationDirectory = string.Empty;

        public static string ExeName = string.Empty;

        protected static string[] Args = null;
        public static string ThisExePath = string.Empty;
        public static string ThisExeConfigPath = string.Empty;

        public static string DataDirName = string.Empty;
        public static string CacheDirName = string.Empty;

        protected static string ApplicationDataDir = string.Empty;
        protected static string CommonDataDir = string.Empty;
        protected static string UserDataDir = string.Empty;

        protected static bool AppDataDirIsWritable = false;

        // mostly used for editors and things that don't call startup
        public static string DataPathOveride = string.Empty;

        public static void Statup (string exePath, string commonPath, string userPath, string exeName, string[] args)
        {
            Args = args;
            ThisExePath = exePath;

            UserDirectory = userPath;

            bool isBinDir = Directory.Exists(Path.Combine(Path.Combine(exePath,"../"),"bin"));

            string exePathNoBin = exePath;
            if (isBinDir)
                exePathNoBin = new DirectoryInfo(Path.Combine(Path.Combine(exePath, "../"), "bin")).FullName;

            if (commonPath != string.Empty)
                CommonDirectory = commonPath;

            string pathsDir = Path.Combine(GetLowestWriteableDir(),"paths.info");
            if (File.Exists(pathsDir))
            {
                FileInfo file = new FileInfo(pathsDir);
                StreamReader sr = file.OpenText();
                ApplicationDirectory = sr.ReadLine();
                CommonDirectory = sr.ReadLine();
                UserDirectory = sr.ReadLine();
                sr.Close();
            }

            ExeName = exeName;

            if (ApplicationDirectory == string.Empty || (exePathNoBin != commonPath && exePathNoBin != userPath))
            {
                // we are not in one of the relaunch paths, so we must be for real
                ApplicationDirectory = exePathNoBin;
            }
            else if (ApplicationDirectory == string.Empty)
                ApplicationDirectory = exePathNoBin;

            AppDataDirIsWritable = FolderIsWritable(ApplicationDataDir);

            ThisExeConfigPath = exePathNoBin;

            FileInfo outfile = new FileInfo(pathsDir);
            FileStream fs = outfile.OpenWrite();
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(ApplicationDirectory);
            sw.WriteLine(CommonDirectory); 
            sw.WriteLine(UserDirectory);
            sw.Close();
            fs.Close();
        }

        protected static void CheckDir(string dir)
        {
            if (Directory.Exists(dir))
                return;

            Directory.CreateDirectory(dir);
        }

        public static string GetChacheFolder()
        {
            string dir = Path.Combine(GetLowestWriteableDir(),"cache");
            CheckDir(dir);

            return dir;
        }

        public static bool LaunchHigherExe()
        {
            if (ThisExePath == UserDirectory)
                return false;

            string path = Path.Combine(UserDirectory, ExeName);
            if (File.Exists(path))
            {
                StartExe(path);
                return true;
            }

            if (ThisExePath == CommonDirectory) 
                return false;

            if (CommonDirectory != string.Empty)
            {
                path = Path.Combine(CommonDirectory, ExeName);
                if (File.Exists(path))
                {
                    StartExe(path);
                    return true;
                }
            }

            return false;
        }

        public static void Relaunch()
        {
            if (!LaunchHigherExe())
                StartExe(Path.Combine(ThisExePath, ExeName));
        }

        protected static void StartExe(string path)
        {
            string args = "";
            foreach (string arg in Args)
                args += arg + " ";
            ExcuteExe(path, args);
        }

        public static void ExcuteExe(string path, string args)
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Unix || System.Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo("mono", path + " " + args));
            }
            else
                System.Diagnostics.Process.Start(new ProcessStartInfo(path, args));
        }

        public static string GetLowestWriteableDir()
        {
            // do we have an all users dir.
            string configPath = CommonDirectory;

            if (configPath == string.Empty || !Directory.Exists(configPath) || !FolderIsWritable(configPath))
            {
                configPath = UserDirectory;

                if (!FolderIsWritable(configPath))
                    throw (new Exception("Unable to write to any config path"));
            }
            return configPath;
        }

        public static string GetLowestConfigDir()
        {
            return GetLowestWriteableDir();
        }

        public static string GetLowestDataDir()
        {
            if (AppDataDirIsWritable)
                return GetApplicationDataDir();

            if (CommonDirectory != string.Empty)
                return GetCommonDataDir();

            return GetUserDataDir();
        }

        public static string GetWritableDataFolder( string path)
        {
            // find the highest level that this file exists at
            string dir = GetUserDataDir();
            if (File.Exists(Path.Combine(dir, path)))
                return dir;

            if (CommonDirectory != string.Empty)
            {
                dir = GetCommonDataDir();
                if (File.Exists(Path.Combine(dir, path)))
                    return dir;
            }

            return GetLowestDataDir();
        }

        public static bool CreateSubDirsForFile(string root, string filePath)
        {
            string[] splits = filePath.Replace('\\','/').Split("/".ToCharArray());

            string subPath = (string)root.Clone();
            for (int i = 0; i < splits.Length - 1; i++)
            {
                subPath = Path.Combine(subPath, splits[i]);
                try
                {
                    if (!Directory.Exists(subPath))
                        Directory.CreateDirectory(subPath);
                }
                catch (System.Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        protected static bool FolderIsWritable(string path)
        {
            FileInfo file = new FileInfo(Path.Combine(path, "dir.info"));
            try
            {
                if (file.Exists)
                    file.Delete();

                FileStream fs = file.OpenWrite();
                if (!fs.CanWrite)
                {
                    fs.Close();
                    return false;
                }
                fs.WriteByte(1);
                fs.Close();
            }
            catch (System.Exception /*ex*/)
            {
                return false;
            }
            return true;
        }

        protected static void BuildDataDirs()
        {
            UserDataDir = Path.Combine(UserDirectory, DataDirName);
            if (CommonDirectory != string.Empty)
                CommonDataDir = Path.Combine(CommonDataDir, DataDirName);

            if (Directory.Exists(Path.Combine(ApplicationDirectory, "../" + DataDirName))) // check to see if we are in a bin dir
                ApplicationDataDir = new DirectoryInfo(Path.Combine(ApplicationDirectory, "../" + DataDirName)).FullName;
            else if (Directory.Exists(Path.Combine(ApplicationDirectory, "../../../" + DataDirName))) // check and see if we are in a debug dir
                ApplicationDataDir = new DirectoryInfo(Path.Combine(ApplicationDirectory, "../../../" + DataDirName)).FullName;
            else
                ApplicationDataDir = UserDataDir; // we have no idea where the data dir is
        }

        protected static Dictionary<string, string> PathCache = new Dictionary<string, string>();

        protected static string GetCleanPath(string path)
        {
            return new FileInfo(path).FullName;
        }

        public static string GetApplicationDataDir()
        {
            if (ApplicationDataDir == string.Empty)
                BuildDataDirs();

            return ApplicationDataDir;
        }

        public static string GetCommonDataDir()
        {
            if (ApplicationDataDir == string.Empty)
                BuildDataDirs();

            return CommonDataDir;
        }

        public static string GetUserDataDir()
        {
            if (ApplicationDataDir == string.Empty)
                BuildDataDirs();

            return UserDataDir;
        }

        public static string FindDataFile(string localPath)
        {
            if (PathCache.ContainsKey(localPath))
                return PathCache[localPath];

            if (DataPathOveride != string.Empty)
            {
                string dir = GetCleanPath(Path.Combine(DataPathOveride, localPath));
                if (File.Exists(dir))
                {
                    PathCache.Add(localPath, dir);
                    return dir;
                }
            }

            if (ApplicationDataDir == string.Empty)
                BuildDataDirs();

            // check the user dir
            string path = GetCleanPath(Path.Combine(UserDataDir, localPath));

            if (!File.Exists(path))
            {
                if (CommonDataDir != string.Empty)
                {
                    path = GetCleanPath(Path.Combine(CommonDataDir, localPath));

                    if (!File.Exists(path))
                        path = GetCleanPath(Path.Combine(ApplicationDataDir, localPath));
                }
            }

            PathCache.Add(localPath, path);
            return path;
        }
    }
}
