namespace MinerWars.AppCode.Game.Entities.VoxelHandShapes
{
    using CommonLIB.AppCode.Networking;
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Voxels;
    using MinerWarsMath;
    using MinerWars.AppCode.Game.Utils;
    using MinerWars.AppCode.Game.Render;
    using System.Text;

    enum MyVoxelHandShapeType : byte
    {
        Sphere = 0,
        Box = 1,
        Cuboid = 2,
        Cylinder = 3,
    }

    // This is the base class for all shapes, that can be applied to voxel maps using voxel hand tool
    abstract class MyVoxelHandShape : MyEntity
    {
        public MyMwcVoxelHandModeTypeEnum ModeType;
        public MyMwcVoxelMaterialsEnum? Material;        
        //protected float m_radiusScale;

        public MyVoxelHandShape()
            : base(false)
        {
            Save = false;
        }


        protected virtual void Init(MyMwcObjectBuilder_VoxelHand_Shape objectBuilder, MyVoxelMap parentVoxelMap)
        {
            base.Init(null, objectBuilder, parentVoxelMap);
            ModeType = objectBuilder.VoxelHandModeType;
            Material = objectBuilder.VoxelHandMaterial;

            SetWorldMatrix(Matrix.CreateWorld(objectBuilder.PositionAndOrientation.Position, objectBuilder.PositionAndOrientation.Forward, objectBuilder.PositionAndOrientation.Up));

            Visible = false;
        }

        // This method overrides parent draw, that expects that phys object has Model. VoxelHandShape doesnt, so we cant call parent's draw.
        public override bool Draw(MyRenderObject renderObject = null)
        {
            DrawShape();            
            return true;
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_VoxelHand_Shape objectBuilder = (MyMwcObjectBuilder_VoxelHand_Shape)base.GetObjectBuilderInternal(getExactCopy);
            objectBuilder.VoxelHandModeType = ModeType;
            objectBuilder.VoxelHandMaterial = Material;
            //objectBuilder.m_scale= m_radiusScale;
            //objectBuilder.Position = GetPosition();
            objectBuilder.PositionAndOrientation = new MyMwcPositionAndOrientation(WorldMatrix);
            return objectBuilder;
        }

        public void SetVoxelProperties(MyMwcVoxelHandModeTypeEnum modeType, MyMwcVoxelMaterialsEnum? materialEnum) 
        {
            this.ModeType = modeType;
            this.Material = materialEnum;
        }

        public abstract int GetPropertiesCount();
        public abstract void SetPropertyValue(int index, float value);
        public abstract float GetPropertyValue(int index);
        public abstract StringBuilder GetPropertyName(int index);

        public abstract MyVoxelHandShapeType GetShapeType();
        public abstract void DrawCone(Vector3 position);
        public abstract void DrawShape();
        public abstract MyVoxelHandShape CreateCopy();
    }
}
