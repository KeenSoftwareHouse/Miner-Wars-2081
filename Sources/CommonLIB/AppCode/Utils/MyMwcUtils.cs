using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MinerWarsMath;
using SysUtils.Utils;
using System.Diagnostics;
using System.Collections;

namespace MinerWars.CommonLIB.AppCode.Utils
{
    public static partial class MyMwcUtils
    {
        [ThreadStatic]
        static Random m_secretRandom;

        static Random m_random
        {
            get
            {
                if (m_secretRandom == null)
                {
                    m_secretRandom = new Random();
                }
                return m_secretRandom;
            }
        }

        //  Email validation regex
        //  I don't recomment using REGULAR_EXPRESSION_EMAIL_DOESNT_CATCH_ALL_FORMAT_MISMATCH because it doesn't catch for example: name.@domain.com (dot before @)
        //  On the other hand, REGULAR_EXPRESSION_EMAIL_GOOD_BUT_FAILS_ON_LONG_TEXT is better - catches everything, but ends in infinite processing if text is longer than 150 or 200 characters
        const string REGULAR_EXPRESSION_EMAIL_DOESNT_CATCH_ALL_FORMAT_MISMATCH = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        const string REGULAR_EXPRESSION_EMAIL_GOOD_BUT_FAILS_ON_LONG_TEXT = @"^([0-9a-zA-Z-_\+]([\.]?[0-9a-zA-Z-_\+])*@([0-9a-zA-Z]([0-9a-zA-Z-_])*\.)+[a-zA-Z]{2,9})$";

        static string REGULAR_EXPRESSION_ALPHA_NUMERIC = @"[^a-zA-Z0-9]";
        static char[] m_charToInt = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static readonly NumberFormatInfo MONEY_BOOKERS_FORMATTER;
        public static readonly NumberFormatInfo CARD_PAY_FORMATTER;
        public static readonly NumberFormatInfo GOCASH_FORMATTER;
        public static readonly CultureInfo GAME_CULTURE_INFO;
        public static readonly StringBuilder EmptyStringBuilder = new StringBuilder();

        public const float DENORMALIZED_BOUND = 1E-38f;

        static MyMwcUtils()
        {
            //m_random = new Random();

            MONEY_BOOKERS_FORMATTER = new NumberFormatInfo();
            MONEY_BOOKERS_FORMATTER.NumberDecimalSeparator = ".";
            MONEY_BOOKERS_FORMATTER.NumberGroupSeparator = "";
            MONEY_BOOKERS_FORMATTER.NumberDecimalDigits = 2;

            GOCASH_FORMATTER = new NumberFormatInfo();
            GOCASH_FORMATTER.NumberDecimalSeparator = ".";
            GOCASH_FORMATTER.NumberGroupSeparator = "";
            GOCASH_FORMATTER.NumberDecimalDigits = 2;

            CARD_PAY_FORMATTER = new NumberFormatInfo();
            CARD_PAY_FORMATTER.NumberDecimalSeparator = ".";
            CARD_PAY_FORMATTER.NumberGroupSeparator = "";
            CARD_PAY_FORMATTER.NumberDecimalDigits = 2;

            GAME_CULTURE_INFO = new CultureInfo("en-US");
            GAME_CULTURE_INFO.NumberFormat = GAME_CULTURE_INFO.NumberFormat.Clone() as NumberFormatInfo;
            GAME_CULTURE_INFO.NumberFormat.CurrencyNegativePattern = 1;
        }

        public static void CheckFloatValues(object graph, string name, ref double? min, ref double? max)
        {
            if (new StackTrace().FrameCount > 1000)
            {
                Debug.Fail("Infinite loop?");
            }

            if (graph == null) return;

            if (graph is float)
            {
                var val = (float)graph;
                if (float.IsInfinity(val) || float.IsNaN(val))
                {
                    Debug.Fail("invalid number");
                    throw new InvalidOperationException("Invalid value: " + name);
                }

                if (!min.HasValue || val < min) min = val;
                if (!max.HasValue || val > max) max = val;
            }

            if (graph is double)
            {
                var val = (double)graph;
                if (double.IsInfinity(val) || double.IsNaN(val))
                {
                    Debug.Fail("invalid number");
                    throw new InvalidOperationException("Invalid value: " + name);
                }

                if (!min.HasValue || val < min) min = val;
                if (!max.HasValue || val > max) max = val;
            }

            if (graph.GetType().IsPrimitive || graph is string || graph is DateTime) return;

            if (graph as IEnumerable != null)
            {
                foreach (var item in graph as IEnumerable)
                {
                    CheckFloatValues(item, name + "[]", ref min, ref max);
                }
                return;
            }
            foreach (var f in graph.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                CheckFloatValues(f.GetValue(graph), name + "." + f.Name, ref min, ref max);
            }
            foreach (var p in graph.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                CheckFloatValues(p.GetValue(graph, null), name + "." + p.Name, ref min, ref max);
            }
        }

        public static byte[] GetRandomByteArray(int size)
        {
            byte[] secretkey = new byte[size];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(secretkey);
            return secretkey;
        }

        //  This method is special for windows services!! Other ways of getting current folder won't work.
        public static string GetWindowsServiceCurrentFolder()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static float SphereVolume(float radius)
        {
            return (4.0f / 3.0f) * MathHelper.Pi * (float)Math.Pow(radius, 3);
        }

        //  Returns hashed password (used when converting password to hashed password)
        //  This is the same method as we use on the website www.minerwars.com
        //  The only disadvantage is that I had to add reference to System.Web - which is weird. Though I hope it won't make problems as it should be OK on all Windows.
        //  Uses SHA1 - don't change it!!! Existing methods and data depends on it.
        public static string GetHashedPassword(string password)
        {
            var hashAlgorithm = SHA1.Create();

            return BitConverter.ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", string.Empty);
        }

        //  Returns the MD5 hash for any given string
        //  Uses MD5 - don't change it!!! Existing methods and data depends on it.
        public static string GetMD5HashedPassword(string password)
        {
            var hashAlgorithm = MD5.Create();

            return BitConverter.ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", string.Empty);
        }

        //  Run process with request for elevation. That process requires admin/UAC rights. In Vista if UAC is on, it will
        //  request confirmation. If user isn't admin, it will ask him to log in as admin.
        //public static void RunElevatedProcess(string fileName, string arguments)
        //{
        //    ProcessStartInfo processInfo = new ProcessStartInfo();
        //    processInfo.Verb = "runas";
        //    processInfo.FileName = fileName;
        //    processInfo.Arguments = arguments;
        //    Process.Start(processInfo);
        //}

        //  Get all files in folder and sub-folders
        public static List<string> GetFilesRecursive(string folder)
        {
            // 1.
            // Store results in the file results list.
            List<string> result = new List<string>();

            // 2.
            // Store a stack of our directories.
            Stack<string> stack = new Stack<string>();

            // 3.
            // Add initial directory.
            stack.Push(folder);

            // 4.
            // Continue while there are directories to process
            while (stack.Count > 0)
            {
                // A.
                // Get top directory
                string dir = stack.Pop();

                try
                {
                    // B
                    // Add all files at this directory to the result List.
                    result.AddRange(Directory.GetFiles(dir, "*.*"));

                    // C
                    // Add all directories at this directory.
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                    // D
                    // Could not open the directory
                }
            }
            return result;
        }

        //  Return file size in bytes. Doesn't check if file exists. That must be done by caller.
        public static long GetFileSize(string filepath)
        {
            return (new FileInfo(filepath)).Length;
        }

        //  Returns "charactersCountFromLeft" characters from left, but never exceed string's length
        //  E.g. GetStringLeft("abcdefgh", 3) returns "abc", but GetStringLeft("abcdefgh", 1000) returns "abcdefgh"
        public static string GetStringLeft(string s, int charactersCountFromLeft)
        {
            return s.Substring(0, (s.Length < charactersCountFromLeft) ? s.Length : charactersCountFromLeft);
        }

        /// <summary>
        /// Return value from 0 - 1 but in normal distribution
        /// Normal distribution is aproximated by exponencial function
        /// </summary>
        /// <param name="value">Value from 0 to 1</param>
        /// <returns></returns>
        public static float NormalDistribution(float value)
        {
            float X = value;
            float L = 0.0f;
            float K = 0.0f;
            float dCND = 0.0f;
            const float a1 = 0.31938153f;
            const float a2 = -0.356563782f;
            const float a3 = 1.781477937f;
            const float a4 = -1.821255978f;
            const float a5 = 1.330274429f;
            L = Math.Abs(X);
            K = 1 / (1 + 0.2316419f * L);
            dCND = 1 - 1 / (float)(Math.Sqrt(2 * Convert.ToDouble(Math.PI.ToString())) * Math.Exp(-L * L / 2) * (K * (a1 + K * (a2 + K * (a3 + K * (a4 + K * a5))))));

            if (X < 0)
            {
                return 1 - dCND;
            }
            else
            {
                return dCND;
            }
        }

        //  Returns a nonnegative random number less than the specified maximum
        public static int GetRandomInt(int maxValue)
        {
            return m_random.Next(maxValue);
        }

        //  Return random short in range <minValue...maxValue>, the range of return values includes minValue but not maxValue
        public static short GetRandomShort(short minValue, short maxValue)
        {
            return (short)m_random.Next(minValue, maxValue);
        }

        //  Return random int in range <minValue...maxValue>, the range of return values includes minValue but not maxValue
        public static int GetRandomInt(int minValue, int maxValue)
        {
            return m_random.Next(minValue, maxValue);
        }

        //  Return random Vector3, always normalized (even if random values can't be normalized - then we do safe normalization)
        //  Direction of vector can be in any "direction", negative, positive, X, Y, Z, etc.
        public static Vector3 GetRandomVector3Normalized()
        {
            Vector3 vec = new Vector3(GetRandomFloat(-1, 1), GetRandomFloat(-1, 1), GetRandomFloat(-1, 1));

            float lengthSquared = vec.LengthSquared();

            if (lengthSquared == 0.0f)
            {
                //  If length of random vector is zero, we can't normalize it. So we just put there Vector3.Up. It's random enouch, because this situation don't occur often.
                vec = Vector3.Up;
            }
            else
            {
                //  Normalize random vector
                float lengthDivider = 1.0f / (float)Math.Sqrt(lengthSquared);
                vec.X *= lengthDivider;
                vec.Y *= lengthDivider;
                vec.Z *= lengthDivider;
            }

            return vec;
        }

        //  Random vector distributed over the hemisphere about normal. 
        //  Returns random vector that always lies in hemisphere (half-sphere) defined by 'normal'
        public static Vector3 GetRandomVector3HemisphereNormalized(Vector3 normal)
        {
            Vector3 randomVector = GetRandomVector3Normalized();

            if (Vector3.Dot(randomVector, normal) < 0)
            {
                return -randomVector;
            }
            else
            {
                return randomVector;
            }
        }


        //  Random vector distributed over the circle about normal. 
        //  Returns random vector that always lies on circle
        public static Vector3 GetRandomVector3CircleNormalized()
        {
            float angle = MyMwcUtils.GetRandomRadian();

            Vector3 v = new Vector3(
                (float)Math.Sin(angle),
                0,
                (float)Math.Cos(angle));

            return v;
        }


        //  Return by random +1 or -1. Nothing else. Propability is 50/50.
        public static float GetRandomSign()
        {
            return Math.Sign((float)m_random.NextDouble() - 0.5f);
        }

        //  Return random float in range <minValue...maxValue>
        public static float GetRandomFloat(float minValue, float maxValue)
        {
            return (float)m_random.NextDouble() * (maxValue - minValue) + minValue;
        }

        //  Return random radian, covering whole circle 0..360 degrees (but returned value is in radians)
        public static float GetRandomRadian()
        {
            return GetRandomFloat(0, 2 * MathHelper.Pi);
        }

        //  Return random bool with probability specified by 'oncePer'.
        //  If it is 1, than every call returns true. If it's 2, than every second call return true (propability is 50/50).
        //  If it is for example 10, then only every 10-th returns true.
        public static bool GetRandomBool(int oncePer)
        {
            //  Doesn't make sense to call this method if it will always return true.
            System.Diagnostics.Debug.Assert(oncePer > 1);
            return (GetRandomInt(0, oncePer) == 0) ? true : false;
        }

        public static bool IsAlphanumeric(string s)
        {
            Regex objAlphaNumericPattern = new Regex(REGULAR_EXPRESSION_ALPHA_NUMERIC);
            return !objAlphaNumericPattern.IsMatch(s);
        }

        public static string RemoveNonAlphanumeric(string s)
        {
            string ret = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (IsAlphanumeric(s[i].ToString())) ret += s[i];
            }
            return ret;
        }

        public static bool IsInt32(string s)
        {
            Int32 res;
            return Int32.TryParse(s, out res);
        }

        public static bool IsDecimal(string s)
        {
            Decimal res;
            return Decimal.TryParse(s, out res);
        }

        public static bool IsDecimal(string s, NumberStyles numberStyles, IFormatProvider formatProvider)
        {
            Decimal res;
            return Decimal.TryParse(s, numberStyles, formatProvider, out res);
        }

        public static bool IsValidEmailAddress(string email)
        {
            //  This is important because our current regex expression will end in infinite loop if email longer than 50 or 100 or 200 characters (I haven't checked max length)
            if (email.Length > MyMwcValidationConstants.EMAIL_LENGTH_MAX) return false;

            Regex regex = new Regex(REGULAR_EXPRESSION_EMAIL_GOOD_BUT_FAILS_ON_LONG_TEXT);
            return regex.IsMatch(email);
        }

        public static bool IsValidUsernameFormat(string username)
        {
            return !((username.Length < MyMwcValidationConstants.USERNAME_LENGTH_MIN) || (username.Length > MyMwcValidationConstants.USERNAME_LENGTH_MAX) || (IsAlphanumeric(username) == false));
        }

        public static bool IsValidPasswordFormat(string password)
        {
            return ((password.Length >= MyMwcValidationConstants.PASSWORD_LENGTH_MIN) && (password.Length <= MyMwcValidationConstants.PASSWORD_LENGTH_MAX) && (IsAlphanumeric(password)));
        }

        //  Method to validate an IP address using the TryParse Method of the IPAddress class
        //  addr - address to validate
        public static bool IsValidIpAddress(string addr)
        {
            //  Boolean variable to hold the status
            bool valid;

            //  Check to make sure an ip address was provided
            if (string.IsNullOrEmpty(addr))
            {
                //  Address wasnt provided so return false
                valid = false;
            }
            else
            {
                //  Create an IPAddress variable, TryParse requires an "out" value that is of the type IPAddress
                //  Use TryParse to see if this is a valid ip address. TryParse returns a boolean based on the validity of the
                //  provided address, so assign that value to our boolean variable
                IPAddress ip;
                valid = IPAddress.TryParse(addr, out ip);
            }

            //  Return the value
            return valid;
        }

        //  Method to validate an IP address using regular expressions. The pattern being used will validate an ip address
        //  with the range of 1.0.0.0 to 255.255.255.255
        public static bool IsValidIpAddress2(string addr)
        {
            //  Create our match pattern
            const string pattern =
                @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";

            //  Create our Regular Expression object
            Regex check = new Regex(pattern);

            //  Boolean variable to hold the status
            bool valid;

            //  Check to make sure an ip address was provided
            if (addr == "")
            {
                //  No address provided so return false
                valid = false;
            }
            else
            {
                //  Address provided so use the IsMatch Method of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }

            //  Return the results
            return valid;
        }

        //  Searches in "orig" and if found pair of "keyword1" and "keyword2", replaces them with "replaceWith1" and "replaceWith2". But only if pair is found.
        //  "keyword1" and "keyword2" may be same or no, it depends on our needs.
        //  Use for example for QUOTE.
        public static string ReplacePairOfKeywords(string keyword1, string keyword2, string replaceWith1, string replaceWith2, string orig)
        {
            int start = 0;
            while (true)
            {
                //  Find first keyword
                start = orig.IndexOf(keyword1, start);
                if (start < 0) break;

                //  Find second keyword
                int end = orig.IndexOf(keyword2, start + keyword1.Length);
                if (end < 0) break;

                string tmp = orig.Substring(start, end - start + keyword2.Length);

                orig = orig.Remove(start, tmp.Length + 1);
                orig = orig.Insert(start, replaceWith1 + tmp.Replace(keyword1, "").Replace(keyword2, "") + replaceWith2);
            }
            return orig;
        }

        //  Returns "charactersCountFromLeft" characters from left, but never exceed string's length
        //  If original string is longer, add "..." at the end
        public static string GetStringLeftSafeWithDots(string s, int charactersCountFromLeft)
        {
            if (s.Length > charactersCountFromLeft)
            {
                return s.Substring(0, charactersCountFromLeft) + "...";
            }
            else
            {
                return s;
            }
        }

        //  Returns "charactersCountFromLeft" characters from left, but never exceed string's length
        public static string GetStringLeftSafe(string s, int charactersCountFromLeft)
        {
            if (s.Length > charactersCountFromLeft)
            {
                return s.Substring(0, charactersCountFromLeft);
            }
            else
            {
                return s;
            }
        }

        public static DateTime? ReadDateTimeNullable(SqlDataReader reader, string columnName)
        {
            int columnOrdinal = reader.GetOrdinal(columnName);

            DateTime? ret = null;
            if (reader.IsDBNull(columnOrdinal) == false)
            {
                ret = reader.GetDateTime(columnOrdinal);
            }

            return ret;
        }

        public static T? ReadEnumNullable<T>(SqlDataReader reader, string columnName) where T : struct
        {
            int columnOrdinal = reader.GetOrdinal(columnName);

            Int32? ret = null;
            if (reader.IsDBNull(columnOrdinal) == false)
            {
                ret = reader.GetInt32(columnOrdinal);
            }

            if (ret == null)
            {
                return null;
            }
            else
            {
                return (T)Enum.ToObject(typeof(T), ret.Value);
            }
        }

        public static Int32? ReadInt32Nullable(SqlDataReader reader, string columnName)
        {
            int columnOrdinal = reader.GetOrdinal(columnName);

            Int32? ret = null;
            if (reader.IsDBNull(columnOrdinal) == false)
            {
                ret = reader.GetInt32(columnOrdinal);
            }

            return ret;
        }

        public static Int64? ReadInt64Nullable(SqlDataReader reader, string columnName)
        {
            int columnOrdinal = reader.GetOrdinal(columnName);

            Int64? ret = null;
            if (reader.IsDBNull(columnOrdinal) == false)
            {
                ret = reader.GetInt64(columnOrdinal);
            }

            return ret;
        }

        public static byte? ReadByteNullable(SqlDataReader reader, string columnName)
        {
            int columnOrdinal = reader.GetOrdinal(columnName);

            byte? ret = null;
            if (reader.IsDBNull(columnOrdinal) == false)
            {
                ret = reader.GetByte(columnOrdinal);
            }

            return ret;
        }

        //  Calculate difference between birthDate and NOW in years - thus age
        public static int GetAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;
            if (now < birthDate.AddYears(age)) age--;
            return age;
        }

        //  Create datetime from three strings: day, month, year. If date isn't valid (e.g. day 31 in month that doesn't have 31 days), we return null.
        public static DateTime? CreateDateTime(int year, int month, int day)
        {
            try
            {
                return new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                //  If date is wrong, we return null and swallow the exception
                return null;
            }
        }

        //  Convert string to int. If not possible (string isn't int), return null.
        public static int? GetInt32FromString(string s)
        {
            int ret;
            if (Int32.TryParse(s, out ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }

        //  Convert object to int. If not possible (object isn't int), return null.
        public static int? GetInt32FromObject(object obj)
        {
            if (obj == null) return null;

            int ret;
            if (Int32.TryParse(obj.ToString(), out ret))
            {
                return ret;
            }
            else
            {
                return null;
            }
        }

        //  Convert object to DateTime. If not possible (object isn't int), return null.
        public static DateTime? GetDateTimeFromObject(object obj)
        {
            if (obj == null) return null;
            return Convert.ToDateTime(obj);
        }

        public static void Swap(StringBuilder sb, int startIndex, int endIndex)
        {
            // Swap the integers  
            System.Diagnostics.Debug.Assert(endIndex >= startIndex);
            int count = (endIndex - startIndex + 1) / 2;
            for (int i = 0; i < count; ++i)
            {
                char temp = sb[startIndex + i];
                sb[startIndex + i] = sb[endIndex - i];
                sb[endIndex - i] = temp;
            }
        }

        public static void ClearStringBuilder(StringBuilder sb)
        {
            sb.Length = 0;
        }

        //  AppendStringBuilder is a retarded class and doesn't have overload for Append-ing another StringBuilder
        //  Solution is to append each character.
        public static void AppendStringBuilder(StringBuilder destination, StringBuilder source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                destination.Append(source[i]);
            }
        }

        private static List<char> m_splitBuffer = new List<char>(16);
        public static void SplitStringBuilder(StringBuilder destination, StringBuilder source, string splitSeparator) 
        {
            char currentChar;                        
            int length = source.Length;
            int splitLength = splitSeparator.Length;
            int currentSplitIndex = 0;

            for (int i = 0; i < length; i++) 
            {
                currentChar = source[i];

                if (currentChar == splitSeparator[currentSplitIndex])
                {
                    currentSplitIndex++;
                    // total split separator match, we append new line
                    if (currentSplitIndex == splitLength)
                    {
                        destination.AppendLine();
                        m_splitBuffer.Clear();
                        currentSplitIndex = 0;
                    }
                    // if only part match, we add to split buffer
                    else
                    {
                        m_splitBuffer.Add(currentChar);
                    }
                }
                else 
                {
                    // if there was part split match, we must append characters from split buffer
                    if (currentSplitIndex > 0)
                    {
                        foreach (char c in m_splitBuffer)
                        {
                            destination.Append(c);
                        }
                        m_splitBuffer.Clear();
                        currentSplitIndex = 0;
                    }
                    // we append current char
                    destination.Append(currentChar);
                }
            }

            // we must append characters which remain in split buffer
            foreach (char c in m_splitBuffer)
            {
                destination.Append(c);
            }
            m_splitBuffer.Clear();
        }

        //  Clamping byte into range <min...max> (including min and max)
        public static byte GetClampByte(byte value, byte min, byte max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        //  Clamping int into range <min...max> (including min and max)
        public static int GetClampInt(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        //  Use only when sending price to PayPal web form
        public static string GetFormatedPriceForPaypalForm(decimal num)
        {
            NumberFormatInfo paypalFormatter = new NumberFormatInfo();
            paypalFormatter.NumberDecimalSeparator = ".";
            paypalFormatter.NumberGroupSeparator = "";
            paypalFormatter.NumberDecimalDigits = 2;
            return num.ToString("N", paypalFormatter);
        }

        //  Use only when sending price to GoCash web form
        public static string GetFormatedPriceForGoCashForm(decimal num)
        {
            return num.ToString("N", GOCASH_FORMATTER);
        }

        //  Use only when sending price to MoneyBookers web form
        public static string GetFormatedPriceForMoneybookersForm(decimal num)
        {
            return num.ToString("N", MONEY_BOOKERS_FORMATTER);
        }

        //  Use only when sending price to CardPay web form
        public static string GetFormatedPriceForCardpayForm(decimal num)
        {
            return num.ToString("N", CARD_PAY_FORMATTER);
        }

        public static string GetFormatedPriceForGame(decimal num)
        {
            string result;
            if (num - (int)num > 0m)
            {
                result = num.ToString("C", GAME_CULTURE_INFO);
            }
            else
            {
                result = num.ToString("C0", GAME_CULTURE_INFO);
            }
            return result;
        }

        //  Calculated fractional part of a float. 
        //  Examples: 
        //      input = 3.1234; output = 0.1234
        //      input = 3.0000; output = 0.0000
        //      input = 3.9999; output = 0.9999
        //  TODO: This method should be replaced by something that don't need casting from/to double.
        public static float GetFloatFrac(float input)
        {
            return input - (float)Math.Floor(input);
        }

        //  Find max numeric value in enum
        public static int GetMaxValueFromEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));

            //  Doesn't make sence to find max in empty enum            
            MyCommonDebugUtils.AssertDebug(values.Length >= 1);

            int max = int.MinValue;
            Type underlyingType = Enum.GetUnderlyingType(typeof(T));
            if (underlyingType == typeof(System.Byte))
            {
                foreach (byte value in values)
                {
                    if (value > max) max = value;
                }
            }
            else if (underlyingType == typeof(System.Int16))
            {
                foreach (short value in values)
                {
                    if (value > max) max = value;
                }
            }
            else if (underlyingType == typeof(System.UInt16))
            {
                foreach (ushort value in values)
                {
                    if (value > max) max = value;
                }
            }
            else if (underlyingType == typeof(System.Int32))
            {
                foreach (int value in values)
                {
                    if (value > max) max = value;
                }
            }
            else
            {
                //  Unhandled underlying type - probably "long"
                throw new MyMwcExceptionApplicationShouldNotGetHere();
            }

            return max;
        }

        //  Try to convert string to int. If not possible, null is returned.
        public static int? GetIntFromString(string s)
        {
            int outInt;
            if (int.TryParse(s, out outInt) == false)
            {
                return null;
            }
            return outInt;
        }

        //  Try to convert string to int. If not possible, null is returned.
        public static byte? GetByteFromString(string s)
        {
            byte outByte;
            if (byte.TryParse(s, out outByte) == false)
            {
                return null;
            }
            return outByte;
        }

        //  Try to convert string to int. If not possible, null is returned.
        //  If 's' can't be converted to a valid float, 'defaultValue' is returned
        public static int GetIntFromString(string s, int defaultValue)
        {
            int? outInt = GetIntFromString(s);
            return (outInt == null) ? defaultValue : outInt.Value;
        }

        //  Try to convert string to float. If not possible, null is returned.
        public static float? GetFloatFromString(string s)
        {
            float outFloat;
            if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out outFloat) == false)
            {
                return null;
            }
            return outFloat;
        }

        // sphere vs sphere overlap test
        public static bool SphereVsSphere(Vector3 p1, float r1, Vector3 p2, float r2)
        {
            float rSum = r1 + r2;
            return (p1 - p2).LengthSquared() <= rSum * rSum;
        }

        //  Try to convert string to float. If not possible, null is returned.
        //  If 's' can't be converted to a valid float, 'defaultValue' is returned
        public static float GetFloatFromString(string s, float defaultValue)
        {
            float? outFloat = GetFloatFromString(s);
            return (outFloat == null) ? defaultValue : outFloat.Value;
        }

        //  Try to convert string to bool. If not possible, null is returned.
        public static bool? GetBoolFromString(string s)
        {
            bool outBool;
            if (bool.TryParse(s, out outBool) == false)
            {
                return null;
            }
            return outBool;
        }

        //  Try to convert string to bool. If not possible, null is returned.
        //  If 's' can't be converted to a valid bool, 'defaultValue' is returned
        public static bool GetBoolFromString(string s, bool defaultValue)
        {
            bool? outBool = GetBoolFromString(s);
            return (outBool == null) ? defaultValue : outBool.Value;
        }

        public static DateTime AddMillisecondsToZeroDateTime(int milliseconds)
        {
            return (new DateTime()).AddMilliseconds(milliseconds);
        }

        public static bool GetEnumFromNumber<T, U>(U val, ref T enumValue)
            where T : struct
        {
            if (Enum.IsDefined(typeof(T), val) == false)
            {
                return false;
            }
            else
            {
                enumValue = (T)Enum.ToObject(typeof(T), val);
                return true;
            }
        }

        //  For converting numbers into enums. If number can't be converted to enum because it has value
        //  that's not in the enum, it returns null.
        public static Nullable<T> GetEnumFromNumber<T, U>(U val) where T : struct
        {
            if (Enum.IsDefined(typeof(T), val) == false)
            {
                return null;
            }
            else
            {
                return (T)Enum.ToObject(typeof(T), val);
            }
        }

        //  Converts filename into safe form (by removing all not-allowed characters)
        public static string MakeSafeFilename(string filename, string replaceWith)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c.ToString(), replaceWith);
            }
            return filename;
        }

        //  Get random Message ID, used when user sends a message to us and we need some number for tracking
        public static string GetRandomMessageId(int length)
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random();

            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                ret.Append(allowedChars[rnd.Next(allowedChars.Length)]);
            }
            return ret.ToString();
        }

        //  Return random password
        public static string GetRandomPassword(int passwordLength)
        {
            const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            Random rnd = new Random();

            StringBuilder sbNewPassword = new StringBuilder();
            for (int i = 0; i < passwordLength; i++)
            {
                sbNewPassword.Append(allowedChars[rnd.Next(allowedChars.Length)]);
            }
            return sbNewPassword.ToString();
        }

        //  Generate serial key composed of 6 chunks, each long 5 alpha-numeric characters, always upper case.
        //  Example: 12345-ABC12-UDH45-ODJF3-HA278
        public static string GetRandomSerialKey(Random rnd)
        {
            return
                GetRandomSerialKeyChunk(rnd, 5) + "-" +
                GetRandomSerialKeyChunk(rnd, 5) + "-" +
                GetRandomSerialKeyChunk(rnd, 5) + "-" +
                GetRandomSerialKeyChunk(rnd, 5) + "-" +
                GetRandomSerialKeyChunk(rnd, 5) + "-" +
                GetRandomSerialKeyChunk(rnd, 5);
        }

        //  Get random Message ID, used when user sends a message to us and we need some number for tracking
        //  Random must be initialized before calling this method, and even better before starting serial key generation sequence,
        //  so then random values won't repeat
        static string GetRandomSerialKeyChunk(Random rnd, int length)
        {
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                ret.Append(allowedChars[rnd.Next(allowedChars.Length)]);
            }
            return ret.ToString();
        }

        public static float Round(float number, float step)
        {
            return ((float)Math.Round(number / step)) * step;
        }

        public static double Round(double number, double step)
        {
            return (Math.Round(number / step)) * step;
        }

        public static bool HasValidLength(Vector3 vec)
        {
            return vec.LengthSquared() > MyMwcMathConstants.EPSILON_SQUARED;
        }

        public static bool HasValidLength(Vector2 vec)
        {
            return vec.LengthSquared() > MyMwcMathConstants.EPSILON_SQUARED;
        }

        public static bool HasValidLength(ref Vector3 vec)
        {
            return vec.LengthSquared() > MyMwcMathConstants.EPSILON_SQUARED;
        }

        public static bool HasValidLength(ref Vector2 vec)
        {
            return vec.LengthSquared() > MyMwcMathConstants.EPSILON_SQUARED;
        }

        [Conditional("DEBUG")]
        public static void AssertVectorValid(ref Vector3 vec)
        {
            System.Diagnostics.Debug.Assert(HasValidLength(ref vec));
        }

        [Conditional("DEBUG")]
        public static void AssertVectorValid(ref Vector2 vec)
        {
            System.Diagnostics.Debug.Assert(HasValidLength(ref vec));
        }

        /// <summary>
        /// Protected normalize with assert
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 Normalize(Vector3 vec)
        {
            //  Check if vector has reasonable length, otherwise Normalize is going to fail
            AssertVectorValid(ref vec);
            return Vector3.Normalize(vec);
        }

        /// <summary>
        /// Protected normalize with assert
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void Normalize(ref Vector3 vec, out Vector3 normalized)
        {
            //  Check if vector has reasonable length, otherwise Normalize is going to fail
            AssertVectorValid(ref vec);
            Vector3.Normalize(ref vec, out normalized);
        }


        /// <summary>
        /// Protected normalize with assert
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector2 Normalize(Vector2 vec)
        {
            AssertVectorValid(ref vec);
            return Vector2.Normalize(vec);
        }

        /// <summary>
        /// Protected normalize with assert
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void Normalize(ref Vector2 vec, out Vector2 normalized)
        {
            AssertVectorValid(ref vec);
            Vector2.Normalize(ref vec, out normalized);
        }

        /// <summary>
        /// Protected normalize with assert
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void Normalize(ref Matrix m, out Matrix normalized)
        {
            normalized = Matrix.CreateWorld(
                m.Translation,
                Normalize(m.Forward),
                Normalize(m.Up));
        }

        public static void FixDenormalizedFloat(ref float value)
        {
            if (Math.Abs(value) < DENORMALIZED_BOUND) value = 0.0f;
        }

        /// <summary>
        /// Better comparison for zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsZero(float value)
        {
            return (value > -MyMwcMathConstants.EPSILON) && (value < MyMwcMathConstants.EPSILON);
        }

        /// <summary>
        /// Better comparison for zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsZero(Vector3 value)
        {
            return IsZero(value.X) && IsZero(value.Y) && IsZero(value.Z);
        }

        /// <summary>
        /// Better comparison for zero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsZero(Vector4 value)
        {
            return IsZero(value.X) && IsZero(value.Y) && IsZero(value.Z) && IsZero(value.W);
        }

        //  Examples:
        //      RoundToNearestMultipleOf(0.1, 5) = 5
        //      RoundToNearestMultipleOf(4.9, 5) = 5
        //      RoundToNearestMultipleOf(5.5, 5) = 10
        //      RoundToNearestMultipleOf(10.23, 5) = 15
        public static decimal RoundToNearestMultipleOf(decimal dec, decimal multipleOf)
        {
            return Convert.ToInt32(Math.Floor(dec / multipleOf)) * multipleOf + multipleOf;
        }

        // Extend Vector3 to allow indexing by dimension.
        public static float GetDim(this Vector3 v, int i)
        {
            switch (i)
            {
                case 0: return v.X;
                case 1: return v.Y;
                case 2: return v.Z;
                default: return v.GetDim((i % 3 + 3) % 3);  // reduce to 0..2
            }
        }
        public static void SetDim(this Vector3 v, int i, float value)
        {
            switch (i)
            {
                case 0: v.X = value; break;
                case 1: v.Y = value; break;
                case 2: v.Z = value; break;
                default: v.SetDim((i % 3 + 3) % 3, value); break;  // reduce to 0..2
            }
        }

        // Extend BoundingBox to provide the surface area and volume
        public static float SurfaceArea(this BoundingBox bb)
        {
            Vector3 span = bb.Max - bb.Min;
            return 2 * (span.X * span.Y + span.X * span.Z + span.Y * span.Z);
        }
        public static float Volume(this BoundingBox bb)
        {
            Vector3 span = bb.Max - bb.Min;
            return span.X * span.Y * span.Z;
        }

        public static Vector3 GetNeighbourSectorShipPosition(Vector3 currentShipPosition)
        {
            Vector3 abs = new Vector3(Math.Abs(currentShipPosition.X), Math.Abs(currentShipPosition.Y), Math.Abs(currentShipPosition.Z));

            if (abs.X > abs.Y)
            {
                if (abs.X > abs.Z)
                {
                    // X is biggest
                    currentShipPosition.X = -currentShipPosition.X;
                }
                else
                {
                    // Z is biggest
                    currentShipPosition.Z = -currentShipPosition.Z;
                }
            }
            else if (abs.Y > abs.Z)
            {
                // Y is biggest
                currentShipPosition.Y = -currentShipPosition.Y;
            }
            else
            {
                // Z is biggest
                currentShipPosition.Z = -currentShipPosition.Z;
            }
            // Clamp all direction to prevent automatic travel when position is diagonally outside sector
            float maxDistance = MyMwcSectorConstants.SECTOR_SIZE_HALF;
            Vector3 border = new Vector3(maxDistance, maxDistance, maxDistance);
            return Vector3.Clamp(currentShipPosition, -border, border);
        }

        public static T GetRandomItem<T>(List<T> list)
        {
            return list[GetRandomInt(list.Count)];
        }

        public static T GetRandomItemOrNull<T>(List<T> list)
            where T : class
        {
            if (list.Count == 0)
                return null;
            return list[GetRandomInt(list.Count)];
        }
    }
}

