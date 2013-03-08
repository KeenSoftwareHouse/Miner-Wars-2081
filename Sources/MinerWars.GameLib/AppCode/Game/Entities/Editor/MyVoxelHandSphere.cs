namespace MinerWars.AppCode.Game.Entities.VoxelHandShapes
{
    using CommonLIB.AppCode.ObjectBuilders;
    using CommonLIB.AppCode.ObjectBuilders.Voxels;
    using MinerWarsMath;
    using Utils;
    using System;
    using System.Text;

    // This phys object represents voxel hand shape - SPHERE. Voxel Hand is applied to voxel maps in order to add, subtract, soften voxels or change material.
    sealed class MyVoxelHandSphere : MyVoxelHandShape
    {
        public float Radius { get; private set; }
        public void Init(MyMwcObjectBuilder_VoxelHand_Sphere objectBuilder, MyVoxelMap parentVoxelMap)
        {
            base.Init(objectBuilder, parentVoxelMap);
            this.Radius = objectBuilder.Radius;
            UpdateLocalVolume();
        }

        protected override MyMwcObjectBuilder_Base GetObjectBuilderInternal(bool getExactCopy)
        {
            MyMwcObjectBuilder_VoxelHand_Sphere objectBuilder = (MyMwcObjectBuilder_VoxelHand_Sphere)base.GetObjectBuilderInternal(getExactCopy);            
            objectBuilder.Radius = this.Radius;
            return objectBuilder;
        }        

         public override int GetPropertiesCount()
        {
            return 1;
        }

         public override StringBuilder GetPropertyName(int index)
        {
            switch (index)
            {
                case 0:
                    return new StringBuilder("Radius");
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
                    return Radius;
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
                    Radius = value;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

            UpdateLocalVolume();
        }


        public override MyVoxelHandShapeType GetShapeType()
        {
            return MyVoxelHandShapeType.Sphere;            
        }

        private void UpdateLocalVolume()
        {
            this.LocalVolume = new BoundingSphere(Vector3.Zero, this.Radius);
        }

        public override void DrawCone(Vector3 position)
        {                        
            Vector3 forwardDirection = Vector3.Normalize(this.GetPosition() - position);
            Vector3 upDirection = this.WorldMatrix.Up + (forwardDirection - this.WorldMatrix.Forward);
            //Matrix world = Matrix.CreateWorld(position, forwardDirection, new Vector3(-direction.X, direction.Y, -direction.Z));
            Matrix world = Matrix.CreateWorld(position, forwardDirection, upDirection);
                        
            float dist = (this.GetPosition() - position).Length();
            float x = (float)Math.Sqrt(dist * dist - this.Radius * this.Radius);
            float alphaRad = (float)Math.Asin(this.Radius / dist);

            float coneLength = x * (float)Math.Cos(alphaRad);
            float coneRadius = x * (float)Math.Sin(alphaRad);

            Vector4 vctColor = new Vector4(0.4f, 0.7f, 0.0f, 0.6f);
            MySimpleObjectDraw.DrawTransparentCone(ref world, -coneLength, coneRadius, ref vctColor, false, 12, 0.5f);
        }

        public override void DrawShape()
        {
            Vector4 vctColor = new Vector4(0.1f, 0.3f, 0.0f, 0.2f);
            Matrix world = this.WorldMatrix;
            
            MySimpleObjectDraw.DrawTransparentSphere(ref world, this.Radius, ref vctColor, false, 12);
        }

        public override MyVoxelHandShape CreateCopy()
        {
            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().StartProfilingBlock("MyVoxelHandSphere::CreateCopy");
            
            MyVoxelHandSphere newSphere = new MyVoxelHandSphere();
            newSphere.Init((MyMwcObjectBuilder_VoxelHand_Sphere)GetObjectBuilderInternal(false), null);
            newSphere.SetWorldMatrix(this.WorldMatrix);

            MinerWars.AppCode.Game.Render.MyRender.GetRenderProfiler().EndProfilingBlock();

            return newSphere;
        }
    }
}
