using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Networking.SectorService;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using SysUtils.Utils;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.Missions;
using SysUtils;
using MinerWars.AppCode.Networking;

namespace MinerWars.AppCode.Game.Managers.Others
{
    //struct MySmallShipTemplateIdentifier
    //{
    //    public string Name;
    //    public MyMwcObjectBuilder_SmallShip_TypesEnum ShipType;
    //}

    static class MySmallShipTemplates
    {
        private const int TEMPLATE_TIER_COUNT = 9;

        static private readonly List<MySmallShipTemplate>[] m_templates =
            new List<MySmallShipTemplate>[MyMwcUtils.GetMaxValueFromEnum<MyMwcObjectBuilder_SmallShip_TypesEnum>() + 1];

        static private readonly Dictionary<int, MySmallShipTemplate[]> m_templateTiers =
            new Dictionary<int, MySmallShipTemplate[]>();

        static MySmallShipTemplates()
        {
            Load();
        }

        public static int GenerateNewID()
        {
            int maxID = 0;
            foreach (var templates in m_templates)
            {
                foreach (var template in templates)
                {
                    if (template.ID > maxID)
                    {
                        maxID = template.ID;
                    }
                }
            }

            return maxID + 1;
        }

        public static void Load(bool displaySuccessMessage = false)
        {
            MyMwcLog.WriteLine("MySmallShipTemplates.Load() - START");
            MyMwcLog.IncreaseIndent();
            for (int i = 0; i < m_templates.Length; i++)
            {
                m_templates[i] = new List<MySmallShipTemplate>();
            }

            try
            {
                MyMwcObjectBuilder_SmallShipTemplates templatesBuilder;

                templatesBuilder = MyLocalCache.LoadGlobalData().Templates;

                foreach (var templateBuilder in templatesBuilder.SmallShipTemplates) 
                {
                    m_templates[(int)templateBuilder.Builder.ShipType].Add(new MySmallShipTemplate(templateBuilder));
                }
                LoadTemplateTiers();
                MyMwcLog.WriteLine("Load state - OK");
                if (displaySuccessMessage) 
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, Localization.MyTextsWrapperEnum.TemplatesLoadSuccess, Localization.MyTextsWrapperEnum.Success, Localization.MyTextsWrapperEnum.Ok, null));
                }
            }
            catch (Exception ex) 
            {
                MyMwcLog.WriteLine("Load state - ERROR");
                MyMwcLog.WriteLine(ex.Message);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, Localization.MyTextsWrapperEnum.TemplatesLoadError, Localization.MyTextsWrapperEnum.Error, Localization.MyTextsWrapperEnum.Ok, onMessageBox));                
            }
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySmallShipTemplates.Load() - END");
        }

        private static void onMessageBox(MyGuiScreenMessageBoxCallbackEnum result)
        {
            MyGuiManager.BackToMainMenu();
        }

        private static MySectorServiceClient GetSectorServiceClient() 
        {
            MySectorServiceClient client = MySectorServiceClient.GetCheckedInstance();
            var channelContext = client.GetChannelContext();
            if (channelContext != null)
            {
                channelContext.OperationTimeout = TimeSpan.FromSeconds(180);
            }

            return client;
        }

        public static void AddTemplate(MySmallShipTemplate newTemplate)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(newTemplate.Name.ToString()));
            Debug.Assert(newTemplate.Builder != null);

            var typeTemplates = m_templates[(int) newTemplate.Builder.ShipType];

            Debug.Assert(GetTemplate(typeTemplates, newTemplate.Name) == null);

            typeTemplates.Add(newTemplate);
        }

        public static MySmallShipTemplate GetTemplate(int templateId)
        {
            foreach (var templatesForType in m_templates)
            {
                foreach (var template in templatesForType)
                {
                    if (template.ID == templateId)
                    {
                        return template;
                    }
                }
            }
            return null;
        }

        public static MySmallShipTemplate GetTemplate(int templateId, int tier)
        {
            Debug.Assert(tier > 0);

            var template = GetTemplate(templateId);
            MySmallShipTemplate[] tiers;
            if (template != null && m_templateTiers.TryGetValue(template.ID, out tiers))
            {
                for (int i = tier - 1; i >= 0; i--)
                {
                    if (tiers[i] != null)
                    {
                        template = tiers[i];
                        break;
                    }
                }
            }

            return template;
        }

        public static MySmallShipTemplate GetTemplateForSpawn(int templateId)
        {
            var tier = MyMissions.GetCurrentTier();
            return tier == 0 ? GetTemplate(templateId) : GetTemplate(templateId, tier);
        }

        public static List<MySmallShipTemplate> GetTemplatesForType(MyMwcObjectBuilder_SmallShip_TypesEnum type)
        {
            return m_templates[(int)type];
        }

        public static MySmallShipTemplate GetTemplate(MyMwcObjectBuilder_SmallShip_TypesEnum type, StringBuilder name)
        {
            var templatesForType = m_templates[(int)type];

            return GetTemplate(templatesForType, name);
        }

        private static MySmallShipTemplate GetTemplate(List<MySmallShipTemplate> typeTemplates, StringBuilder name)
        {
            foreach (var existingTemplate in typeTemplates)
            {
                if (existingTemplate.Name.Equals(name))
                {
                    return existingTemplate;
                }
            }

            return null;
        }

        public static void DeleteTemplate(MySmallShipTemplate template)
        {
            Debug.Assert(!template.SavedToServer, "Cannot delete template that is already saved on server!");

            var typeTemplates = m_templates[(int)template.Builder.ShipType];

            bool deleted = typeTemplates.Remove(template);
            Debug.Assert(deleted);
        }

        public static void SaveToServer(bool displaySuccessMessage = false)
        {
            MyMwcLog.WriteLine("MySmallShipTemplates.SaveToServer() - START");
            MyMwcLog.IncreaseIndent();
            List<MyMwcObjectBuilder_SmallShipTemplate> templates = new List<MyMwcObjectBuilder_SmallShipTemplate>();
            foreach (var templatesForType in m_templates)
            {
                foreach (var template in templatesForType)
                {
                    template.SavedToServer = true;
                    templates.Add(template.GetObjectBuilder());
                }
            }

            try
            {
                var client = GetSectorServiceClient();
                var templatesBuilder = new MyMwcObjectBuilder_SmallShipTemplates(templates);
                client.SaveSmallShipTemplates(templatesBuilder.ToBytes());
                MySectorServiceClient.SafeClose();
                MyMwcLog.WriteLine("Save state - OK");
                if (displaySuccessMessage)
                {
                    MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.MESSAGE, Localization.MyTextsWrapperEnum.TemplatesSaveSuccess, Localization.MyTextsWrapperEnum.Success, Localization.MyTextsWrapperEnum.Ok, null));
                }
            }
            catch (Exception ex)
            {
                MyMwcLog.WriteLine("Save state - ERROR");
                MyMwcLog.WriteLine(ex.Message);
                MyGuiManager.AddScreen(new MyGuiScreenMessageBox(MyMessageBoxType.ERROR, Localization.MyTextsWrapperEnum.TemplatesSaveError, Localization.MyTextsWrapperEnum.Error, Localization.MyTextsWrapperEnum.Ok, null));
            }
            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MySmallShipTemplates.SaveToServer() - END");
        }

        private static void LoadTemplateTiers()
        {
            string nameFormat = "{0}_TIER{1}";
            
            m_templateTiers.Clear();
            foreach (var templatesForType in m_templates)
            {
                foreach (var template in templatesForType)
                {
                    string templateName = template.Name.ToString();
                    if (!templateName.Contains("_TIER"))
                    {
                        for (int i = 0; i < TEMPLATE_TIER_COUNT; i++)
                        {
                            var templateTier = FindTemplateByName(string.Format(nameFormat, templateName, i+1));
                            if (templateTier != null)
                            {
                                if (!m_templateTiers.ContainsKey(template.ID))
                                {
                                    m_templateTiers.Add(template.ID, new MySmallShipTemplate[TEMPLATE_TIER_COUNT]);
                                }

                                m_templateTiers[template.ID][i] = templateTier;
                            }
                        }
                    }
                }
            }
        }

        private static MySmallShipTemplate FindTemplateByName(string name)
        {
            foreach (var templatesForType in m_templates)
            {
                foreach (var template in templatesForType)
                {
                    if (string.Compare(template.Name.ToString(), name, true) == 0)
                    {
                        return template;
                    }
                }
            }
            return null;
        }
    }
}
