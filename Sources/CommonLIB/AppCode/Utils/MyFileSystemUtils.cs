using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;

namespace SysUtils.Utils
{
    /// <summary>
    /// MyFileSystemUtils
    /// </summary>
    public class MyFileSystemUtils
    {
        /// <summary>
        /// Creates folder with subfolder for this file, if already doesn't exists
        /// </summary>
        /// <param name="file"></param>
        public static void CreateFolderForFile(String file)
        {
            int i_IndexOfSlash = file.LastIndexOf(@"\");
            if (i_IndexOfSlash >= 0)
            {
                String folder = file.Substring(0, i_IndexOfSlash);
                if (FolderExists(folder) == false)
                {
                    CreateFolder(folder);
                }
            }
        }


        /// <summary>
        /// Checks if specified folder exists
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool FolderExists(String folderPath)
        {
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(folderPath);
            return directoryInfo.Exists;
        }



        /// <summary>
        //	Vytvori zadany adresar. Automaticky povytvara celu adresarovu strukturu, teda ak chcem vytvorit c:\volaco\opica
        //	a c:\volaco zatial neexistuje, tak tato metoda ho vytvori.
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateFolder(String folderPath)
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }


        //  Return folder where our application can store user specific data (log, config, ...)
        //  On my machine it's: c:\Users\Usa\AppData\Roaming\MinerWars\
        public static string GetApplicationUserDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MinerWars\\");
        }


        //  Compute hash from file's content and return it as a Base64 string.  
        public static string GetFileHash(string filename)
        {
            using (HashAlgorithm hashAlg = new SHA1Managed())
            {
                using (Stream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    byte[] hash = hashAlg.ComputeHash(file);
                    return Convert.ToBase64String(hash);
                }
            }
        }

        //  Compute hash from file's content and return it as a Base64 string. (overload with specific sharing mode)
        public static string GetFileHash(string filename, FileShare sharingMode)
        {
            if (!FileExists(filename))
            {
                return "";
            }

            using (HashAlgorithm hashAlg = new SHA1Managed())
            {
                {
                    using (Stream file = new FileStream(filename, FileMode.Open, FileAccess.Read, sharingMode))
                    {
                        byte[] hash = hashAlg.ComputeHash(file);
                        return Convert.ToBase64String(hash);
                    }
                }
            }
        }

        //	Checks if specified file exists
        public static bool FileExists(String filePath)
        {
            return File.Exists(filePath);
        }


        //  Download file synchronously - blocks caller until download is finished. Use only for small files. 
        //  This method doesn't have timeout, because WebClient doesn't have it.
        //  Return true if OK. If any exception occurs, we ignore it and return false.
        public static bool DownloadFileSynchronously(string url, string filepath)
        {
            WebClient client = null;
            try
            {
                client = new WebClient();

                //  Setting PROXY to null is very important, because if it is null, this web request will not
                //  use any proxy. It will connect to our update server directly, no matter what is specified
                //  in Internet Explorer proxy settings.
                //  If we leave there default values, it works on 99% of computers. But 1% has some weird proxy
                //  settings in IE - they don't use proxy, but this web request thinks they use proxy...
                //  E.g. http://dotnetdreaming.wordpress.com/2009/03/20/httpwebrequest-and-proxies/
                client.Proxy = null;

                client.DownloadFile(url, filepath);
            }
            catch (Exception ex)
            {
                MyMwcLog.WriteLine(ex);
                //  Ignore exception!
                return false;
            }
            finally
            {
                if (client != null) client.Dispose();
            }

            return true;
        }
    }
}
