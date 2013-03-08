/******************************************************
                  DirectShow .NET
		      netmaster@swissonline.ch
*******************************************************/
//					DsExtend
// Extended streaming interfaces, ported from axextend.idl

using System;
using System.Runtime.InteropServices;

namespace DShowNET
{



	[ComVisible(true), ComImport,
	Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface ICaptureGraphBuilder2
{
		[PreserveSig]
	int SetFiltergraph( [In] IGraphBuilder pfg );

		[PreserveSig]
	int GetFiltergraph( [Out] out IGraphBuilder ppfg );

		[PreserveSig]
	int SetOutputFileName(
		[In]											ref Guid		pType,
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpstrFile,
		[Out]										out IBaseFilter		ppbf,
		[Out]										out IFileSinkFilter	ppSink );

		[PreserveSig]
	int FindInterface(
		[In]											ref Guid		pCategory,
		[In]											ref Guid		pType,
		[In]											IBaseFilter		pbf,
		[In]											ref Guid		riid,
		[Out, MarshalAs(UnmanagedType.IUnknown) ]		out	object		ppint );

		[PreserveSig]
	int RenderStream(
		[In]										ref Guid		pCategory,
		[In]										ref Guid		pType,
		[In, MarshalAs(UnmanagedType.IUnknown)]			object			pSource,
		[In]											IBaseFilter		pfCompressor,
		[In]											IBaseFilter		pfRenderer );

		[PreserveSig]
	int ControlStream(
		[In]										ref Guid			pCategory,
		[In]										ref Guid			pType,
		[In]											IBaseFilter		pFilter,
		[In]											IntPtr			pstart,
		[In]											IntPtr			pstop,
		[In]											short			wStartCookie,
		[In]											short			wStopCookie );

		[PreserveSig]
	int AllocCapFile(
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpstrFile,
		[In]											long			dwlSize );

		[PreserveSig]
	int CopyCaptureFile(
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpwstrOld,
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpwstrNew,
		[In]											int							fAllowEscAbort,
		[In]											IAMCopyCaptureFileProgress	pFilter );


		[PreserveSig]
	int FindPin(
		[In]											object			pSource,
		[In]											int				pindir,
		[In]										ref Guid			pCategory,
		[In]										ref Guid			pType,
		[In, MarshalAs(UnmanagedType.Bool) ]			bool			fUnconnected,
		[In]											int				num,
		[Out]										out IPin			ppPin );
}





	[ComVisible(true), ComImport,
	Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IGraphBuilder
{
	#region "IFilterGraph Methods"
			[PreserveSig]
		int AddFilter(
			[In] IBaseFilter pFilter,
			[In, MarshalAs(UnmanagedType.LPWStr)]			string			pName );

			[PreserveSig]
		int RemoveFilter( [In] IBaseFilter pFilter );

			[PreserveSig]
		int EnumFilters( [Out] out IEnumFilters ppEnum );

			[PreserveSig]
		int FindFilterByName(
			[In, MarshalAs(UnmanagedType.LPWStr)]			string			pName,
			[Out]										out IBaseFilter		ppFilter );

			[PreserveSig]
		int ConnectDirect( [In] IPin ppinOut, [In] IPin ppinIn,
			[In, MarshalAs(UnmanagedType.LPStruct)]			AMMediaType	pmt );

			[PreserveSig]
		int Reconnect( [In] IPin ppin );

			[PreserveSig]
		int Disconnect( [In] IPin ppin );

			[PreserveSig]
		int SetDefaultSyncSource();
	#endregion

		[PreserveSig]
	int Connect( [In] IPin ppinOut, [In] IPin ppinIn );

		[PreserveSig]
	int Render( [In] IPin ppinOut );

		[PreserveSig]
	int RenderFile(
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpcwstrFile,
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpcwstrPlayList );

		[PreserveSig]
	int AddSourceFilter(
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpcwstrFileName,
		[In, MarshalAs(UnmanagedType.LPWStr)]			string			lpcwstrFilterName,
		[Out]										out IBaseFilter		ppFilter );

		[PreserveSig]
	int SetLogFile( IntPtr hFile );

		[PreserveSig]
	int Abort();

		[PreserveSig]
	int ShouldOperationContinue();
}







// ---------------------------------------------------------------------------------------


	[ComVisible(true), ComImport,
	Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IFileSinkFilter
{
	[PreserveSig]
	int SetFileName(
		[In, MarshalAs(UnmanagedType.LPWStr)]			string		pszFileName,
		[In, MarshalAs(UnmanagedType.LPStruct)]			AMMediaType	pmt );
	
	[PreserveSig]
	int GetCurFile(
		[Out, MarshalAs(UnmanagedType.LPWStr) ]		out	string		pszFileName,
		[Out, MarshalAs(UnmanagedType.LPStruct) ]		AMMediaType pmt );
}






// ---------------------------------------------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IAMCopyCaptureFileProgress
{
		[PreserveSig]
	int Progress( int iProgress );
}








// ---------------------------------------------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("e46a9787-2b71-444d-a4b5-1fab7b708d6a"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IVideoFrameStep
{
		[PreserveSig]
	int Step( int dwFrames,
		[In, MarshalAs(UnmanagedType.IUnknown)]			object			pStepObject );

		[PreserveSig]
	int CanStep( int bMultiple,
		[In, MarshalAs(UnmanagedType.IUnknown)]			object			pStepObject );

		[PreserveSig]
	int CancelStep();
}








// ---------------------------------------------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IAMStreamConfig
{
		[PreserveSig]
	int SetFormat(
		[In, MarshalAs(UnmanagedType.LPStruct)]			AMMediaType	pmt );

		[PreserveSig]
	int GetFormat(
		[Out, MarshalAs(UnmanagedType.LPStruct)]		AMMediaType	pmt );

		[PreserveSig]
	int GetNumberOfCapabilities( out int piCount, out int piSize );

		[PreserveSig]
	int GetStreamCaps( int iIndex,
		[Out, MarshalAs(UnmanagedType.LPStruct)]	out AMMediaType	ppmt,
														IntPtr		pSCC );
    }














// =============================================================================
//											TUNER
// =============================================================================

	[ComVisible(false)]
public enum AMTunerSubChannel
{
	NoTune		= -2,	// AMTUNER_SUBCHAN_NO_TUNE : don't tune
	Default		= -1	// AMTUNER_SUBCHAN_DEFAULT : use default sub chan
}

	[ComVisible(false)]
public enum AMTunerSignalStrength
{
	NA				= -1,	// AMTUNER_HASNOSIGNALSTRENGTH : cannot indicate signal strength
	NoSignal		= 0,	// AMTUNER_NOSIGNAL : no signal available
	SignalPresent	= 1		// AMTUNER_SIGNALPRESENT : signal present
}

	[Flags, ComVisible(false)]
public enum AMTunerModeType
{
	Default		= 0x0000,	// AMTUNER_MODE_DEFAULT : default tuner mode
	TV			= 0x0001,	// AMTUNER_MODE_TV : tv
	FMRadio		= 0x0002,	// AMTUNER_MODE_FM_RADIO : fm radio
	AMRadio		= 0x0004,	// AMTUNER_MODE_AM_RADIO : am radio
	Dss			= 0x0008	// AMTUNER_MODE_DSS : dss
}

	[ComVisible(false)]
public enum AMTunerEventType
{
	Changed		= 0x0001,	// AMTUNER_EVENT_CHANGED : status changed
}


// ---------------------------------------------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("211A8761-03AC-11d1-8D13-00AA00BD8339"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IAMTuner
{
		[PreserveSig]
	int put_Channel( int lChannel, AMTunerSubChannel lVideoSubChannel, AMTunerSubChannel lAudioSubChannel );

		[PreserveSig]
	int get_Channel( out int plChannel, out int plVideoSubChannel, out int plAudioSubChannel );
    
		[PreserveSig]
	int ChannelMinMax( out int lChannelMin, out int lChannelMax );
        
		[PreserveSig]
	int put_CountryCode( int lCountryCode );

		[PreserveSig]
	int get_CountryCode( out int plCountryCode );

		[PreserveSig]
	int put_TuningSpace( int lTuningSpace );

		[PreserveSig]
	int get_TuningSpace( out int plTuningSpace );

		[PreserveSig]
	int Logon( IntPtr hCurrentUser );
    
		[PreserveSig]
	int Logout();

		[PreserveSig]
	int SignalPresent( out AMTunerSignalStrength plSignalStrength );
    
		[PreserveSig]
	int put_Mode( AMTunerModeType lMode );

		[PreserveSig]
	int get_Mode( out AMTunerModeType plMode );
    
		[PreserveSig]
	int GetAvailableModes( out AMTunerModeType plModes );
    
		[PreserveSig]
	int RegisterNotificationCallBack( IAMTunerNotification pNotify, AMTunerEventType lEvents );

		[PreserveSig]
	int UnRegisterNotificationCallBack( IAMTunerNotification pNotify );
}


// ---------------------------------------------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("211A8760-03AC-11d1-8D13-00AA00BD8339"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IAMTunerNotification
{
		[PreserveSig]
	int OnEvent( AMTunerEventType Event );
}


// ---------------------------------------------------------------------------------------
	[Flags, ComVisible(false)]
public enum AnalogVideoStandard
{
	None		= 0x00000000,  // This is a digital sensor
	NTSC_M		= 0x00000001,  //        75 IRE Setup
	NTSC_M_J	= 0x00000002,  // Japan,  0 IRE Setup
	NTSC_433	= 0x00000004,
	PAL_B		= 0x00000010,
	PAL_D		= 0x00000020,
	PAL_G		= 0x00000040,
	PAL_H		= 0x00000080,
	PAL_I		= 0x00000100,
	PAL_M		= 0x00000200,
	PAL_N		= 0x00000400,
	PAL_60		= 0x00000800,
	SECAM_B		= 0x00001000,
	SECAM_D		= 0x00002000,
	SECAM_G		= 0x00004000,
	SECAM_H		= 0x00008000,
	SECAM_K		= 0x00010000,
	SECAM_K1	= 0x00020000,
	SECAM_L		= 0x00040000,
	SECAM_L1	= 0x00080000,
	PAL_N_COMBO	= 0x00100000	// Argentina
}

	[ComVisible(false)]
public enum TunerInputType
{
	Cable,
	Antenna
}


// ---------------------------------------------------------------------------------------

	[ComVisible(true), ComImport,
	Guid("211A8766-03AC-11d1-8D13-00AA00BD8339"),
	InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
public interface IAMTVTuner
{

	#region "IAMTuner Methods"
			[PreserveSig]
		int put_Channel( int lChannel, AMTunerSubChannel lVideoSubChannel, AMTunerSubChannel lAudioSubChannel );

			[PreserveSig]
		int get_Channel( out int plChannel, out int plVideoSubChannel, out int plAudioSubChannel );
	    
			[PreserveSig]
		int ChannelMinMax( out int lChannelMin, out int lChannelMax );
	        
			[PreserveSig]
		int put_CountryCode( int lCountryCode );

			[PreserveSig]
		int get_CountryCode( out int plCountryCode );

			[PreserveSig]
		int put_TuningSpace( int lTuningSpace );

			[PreserveSig]
		int get_TuningSpace( out int plTuningSpace );

			[PreserveSig]
		int Logon( IntPtr hCurrentUser );
	    
			[PreserveSig]
		int Logout();

			[PreserveSig]
		int SignalPresent( out AMTunerSignalStrength plSignalStrength );
	    
			[PreserveSig]
		int put_Mode( AMTunerModeType lMode );

			[PreserveSig]
		int get_Mode( out AMTunerModeType plMode );
	    
			[PreserveSig]
		int GetAvailableModes( out AMTunerModeType plModes );
	    
			[PreserveSig]
		int RegisterNotificationCallBack( IAMTunerNotification pNotify, AMTunerEventType lEvents );

			[PreserveSig]
		int UnRegisterNotificationCallBack( IAMTunerNotification pNotify );
	#endregion

		[PreserveSig]
	int get_AvailableTVFormats( out AnalogVideoStandard lAnalogVideoStandard );

		[PreserveSig]
	int get_TVFormat( out AnalogVideoStandard lAnalogVideoStandard );
    
		[PreserveSig]
	int AutoTune( int lChannel, out int plFoundSignal );
    
		[PreserveSig]
	int StoreAutoTune();
    
		[PreserveSig]
	int get_NumInputConnections( out int plNumInputConnections );
    
		[PreserveSig]
	int put_InputType( int lIndex, TunerInputType inputType );
    
		[PreserveSig]
	int get_InputType( int lIndex, out TunerInputType inputType );

		[PreserveSig]
	int put_ConnectInput( int lIndex );
    
		[PreserveSig]
	int get_ConnectInput( out int lIndex );

		[PreserveSig]
	int get_VideoFrequency( out int lFreq );

		[PreserveSig]
	int get_AudioFrequency( out int lFreq );
}


} // namespace DShowNET
