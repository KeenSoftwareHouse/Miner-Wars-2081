using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using SysUtils.Utils;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Utils;

using SysUtils;
using SharpDX.Direct3D9;


namespace MinerWars.AppCode.Game.Utils
{
    

    public class MyGraphicTest
    {
        private UInt32 m_VertexShaderVersionMinor;
        private UInt32 m_VertexShaderVersionMajor;
        private UInt32 m_PixelShaderVersionMinor;
        private UInt32 m_PixelShaderVersionMajor;
        private bool m_SeparateAlphaBlend;
        private bool m_DestBlendSrcAlphaSat;
        private UInt32 m_MaxPrimitiveCount;
        private bool m_IndexElementSize32;
        private int m_MaxVertexStreams;
        private int m_MaxStreamStride;
        private int m_MaxTextureSize;
        private int m_MaxVolumeExtent;
        private int m_MaxTextureAspectRatio;
        private int m_MaxVertexSamplers;
        private int m_MaxRenderTargets;
        private bool m_NonPow2Unconditional;
        private bool m_NonPow2Cube;
        private bool m_NonPow2Volume;
        private List<Format> m_ValidTextureFormats;
        private List<Format> m_ValidCubeFormats;
        private List<Format> m_ValidVolumeFormats;
        private List<Format> m_ValidVertexTextureFormats;
        private List<Format> m_InvalidFilterFormats;
        private List<Format> m_InvalidBlendFormats;
        private List<DeclarationType> m_ValidVertexFormats;

        public int MaxTextureSize { get; private set; }
        public bool Rgba1010102Supported { get; private set; }
        public bool MipmapNonPow2Supported { get; private set; }

        public MyGraphicTest()
        {
            MaxTextureSize = 2048;
            Rgba1010102Supported = false;
            MipmapNonPow2Supported = false;
        }

        // Testing function call - creates DX9 device & present test:
        public bool TestDX()
        {
            bool isGraphicsSupported = true;
            MyMwcLog.WriteLine("MyGraphicTest.TestDX() - START");
            MyMwcLog.IncreaseIndent();

            PresentParameters newPresentParameters;
            Direct3D d3dh = null;
            Device d3d = null;

            try
            {
                MyMwcLog.WriteLine("Direct3D call");
                d3dh = new Direct3D();
                MyMwcLog.WriteLine("Direct3D call end");

                if (d3dh == null)
                {
                    throw new Exception("Cannot create Direct3D object");
                }
                else
                    MyMwcLog.WriteLine("d3dh handle ok ");

                // DX:
                newPresentParameters = new PresentParameters();
                newPresentParameters.Windowed = true;
                newPresentParameters.InitDefaults();

                d3d = new Device(d3dh, 0, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, newPresentParameters);

                if (d3d == null)
                {
                    throw new Exception("Cannot create Direct3D Device");
                }
                else
                    MyMwcLog.WriteLine("d3d handle ok ");

                isGraphicsSupported &= !TestCapabilities(d3d, d3dh);

                MaxTextureSize = d3d.Capabilities.MaxTextureWidth;

                Rgba1010102Supported = d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.RenderTarget, ResourceType.Surface, Format.A2R10G10B10);
                MipmapNonPow2Supported = !d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.Pow2) &&
                    !d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.NonPow2Conditional) &&
                    d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.MipMap);
            }
            catch (Exception ex)
            {
                MyMwcLog.WriteLine("Exception throwed by DX test. Source: " + ex.Source);
                MyMwcLog.WriteLine("Message: " + ex.Message);
                MyMwcLog.WriteLine("Inner exception: " + ex.InnerException);
                MyMwcLog.WriteLine("Exception details" + ex.ToString());
                //consider returning error here
                //retValue = false;
            }
            finally
            {
                if (d3dh != null)
                {
                    d3dh.Dispose();
                    d3dh = null;
                }
                if (d3d != null)
                {
                    d3d.Dispose();
                    d3d = null;
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGraphicTest.TestDX() - END");

            return isGraphicsSupported;
        }

        // Testing values for correct Reach: 
        private void SetReachTestSettings()
        {
            {
                m_VertexShaderVersionMinor = 0;
                m_VertexShaderVersionMajor = 2;
                m_PixelShaderVersionMinor = 0;
                m_PixelShaderVersionMajor = 2;
                m_SeparateAlphaBlend = false;
                m_DestBlendSrcAlphaSat = false;
                m_MaxPrimitiveCount = 65535;
                m_IndexElementSize32 = false;
                m_MaxVertexStreams = 16;
                m_MaxStreamStride = 255;
                m_MaxTextureSize = 2048;
                m_MaxVolumeExtent = 0;
                m_MaxTextureAspectRatio = 2048;
                m_MaxVertexSamplers = 0;
                m_MaxRenderTargets = 1;
                m_NonPow2Unconditional = false;
                m_NonPow2Cube = false;
                m_NonPow2Volume = false;
                m_ValidTextureFormats = new List<Format>();
                m_ValidTextureFormats.Add(Format.A8R8G8B8);
                m_ValidTextureFormats.Add(Format.R5G6B5);
                m_ValidTextureFormats.Add(Format.A1R5G5B5);
                m_ValidTextureFormats.Add(Format.A4R4G4B4);
                m_ValidTextureFormats.Add(Format.Dxt1);
                m_ValidTextureFormats.Add(Format.Dxt3);
                m_ValidTextureFormats.Add(Format.Dxt5);
                m_ValidTextureFormats.Add(Format.Q8W8V8U8);
                /*(SurfaceFormat.Color,SurfaceFormat.Bgr565,SurfaceFormat.Bgra5551,SurfaceFormat.Bgra4444,
                    SurfaceFormat.Dxt1, SurfaceFormat.Dxt3, SurfaceFormat.Dxt5,
                    SurfaceFormat.NormalizedByte2, SurfaceFormat.NormalizedByte4);*/
                m_ValidCubeFormats = new List<Format>();
                m_ValidCubeFormats.Add(Format.A8R8G8B8);
                m_ValidCubeFormats.Add(Format.R5G6B5);
                m_ValidCubeFormats.Add(Format.A1R5G5B5);
                m_ValidCubeFormats.Add(Format.A4R4G4B4);
                m_ValidCubeFormats.Add(Format.Dxt1);
                m_ValidCubeFormats.Add(Format.Dxt3);
                m_ValidCubeFormats.Add(Format.Dxt5);
                /*(SurfaceFormat.Color,SurfaceFormat.Bgr565,SurfaceFormat.Bgra5551,SurfaceFormat.Bgra4444,
                    SurfaceFormat.Dxt1, SurfaceFormat.Dxt3, SurfaceFormat.Dxt5);*/
                m_ValidVolumeFormats = new List<Format>();
                m_ValidVertexTextureFormats = new List<Format>();
                m_InvalidFilterFormats = new List<Format>();
                m_InvalidBlendFormats = new List<Format>();
                m_ValidVertexFormats = new List<DeclarationType>();
                m_ValidVertexFormats.Add(DeclarationType.Color);
                m_ValidVertexFormats.Add(DeclarationType.Float1);
                m_ValidVertexFormats.Add(DeclarationType.Float2);
                m_ValidVertexFormats.Add(DeclarationType.Float3);
                m_ValidVertexFormats.Add(DeclarationType.Float4);
                m_ValidVertexFormats.Add(DeclarationType.UByte4N);
                m_ValidVertexFormats.Add(DeclarationType.Short2);
                m_ValidVertexFormats.Add(DeclarationType.Short4);
                m_ValidVertexFormats.Add(DeclarationType.Short2N);
                m_ValidVertexFormats.Add(DeclarationType.Short4N);
                m_ValidVertexFormats.Add(DeclarationType.HalfTwo);
                m_ValidVertexFormats.Add(DeclarationType.HalfFour);
            }
        }

         // Same settings as Reach but with pixel & vertex shaders v 3_0 and above:
        private void SetMinerWarsTestSettings()
        {
            // same as Reach but with pixel & vertex sahder >=3.0:
            SetReachTestSettings();
            {
                m_VertexShaderVersionMinor = 0;
                m_VertexShaderVersionMajor = 3;
                m_PixelShaderVersionMinor = 0;
                m_PixelShaderVersionMajor = 3;
            }
        }

        // Test profile:
        private bool TestCapabilities(Device d3d, Direct3D d3dh)
        {
            SetMinerWarsTestSettings();
            return TestCurrentSettings(d3d, d3dh);
        }

        // Own DX capability testing function:
        private bool TestCurrentSettings(Device d3d, Direct3D d3dh)
        {
            MyMwcLog.WriteLine("MyGraphicTest.TestCurrentSettings() - START");
            MyMwcLog.IncreaseIndent();

            bool isError = false;

            // Write adapter information to the LOG file:
            MyMwcLog.WriteLine("Adapters count: " + d3dh.AdapterCount);
            MyMwcLog.WriteLine("Adapter:");
                                 
            MyMwcLog.IncreaseIndent();
            for(int i = 0; i < d3dh.AdapterCount; i++)
            {
                var detail = d3dh.GetAdapterIdentifier(i);
                if (detail != null)
                {
           
                    MyMwcLog.WriteLine("Ordinal ID: " + detail.DeviceId);
                    MyMwcLog.WriteLine("Description: " + detail.Description);
                    MyMwcLog.WriteLine("Vendor ID: " + detail.VendorId);
                    MyMwcLog.WriteLine("Device name: " + detail.DeviceName);
                    MyMwcLog.WriteLine("Device identifier: " + detail.DeviceIdentifier.ToString());
                    MyMwcLog.WriteLine("Driver name: " + detail.Driver);
                    MyMwcLog.WriteLine("Driver version: " + detail.DriverVersion);
                    MyMwcLog.WriteLine("Identifier of the adapret chip: " + detail.DeviceId);
                    MyMwcLog.WriteLine("Adapter certified: " + (detail.Certified ? "YES" : "NO"));
                    if (detail.Certified) MyMwcLog.WriteLine("Certification date: " + detail.CertificationDate);
                    MyMwcLog.WriteLine("Adapter revision: " + detail.Revision);
                    MyMwcLog.WriteLine("Subsystem ID: " + detail.SubsystemId);
                    MyMwcLog.WriteLine("WHQL level: " + detail.WhqlLevel);
                    MyMwcLog.WriteLine("Vertex shader version: " + d3d.Capabilities.PixelShaderVersion.Major + "." + d3d.Capabilities.PixelShaderVersion.Minor);
                    MyMwcLog.WriteLine("Pixel shader version:  " + d3d.Capabilities.PixelShaderVersion.Major + "." + d3d.Capabilities.PixelShaderVersion.Minor);
                    MyMwcLog.WriteLine("Max primitives count:  " + d3d.Capabilities.MaxPrimitiveCount);
                    MyMwcLog.WriteLine("Max texture width:     " + d3d.Capabilities.MaxTextureWidth);
                    MyMwcLog.WriteLine("Max texture height:    " + d3d.Capabilities.MaxTextureHeight);
                    MyMwcLog.WriteLine("Max vertex streams:    " + d3d.Capabilities.MaxStreams);
                    MyMwcLog.WriteLine("Max render targets:    " + d3d.Capabilities.SimultaneousRTCount);
                }
            }
            MyMwcLog.DecreaseIndent();

            
            StringBuilder deviceAbility = new StringBuilder();

            // Test only shared versions from now:
            // Test vertex shader version:
            if (!(d3d.Capabilities.VertexShaderVersion.Major >= m_VertexShaderVersionMajor &&
                d3d.Capabilities.VertexShaderVersion.Minor >= m_VertexShaderVersionMinor))
            {
                MyMessageBox.Show(MyTextConstants.APP_NAME, MyTextConstants.REQUIRE_PIXEL_SHADER_3);
                MyMwcLog.WriteLine(MyTextConstants.REQUIRE_PIXEL_SHADER_3);
                isError = true;
            }
            // Test pixel shader version:
            if (!(d3d.Capabilities.PixelShaderVersion.Major >= m_PixelShaderVersionMajor &&
                d3d.Capabilities.PixelShaderVersion.Minor >= m_PixelShaderVersionMinor))
            {
                MyMwcLog.WriteLine(MyTextConstants.REQUIRE_VERTEX_SHADER_3);
                isError = true;
            }
            // Test basic rendering caps:
            if (d3d.Capabilities.MaxPrimitiveCount < m_MaxPrimitiveCount)
            {
                MyMwcLog.WriteLine("MaxPrimitiveCount smaller than needed");
                isError = true;
            }
            if (d3d.Capabilities.MaxStreams < m_MaxVertexStreams)
            {
                MyMwcLog.WriteLine("MaxVertexStreams smaller than needed");
                isError = true;
            }
            if (d3d.Capabilities.MaxStreamStride < m_MaxStreamStride)
            {
                MyMwcLog.WriteLine("MaxStreamStride smaller than needed");
                isError = true;
            }
            if (d3d.Capabilities.MaxVertexIndex < (m_IndexElementSize32 ? 16777214 : 65634))
            {
                MyMwcLog.WriteLine("MaxVertexIndex smaller than needed");
                isError = true;
            }
            if (!(d3d.Capabilities.DeviceCaps2.HasFlag(DeviceCaps2.CanStretchRectFromTextures)))
            {
                MyMwcLog.WriteLine("Device doesn't have  RectFromTextures");
                isError = true;
            }
            if (!(d3d.Capabilities.DeviceCaps2.HasFlag(DeviceCaps2.StreamOffset)))
            {
                MyMwcLog.WriteLine("Device doesn't have StreamOffset ability");
                isError = true;
            }
            if (!(d3d.Capabilities.RasterCaps.HasFlag(RasterCaps.DepthBias)))
            {
                MyMwcLog.WriteLine("Device doesn't have DepthBias ability in RasterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.RasterCaps.HasFlag(RasterCaps.MipMapLodBias)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipMapLodBias ability in RasterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.RasterCaps.HasFlag(RasterCaps.ScissorTest)))
            {
                MyMwcLog.WriteLine("Device doesn't have ScissorTest ability in RasterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.RasterCaps.HasFlag(RasterCaps.SlopeScaleDepthBias)))
            {
                MyMwcLog.WriteLine("Device doesn't have SlopeScaleDepthBias ability in RasterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.ShadeCaps.HasFlag(ShadeCaps.ColorGouraudRgb)))
            {
                MyMwcLog.WriteLine("Device doesn't have ColorGouraudRgb ability in ShadeCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.ShadeCaps.HasFlag(ShadeCaps.AlphaGouraudBlend)))
            {
                MyMwcLog.WriteLine("Device doesn't have AlphaGouraudBlend ability in ShadeCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.MaskZ)))
            {
                MyMwcLog.WriteLine("Device doesn't have MaskZ ability in PrimitiveMiscCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.CullNone)))
            {
                MyMwcLog.WriteLine("Device doesn't have CullNone ability in PrimitiveMiscCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.CullCW)))
            {
                MyMwcLog.WriteLine("Device doesn't have CullCW ability in PrimitiveMiscCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.CullCCW)))
            {
                MyMwcLog.WriteLine("Device doesn't have CullCCW ability in PrimitiveMiscCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.ColorWriteEnable)))
            {
                MyMwcLog.WriteLine("Device doesn't have ColorWriteEnable ability in PrimitiveMiscCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.BlendOperation)))
            {
                MyMwcLog.WriteLine("Device doesn't have BlendOperation ability in PrimitiveMiscCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.LineCaps.HasFlag(LineCaps.Blend)))
            {
                MyMwcLog.WriteLine("Device doesn't have Blend ability in LineCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.LineCaps.HasFlag(LineCaps.Texture)))
            {
                MyMwcLog.WriteLine("Device doesn't have Texture ability in LineCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.LineCaps.HasFlag(LineCaps.DepthTest)))
            {
                MyMwcLog.WriteLine("Device doesn't have DepthTest ability in LineCaps");
                isError = true;
            }
            // Test depth test:
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.Always)))
            {
                MyMwcLog.WriteLine("Device doesn't have Always ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.Equal)))
            {
                MyMwcLog.WriteLine("Device doesn't have Equal ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.Greater)))
            {
                MyMwcLog.WriteLine("Device doesn't have Greater ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.GreaterEqual)))
            {
                MyMwcLog.WriteLine("Device doesn't have GreaterEqual ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.Less)))
            {
                MyMwcLog.WriteLine("Device doesn't have Less ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.LessEqual)))
            {
                MyMwcLog.WriteLine("Device doesn't have LessEqual ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.Never)))
            {
                MyMwcLog.WriteLine("Device doesn't have Never ability in DepthCompareCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DepthCompareCaps.HasFlag(CompareCaps.NotEqual)))
            {
                MyMwcLog.WriteLine("Device doesn't have NotEqual ability in DepthCompareCaps");
                isError = true;
            }
            // Test stencil test:
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.Decrement)))
            {
                MyMwcLog.WriteLine("Device doesn't have Decrement ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.DecrementClamp)))
            {
                MyMwcLog.WriteLine("Device doesn't have DecrementClamp ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.Increment)))
            {
                MyMwcLog.WriteLine("Device doesn't have Increment ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.IncrementClamp)))
            {
                MyMwcLog.WriteLine("Device doesn't have IncrementClamp ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.Invert)))
            {
                MyMwcLog.WriteLine("Device doesn't have Invert ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.Keep)))
            {
                MyMwcLog.WriteLine("Device doesn't have Keep ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.Replace)))
            {
                MyMwcLog.WriteLine("Device doesn't have Replace ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.TwoSided)))
            {
                MyMwcLog.WriteLine("Device doesn't have TwoSided ability in StencilCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.StencilCaps.HasFlag(StencilCaps.Zero)))
            {
                MyMwcLog.WriteLine("Device doesn't have Zero ability in StencilCaps");
                isError = true;
            }
            // Test blending caps:
            // source:
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.BlendFactor)))
            {
                MyMwcLog.WriteLine("Device doesn't have BlendFactor ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.DestinationAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have DestinationAlpha ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.DestinationColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have DestinationColor ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.InverseDestinationAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseDestinationAlpha ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.InverseDestinationColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseDestinationColor ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.InverseSourceAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseSourceAlpha ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.InverseSourceColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseSourceColor ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.One)))
            {
                MyMwcLog.WriteLine("Device doesn't have One ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.SourceAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have SourceAlpha ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.SourceAlphaSaturated)))
            {
                MyMwcLog.WriteLine("Device doesn't have SourceAlphaSaturated ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.SourceColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have SourceColor ability in SourceBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.SourceBlendCaps.HasFlag(BlendCaps.Zero)))
            {
                MyMwcLog.WriteLine("Device doesn't have Zero ability in SourceBlendCaps");
                isError = true;
            }
            // destination:
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.BlendFactor)))
            {
                MyMwcLog.WriteLine("Device doesn't have BlendFactor ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.DestinationAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have DestinationAlpha ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.DestinationColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have DestinationColor ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.InverseDestinationAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseDestinationAlpha ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.InverseDestinationColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseDestinationColor ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.InverseSourceAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseSourceAlpha ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.InverseSourceColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have InverseSourceColor ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.One)))
            {
                MyMwcLog.WriteLine("Device doesn't have One ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.SourceAlpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have SourceAlpha ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.SourceColor)))
            {
                MyMwcLog.WriteLine("Device doesn't have SourceColor ability in DestinationBlendCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.Zero)))
            {
                MyMwcLog.WriteLine("Device doesn't have Zero ability in DestinationBlendCaps");
                isError = true;
            }
            // simply test blend source alpha saturation:
            if (m_DestBlendSrcAlphaSat)
            {
                if (!(d3d.Capabilities.DestinationBlendCaps.HasFlag(BlendCaps.SourceAlphaSaturated)))
                {
                    MyMwcLog.WriteLine("Device doesn't have BlendSourceAlphaSaturated ability in DestinationBlendCaps");
                    isError = true;
                }
            }
            // Simply test separate alpha blend:
            if (m_SeparateAlphaBlend)
            {
                if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.SeparateAlphaBlend)))
                {
                    MyMwcLog.WriteLine("Device doesn't have SeparateAlphaBlend ability in PrimitiveMiscCaps");
                    isError = true;
                }
            }
            // Test multiple render targets:
            if (d3d.Capabilities.SimultaneousRTCount < m_MaxRenderTargets)
            {
                MyMwcLog.WriteLine("MaxRenderTargets smaller than needed");
                isError = true;
            }
            if (m_MaxRenderTargets > 1)
            {
                if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.IndependentWriteMasks)))
                {
                    MyMwcLog.WriteLine("Device doesn't have IndependentWriteMasks ability in PrimitiveMiscCaps for more than 1 render targets");
                    isError = true;
                }
                if (!(d3d.Capabilities.PrimitiveMiscCaps.HasFlag(PrimitiveMiscCaps.MrtPostPixelShaderBlending)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MrtPostPixelShaderBlending ability in PrimitiveMiscCaps for more than 1 render targets");
                    isError = true;
                }
            }
            // Test texturing abilities:
            if (d3d.Capabilities.MaxTextureWidth < m_MaxTextureSize)
            {
                MyMwcLog.WriteLine("MaxTextureWidth smaller than needed");
                isError = true;
            }
            if (d3d.Capabilities.MaxTextureHeight < m_MaxTextureSize)
            {
                MyMwcLog.WriteLine("MaxTextureHeight smaller than needed");
                isError = true;
            }
            // Test aspect ration:
            if (d3d.Capabilities.MaxTextureAspectRatio > 0)
            {
                if (d3d.Capabilities.MaxTextureAspectRatio < m_MaxTextureAspectRatio)
                {
                    MyMwcLog.WriteLine("MaxTextureAspectRatio smaller than needed");
                    isError = true;
                }
            }
            // Test textures abilities:
            if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.Alpha)))
            {
                MyMwcLog.WriteLine("Device doesn't have Alpha ability in TextureCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.MipMap)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipMap ability in TextureCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.CubeMap)))
            {
                MyMwcLog.WriteLine("Device doesn't have CubeMap ability in TextureCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.MipCubeMap)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipCubeMap ability in TextureCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.Perspective)))
            {
                MyMwcLog.WriteLine("Device doesn't have Perspective ability in TextureCaps");
                isError = true;
            }
            if (d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.SquareOnly))
            {
                MyMwcLog.WriteLine("Device doesn't have SquareOnly ability in TextureCaps");
                isError = true;
            }
            // Test texture address caps:
            if (!(d3d.Capabilities.TextureAddressCaps.HasFlag(TextureAddressCaps.Clamp)))
            {
                MyMwcLog.WriteLine("Device doesn't have Clamp ability in TextureAddressCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureAddressCaps.HasFlag(TextureAddressCaps.Wrap)))
            {
                MyMwcLog.WriteLine("Device doesn't have Wrap ability in TextureAddressCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureAddressCaps.HasFlag(TextureAddressCaps.Mirror)))
            {
                MyMwcLog.WriteLine("Device doesn't have Mirror ability in TextureAddressCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureAddressCaps.HasFlag(TextureAddressCaps.IndependentUV)))
            {
                MyMwcLog.WriteLine("Device doesn't have IndependentUV ability in TextureAddressCaps");
                isError = true;
            }
            // Test texture filter caps:
            if (!(d3d.Capabilities.TextureFilterCaps.HasFlag(FilterCaps.MagPoint)))
            {
                MyMwcLog.WriteLine("Device doesn't have MagPoint ability in TextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureFilterCaps.HasFlag(FilterCaps.MagLinear)))
            {
                MyMwcLog.WriteLine("Device doesn't have MagLinear ability in TextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureFilterCaps.HasFlag(FilterCaps.MinPoint)))
            {
                MyMwcLog.WriteLine("Device doesn't have MinPoint ability in TextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureFilterCaps.HasFlag(FilterCaps.MinLinear)))
            {
                MyMwcLog.WriteLine("Device doesn't have MinLinear ability in TextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureFilterCaps.HasFlag(FilterCaps.MipPoint)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipPoint ability in TextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.TextureFilterCaps.HasFlag(FilterCaps.MipLinear)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipLinear ability in TextureFilterCaps");
                isError = true;
            }
            // test cube texture filter caps:
            if (!(d3d.Capabilities.CubeTextureFilterCaps.HasFlag(FilterCaps.MagPoint)))
            {
                MyMwcLog.WriteLine("Device doesn't have MagPoint ability in CubeTextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.CubeTextureFilterCaps.HasFlag(FilterCaps.MagLinear)))
            {
                MyMwcLog.WriteLine("Device doesn't have MagLinear ability in CubeTextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.CubeTextureFilterCaps.HasFlag(FilterCaps.MinPoint)))
            {
                MyMwcLog.WriteLine("Device doesn't have MinPoint ability in CubeTextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.CubeTextureFilterCaps.HasFlag(FilterCaps.MinLinear)))
            {
                MyMwcLog.WriteLine("Device doesn't have MinLinear ability in CubeTextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.CubeTextureFilterCaps.HasFlag(FilterCaps.MipPoint)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipPoint ability in CubeTextureFilterCaps");
                isError = true;
            }
            if (!(d3d.Capabilities.CubeTextureFilterCaps.HasFlag(FilterCaps.MipLinear)))
            {
                MyMwcLog.WriteLine("Device doesn't have MipLinear ability in CubeTextureFilterCaps");
                isError = true;
            }
            // test volume texures:
            if (m_MaxVolumeExtent > 0)
            {
                if (d3d.Capabilities.MaxVolumeExtent < m_MaxVolumeExtent)
                {
                    MyMwcLog.WriteLine("MaxVolumeExtent smaller than needed");
                    isError = true;
                }

                // test volume maps:
                if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.VolumeMap)))
                {
                    MyMwcLog.WriteLine("Device doesn't have VolumeMap ability in TextureCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.MipVolumeMap)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MipVolumeMap ability in TextureCaps");
                    isError = true;
                }
                // test volume texture address caps:
                if (!(d3d.Capabilities.VolumeTextureAddressCaps.HasFlag(TextureAddressCaps.Clamp)))
                {
                    MyMwcLog.WriteLine("Device doesn't have Clamp ability in VolumeTextureAddressCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureAddressCaps.HasFlag(TextureAddressCaps.Wrap)))
                {
                    MyMwcLog.WriteLine("Device doesn't have Wrap ability in VolumeTextureAddressCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureAddressCaps.HasFlag(TextureAddressCaps.Mirror)))
                {
                    MyMwcLog.WriteLine("Device doesn't have Mirror ability in VolumeTextureAddressCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureAddressCaps.HasFlag(TextureAddressCaps.IndependentUV)))
                {
                    MyMwcLog.WriteLine("Device doesn't have IndependentUV ability in VolumeTextureAddressCaps");
                    isError = true;
                }
                // test volume texture filter caps:
                if (!(d3d.Capabilities.VolumeTextureFilterCaps.HasFlag(FilterCaps.MagPoint)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MagPoint ability in VolumeTextureFilterCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureFilterCaps.HasFlag(FilterCaps.MagLinear)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MagLinear ability in VolumeTextureFilterCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureFilterCaps.HasFlag(FilterCaps.MinPoint)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MinPoint ability in VolumeTextureFilterCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureFilterCaps.HasFlag(FilterCaps.MinLinear)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MinLinear ability in VolumeTextureFilterCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureFilterCaps.HasFlag(FilterCaps.MipPoint)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MipPoint ability in VolumeTextureFilterCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VolumeTextureFilterCaps.HasFlag(FilterCaps.MipLinear)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MipLinear ability in VolumeTextureFilterCaps");
                    isError = true;
                }
            }
            // test non power of two textures:
            if (m_NonPow2Unconditional)
            {
                if (d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.Pow2))
                {
                    MyMwcLog.WriteLine("Device doesn't have Pow2textures ability in TextureCaps");
                    isError = true;
                }
            }
            else
            {
                if (d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.Pow2))
                {
                    if (!(d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.NonPow2Conditional)))
                    {
                        MyMwcLog.WriteLine("Device doesn't have NonPow2Conditional ability in TextureCaps");
                        isError = true;
                    }
                }
            }
            // test non power of two cube textures:
            if (m_NonPow2Cube)
            {
                if (d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.CubeMapPow2))
                {
                    MyMwcLog.WriteLine("Device doesn't have CubeMapPow2 ability in TextureCaps");
                    isError = true;
                }
            }
            // test non power of two volume textures:
            if (m_NonPow2Volume)
            {
                if (d3d.Capabilities.TextureCaps.HasFlag(TextureCaps.VolumeMapPow2))
                {
                    MyMwcLog.WriteLine("Device doesn't have VolumeMapPow2 ability in TextureCaps");
                    isError = true;
                }
            }
            // Test vertex texturing:
            if (m_MaxVertexSamplers > 0)
            {
                if (!(d3d.Capabilities.VertexTextureFilterCaps.HasFlag(FilterCaps.MagPoint)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MagPoint ability in VertexTextureFilterCaps");
                    isError = true;
                }
                if (!(d3d.Capabilities.VertexTextureFilterCaps.HasFlag(FilterCaps.MinPoint)))
                {
                    MyMwcLog.WriteLine("Device doesn't have MinPoint ability in VertexTextureFilterCaps");
                    isError = true;
                }
            }
            // Test vertex element formats:
            if (m_ValidVertexFormats != null)
            {
                foreach (DeclarationType format in m_ValidVertexFormats)
                {
                    switch (format)
                    {
                        case DeclarationType.Color:
                            if (!(d3d.Capabilities.DeclarationTypes.HasFlag(DeclarationTypeCaps.UByte4N)))
                            {
                                MyMwcLog.WriteLine("Device doesn't have UByte4N as VertexElementFormat type in Declaration");
                                isError = true;
                            }
                            break;
                        case DeclarationType.UByte4N:
                            if (!(d3d.Capabilities.DeclarationTypes.HasFlag(DeclarationTypeCaps.UByte4)))
                            {
                                MyMwcLog.WriteLine("Device doesn't have UByte4 as VertexElementFormat type in Declaration");
                                isError = true;
                            }
                            break;
                        case DeclarationType.Short2N:
                            if (!(d3d.Capabilities.DeclarationTypes.HasFlag(DeclarationTypeCaps.Short2N)))
                            {
                                MyMwcLog.WriteLine("Device doesn't have NormalizedShort2 as VertexElementFormat type in Declaration");
                                isError = true;
                            }
                            break;
                        case DeclarationType.Short4N:
                            if (!(d3d.Capabilities.DeclarationTypes.HasFlag(DeclarationTypeCaps.Short4N)))
                            {
                                MyMwcLog.WriteLine("Device doesn't have Short4N as VertexElementFormat type in Declaration");
                                isError = true;
                            }
                            break;
                        case DeclarationType.HalfTwo:
                            if (!(d3d.Capabilities.DeclarationTypes.HasFlag(DeclarationTypeCaps.HalfTwo)))
                            {
                                MyMwcLog.WriteLine("Device doesn't have HalfTwo as VertexElementFormat type in Declaration");
                                isError = true;
                            }
                            break;
                        case DeclarationType.HalfFour:
                            if (!(d3d.Capabilities.DeclarationTypes.HasFlag(DeclarationTypeCaps.HalfFour)))
                            {
                                MyMwcLog.WriteLine("Device doesn't have UByte4N as VertexElementFormat type in Declaration");
                                isError = true;
                            }
                            break;
                    }
                }
            }
            // Test texture formats:
            if (m_ValidTextureFormats != null)
            {
                foreach (Format format in m_ValidTextureFormats)
                {
                    // format supported?
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, 0, ResourceType.Texture, format))
                    {
                        string text = String.Format("Device doesn't support DX texture format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // does this format support mipmapping?
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.QueryWrapAndMip, ResourceType.Texture, format))
                    {
                        string text = String.Format("Device doesn't support MipMapping for texture DX format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // does this format support filtering?
                    if (!m_InvalidFilterFormats.Contains(format))
                    {
                        if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.QueryFilter, ResourceType.Texture, format))
                        {
                            string text = String.Format("Device doesn't support QueryFiltering for texture DX format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                            MyMwcLog.WriteLine(text);
                            isError = true;
                        }
                    }
                }
            }
            // Test cubemap formats:
            if (m_ValidCubeFormats != null)
            {
                foreach (Format format in m_ValidCubeFormats)
                {
                    // format supported?
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, 0, ResourceType.CubeTexture, format))
                    {
                        string text = String.Format("Device doesn't support DX texture format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // does this format support mipmapping?
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.QueryWrapAndMip, ResourceType.CubeTexture, format))
                    {
                        string text = String.Format("Device doesn't support MipMapping for texture DX format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // does this format support filtering?
                    if (!m_InvalidFilterFormats.Contains(format))
                    {
                        if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.QueryFilter, ResourceType.CubeTexture, format))
                        {
                            string text = String.Format("Device doesn't support QueryFiltering for texture DX format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                            MyMwcLog.WriteLine(text);
                            isError = true;
                        }
                    }
                }
            }
            // Test volume texture formats:
            if (m_ValidVolumeFormats != null)
            {
                foreach (Format format in m_ValidVolumeFormats)
                {
                    // format supported?
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, 0, ResourceType.VolumeTexture, format))
                    {
                        string text = String.Format("Device doesn't support DX texture format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // does this format support mipmapping?
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.QueryWrapAndMip, ResourceType.VolumeTexture, format))
                    {
                        string text = String.Format("Device doesn't support MipMapping for texture DX format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // does this format support filtering?
                    if (!m_InvalidFilterFormats.Contains(format))
                    {
                        if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, Usage.QueryFilter, ResourceType.VolumeTexture, format))
                        {
                            string text = String.Format("Device doesn't support QueryFiltering for texture DX format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                            MyMwcLog.WriteLine(text);
                            isError = true;
                        }
                    }
                }
            }
            // Test vertex texture format:
            if (m_ValidVertexTextureFormats != null)
            {
                foreach (Format format in m_ValidVertexTextureFormats)
                {
                    Usage usage = Usage.QueryVertexTexture | Usage.QueryWrapAndMip;
                    if (!m_InvalidBlendFormats.Contains(format))
                    {
                        usage |= Usage.QueryFilter;
                    }
                    // 2D vertex texture:
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, usage, ResourceType.Texture, format))
                    {
                        string text = String.Format("Device doesn't support VertexTextureFormat for DX texture format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // Cubemap vertex texture:
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, usage, ResourceType.Texture, format))
                    {
                        string text = String.Format("Device doesn't support VertexCubemapTextureFormat for DX texture format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                    // Volume vertex texture:
                    if (!d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, usage, ResourceType.Texture, format))
                    {
                        string text = String.Format("Device doesn't support VertexVolumeTextureFormat for DX texture format { 0 } [XNA format: { 0 }]", format.ToString(), format.ToString());
                        MyMwcLog.WriteLine(text);
                        isError = true;
                        continue;
                    }
                }
            }
            // Test render target format:
            if (m_InvalidBlendFormats != null)
            {
                Usage usage = Usage.RenderTarget;
                if (!m_InvalidBlendFormats.Contains(Format.A8R8G8B8))
                {
                    usage |= Usage.QueryPostPixelShaderBlending;
                }
                if (!(d3dh.CheckDeviceFormat(0, DeviceType.Hardware, Format.X8R8G8B8, usage, ResourceType.Surface, Format.A8R8G8B8)))
                {
                    string text = String.Format("Device doesn't support RenderTarget for DX texture format { 0 } [XNA format: { 0 }]", Format.A8R8G8B8.ToString(), Format.A8R8G8B8.ToString());
                    MyMwcLog.WriteLine(text);
                    isError = true;
                }
            }

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyGraphicTest.TestCurrentSettings() - END");

            return isError;
        }
    }
}
