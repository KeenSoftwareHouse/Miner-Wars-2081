using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace SysUtils.Utils
{
    public class MyStringUtils
    {
        public const string C_CRLF = "\r\n";
        /// <summary>
        /// SearchStringInArray
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="lookFor"></param>
        /// <returns></returns>
        public static bool SearchStringInArray(string[] strings, string lookFor)
        {
            foreach (string s in strings)
            {
                if (s.Equals(lookFor)) return true;
            }
            return false;
        }


        //  Example: for AlignIntToRight(12, 4, "0") it returns "0012"
        public static string AlignIntToRight(int value, int charsCount, char ch)
        {
            string ret = value.ToString();
            int length = ret.Length;
            if (length > charsCount) return ret;
            return new string(ch, charsCount - length) + ret;
        }


        //  Download string from specified url using specified timeout. If url doesn't exist or 
        //  timeout is raised, method returns null. Otherwise content of the url.
        //  Method ignores any exception that occurs during its execution!
        public static string DownloadStringSynchronously(string url, int timeoutInMilliseconds, string userAgent, Encoding fileEncoding)
        {
            MyMwcLog.WriteLine("DownloadStringSynchronously - START");
            MyMwcLog.IncreaseIndent();

            MyMwcLog.WriteLine("Url: " + url);
            MyMwcLog.WriteLine("TimeoutInMilliseconds: " + timeoutInMilliseconds);

            string pageContent = null;

            int retryCount = 3;
            while (retryCount-- > 0)
            {
                try
                {
                    // Open a connection
                    HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);

                    // You can also specify additional header values like the user agent or the referer:
                    webRequest.UserAgent = userAgent;
                    webRequest.Timeout = timeoutInMilliseconds;

                    //  Setting PROXY to null is very important, because if it is null, this web request will not
                    //  use any proxy. It will connect to our update server directly, no matter what is specified
                    //  in Internet Explorer proxy settings.
                    //  If we leave there default values, it works on 99% of computers. But 1% has some weird proxy
                    //  settings in IE - they don't use proxy, but this web request thinks they use proxy...
                    //  E.g. http://dotnetdreaming.wordpress.com/2009/03/20/httpwebrequest-and-proxies/
                    webRequest.Proxy = null;

                    // Request response:
                    WebResponse response = null;
                    try
                    {
                        response = webRequest.GetResponse();

                        // Open data stream:
                        Stream stream = null;
                        try
                        {
                            stream = response.GetResponseStream();

                            // Create reader object:
                            StreamReader reader = null;
                            try
                            {
                                reader = new StreamReader(stream, fileEncoding);

                                // Read the entire stream content:
                                pageContent = reader.ReadToEnd();
                            }
                            catch (Exception ex)
                            {
                                pageContent = null;
                                MyMwcLog.WriteLine(ex);
                                //  Ignore exception!
                            }
                            finally
                            {
                                if (reader != null) reader.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            pageContent = null;
                            MyMwcLog.WriteLine(ex);
                            //  Ignore exception!
                        }
                        finally
                        {
                            if (stream != null) stream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        pageContent = null;
                        MyMwcLog.WriteLine(ex);
                        //  Ignore exception!
                    }
                    finally
                    {
                        if (response != null) response.Close();
                    }
                }
                catch (Exception ex)
                {
                    pageContent = null;
                    MyMwcLog.WriteLine(ex);
                    //  Ignore exception!
                }

                if (pageContent != null)
                    break;
                else
                {
                    Thread.Sleep(3000);
                    MyMwcLog.WriteLine("Retrying...");
                }
            }

            //MyMwcLog.AddToLog("Page content: " + pageContent);
            MyMwcLog.WriteLine("Characters count: " + (pageContent == null ? "null" : pageContent.Length.ToString()));

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("DownloadStringSynchronously - END");

            return pageContent;
        }


        //  Returns "charactersCountFromRight" characters from right, but never exceed string's length
        //  E.g. GetStringRight("abcdefgh", 3) returns "fgh", but GetStringRight("abcdefgh", 1000) returns "abcdefgh"
        public static string GetStringRight(string s, int charactersCountFromRight)
        {
            if (s.Length <= charactersCountFromRight)
            {
                return s;
            }
            else
            {
                return s.Substring(s.Length - charactersCountFromRight, charactersCountFromRight);
            }
        }

        public static string EscapeSemicolons(string s)
        {
            var result = new StringBuilder();
            for (int i = 0; i < s.Length; i++) switch (s[i])
            {
                case '\\': result.Append("\\\\"); break; // \ -> \\
                case '\'': result.Append("\\A"); break;  // ' -> \A
                case '"': result.Append("\\Q"); break;   // " -> \Q
                case ';': result.Append("\\B"); break;   // ; -> \B
                case '\n': result.Append("\\n"); break;  // newline -> \n
                case '\r': result.Append("\\r"); break;  // carriage return -> \r
                case '\t': result.Append("\\t"); break;  // tab -> \t
                default: result.Append(s[i]); break;
            }
            return result.ToString();
        }

        public static string UnescapeSemicolons(string s)
        {
            var result = new StringBuilder();
            for (int i = 0; i < s.Length; i++) switch (s[i])
            {
                case '\\':
                    if (++i == s.Length) result.Append("\\");  // '\' at the end of input: inconcievable!
                    else switch (s[i])
                    {
                        case '\\': result.Append("\\"); break; // \\ -> \
                        case 'A': result.Append("'"); break;   // \A -> '
                        case 'Q': result.Append("\""); break;  // \Q -> "
                        case 'B': result.Append(";"); break;   // \B -> ;
                        case 'n': result.Append("\n"); break;  // \n -> newline
                        case 'r': result.Append("\r"); break;  // \r -> carriage return
                        case 't': result.Append("\t"); break;  // \t -> tab
                        default: result.Append("\\").Append(s[i]); break;  // unknown char, copy verbatim
                    }
                    break;
                default:
                    result.Append(s[i]);
                    break;
            }
            return result.ToString();
        }
    
    }
}
