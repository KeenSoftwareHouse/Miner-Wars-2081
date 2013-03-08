namespace MinerWars.AppCode.Game.Entities.VoxelHandShapes
{
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Voxels;
    using MinerWarsMath;
    using Utils;
    using System;
    using System.Text;

    // This phys object represents voxel hand shape - SPHERE. Voxel Hand is applied to voxel maps in order to add, subtract, soften voxels or change material.
    sealed class MyVoxelHandCuboid : MyVoxelHandShape
    {        
        public float Width1 { get; private set; }
        public float Depth1 { get; private set; }
        public float Width2 { get; private set; }
        public float Depth2 { get; private set; }
        public float Length { get; private set; }        

        private BoundingBox m_localBoundingBox;
        private MyCuboid m_cuboid = new MyCuboid();

        public void Init(MyMwcObjectBuilder_VoxelHand_Cuboid objectBuilder, MyVoxelMap parentVoxelMap)
        {
            base.Init(objectBuilder, parentVoxelMap);

            Width1 = objectBuilder.Width1;
            Depth1 = objectBuilder.Depth1;
            Width2 = objectBuilder.Width2;
            Depth2 = objectBuilder.Depth2;
            Length = objectBuilder.Length;

            UpdateLocalVolume();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_VoxelHand_Cuboid objectBuilder = (MyMwcObjectBuilder_VoxelHand_Cuboid)base.GetObjectBuilderInternal(getExactCopy);
            objectBuilder.Width1 = this.Width1;
            objectBuilder.Depth1 = this.Depth1;
            objectBuilder.Width2 = this.Width2;
            objectBuilder.Depth2 = this.Depth2;
            objectBuilder.Length = this.Length;
            return objectBuilder;
        }       


        public override int GetPropertiesCount()
        {
            return 5;
        }

        public override StringBuilder GetPropertyName(int index)
        {
            switch (index)
            {
                case 0:
                    return new StringBuilder("Width 1");
                    break;
                case 1:
                    return new StringBuilder("Depth 1");
                    break;
                case 2:
                    return new StringBuilder("Width 2");
                    break;
                case 3:
                    return new StringBuilder("Depth 2");
                    break;
                case 4:
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
                    return Width1;
                    break;
                case 1:
                    return Depth1;
                    break;
                case 2:
                    return Width2;
                    break;
                case 3:
                    return Depth2;
                    break;
                case 4:
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
                    Width1 = value;
                    break;
                case 1:
                    Depth1 = value;
                    break;
                case 2:
                    Width2 = value;
                    break;
                case 3:
                    Depth2 = value;
                    break;
                case 4:
                    Length = value;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            UpdateLocalVolume();
        }

        void UpdateLocalVolume()
        {
            m_cuboid.CreateFromSizes(Width1, Depth1, Width2, Depth2, Length);
            this.m_localBoundingBox = m_cuboid.GetLocalAABB();
            BoundingSphere.CreateFromBoundingBox(ref m_localBoundingBox, out m_localVolume);
        }

        public override MyVoxelHandShapeType GetShapeType()
        {
            return MyVoxelHandShapeType.Cuboid;
        }

        public override void DrawCone(Vector3 position)
        {          /*  
            MyQuad backQuad;

            Matrix world = this.WorldMatrix;
            Vector3 position2 = this.GetPosition() - world.Forward * LocalVolume.Radius;
            MyUtils.GenerateQuad(out backQuad, ref position2, Width1 / 2, this.Length / 2.0f, ref world);

            Vector4 vctColor = new Vector4(0.4f, 0.7f, 0.0f, 0.6f);
            MySimpleObjectDraw.DrawTransparentPyramid(ref position, ref backQuad, ref vctColor, 1, 0.5f);*/
        }

        public override void DrawShape()
        {
            Vector4 vctColor = new Vector4(0.1f, 0.3f, 0.0f, 2f);
            Vector4 vctColor2 = new Vector4(1.0f, 0.0f, 0.0f, 10.0f);
            Matrix world = this.WorldMatrix;
            
            
            
            MySimpleObjectDraw.DrawTransparentCuboid(ref world, m_cuboid, ref vctColor, true, 2);            
             
               /*
            Vector3 tr = world.Translation;

            Vector3 triTransl = new Vector3(4600, 2380, 3330);

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(400, 0, 0);
            Vector3 p3 = new Vector3(400, 0, 400);

          

            Vector3 t1 = p1 + triTransl;
            Vector3 t2 = p2 + triTransl;
            Vector3 t3 = p3 + triTransl;

            MySimpleObjectDraw.DrawLine(t1, t2, null, ref vctColor, 1);
            MySimpleObjectDraw.DrawLine(t2, t3, null, ref vctColor, 1);
            MySimpleObjectDraw.DrawLine(t3, t1, null, ref vctColor, 1);

            Matrix m = Matrix.CreateWorld(tr, Vector3.Forward, Vector3.Up);
            MyTriangle_Vertexes tri = new MyTriangle_Vertexes();
            tri.Vertex0 = t1;
            tri.Vertex1 = t2;
            tri.Vertex2 = t3;
            bool ins = MyUtils.IsPointInTriangle(ref tr, ref tri);
            if (ins)
            {
            }
            Vector4 clr = ins ? vctColor2 : vctColor;
            MySimpleObjectDraw.DrawTransparentSphere(ref world, 1.0f, ref clr, false, 8);
                */
        }

        public BoundingBox GetLocalBoundingBox()
        {
            return this.m_localBoundingBox;
        }

        public override MyVoxelHandShape CreateCopy()
        {
            MyVoxelHandCuboid newCuboid = new MyVoxelHandCuboid();
            newCuboid.Init((MyMwcObjectBuilder_VoxelHand_Cuboid)GetObjectBuilderInternal(false), null);
            newCuboid.SetWorldMatrix(this.WorldMatrix);

            return newCuboid;
        }

        public MyCuboid Cuboid
        {
            get { return m_cuboid; }
        }
    }
}