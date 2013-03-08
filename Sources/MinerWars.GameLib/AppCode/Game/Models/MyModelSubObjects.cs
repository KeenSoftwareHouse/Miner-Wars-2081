using System;
using System.Collections.Generic;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.Utils;
using SysUtils.Utils;
using MinerWars.CommonLIB.AppCode.Import;


//  Here we define position and orientation of model subobjects like engine thrusts, reflector, machine gun, etc.
//  This list contains sub-object for every model.
//  All this position are relative to their models.

namespace MinerWars.AppCode.Game.Models
{
    class MyModelSubObject
    {
        public string Name;
        public bool Enabled;                    //  If this sub-object is enabled
        public Vector3 Position;
        public Vector3? ForwardVector;
        public Vector3? LeftVector;
        public Vector3? UpVector;
        public float? Scale;
        public int AuxiliaryParam0;          //  This aux-param may be used for different purposes for each sub-object. Sometime it's radius of something, sometime length, etc
        public Dictionary<string, object> CustomData;   // Custom data from dummies

        public MyModelSubObject(string name, bool enabled, Vector3 position, Vector3? forward, Vector3? leftVector, Vector3? upVector, float? scale, int auxiliaryParam0, Dictionary<string, object> customData)
        {
            Name = name;
            Enabled = enabled;
            Position = position;
            ForwardVector = forward;
            LeftVector = leftVector;
            UpVector = upVector;
            Scale = scale;
            AuxiliaryParam0 = auxiliaryParam0;
            CustomData = customData;
        }

        public override string ToString()
        {
            return Name;
        }
    }
    
    static class MyModelSubObjects
    {
        //static MyModelSubObject[] m_subObjects;
        static Dictionary<int, List<MyModelSubObject>> m_subObjectsMap = new Dictionary<int, List<MyModelSubObject>>();

        public static void LoadContent()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyModelSubobjects::LoadContent");

            m_subObjectsMap.Clear();

            //Subobjects from dummies
            AddSubObjectsFromModel(MyModelsEnum.Liberator);
            AddSubObjectsFromModel(MyModelsEnum.Enforcer);
            AddSubObjectsFromModel(MyModelsEnum.Kammler);
            AddSubObjectsFromModel(MyModelsEnum.Gettysburg);
            AddSubObjectsFromModel(MyModelsEnum.Virginia);
            AddSubObjectsFromModel(MyModelsEnum.Baer);
            AddSubObjectsFromModel(MyModelsEnum.Hewer);
            AddSubObjectsFromModel(MyModelsEnum.Razorclaw);
            AddSubObjectsFromModel(MyModelsEnum.Greiser);
            AddSubObjectsFromModel(MyModelsEnum.Tracer);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Jacknife);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Doon);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Hammer);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_YG_Closed);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_ORG);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Hawk);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Phoenix);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Leviathan);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Rockheater);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_SteelHead);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Talon);
            AddSubObjectsFromModel(MyModelsEnum.SmallShip_Stanislav);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();
        }

        public static void UnloadContent()
        {
            MyMwcLog.WriteLine("MyModelSubObjects.UnloadContent - START");
            MyMwcLog.IncreaseIndent();

            m_subObjectsMap.Clear();

            MyMwcLog.DecreaseIndent();
            MyMwcLog.WriteLine("MyModelSubObjects.UnloadContent - END");
        }
        static void AddSubObjectsFromModel(MyModelsEnum modelEnum)
        {
            MyModel model = MyModels.GetModelOnlyDummies(modelEnum);
            string prefix = modelEnum.ToString() + "_";

            List<MyModelSubObject> subObjectList;
            m_subObjectsMap.TryGetValue((int)modelEnum, out subObjectList);
            if (subObjectList == null)
            {
                subObjectList = new List<MyModelSubObject>();
                m_subObjectsMap.Add((int)modelEnum, subObjectList);
            }

            foreach (KeyValuePair<string, MyModelDummy> pair in model.Dummies)
            {
                subObjectList.Add(new MyModelSubObject(pair.Key, true, pair.Value.Matrix.Translation, MyMwcUtils.Normalize(pair.Value.Matrix.Forward), MyMwcUtils.Normalize(pair.Value.Matrix.Left), MyMwcUtils.Normalize(pair.Value.Matrix.Up), pair.Value.Matrix.Forward.Length(), 0, pair.Value.CustomData)); 
            }
        }

        public static MyModelSubObject GetModelSubObject(MyModelsEnum modelEnum, string subObjectName)
        {
            List<MyModelSubObject> subObjectList;
            m_subObjectsMap.TryGetValue((int)modelEnum, out subObjectList);
            if (subObjectList == null)
                return null;

            return subObjectList.Find(x => x.Name == subObjectName);
        }

        public static List<MyModelSubObject> GetModelSubObjects(MyModelsEnum modelEnum) 
        {
            List<MyModelSubObject> subObjectList;
            m_subObjectsMap.TryGetValue((int)modelEnum, out subObjectList);
            return subObjectList;
    }
}
}
