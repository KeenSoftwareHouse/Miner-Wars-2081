using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Localization;
using SysUtils.Utils;
using MinerWars.AppCode.Game.TransparentGeometry;

namespace MinerWars.AppCode.Game.World.Global
{
    class MyFactionProperties
    {
        public MyFactionProperties(
            MyTextsWrapperEnum NameEnum,
            MyTextsWrapperEnum DescriptionEnum,
            MyTransparentMaterialEnum? SolarMapIcon)
        {
            this.NameEnum = NameEnum;
            this.DescriptionEnum = DescriptionEnum;
            this.SolarMapIcon = SolarMapIcon;
        }

        public readonly MyTextsWrapperEnum NameEnum;
        public readonly MyTextsWrapperEnum DescriptionEnum;
        public readonly MyTransparentMaterialEnum? SolarMapIcon;

        public StringBuilder Name
        {
            get { return MyTextsWrapper.Get(NameEnum); }
        }

        public StringBuilder Description
        {
            get { return MyTextsWrapper.Get(DescriptionEnum); }
        }

    }

    static class MyFactionConstants
    {
        static readonly Dictionary<int, MyFactionProperties> m_factions = new Dictionary<int, MyFactionProperties>();

        static MyFactionConstants()
        {
            m_factions[(int)MyMwcObjectBuilder_FactionEnum.None] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionNone,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Euroamerican] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionEuroamerican,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionEAC
                );
      
            m_factions[(int)MyMwcObjectBuilder_FactionEnum.China] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionChinese,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionChina
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.FourthReich] = new MyFactionProperties(
                NameEnum:  MyTextsWrapperEnum.FactionFourthReich,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Omnicorp] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionOmnicorp,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );
         
            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Russian] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionRussian,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionRussia
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Japan] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionJapan,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionJapan
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.India] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionIndia,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionIndia
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Saudi] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionSaudi,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionSaudi
                );
      
            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Church] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionChurch,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionChurch
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.FSRE] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionFSRE,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.FreeAsia] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionFreeAsia,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionFreeAsia
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Pirates] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionPirate,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Miners] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionMiners,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Freelancers] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionFreelancers,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Ravens] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionRavens,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Traders] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionTraders,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Syndicate] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionSyndicate,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Templars] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionTemplars,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Rangers] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionRangers,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.TTLtd] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionTTLtd,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.SMLtd] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionSMLtd,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.CSR] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionCSR,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionCSR
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Russian_KGB] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionRussianKGB,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionRussia
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Slavers] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionSlavers,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionSlavers
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.WhiteWolves] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionWhiteWolves,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: MyTransparentMaterialEnum.FactionFourthReich
                );

            m_factions[(int)MyMwcObjectBuilder_FactionEnum.Rainiers] = new MyFactionProperties(
                NameEnum: MyTextsWrapperEnum.FactionRainiers,
                DescriptionEnum: MyTextsWrapperEnum.General,
                SolarMapIcon: null
                );            

            foreach (MyMwcObjectBuilder_FactionEnum faction in Enum.GetValues(typeof(MyMwcObjectBuilder_FactionEnum)))
            {
                System.Diagnostics.Debug.Assert(m_factions[(int)faction] != null);
            }
        }

        public static MyFactionProperties GetFactionProperties(MyMwcObjectBuilder_FactionEnum faction)
        {
            return m_factions[(int)faction];
        }
    }
}
