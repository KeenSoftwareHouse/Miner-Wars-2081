namespace MinerWars.AppCode.Game.Entities.Weapons
{
    using System.Text;
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.SubObjects.SmallShipTools;
    using MinerWarsMath;
    using Models;
    using Utils;
    using System;
    using MinerWars.AppCode.Game.Gameplay;
    using MinerWars.AppCode.Game.World.Global;
    using MinerWars.AppCode.Game.Managers.Session;

    abstract class MyGunBase : MyEntity
    {
        //  This is 'm_positionMuzzle' transformed into world space during every update (world space)
        protected Vector3 m_positionMuzzleInWorldSpace;

        // In multiplayer, weapon which is not owned by player is dummy, it don't deal damage, and physics force, projectiles won't explode, etc...
        //public bool IsDummy;
        public MyEntityIdentifier? LastShotId { get; protected set; }

        public MyGunBase()
            : base(false)
        {
            Save = false;
        }

        public virtual void Init(StringBuilder hudLabelText, MyModelsEnum? modelEnum, MyMaterialType materialType,
            MyEntity parentObject, Vector3 position, Vector3 forwardVector, Vector3 upVector,
            MyMwcObjectBuilder_Base objectBuilder, MyModelsEnum? collisionModelEnum = null)            
        {
            base.Init(hudLabelText, modelEnum, null, parentObject, null, objectBuilder, modelCollision: collisionModelEnum);
            this.LocalMatrix = Matrix.CreateWorld(position, forwardVector, upVector);
            //NeedsUpdate = true; //Smallship updates weapons manually
            CastShadows = false;

            PreloadTextures();
        }

        //  Every child of this base class must implement Shot() method, which shots projectile or missile.
        //  Method returns true if something was shot. False if not (because interval between two shots didn't pass)
        public abstract bool Shot(MyMwcObjectBuilder_SmallShip_Ammo usedAmmo);

        // This gun or some of its ammo can have special functions. Implement this method to do something special.
        public virtual void InvokeAmmoSpecialFunction() { }


        public override void UpdateAfterSimulation()
        {
            base.UpdateAfterSimulation();

            Matrix worldMatrix = this.WorldMatrix;

            const float MUZZLE_POSITION_SAFE_EPSILON = 0.7f;
            if (ModelLod0 != null)
            {
                m_positionMuzzleInWorldSpace = MyUtils.GetTransform(
                    new Vector3(0, 0, -(ModelLod0.BoundingSphere.Radius / 1.0f) - MUZZLE_POSITION_SAFE_EPSILON),
                    ref worldMatrix);
            }
        }

        public abstract bool IsThisGunFriendly();

        public float GetDeviatedAngleFromDamageRatio() 
        {
            MyEntity topMostParent = GetTopMostParent();
            if(MySession.PlayerShip != null &&
               MyFactions.GetFactionsRelation(topMostParent, MySession.PlayerShip) == MyFactionRelationEnum.Enemy)
            {
                float degrees = (float)Math.Pow(120, topMostParent.GetDamageRatio() * 1.5 - 1.2) * 4f;
                return MathHelper.ToRadians(degrees);
            }
            return 0f;
        }

        protected Vector3 GetDeviatedVector(MyAmmoProperties ammoProperties)
        {
            float deviateAngle = ammoProperties.DeviateAngle;

            //  Create one projectile - but deviate projectile direction be random angle
            if (IsThisGunFriendly())            
                deviateAngle += MyGameplayConstants.GameplayDifficultyProfile.DeviatingAnglePlayerOnEnemy;            
            else            
                deviateAngle += MyGameplayConstants.GameplayDifficultyProfile.DeviatingAngleEnemyBotOnPlayer;

            return MyUtilRandomVector3ByDeviatingVector.GetRandom(WorldMatrix.Forward, deviateAngle);
        }        
    }
}
