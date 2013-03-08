using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Textures;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Localization;
using MinerWars.AppCode.Game.GUI.Core;

namespace MinerWars.AppCode.Game.World.Global
{
    public enum MyActorEnum
    {
        //Temp1 = 0,
        //Temp2 = 1,
        APOLLO = 2,
        MARCUS = 3,
        EacSurveySite_StationOperator = 4,
        EacSurveySite_MilitaryOfficer = 5,
        MADELYN = 6,
        Researcher1 = 7,
        Researcher2 = 8,
        RussianCF1 = 10,
        RussianCF2 = 11,
        RussianGeneral = 12,
        RussianCaptain = 13,
        TemplarPatriarchLamorak = 14,
        TemplarSirBedivere = 15,
        LorraineCardin = 16,
        TemplarGuard = 17,
        SmugglerInformator = 18,
        //MANJEET = 19,
        TheCrook = 20,
        LAIKA_OPERATOR = 21,
        ChineseGuard = 22,
        VALENTIN = 23,
        TARJA = 24,
        Slave = 25,
        SPEEDSTER = 26,

        EAC_CAPTAIN_FEMALE = 29,
        EAC_CAPTAIN = 30,
        FATHER_TOBIAS = 31,
        Fourth_Reich_Colone_2 = 32,
        FRANCIS_REEF = 33,
        MANJEET = 34,
        THOMAS = 36,
        White_Wolves_General_2 = 37,

        RiftOperator = 38,
        RiftTourist = 39,

        ShouterReichJE = 40,
        CHINESE_OFFICER = 41, 
        ShouterRussianJE = 42,
        BLONDI = 43,
        RAIDER = 44,      
        RussianGeneralRecording = 46,
        RussianHeadquartersRecording = 47,
        VOLODIA_STRANGER = 48,
        VOLODIA = 49,
        SLAVER_BASE_CAPTAIN = 50,
        CEDRIC,
        GATEKEEPER,
        SLAVER_LEADER,
        CAPTIVE,
        CAPTIVE_PILOT,
        ZAPPAS_GANGMAN,
        MOMO_ZAPPA,
        DEALER,
        GANGSTER,
        CHINESE_COMMANDO,
        WALTHER,
        TRANSPORTER_CAPTAIN,
        ERHARD,
        FOR_CAPTAIN,
        CHINESE_PILOT,
        RAIDER_LEADER,
        RAIDER_NAVIGATOR,
        SIR_GERAINT,
        COUNCIL_GUARD,
        PATRIARCH_LAMORAK,
        PATRIARCH_DAGONET,
        PATRIARCH_CARADOC,
        UNKNOWN,
        EVERYONE,
        RUSSIAN_GENERAL,
        PIRATE_MALE,
        PIRATE2_MALE,
        CLEGG,
        VANE,
        GORG,
        SLAVER_COLE,
        SLAVER_JEFF,

        RIME_CLIENT1,
        RIME_CLIENT2,
        RIME_BOUNCER,
        RIME_CLIENT3,
        RIME_BARKEEPER,
        RIME_MITCHEL,
        RIME_SMUGGLER,
        RIME_GUARD,

        REICHSTAG_OFFICER,
        REICHSTAG_CAPTAIN,

        RESEARCH_VESSEL_CAPTAIN,
        EacSurveySite_MilitaryCaptain,
   }

    internal class MyActorProperties
    {
        public string Name;
        public MyTextsWrapperEnum DisplayName;
        public MyTexture2D AvatarImage;
        public MyTexture2D Icon;
        public MyMwcObjectBuilder_FactionEnum Faction;

        public MyActorProperties(MyTextsWrapperEnum displayName, MyTexture2D avatarImage, MyTexture2D icon, MyMwcObjectBuilder_FactionEnum faction, string name = null)
        {
            Name = name;
            DisplayName = displayName;
            AvatarImage = avatarImage;
            Icon = icon;
            Faction = faction;
        }
    }

    static class MyActorConstants
    {
        static readonly int MAX_ACTORNAME_LENGTH = 30;

        static readonly Dictionary<int, MyActorProperties> m_actorProperties = new Dictionary<int, MyActorProperties>();

        static readonly HashSet<MyActorEnum> m_noiseActors = new HashSet<MyActorEnum>(){
            MyActorEnum.RussianCF1, MyActorEnum.RussianCF2, MyActorEnum.SmugglerInformator, MyActorEnum.ChineseGuard, MyActorEnum.SPEEDSTER, MyActorEnum.Slave
        };

        /// <summary>
        /// this method is there because we need load and check asserts when the game started
        /// </summary>
        public static void Check()
        {
            // TODO
        }

        public static MyActorProperties GetActorProperties(MyActorEnum id)
        {
            MyActorProperties result = null;
            m_actorProperties.TryGetValue((int)id, out result);
            return result;
        }

        public static void SetActorFaction(MyActorEnum id, MyMwcObjectBuilder_FactionEnum newFaction)
        {
            var actorProperties = GetActorProperties(id);
            if (actorProperties == null) return;
            actorProperties.Faction = newFaction;
        }

        public static bool IsNoiseActor(MyActorEnum id) {
            return m_noiseActors.Contains(id);
        }

        static MyActorConstants()
        {
            //m_actorProperties.Add((int)MyActorEnum.Temp1, new MyActorProperties(MyTextsWrapperEnum.ActorNameTemp1, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Temp1", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            //m_actorProperties.Add((int)MyActorEnum.Temp2, new MyActorProperties(MyTextsWrapperEnum.ActorNameTemp2, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Temp2", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            m_actorProperties.Add((int)MyActorEnum.APOLLO, new MyActorProperties(MyTextsWrapperEnum.Actor_Apollo, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Apollo", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers, "Player"));
            m_actorProperties.Add((int)MyActorEnum.MARCUS, new MyActorProperties(MyTextsWrapperEnum.Actor_Marcus, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Marcus", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers, "Marcus"));
            m_actorProperties.Add((int)MyActorEnum.EacSurveySite_StationOperator, new MyActorProperties(MyTextsWrapperEnum.Actor_EacSurveySite_StationOperator, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\pc_voice", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.EacSurveySite_MilitaryOfficer, new MyActorProperties(MyTextsWrapperEnum.Actor_EacSurveySite_MilitaryOfficer, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Officer", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.EacSurveySite_MilitaryCaptain, new MyActorProperties(MyTextsWrapperEnum.Actor_EacSurveySite_MilitaryCaptain, null, null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.MADELYN, new MyActorProperties(MyTextsWrapperEnum.Actor_Madelyn, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Madelyn", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers, "Madelyn"));
            m_actorProperties.Add((int)MyActorEnum.Researcher1, new MyActorProperties(MyTextsWrapperEnum.Actor_Researcher1, null, null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.Researcher2, new MyActorProperties(MyTextsWrapperEnum.Actor_Researcher2, null, null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.RussianCF1, new MyActorProperties(MyTextsWrapperEnum.Actor_RussianCF1, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            m_actorProperties.Add((int)MyActorEnum.RussianCF2, new MyActorProperties(MyTextsWrapperEnum.Actor_RussianCF2, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            m_actorProperties.Add((int)MyActorEnum.RussianGeneral, new MyActorProperties(MyTextsWrapperEnum.Actor_RussianGeneral, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            m_actorProperties.Add((int)MyActorEnum.RussianCaptain, new MyActorProperties(MyTextsWrapperEnum.Actor_RussianCaptain, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            m_actorProperties.Add((int)MyActorEnum.LAIKA_OPERATOR, new MyActorProperties(MyTextsWrapperEnum.Actor_LaikaOperator, null, null, MyMwcObjectBuilder_FactionEnum.Russian));
            m_actorProperties.Add((int)MyActorEnum.TemplarPatriarchLamorak, new MyActorProperties(MyTextsWrapperEnum.Actor_TemplarPatriarchLamorak, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Templar", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.TemplarSirBedivere, new MyActorProperties(MyTextsWrapperEnum.Actor_TemplarSirBedivere, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Templar", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.LorraineCardin, new MyActorProperties(MyTextsWrapperEnum.Actor_Lorraine, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\LorraineCardin", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Pirates));
            m_actorProperties.Add((int)MyActorEnum.TemplarGuard, new MyActorProperties(MyTextsWrapperEnum.Actor_TemplarGuard, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\TemplarGuard", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.SmugglerInformator, new MyActorProperties(MyTextsWrapperEnum.Actor_SmugglerInformator, null, null, MyMwcObjectBuilder_FactionEnum.None));
            //m_actorProperties.Add((int)MyActorEnum.MANJEET, new MyActorProperties(MyTextsWrapperEnum.Actor_IndianSmuggler, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Manjeet_Lata", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers));
            m_actorProperties.Add((int)MyActorEnum.TheCrook, new MyActorProperties(MyTextsWrapperEnum.Actor_TheCrook, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Officer", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Slavers));
            m_actorProperties.Add((int)MyActorEnum.ChineseGuard, new MyActorProperties(MyTextsWrapperEnum.Actor_ChineseGuard, null, null, MyMwcObjectBuilder_FactionEnum.China));
            m_actorProperties.Add((int)MyActorEnum.TARJA, new MyActorProperties(MyTextsWrapperEnum.Actor_RavenGirl, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\BlackRaven_Tarja", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers, "RavenGirl"));
            m_actorProperties.Add((int)MyActorEnum.VALENTIN, new MyActorProperties(MyTextsWrapperEnum.Actor_RavenGuy, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\BlackRaven_Valentin", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers, "RavenGuy"));
            m_actorProperties.Add((int)MyActorEnum.Slave, new MyActorProperties(MyTextsWrapperEnum.Actor_Slave, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.SPEEDSTER, new MyActorProperties(MyTextsWrapperEnum.Actor_RaceChallenger, null, null, MyMwcObjectBuilder_FactionEnum.Traders, "Challenger"));

            m_actorProperties.Add((int)MyActorEnum.EAC_CAPTAIN_FEMALE, new MyActorProperties(MyTextsWrapperEnum.Actor_EAC_Police_Mothership_Commander_F, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\EAC_Police_Mothership_Commander_F", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.EAC_CAPTAIN, new MyActorProperties(MyTextsWrapperEnum.Actor_EAC_Police_Mothership_Commander_M, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\EAC_Police_Mothership_Commander_M", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Euroamerican));
            m_actorProperties.Add((int)MyActorEnum.FATHER_TOBIAS, new MyActorProperties(MyTextsWrapperEnum.Actor_Father_Tobias_Last_Hope, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Father_Tobias-Last_Hope", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Church));
            m_actorProperties.Add((int)MyActorEnum.Fourth_Reich_Colone_2, new MyActorProperties(MyTextsWrapperEnum.Actor_Fourth_Reich_Colone_2, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Fourth_Reich_Colone_2", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.FRANCIS_REEF, new MyActorProperties(MyTextsWrapperEnum.Actor_Francis_Reef, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Francis_Reef", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.MANJEET, new MyActorProperties(MyTextsWrapperEnum.Actor_Manjeet_Lata, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Manjeet_Lata", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.India, "Manjeet"));
            m_actorProperties.Add((int)MyActorEnum.THOMAS, new MyActorProperties(MyTextsWrapperEnum.Actor_Thomas_Barth, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\Thomas_Barth", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.Rainiers));
            m_actorProperties.Add((int)MyActorEnum.White_Wolves_General_2, new MyActorProperties(MyTextsWrapperEnum.Actor_White_Wolves_General_2, MyTextureManager.GetTexture<MyTexture2D>("Textures\\Gui\\Actors\\White_Wolves_General_2", flags: TextureFlags.IgnoreQuality), null, MyMwcObjectBuilder_FactionEnum.WhiteWolves));
            m_actorProperties.Add((int)MyActorEnum.RiftOperator, new MyActorProperties(MyTextsWrapperEnum.Actor_Rift_Operator, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RiftTourist, new MyActorProperties(MyTextsWrapperEnum.Actor_Rift_Tourist, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));

            m_actorProperties.Add((int)MyActorEnum.ShouterReichJE, new MyActorProperties(MyTextsWrapperEnum.ShouterReichJE, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.ShouterRussianJE, new MyActorProperties(MyTextsWrapperEnum.ShouterReichJE, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.CHINESE_OFFICER, new MyActorProperties(MyTextsWrapperEnum.ChineseOfficer, null, null, MyMwcObjectBuilder_FactionEnum.China));
            m_actorProperties.Add((int)MyActorEnum.BLONDI, new MyActorProperties(MyTextsWrapperEnum.Blondi, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.RAIDER, new MyActorProperties(MyTextsWrapperEnum.Raider, null, null, MyMwcObjectBuilder_FactionEnum.Slavers));

            m_actorProperties.Add((int)MyActorEnum.RussianGeneralRecording, new MyActorProperties(MyTextsWrapperEnum.Actor_RussianGeneral_Recording, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));
            m_actorProperties.Add((int)MyActorEnum.RussianHeadquartersRecording, new MyActorProperties(MyTextsWrapperEnum.Actor_RussianGeneral_Headquarters, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));

            m_actorProperties.Add((int)MyActorEnum.VOLODIA_STRANGER, new MyActorProperties(MyTextsWrapperEnum.VolodiaStranger, null, null, MyMwcObjectBuilder_FactionEnum.Traders));
            m_actorProperties.Add((int)MyActorEnum.VOLODIA, new MyActorProperties(MyTextsWrapperEnum.Volodia, null, null, MyMwcObjectBuilder_FactionEnum.Traders));

            m_actorProperties.Add((int)MyActorEnum.SLAVER_BASE_CAPTAIN, new MyActorProperties(MyTextsWrapperEnum.SlaverBaseCaptain, null, null, MyMwcObjectBuilder_FactionEnum.Slavers));

            m_actorProperties.Add((int)MyActorEnum.CEDRIC, new MyActorProperties(MyTextsWrapperEnum.Cedric, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.GATEKEEPER, new MyActorProperties(MyTextsWrapperEnum.GateKeeper, null, null, MyMwcObjectBuilder_FactionEnum.Templars));

            m_actorProperties.Add((int)MyActorEnum.SLAVER_LEADER, new MyActorProperties(MyTextsWrapperEnum.SlaverLeader, null, null, MyMwcObjectBuilder_FactionEnum.Slavers));
            m_actorProperties.Add((int)MyActorEnum.SLAVER_COLE, new MyActorProperties(MyTextsWrapperEnum.SlaverCole, null, null, MyMwcObjectBuilder_FactionEnum.Slavers));
            m_actorProperties.Add((int)MyActorEnum.SLAVER_JEFF, new MyActorProperties(MyTextsWrapperEnum.SlaverJeff, null, null, MyMwcObjectBuilder_FactionEnum.Slavers));
            m_actorProperties.Add((int)MyActorEnum.CAPTIVE, new MyActorProperties(MyTextsWrapperEnum.Captive, null, null, MyMwcObjectBuilder_FactionEnum.Church));
            m_actorProperties.Add((int)MyActorEnum.CAPTIVE_PILOT, new MyActorProperties(MyTextsWrapperEnum.CaptivePilot, null, null, MyMwcObjectBuilder_FactionEnum.Church));
            m_actorProperties.Add((int)MyActorEnum.ZAPPAS_GANGMAN, new MyActorProperties(MyTextsWrapperEnum.ZappasGangman, null, null, MyMwcObjectBuilder_FactionEnum.Traders));
            m_actorProperties.Add((int)MyActorEnum.MOMO_ZAPPA, new MyActorProperties(MyTextsWrapperEnum.MomoZappa, null, null, MyMwcObjectBuilder_FactionEnum.Slavers));
            m_actorProperties.Add((int)MyActorEnum.DEALER, new MyActorProperties(MyTextsWrapperEnum.Dealer, null, null, MyMwcObjectBuilder_FactionEnum.Traders));
            m_actorProperties.Add((int)MyActorEnum.GANGSTER, new MyActorProperties(MyTextsWrapperEnum.Gangster, null, null, MyMwcObjectBuilder_FactionEnum.Traders));
            m_actorProperties.Add((int)MyActorEnum.CHINESE_COMMANDO, new MyActorProperties(MyTextsWrapperEnum.ChineseCommando, null, null, MyMwcObjectBuilder_FactionEnum.China));
            m_actorProperties.Add((int)MyActorEnum.WALTHER, new MyActorProperties(MyTextsWrapperEnum.WaltherStaufenberg, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.TRANSPORTER_CAPTAIN, new MyActorProperties(MyTextsWrapperEnum.TransporterCaptain, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.ERHARD, new MyActorProperties(MyTextsWrapperEnum.Erhard, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.FOR_CAPTAIN, new MyActorProperties(MyTextsWrapperEnum.FORCaptain, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.CHINESE_PILOT, new MyActorProperties(MyTextsWrapperEnum.ChinesePilot, null, null, MyMwcObjectBuilder_FactionEnum.China));
            m_actorProperties.Add((int)MyActorEnum.RAIDER_LEADER, new MyActorProperties(MyTextsWrapperEnum.RaiderLeader, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.RAIDER_NAVIGATOR, new MyActorProperties(MyTextsWrapperEnum.RaiderNavigator, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));


            m_actorProperties.Add((int)MyActorEnum.SIR_GERAINT, new MyActorProperties(MyTextsWrapperEnum.SIR_GERAINT, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.COUNCIL_GUARD, new MyActorProperties(MyTextsWrapperEnum.COUNCIL_GUARD, null, null, MyMwcObjectBuilder_FactionEnum.Templars));

            m_actorProperties.Add((int)MyActorEnum.PATRIARCH_CARADOC, new MyActorProperties(MyTextsWrapperEnum.PATRIARCH_CARADOC, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.PATRIARCH_DAGONET, new MyActorProperties(MyTextsWrapperEnum.PATRIARCH_DAGONET, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.PATRIARCH_LAMORAK, new MyActorProperties(MyTextsWrapperEnum.PATRIARCH_LAMORAK, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.UNKNOWN, new MyActorProperties(MyTextsWrapperEnum.UNKNOWN, null, null, MyMwcObjectBuilder_FactionEnum.Templars));
            m_actorProperties.Add((int)MyActorEnum.EVERYONE, new MyActorProperties(MyTextsWrapperEnum.Everyone, null, null, MyMwcObjectBuilder_FactionEnum.Rainiers));
            m_actorProperties.Add((int)MyActorEnum.RUSSIAN_GENERAL, new MyActorProperties(MyTextsWrapperEnum.RussianGeneral, null, null, MyMwcObjectBuilder_FactionEnum.Russian_KGB));

            m_actorProperties.Add((int)MyActorEnum.PIRATE_MALE, new MyActorProperties(MyTextsWrapperEnum.Pirate, null, null, MyMwcObjectBuilder_FactionEnum.Pirates));
            m_actorProperties.Add((int)MyActorEnum.PIRATE2_MALE, new MyActorProperties(MyTextsWrapperEnum.Pirate, null, null, MyMwcObjectBuilder_FactionEnum.Pirates));
            m_actorProperties.Add((int)MyActorEnum.CLEGG, new MyActorProperties(MyTextsWrapperEnum.PirateCaptainClegg, null, null, MyMwcObjectBuilder_FactionEnum.Pirates));
            m_actorProperties.Add((int)MyActorEnum.VANE, new MyActorProperties(MyTextsWrapperEnum.PirateCaptainVane, null, null, MyMwcObjectBuilder_FactionEnum.Pirates));
            m_actorProperties.Add((int)MyActorEnum.GORG, new MyActorProperties(MyTextsWrapperEnum.PirateCaptainGorg, null, null, MyMwcObjectBuilder_FactionEnum.Pirates));

            m_actorProperties.Add((int)MyActorEnum.RIME_CLIENT1, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeClient1, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_CLIENT2, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeClient2, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_CLIENT3, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeClient3, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_BOUNCER, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeBouncer, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_BARKEEPER, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeBarkeeper, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_MITCHEL, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeMitchel, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_SMUGGLER, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeSmuggler, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));
            m_actorProperties.Add((int)MyActorEnum.RIME_GUARD, new MyActorProperties(MyTextsWrapperEnum.Actor_RimeGuard, null, null, MyMwcObjectBuilder_FactionEnum.Omnicorp));

            m_actorProperties.Add((int)MyActorEnum.REICHSTAG_OFFICER, new MyActorProperties(MyTextsWrapperEnum.Actor_ReichstagOfficer, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));
            m_actorProperties.Add((int)MyActorEnum.REICHSTAG_CAPTAIN, new MyActorProperties(MyTextsWrapperEnum.Actor_ReichstagCaptain, null, null, MyMwcObjectBuilder_FactionEnum.FourthReich));

            m_actorProperties.Add((int)MyActorEnum.RESEARCH_VESSEL_CAPTAIN, new MyActorProperties(MyTextsWrapperEnum.ResearchVesselCaptain, null, null, MyMwcObjectBuilder_FactionEnum.Euroamerican));


            foreach (var actorProperties in m_actorProperties)
            {
                System.Diagnostics.Debug.Assert(MyTextsWrapper.Get(actorProperties.Value.DisplayName).Length <= MAX_ACTORNAME_LENGTH);
            }
        }

        public static MyTextsWrapperEnum GetActorDisplayName(MyActorEnum actor)
        {
            return GetActorProperties(actor).DisplayName;
        }

        public static string GetActorName(MyActorEnum actor)
        {
            return GetActorProperties(actor).Name;
        }
    }
}
