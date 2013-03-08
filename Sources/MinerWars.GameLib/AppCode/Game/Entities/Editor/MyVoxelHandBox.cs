namespace MinerWars.AppCode.Game.Entities.VoxelHandShapes
{
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Voxels;
    using MinerWarsMath;
    using Utils;
    using System;
    using System.Text;

    // This phys object represents voxel hand shape - SPHERE. Voxel Hand is applied to voxel maps in order to add, subtract, soften voxels or change material.
    sealed class MyVoxelHandBox : MyVoxelHandShape
    {
        public float Size { get; private set; }
        public float Size2 { get; private set; }
        public float Size3 { get; private set; }
        private BoundingBox m_localBoundingBox;

        public void Init(MyMwcObjectBuilder_VoxelHand_Box objectBuilder, MyVoxelMap parentVoxelMap)
        {
            base.Init(objectBuilder, parentVoxelMap);
                                    
            this.Size = objectBuilder.Size;
            this.Size2 = objectBuilder.Size2;
            this.Size3 = objectBuilder.Size3;

            UpdateLocalVolume();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_VoxelHand_Box objectBuilder = (MyMwcObjectBuilder_VoxelHand_Box)base.GetObjectBuilderInternal(getExactCopy);            
            objectBuilder.Size = this.Size;
            objectBuilder.Size2 = this.Size2;
            objectBuilder.Size3 = this.Size3;
            return objectBuilder;
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
                    return new StringBuilder("Width");
                    break;
                case 1:
                    return new StringBuilder("Length");
                    break;
                case 2:
                    return new StringBuilder("Depth");
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
                    return Size;
                    break;
                case 1:
                    return Size2;
                    break;
                case 2:
                    return Size3;
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
                    Size = value;
                    break;
                case 1:
                    Size2 = value;
                    break;
                case 2:
                    Size3 = value;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            UpdateLocalVolume();
        }

        void UpdateLocalVolume()
        {
            float halfSize1 = this.Size / 2.0f;
            float halfSize2 = this.Size2 / 2.0f;
            float halfSize3 = this.Size3 / 2.0f;
            this.m_localBoundingBox = new BoundingBox(new Vector3(-halfSize1, -halfSize2, -halfSize3), new Vector3(halfSize1, halfSize2, halfSize3));

            float radius = (float)Math.Sqrt(3) * this.Size;
            BoundingSphere.CreateFromBoundingBox(ref m_localBoundingBox, out m_localVolume);
        }

        public override MyVoxelHandShapeType GetShapeType()
        {
            return MyVoxelHandShapeType.Box;
        }

        public override void DrawCone(Vector3 position)
        {                        
            MyQuad backQuad;            

            Matrix world = this.WorldMatrix;
            Vector3 position2 = this.GetPosition() - world.Forward * (this.Size / 2.0f);
            MyUtils.GenerateQuad(out backQuad, ref position2, this.Size / 2.0f, this.Size / 2.0f, ref world);

            Vector4 vctColor = new Vector4(0.4f, 0.7f, 0.0f, 0.6f);
            MySimpleObjectDraw.DrawTransparentPyramid(ref position, ref backQuad, ref vctColor, 1, 0.5f);
        }

        public override void DrawShape()
        {
            Vector4 vctColor = new Vector4(0.1f, 0.3f, 0.0f, 0.2f);
            Matrix world = this.WorldMatrix;
            BoundingBox localBoundingBox = this.m_localBoundingBox;
            MySimpleObjectDraw.DrawTransparentBox(ref world, ref localBoundingBox, ref vctColor, true, 1);            
        }

        public BoundingBox GetLocalBoundingBox() 
        {
            return this.m_localBoundingBox;
        }

        public override MyVoxelHandShape CreateCopy()
        {
            MyVoxelHandBox newBox = new MyVoxelHandBox();
            newBox.Init((MyMwcObjectBuilder_VoxelHand_Box)GetObjectBuilderInternal(false), null);
            newBox.SetWorldMatrix(this.WorldMatrix);

            return newBox;
        }
    }
}
