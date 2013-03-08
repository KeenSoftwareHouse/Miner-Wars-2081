namespace MinerWars.AppCode.Game.Entities.VoxelHandShapes
{
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Voxels;
    using MinerWarsMath;
    using Utils;
    using System;
    using System.Text;

    sealed class MyVoxelHandCylinder : MyVoxelHandShape
    {        
        public float Radius1 { get; private set; }
        public float Radius2 { get; private set; }
        public float Length { get; set; }
        private BoundingBox m_localBoundingBox;

        public void Init(MyMwcObjectBuilder_VoxelHand_Cylinder objectBuilder, MyVoxelMap parentVoxelMap)
        {
            base.Init(objectBuilder, parentVoxelMap);

            this.Radius1 = objectBuilder.Radius1;
            this.Radius2 = objectBuilder.Radius2;
            this.Length = objectBuilder.Length;

            UpdateLocalVolume();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_VoxelHand_Cylinder objectBuilder = (MyMwcObjectBuilder_VoxelHand_Cylinder)base.GetObjectBuilderInternal(getExactCopy);
            objectBuilder.Radius1 = this.Radius1;
            objectBuilder.Radius2 = this.Radius2;
            objectBuilder.Length = this.Length;
            return objectBuilder;
        }       

      
        private void UpdateLocalVolume()
        {
            float maxRadius = Math.Max(Radius1, Radius2);
            this.LocalVolume = new BoundingSphere(Vector3.Zero, Math.Max(maxRadius, Length / 2));

            this.m_localBoundingBox = new BoundingBox(new Vector3(-maxRadius, -Length / 2, -maxRadius), new Vector3(maxRadius, Length / 2, maxRadius));
        }

        public override int GetPropertiesCount()
        {
            return 3;
        }

        public override StringBuilder GetPropertyName(int index)
        {
            switch (index)
            {
                case 0:
                    return new StringBuilder("Radius 1");
                    break;
                case 1:
                    return new StringBuilder("Radius 2");
                    break;
                case 2:
                    return new StringBuilder("Length");
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            return new StringBuilder();
        }

        public override float GetPropertyValue(int index)
        {
            switch (index)
            {
                case 0:
                    return Radius1;
                    break;
                case 1:
                    return Radius2;
                    break;
                case 2:
                    return Length;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            return 0;
        }

        public override void SetPropertyValue(int index, float value)
        {
            switch (index)
            {
                case 0:
                    Radius1 = value;
                    break;
                case 1:
                    Radius2 = value;
                    break;
                case 2:
                    Length = value;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            UpdateLocalVolume();
        }

        public override MyVoxelHandShapeType GetShapeType()
        {
            return MyVoxelHandShapeType.Cylinder;
        }

        public override void DrawCone(Vector3 position)
        {            
            MyQuad backQuad;

            Matrix world = this.WorldMatrix;
            Vector3 position2 = this.GetPosition() - world.Forward * (Radius1 / 2.0f);
            MyUtils.GenerateQuad(out backQuad, ref position2, Radius1, this.Length / 2.0f, ref world);

            Vector4 vctColor = new Vector4(0.4f, 0.7f, 0.0f, 0.6f);
            MySimpleObjectDraw.DrawTransparentPyramid(ref position, ref backQuad, ref vctColor, 1, 0.5f);
        }

        public override void DrawShape()
        {
            Vector4 vctColor = new Vector4(0.1f, 0.3f, 0.0f, 0.2f);
            Matrix world = this.WorldMatrix;
            BoundingBox localBoundingBox = this.m_localBoundingBox;
            MySimpleObjectDraw.DrawTransparentCylinder(ref world, Radius1, Radius2, Length, ref vctColor, true, 32, 1);            
        }

        public BoundingBox GetLocalBoundingBox()
        {
            return this.m_localBoundingBox;
        }

        public override MyVoxelHandShape CreateCopy()
        {
            MyVoxelHandCylinder newCylinder = new MyVoxelHandCylinder();
            newCylinder.Init((MyMwcObjectBuilder_VoxelHand_Cylinder)GetObjectBuilderInternal(false), null);
            newCylinder.SetWorldMatrix(this.WorldMatrix);

            return newCylinder;
        }
    }
}