using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWarsMath;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;
using MinerWars.AppCode.Game.Utils;
using MinerWars.AppCode.Game.Entities;
using MinerWars.AppCode.Game.GUI.Prefabs;
using MinerWars.AppCode.Game.GUI;
using MinerWars.AppCode.Game.World.Global;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects.Prefabs;

namespace MinerWars.AppCode.Game.Prefabs
{
    class MyPrefabCamera : MyPrefabBase, IMyUseableEntity, IMyHasGuiControl
    {
        public float Yaw;
        public float Pitch;

        public const float YawLimit = 0.785398163f;
        public const float PitchLimit = 0.785398163f;

        public MyUseProperties UseProperties { get; set; }


        public MyPrefabCamera(MyPrefabContainer owner)
            : base(owner)
        {
        }              

        protected override void InitPrefab(string displayName, Vector3 relativePosition, Matrix localOrientation, MyMwcObjectBuilder_PrefabBase objectBuilder, MyPrefabConfiguration prefabConfig)
        {
            UseProperties = new MyUseProperties(MyUseType.FromHUB | MyUseType.Solo, MyUseType.None);
            if (objectBuilder.UseProperties == null)
            {
                UseProperties.Init(MyUseType.FromHUB | MyUseType.Solo, MyUseType.None, 0, 1, false);                
            }
            else
            {                
                UseProperties.Init(objectBuilder.UseProperties);
            }            
            
            Enabled = true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_PrefabCamera objectBuilder = base.GetObjectBuilderInternal(getExactCopy) as MyMwcObjectBuilder_PrefabCamera;            
            objectBuilder.UseProperties = UseProperties.GetObjectBuilder();

            return objectBuilder;
        }

        public override string GetCorrectDisplayName()
        {

            return base.GetCorrectDisplayName();
        }

        public bool CanBeHacked(MySmallShip hackedBy)
        {
            return false;
        }
        public bool CanBeUsed(MySmallShip usedBy)
        {
            return IsWorking();
        }

        public void Use(MySmallShip useBy)
        {
            MyGuiScreenGamePlay.Static.TakeControlOfCamera(this);
        }

        public void UseFromHackingTool(MySmallShip useBy, int hackingLevelDifference)
        {
            throw new NotSupportedException();
        }

        public GUI.Prefabs.MyGuiControlEntityUse GetGuiControl(GUI.Core.IMyGuiControlsParent parent)
        {
            return new MyGuiControlPrefabCameraUse(parent, this);
        }

        public MyEntity GetEntity() 
        {
            return this;
        }

        internal Matrix GetViewMatrix()
        {
            Matrix world = Matrix.CreateFromYawPitchRoll(MathHelper.Pi + Yaw, Pitch, 0) * Matrix.CreateTranslation(Vector3.Forward * -3) * WorldMatrix;

            Matrix view = Matrix.Invert(world);

            return view;
        }

        internal void HandleInput(Vector2 rotationIndicator)
        {
            rotationIndicator *= MyGuiConstants.PREFAB_CAMERA_ROTATION_SENSITIVITY;

            Yaw -= rotationIndicator.Y;
            Pitch -= rotationIndicator.X;

            Yaw = MathHelper.Clamp(Yaw, -YawLimit, YawLimit);
            Pitch = MathHelper.Clamp(Pitch, -PitchLimit, PitchLimit);
        }
    }
}
