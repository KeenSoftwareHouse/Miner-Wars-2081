using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.SubObjects;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Prefabs;
using MinerWarsMath;
using MinerWars.AppCode.Game.Editor;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;
using MinerWars.AppCode.Game.Gameplay;

namespace MinerWars.AppCode.Game.Entities.Prefabs
{
    /// <summary>
    /// MyPrefabFactory
    /// </summary>
    class MyPrefabFactory
    {
        private static MyPrefabFactory m_Instance = null;

        private MyPrefabFactory() 
        { 
        }

        public static MyPrefabFactory GetInstance()
        {
            if (m_Instance == null)
                m_Instance = new MyPrefabFactory();

            return m_Instance;
        }

        /// <summary>
        /// CreatePrefab
        /// </summary>
        /// <param name="hudLabelText"></param>
        /// <param name="objBuilder"></param>
        /// <returns></returns>
        public MyPrefabBase CreatePrefab(string hudLabelText, MyPrefabContainer prefabContainer, MyMwcObjectBuilder_PrefabBase prefabBuilder)
        {
            Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyPrefabFactory.CreatePrefab");

            MyPrefabConfiguration config = MyPrefabConstants.GetPrefabConfiguration(prefabBuilder.GetObjectBuilderType(), prefabBuilder.GetObjectBuilderId().Value);
            Vector3 relativePosition = MyPrefabContainer.GetRelativePositionInAbsoluteCoords(prefabBuilder.PositionInContainer);
            Matrix prefabLocalOrientation = Matrix.CreateFromYawPitchRoll(prefabBuilder.AnglesInContainer.X, prefabBuilder.AnglesInContainer.Y, prefabBuilder.AnglesInContainer.Z);

            MyPrefabBase prefab = null;
            if (config is MyPrefabConfigurationKinematic)
            {
                prefab = new MyPrefabKinematic(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationLight)
            {
                prefab = new MyPrefabLight(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationLargeWeapon)
            {
                prefab = new MyPrefabLargeWeapon(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationSound)
            {
                prefab = new MyPrefabSound(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationParticles)
            {
                prefab = new MyPrefabParticles(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationLargeShip)
            {
                prefab = new MyPrefabLargeShip(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationHangar)
            {
                prefab = new MyPrefabHangar(prefabContainer);                
            }
            else if(config is MyPrefabConfigurationFoundationFactory)
            {
                prefab = new MyPrefabFoundationFactory(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationSecurityControlHUB) 
            {
                prefab = new MyPrefabSecurityControlHUB(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationBankNode) 
            {
                prefab = new MyPrefabBankNode(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationGenerator) 
            {
                prefab = new MyPrefabGenerator(prefabContainer);                
            }
            else if (config is MyPrefabConfigurationScanner) 
            {
                prefab = new MyPrefabScanner(prefabContainer);
            }
            else if (config is MyPrefabConfigurationCamera)
            {
                prefab = new MyPrefabCamera(prefabContainer);
            }
            else if (config is MyPrefabConfigurationAlarm)
            {
                prefab = new MyPrefabAlarm(prefabContainer);
            }            
            else
            {
                prefab = new MyPrefab(prefabContainer);
                //prefab.Init(hudLabelText, relativePosition, prefabLocalOrientation, prefabBuilder, config);
            }
            prefab.Init(hudLabelText, relativePosition, prefabLocalOrientation, prefabBuilder, config);

            Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return prefab;
        }



        /// <summary>
        /// CreatePrefabObjectBuilder
        /// </summary>
        /// <param name="prefabCategory"></param>
        /// <returns></returns>
        public MyMwcObjectBuilder_PrefabBase CreatePrefabObjectBuilder(MyMwcObjectBuilderTypeEnum prefabType, int prefabId, MyMwcObjectBuilder_Prefab_AppearanceEnum textureEnum)
        {
            MyGameplayProperties gameplayProperties = MyGameplayConstants.GetGameplayProperties(prefabType, prefabId, MyMwcObjectBuilder_FactionEnum.Euroamerican);
            MyPrefabConfiguration prefabConfig = MyPrefabConstants.GetPrefabConfiguration(prefabType, prefabId);
            MyMwcVector3Short pos = new MyMwcVector3Short(0, 0, 0);

            MyMwcObjectBuilder_PrefabBase objBuilder = MyMwcObjectBuilder_Base.CreateNewObject(prefabType, prefabId) as MyMwcObjectBuilder_PrefabBase;
            objBuilder.PositionInContainer = pos;
            objBuilder.AnglesInContainer = Vector3.Zero;
            objBuilder.PrefabHealthRatio = MyGameplayConstants.HEALTH_RATIO_MAX;
            objBuilder.PrefabMaxHealth = null;
            objBuilder.FactionAppearance = textureEnum;
            objBuilder.IsDestructible = gameplayProperties.IsDestructible;
            if (prefabConfig.DisplayHud)
            {
                objBuilder.PersistentFlags |= MyPersistentEntityFlags.DisplayOnHud;
            }
            
            return objBuilder;
        }
    }
}
