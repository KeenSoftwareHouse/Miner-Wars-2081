using System;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.App;
using MinerWarsMath.Graphics;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.InfluenceSpheres;
using MinerWars.AppCode.Game.Textures;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    using MinerWars.AppCode.Game.Audio;
    using System.Diagnostics;
    using SysUtils.Utils;
    using MinerWars.AppCode.Game.Utils;

    static class MyGuiInfluenceSphereHelpers
    {
        static Dictionary<short, MyGuiInfluenceSphereHelper> m_influenceSphereSoundHelpers = new Dictionary<short, MyGuiInfluenceSphereHelper>();

        //Arrays of enums values
        public static Array MyInfluenceSphereSoundHelperTypesEnumValues { get; private set; }

        static MyGuiInfluenceSphereHelpers()
        {
            MyMwcLog.WriteLine("MyGuiInfluenceSphereHelpers()");

            MyInfluenceSphereSoundHelperTypesEnumValues = Enum.GetValues(typeof(MySoundCuesEnum));
        }

        public static MyGuiInfluenceSphereHelper GetInfluenceSphereSoundHelper(MySoundCuesEnum soundEnum)
        {
            MyGuiInfluenceSphereHelper ret;
            if (m_influenceSphereSoundHelpers.TryGetValue((short)soundEnum, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {
            m_influenceSphereSoundHelpers.Clear();
        }

        public static void LoadContent()
        {

            #region Sound influence sphere music helpers
            
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomLarge01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomLarge01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomLarge02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomLarge02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomLarge03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomLarge03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomLarge04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomLarge04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomLarge05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomLarge05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomMed01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomMed01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomMed02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomMed02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomMed03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomMed03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomMed04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomMed04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomMed05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomMed05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomSmall01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomSmall01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomSmall02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomSmall02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomSmall03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomSmall03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RoomSmall04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RoomSmall04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_StressLoop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_StressLoop]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_TunnelLarge01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_TunnelLarge01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_TunnelMedium01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_TunnelMedium01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_TunnelSmall01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_TunnelSmall01]));
            /*
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Electrical01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Electrical01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Electrical02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Electrical02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanLargeDamaged,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanLargeDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanLargeDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanLargeDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanLargeNormal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanLargeNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanMediumDamaged,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanMediumDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanMediumDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanMediumDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanMediumNormal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanMediumNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanSmallDamaged,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanSmallDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanSmallDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanSmallDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_FanSmallNormal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_FanSmallNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenLargeDamaged,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenLargeDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenLargeDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenLargeDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenLargeNormal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenLargeNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenMediumDamaged,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenMediumDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenMediumDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenMediumDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenMediumNormal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenMediumNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenSmallDamaged,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenSmallDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenSmallDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenSmallDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenSmallNormal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenSmallNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_PipeFlow01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_PipeFlow01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterAllied01_04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterAllied01_04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterAllied05_08,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterAllied05_08]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterAllied09_12,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterAllied09_12]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterAllied13_16,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterAllied13_16]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterChinese01_04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterChinese01_04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterChinese05_08,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterChinese05_08]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterChinese09_12,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterChinese09_12]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterChinese13_16,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterChinese13_16]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterRussian01_04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterRussian01_04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterRussian05_08,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterRussian05_08]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterRussian09_12,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterRussian09_12]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_RadioChatterRussian13_16,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_RadioChatterRussian13_16]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Spark01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Spark01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamDischarge01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamDischarge01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamDischarge02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamDischarge02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamDischarge03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamDischarge03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamDischarge04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamDischarge04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamLoop01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamLoop01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamLoop02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamLoop02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamLoop03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamLoop03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamLoop04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamLoop04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_SteamLoop05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_SteamLoop05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_ThunderClapLarge,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_ThunderClapLarge]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_ThunderClapMed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_ThunderClapMed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_ThunderClapSmall,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_ThunderClapSmall]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Welding01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Welding01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_PrefabFire,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_PrefabFire]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_EngineThrust,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_EngineThrust]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiMouseClick,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiClick));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorFlyOutsideBorder,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.GuiEditorFlyOutsideBorder]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectAttach,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.GuiEditorObjectAttach]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectDelete,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.GuiEditorObjectDelete]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectDetach,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.GuiEditorObjectDetach]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectMoveInvalid,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorObjectMoveInvalid));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectMoveStep,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorObjectMoveStep));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectRotateStep,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorObjectRotateStep));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorObjectSelect,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorObjectSelect));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorPrefabCommit,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorPrefabCommit));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorPrefabEnter,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorPrefabEnter));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorPrefabExit,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorPrefabExit));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorVoxelHandAdd,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorVoxelHandAdd));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorVoxelHandMaterial,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorVoxelHandMaterial));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorVoxelHandRemove,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorVoxelHandRemove));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorVoxelHandSoften,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorVoxelHandSoften));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiEditorVoxelHandSwitch,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiEditorVoxelHandSwitch));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiMouseOver,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiMouseOver));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiWheelControlClose,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiWheelControlClose));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.GuiWheelControlOpen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.GuiWheelControlOpen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpBulletHitGlass,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpBulletHitGlass));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpBulletHitMetal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpBulletHitMetal));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpBulletHitRock,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpBulletHitRock));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpBulletHitShip,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpBulletHitShip));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpExpHitGlass,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpExpBulletHitGlass));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpExpHitMetal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpExpBulletHitMetal));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpExpHitRock,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpExpBulletHitRock));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpExpHitShip,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpExpBulletHitShip));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpPlayerShipCollideMetal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpPlayerShipCollideMetal));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpPlayerShipCollideRock,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpPlayerShipCollideRock));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpPlayerShipCollideShip,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpPlayerShipCollideShip));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpPlayerShipScrapeShipLoop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpPlayerShipScrapeShipLoop));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpPlayerShipScrapeShipRelease,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpPlayerShipScrapeShipRelease));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpRockCollideMetal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpRockCollideMetal));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpRockCollideRock,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpRockCollideRock));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpShipCollideMetal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpShipCollideMetal));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpShipCollideRock,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpShipCollideRock));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.ImpShipQuake,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.ImpShipQuake));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MenuWelcome,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MenuWelcome));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock1End,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock1End));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock1Loop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock1Loop));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock1Start,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock1Start));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock2End,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock2End));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock2Loop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock2Loop));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock2Start,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock2Start));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock3End,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock3End));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock3Loop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock3Loop));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDock3Start,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDock3Start));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor1AClose,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor1AClose));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor1AOpen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor1AOpen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor1BClose,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor1BClose));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor1BOpen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor1BOpen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor2AClose,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor2AClose));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor2AOpen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor2AOpen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor2BClose,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor2BClose));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor2BOpen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor2BOpen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor3AClose,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor3AClose));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MovDoor3AOpen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MovDoor3AOpen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudAmmoCriticalWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxAmmoCriticalWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudAmmoLowWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxAmmoLowWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudArmorCriticalWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxArmorCriticalWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudArmorLowWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxArmorLowWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxClaxonAlert,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxClaxonAlert));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudDamageAlertWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxDamageAlertWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudDamageCriticalWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxDamageCriticalWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudDestinationReached,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxDestinationReached));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudEnemyAlertWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxEnemyAlertWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudFriendlyFireWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxFriendlyFireWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxGeigerCounterHeavyLoop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxGeigerBeep));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxGps,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxGps));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxGpsFail,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxGpsFail));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudHarvestingComplete,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHarvestingComplete));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudHealthCriticalWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHealthCriticalWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudHealthLowWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHealthLowWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAutolevelingOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAutolevelingOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAutolevelingOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAutolevelingOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudBackcameraOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudBackcameraOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudBackcameraOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudBackcameraOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudCockpitOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudCockpitOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudCockpitOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudCockpitOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudRadarMode,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudRadarMode));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudReflectorRange,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudReflectorRange));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudSlowMovementOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudSlowMovementOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudSlowMovementOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudSlowMovementOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudWeaponScroll,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudWeaponScroll));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudWeaponSelect,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudWeaponSelect));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudInventoryComplete,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxInventoryComplete));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudInventoryFullWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxInventoryFullWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudInventoryTransfer,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxInventoryTransfer));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudFuelLowWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxLowFuelWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudOxygenLowWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxLowOxygenWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudFuelCriticalWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxNoFuelWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudOxygenCriticalWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxNoOxygenWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudObjectiveComplete,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxObjectiveComplete));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxPlayerBreath,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxPlayerBreath));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudRadarJammedWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxRadarJammedWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudRadiationWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxRadiationWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxShipSmallExplosion,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxShipSmallExplosion));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudSolarFlareWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxSolarFlareWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxSolarWind,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxSolarWind));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxSpark,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxSpark));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudTargetDestroyed,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxTargetDestroyed));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehCH1EngineHigh2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehCH1EngineHigh2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehCH1EngineHigh3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehCH1EngineHigh3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehCH1EngineIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehCH1EngineIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehCH1EngineIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehCH1EngineIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehEL1EngineHigh2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehEL1EngineHigh2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehEL1EngineHigh3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehEL1EngineHigh3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehEL1EngineIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehEL1EngineIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehEL1EngineIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehEL1EngineIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehEL1EngineOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehEL1EngineOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehEL1EngineOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehEL1EngineOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeColliding2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeColliding2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeColliding3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeColliding3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeCollision2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeCollision2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeCollision3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeCollision3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeImplode2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeImplode2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeImplode3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeImplode3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeMovingLoop2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeMovingLoop2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeMovingLoop3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeMovingLoop3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehHarvesterTubeRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehHarvesterTubeRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehNU1EngineHigh2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehNU1EngineHigh2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehNU1EngineHigh3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehNU1EngineHigh3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehNU1EngineIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehNU1EngineIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehNU1EngineIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehNU1EngineIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaEngineHigh2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaEngineHigh2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaEngineHigh3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaEngineHigh3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaEngineIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaEngineIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaEngineIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaEngineIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaEngineOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaEngineOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaEngineOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaEngineOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaLightsOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaLightsOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaLightsOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaLightsOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaThrust2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaThrust2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehShipaThrust3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehShipaThrust3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillColliding2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillColliding2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillColliding3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillColliding3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillCollidingRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillCollidingRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillCollidingRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillCollidingRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillLoop2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillLoop2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillLoop3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillLoop3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolCrusherDrillRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolCrusherDrillRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillLoop2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillLoop2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillLoop3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillLoop3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawCut2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawCut2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawCut3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawCut3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawLoop2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawLoop2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawLoop3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawLoop3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolSawRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolSawRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillLoop2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillLoop2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillLoop3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillLoop3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepArsHighShot2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepArsHighShot2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepArsHighShot3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepArsHighShot3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepArsNormShot2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepArsNormShot2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepArsNormShot3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepArsNormShot3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepAutocanonFire2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepAutocanonFire2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepAutocanonFire3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepAutocanonFire3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepAutocanonRel2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepAutocanonRel2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepAutocanonRel3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepAutocanonRel3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombExplosion,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombExplosion));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombFlash,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombFlash));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombGravSuck,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombGravSuck));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepEpmExplosion,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepEMPExplosion));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombSmartDrone,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombSmartDrone));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombSmartPlant,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombSmartPlant));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombSmartSmoke,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombSmartSmoke));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepBombSmartTimer,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepBombSmartTimer));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepLargeShipAutocannonRotate,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepLargeShipAutocannonRotate));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepLargeShipAutocannonRotateRelease,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepLargeShipAutocannonRotateRelease));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunHighFire2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunHighFire2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunHighFire3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunHighFire3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunHighRel2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunHighRel2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunHighRel3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunHighRel3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunNormFire2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunNormFire2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunNormFire3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunNormFire3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunNormRel2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunNormRel2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMachineGunNormRel3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMachineGunNormRel3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMineMoveALoop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMineMoveALoop));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMissileExplosion,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMissileExplosion));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMissileFly,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMissileFly));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMissileLaunch2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMissileLaunch2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMissileLaunch3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMissileLaunch3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepMissileLock,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepMissileLock));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepRailHighShot2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepRailHighShot2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepRailHighShot3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepRailHighShot3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepRailNormShot2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepRailNormShot2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepRailNormShot3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepRailNormShot3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepShotgunHighShot2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepShotgunHighShot2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepShotgunHighShot3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepShotgunHighShot3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepShotgunNormShot2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepShotgunNormShot2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepShotgunNormShot3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepShotgunNormShot3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepSniperHighFire2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepSniperHighFire2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepSniperHighFire3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepSniperHighFire3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepSniperNormFire2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepSniperNormFire2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepSniperNormFire3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepSniperNormFire3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepSniperScopeZoomALoop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepSniperScopeZoomALoop));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepSniperScopeZoomRel,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepSniperScopeZoomRel));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepUnivLaunch2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepUnivLaunch2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepUnivLaunch3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepUnivLaunch3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillColliding2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillColliding2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillColliding3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillColliding3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillCollidingRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillCollidingRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolLaserDrillCollidingRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolLaserDrillCollidingRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillColliding2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillColliding2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillColliding3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillColliding3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillCollidingRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillCollidingRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolThermalDrillCollidingRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehToolThermalDrillCollidingRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxProgressHack,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxProgressHack));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxCancelHack,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxCancelHack));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxProgressRepair,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxProgressRepair));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxProgressBuild,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxProgressBuild));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxCancelRepair,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxCancelRepair));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxCancelBuild,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxCancelBuild));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenXstart,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenXstart]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenXloop,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenXloop]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAlertVoc,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxAlertVoc));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_GenXend,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_GenXend]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Temple1,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Temple1]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Temple2,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Temple2]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb3D_Temple3,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb3D_Temple3]));
            */


            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_City,
                         new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                            MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_City]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ComputerRoom,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ComputerRoom]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Factory,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Factory]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_LostPlace,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_LostPlace]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RedHeat,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RedHeat]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_War,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_War]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_City2,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_City2]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Factory2,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Factory2]));        
    
            /*
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxShipLargeExplosion,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxShipLargeExplosion));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusCalmAtmosphere_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusCalmAtmosphere_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusCalmAtmosphere_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusCalmAtmosphere_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusCalmAtmosphere_KA03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusCalmAtmosphere_KA03));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusCalmAtmosphere_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusCalmAtmosphere_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusCalmAtmosphere_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusCalmAtmosphere_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusDesperateWithStress_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusDesperateWithStress_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusDesperateWithStress_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusDesperateWithStress_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusDesperateWithStress_KA03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusDesperateWithStress_KA03));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusDesperateWithStress_KA04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusDesperateWithStress_KA04));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusDesperateWithStress_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusDesperateWithStress_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHorror_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHorror_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHorror_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHorror_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHorror_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHorror_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHorror_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHorror_MM02));            
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusMainMenu_KA_MM,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusMainMenu_KAandMM));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusMystery_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusMystery_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusMystery_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusMystery_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusMystery_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusMystery_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusMystery_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusMystery_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSadnessOrDesperation_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSadnessOrDesperation_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSadnessOrDesperation_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSadnessOrDesperation_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSadnessOrDesperation_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSadnessOrDesperation_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSadnessOrDesperation_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSadnessOrDesperation_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStealthAction_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStealthAction_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStealthAction_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStealthAction_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStealthAction_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStealthAction_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStressOrTimeRush_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStressOrTimeRush_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStressOrTimeRush_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStressOrTimeRush_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStressOrTimeRush_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStressOrTimeRush_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusStressOrTimeRush_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusStressOrTimeRush_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusTensionBeforeAnAction_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusTensionBeforeAnAction_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusTensionBeforeAnAction_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusTensionBeforeAnAction_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusTensionBeforeAnAction_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusTensionBeforeAnAction_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusTensionBeforeAnAction_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusTensionBeforeAnAction_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusVictory_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusVictory_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusVictory_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusVictory_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusVictory_MM01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusVictory_MM01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusVictory_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusVictory_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA03));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA04));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA05));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA07,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA07));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA08,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA08));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA10,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA10));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA11,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA11));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_MM02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_MM02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_MM03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_MM03));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_KA01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_KA01));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_KA03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_KA03));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA12,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA12));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_KA05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_KA05));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_KA02,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_KA02));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_KA07,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_KA07));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_MM04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_MM04));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusHeavyFight_KA04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusHeavyFight_KA04));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_KA03,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_KA03));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusLightFight_KA27,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusLightFight_KA27));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_KA04,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_KA04));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_KA05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_KA05));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.MusSpecial_MM05,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.MusSpecial_MM05));   

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAlarmIncoming,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAlarmIncoming));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAlarmDamageA,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAlarmDamageA));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAlarmDamageB,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAlarmDamageB));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAlarmDamageC,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAlarmDamageC));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAlarmDamageD,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAlarmDamageD));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxHudAlarmDamageE,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.SfxHudAlarmDamageE));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepCannon3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepCannon3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepCannon2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepCannon2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.WepNoAmmo,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.WepNoAmmo));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehCH1EngineOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehCH1EngineOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehCH1EngineOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehCH1EngineOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehNU1EngineOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehNU1EngineOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehNU1EngineOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyTextsWrapperEnum.VehNU1EngineOn));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillBlast2d, 
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillBlast2d));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillIdle2d));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillRecharge2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillRecharge2d));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillBlastRock2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillBlastRock2d));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxProgressActivation,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxProgressActivation));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxCancelActivation,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxCancelActivation));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxTakeAllUniversal,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxTakeAllUniversal));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxTakeAllAmmo,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxTakeAllAmmo));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxTakeAllFuel,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxTakeAllFuel));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxTakeAllMedkit,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxTakeAllMedkit));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxTakeAllOxygen,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxTakeAllOxygen));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxTakeAllRepair,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxTakeAllRepair));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAcquireWeaponOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAcquireWeaponOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAcquireWeaponOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAcquireWeaponOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAcquireDroneOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAcquireDroneOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAcquireDroneOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAcquireDroneOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAcquireCameraOn,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAcquireCameraOn));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxAcquireCameraOff,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAcquireCameraOff));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxProgressTake,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxProgressTake));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxCancelTake,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxCancelTake));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxProgressPut,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxProgressPut));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.SfxCancelPut,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxCancelPut));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudAmmoNoWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxAmmoNoWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudArmorNoWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.SfxArmorNoWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehLoopDrone,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehLoopDrone));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehLoopCamera,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehLoopCamera));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.HudOxygenLeakingWarning,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.HudOxygenLeakingWarning));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehLoopLargeShip,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehLoopLargeShip));


            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillLoop2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillLoop2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillLoop3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillLoop3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillColliding2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillColliding2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillColliding3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillColliding3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillCollidingOther2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillCollidingOther2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillCollidingOther3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillCollidingOther3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillCollidingRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillCollidingRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillCollidingRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillCollidingRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillCollidingOtherRelease2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillCollidingOtherRelease2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillCollidingOtherRelease3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillCollidingOtherRelease3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillIdle2d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillIdle2d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolNuclearDrillIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolNuclearDrillIdle3d));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillBlast3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillBlast3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillBlastRock3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillBlastRock3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillIdle3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyTextsWrapperEnum.VehToolPressureDrillIdle3d));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.VehToolPressureDrillRecharge3d,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.VehToolPressureDrillRecharge3d]));
                                                                        */
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RacingFans,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RacingFans]));  
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RedHeat2,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RedHeat2]));

            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Electrical01,
                new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                   MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Electrical01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Electrical02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Electrical02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_EngineThrust,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_EngineThrust]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanLargeDamaged,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanLargeDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanLargeDestroyed,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanLargeDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanLargeNormal,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanLargeNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanMediumDamaged,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanMediumDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanMediumDestroyed,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanMediumDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanMediumNormal,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanMediumNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanSmallDamaged,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanSmallDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanSmallDestroyed,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanSmallDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_FanSmallNormal,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_FanSmallNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenLargeDamaged,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenLargeDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenLargeDestroyed,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenLargeDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenLargeNormal,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenLargeNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenMediumDamaged,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenMediumDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenMediumDestroyed,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenMediumDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenMediumNormal,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenMediumNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenSmallDamaged,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenSmallDamaged]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenSmallDestroyed,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenSmallDestroyed]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenSmallNormal,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenSmallNormal]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenXend,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenXend]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenXloop,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenXloop]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_GenXstart,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_GenXstart]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_PipeFlow01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_PipeFlow01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_PrefabFire01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_PrefabFire01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL05,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL06,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL06]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL07,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL07]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL08,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL08]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL09,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL09]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL10,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL10]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL11,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL11]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL12,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL12]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL13,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL13]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL14,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL14]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL15,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL15]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterAL16,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterAL16]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH05,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH06,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH06]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH07,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH07]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH08,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH08]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH09,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH09]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH10,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH10]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH11,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH11]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH12,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH12]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH13,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH13]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH14,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH14]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH15,                        
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH15]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterCH16,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterCH16]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS05,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS06,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS06]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS07,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS07]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS08,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS08]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS09,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS09]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS10,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS10]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS11,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS11]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS12,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS12]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS13,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS13]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS14,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS14]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS15,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS15]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_RadioChatterRS16,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_RadioChatterRS16]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Spark01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Spark01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamDischarge01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamDischarge01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamDischarge02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamDischarge02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamDischarge03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamDischarge03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamDischarge04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamDischarge04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamLoop01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamLoop01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamLoop02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamLoop02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamLoop03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamLoop03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamLoop04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamLoop04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_SteamLoop05,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_SteamLoop05]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Temple1,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Temple1]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Temple2,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Temple2]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Temple3,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Temple3]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapLarge01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapLarge01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapMed01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapMed01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapMed02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapMed02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapMed03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapMed03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapMed04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapMed04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapSmall01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapSmall01]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapSmall02,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapSmall02]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapSmall03,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapSmall03]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_ThunderClapSmall04,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_ThunderClapSmall04]));
            m_influenceSphereSoundHelpers.Add((short)MySoundCuesEnum.Amb2D_Welding01,
                            new MyGuiInfluenceSphereHelper(MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\GuiHelpers\\IconSound", flags: TextureFlags.IgnoreQuality),
                                               MyEnumsToStrings.Sounds[(short)MySoundCuesEnum.Amb2D_Welding01])); 
            
            foreach (MySoundCuesEnum soundCue in Enum.GetValues(typeof(MySoundCuesEnum))) 
            {
                if(Utils.MyEnumsToStrings.Sounds[(int)soundCue].StartsWith("Amb2d"))
                    Debug.Assert(m_influenceSphereSoundHelpers.ContainsKey((short)soundCue));
            }
            
            #endregion
        }

    }
}
