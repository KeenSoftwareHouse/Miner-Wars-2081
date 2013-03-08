using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SysUtils
{
	public class MyTraceClient
	{
		public enum E_TraceMessageType
		{
			E_TMT_CLEAR_LIST = 0,
			E_TMT_INFO,
			E_TMT_WARNING,
			E_TMT_ERROR,
			E_TMT_STOP_ERROR
		};

		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpData;
		}

		readonly static uint WM_COPYDATA = 0x004A;
		//[DllImport("coredll.dll",EntryPoint="SendMessage", SetLastError=true)]	//Win7
		[DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]	//XP
		public static extern int SendMessage(IntPtr hWnd, uint uMsg, int wParam, ref COPYDATASTRUCT lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr FindWindow(string strClassName, string strWindowName);

		private static IntPtr m_hWnd;

		/// <summary>
		/// InitTrace - run external TraceServer application
		/// </summary>
		/// <param name="fullPath"></param>
		/// <returns></returns>
		public static bool InitTrace(String fullPath)
		{
			//run trace server
			try
			{
				Process.Start(fullPath);
			}
			catch (System.ComponentModel.Win32Exception)
			{
			}

			m_hWnd = FindWindow("#32770", "TraceServer");
			if (m_hWnd == null)
				return false;

			Trace(E_TraceMessageType.E_TMT_CLEAR_LIST, String.Empty);
			return true;
		}

		/// <summary>
		/// Trace
		/// </summary>
		/// <param name="type"></param>
		/// <param name="str"></param>
        [Conditional("DEBUG"), Conditional("DEVELOP")]
		public static void Trace(E_TraceMessageType type, String str)
		{
			if (m_hWnd == null)
				return;

			StackTrace trace = new StackTrace();
			StackFrame frame = trace.GetFrame(2);
			System.Reflection.MethodBase method = frame.GetMethod();
			String methodName = method.Name;
			String file = frame.GetFileName();
			int lineNumber = frame.GetFileLineNumber();


			byte[] cArr = System.Text.Encoding.Default.GetBytes(str);
			COPYDATASTRUCT myCD;
			myCD.dwData = (IntPtr)500;
			int size = 4 * 3 + str.Length + 1 + methodName.Length + 1;	//4*3 4bytes per size + type + lineNum
			myCD.cbData = size;

			byte[] strSize = BitConverter.GetBytes(size);
			byte[] strType = BitConverter.GetBytes((int)type);
			byte[] strLineNum = BitConverter.GetBytes(lineNumber);

			String strTmp = System.Text.ASCIIEncoding.ASCII.GetString(strSize);
			strTmp += System.Text.ASCIIEncoding.ASCII.GetString(strType);
			strTmp += System.Text.ASCIIEncoding.ASCII.GetString(strLineNum);

			byte[] info = System.Text.Encoding.ASCII.GetBytes(str);
			strTmp += System.Text.ASCIIEncoding.ASCII.GetString(info);

			byte[] zero = { 0 };
			strTmp += System.Text.ASCIIEncoding.ASCII.GetString(zero);	//0

			info = System.Text.Encoding.ASCII.GetBytes(methodName);
			strTmp += System.Text.ASCIIEncoding.ASCII.GetString(info);

			strTmp += System.Text.ASCIIEncoding.ASCII.GetString(zero);	//0

			myCD.lpData = strTmp;//

			try
			{
				SendMessage(m_hWnd, WM_COPYDATA, 0, ref myCD);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// TraceI
		/// </summary>
		/// <param name="str"></param>
        [Conditional("DEBUG"), Conditional("DEVELOP")]
		public static void TraceI(String str)
		{
			Trace(E_TraceMessageType.E_TMT_INFO, str);
		}
        
        [Conditional("DEBUG"), Conditional("DEVELOP")]
		public static void TraceW(String str) 
		{
			Trace(E_TraceMessageType.E_TMT_WARNING, str);
		}

        [Conditional("DEBUG"), Conditional("DEVELOP")]
		public static void TraceE(String str)
		{
			Trace(E_TraceMessageType.E_TMT_ERROR, str);
		}
	}
}
