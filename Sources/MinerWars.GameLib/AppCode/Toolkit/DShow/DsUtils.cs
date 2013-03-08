/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsUtils
// DirectShow utility classes, partial from the SDK Common sources

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DShowNET
{

		[ComVisible(false)]
	public class DsUtils
	{

		public static bool IsCorrectDirectXVersion()
		{
			return File.Exists( Path.Combine( Environment.SystemDirectory, @"dpnhpast.dll" ) );
		}


		public static bool ShowCapPinDialog( ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd )
		{
			int hr;
			object comObj = null;
			ISpecifyPropertyPages	spec = null;
			DsCAUUID cauuid = new DsCAUUID();

			try {
				Guid cat  = PinCategory.Capture;
				Guid type = MediaType.Interleaved;
				Guid iid = typeof(IAMStreamConfig).GUID;
				hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
				if( hr != 0 )
				{
					type = MediaType.Video;
					hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
					if( hr != 0 )
						return false;
				}
				spec = comObj as ISpecifyPropertyPages;
				if( spec == null )
					return false;

				hr = spec.GetPages( out cauuid );
				hr = OleCreatePropertyFrame( hwnd, 30, 30, null, 1,
						ref comObj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero );
				return true;
			}
			catch( Exception ee )
			{
				Trace.WriteLine( "!Ds.NET: ShowCapPinDialog " + ee.Message );
				return false;
			}
			finally
			{
				if( cauuid.pElems != IntPtr.Zero )
					Marshal.FreeCoTaskMem( cauuid.pElems );
					
				spec = null;
				if( comObj != null )
					Marshal.ReleaseComObject( comObj ); comObj = null;
			}
		}

		public static bool ShowTunerPinDialog( ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd )
		{
			int hr;
			object comObj = null;
			ISpecifyPropertyPages	spec = null;
			DsCAUUID cauuid = new DsCAUUID();

			try {
				Guid cat  = PinCategory.Capture;
				Guid type = MediaType.Interleaved;
				Guid iid = typeof(IAMTVTuner).GUID;
				hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
				if( hr != 0 )
				{
					type = MediaType.Video;
					hr = bld.FindInterface( ref cat, ref type, flt, ref iid, out comObj );
					if( hr != 0 )
						return false;
				}
				spec = comObj as ISpecifyPropertyPages;
				if( spec == null )
					return false;

				hr = spec.GetPages( out cauuid );
				hr = OleCreatePropertyFrame( hwnd, 30, 30, null, 1,
						ref comObj, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero );
				return true;
			}
			catch( Exception ee )
			{
				Trace.WriteLine( "!Ds.NET: ShowCapPinDialog " + ee.Message );
				return false;
			}
			finally
			{
				if( cauuid.pElems != IntPtr.Zero )
					Marshal.FreeCoTaskMem( cauuid.pElems );
					
				spec = null;
				if( comObj != null )
					Marshal.ReleaseComObject( comObj ); comObj = null;
			}
		}


		// from 'DShowUtil.cpp'
		public int GetPin( IBaseFilter filter, PinDirection dirrequired, int num, out IPin ppPin )
		{
			ppPin = null;
			int hr;
			IEnumPins pinEnum;
			hr = filter.EnumPins( out pinEnum );
			if( (hr < 0) || (pinEnum == null) )
				return hr;

			IPin[] pins = new IPin[1];
			int f;
			PinDirection dir;
			do
			{
				hr = pinEnum.Next( 1, out pins, out f );
				if( (hr != 0) || (pins[0] == null) )
					break;
				dir = (PinDirection) 3;
				hr = pins[0].QueryDirection( out dir );
				if( (hr == 0) && (dir == dirrequired) )
				{
					if( num == 0 )
					{
						ppPin = pins[0];
						pins[0] = null;
						break;
					}
					num--;
				}
				Marshal.ReleaseComObject( pins[0] ); pins[0] = null;
			}
			while( hr == 0 );

			Marshal.ReleaseComObject( pinEnum ); pinEnum = null;
			return hr;
		}

		[DllImport("olepro32.dll", CharSet=CharSet.Unicode, ExactSpelling=true) ]
		private static extern int OleCreatePropertyFrame( IntPtr hwndOwner, int x, int y,
			string lpszCaption, int cObjects,
			[In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
			int cPages,	IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved );
	}


// ---------------------------------------------------------------------------------------

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public struct DsPOINT		// POINT
{
	public int		X;
	public int		Y;
}


// ---------------------------------------------------------------------------------------

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public struct DsRECT		// RECT
{
	public int		Left;
	public int		Top;
	public int		Right;
	public int		Bottom;
}


// ---------------------------------------------------------------------------------------

	[StructLayout(LayoutKind.Sequential, Pack=2), ComVisible(false)]
public struct DsBITMAPINFOHEADER
	{
	public int      Size;
	public int      Width;
	public int      Height;
	public short    Planes;
	public short    BitCount;
	public int      Compression;
	public int      ImageSize;
	public int      XPelsPerMeter;
	public int      YPelsPerMeter;
	public int      ClrUsed;
	public int      ClrImportant;
	}




// ---------------------------------------------------------------------------------------

		[ComVisible(false)]
	public class DsROT
	{
		public static bool AddGraphToRot( object graph, out int cookie )
		{
			cookie = 0;
			int hr = 0;
			UCOMIRunningObjectTable rot = null;
			UCOMIMoniker mk = null;
			try {
				hr = GetRunningObjectTable( 0, out rot );
				if( hr < 0 )
					Marshal.ThrowExceptionForHR( hr );

				int id = GetCurrentProcessId();
				IntPtr iuPtr = Marshal.GetIUnknownForObject( graph );
				int iuInt = (int) iuPtr;
				Marshal.Release( iuPtr );
				string item = string.Format( "FilterGraph {0} pid {1}", iuInt.ToString("x8"), id.ToString("x8") );
				hr = CreateItemMoniker( "!", item, out mk );
				if( hr < 0 )
					Marshal.ThrowExceptionForHR( hr );
				
				rot.Register( ROTFLAGS_REGISTRATIONKEEPSALIVE, graph, mk, out cookie );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
			finally
			{
				if( mk != null )
					Marshal.ReleaseComObject( mk ); mk = null;
				if( rot != null )
					Marshal.ReleaseComObject( rot ); rot = null;
			}
		}

		public static bool RemoveGraphFromRot( ref int cookie )
		{
			UCOMIRunningObjectTable rot = null;
			try {
				int hr = GetRunningObjectTable( 0, out rot );
				if( hr < 0 )
					Marshal.ThrowExceptionForHR( hr );

				rot.Revoke( cookie );
				cookie = 0;
				return true;
			}
			catch( Exception )
			{
				return false;
			}
			finally
			{
				if( rot != null )
					Marshal.ReleaseComObject( rot ); rot = null;
			}
		}

		private const int ROTFLAGS_REGISTRATIONKEEPSALIVE	= 1;

		[DllImport("ole32.dll", ExactSpelling=true) ]
		private static extern int GetRunningObjectTable( int r,
			out UCOMIRunningObjectTable pprot );

		[DllImport("ole32.dll", CharSet=CharSet.Unicode, ExactSpelling=true) ]
		private static extern int CreateItemMoniker( string delim,
			string item, out UCOMIMoniker ppmk );

		[DllImport("kernel32.dll", ExactSpelling=true) ]
		private static extern int GetCurrentProcessId();
	}





// ---------------------------------- ocidl.idl ------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ISpecifyPropertyPages
{
		[PreserveSig]
	int GetPages( out DsCAUUID pPages );
}

	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public struct DsCAUUID		// CAUUID
{
	public int		cElems;
	public IntPtr	pElems;
}

// ---------------------------------------------------------------------------------------


	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public class DsOptInt64
{
	public DsOptInt64( long Value )
	{
		this.Value = Value;
	}
	public long		Value;
}


	[StructLayout(LayoutKind.Sequential), ComVisible(false)]
public class DsOptIntPtr
{
	public IntPtr	Pointer;
}



} // namespace DShowNET
