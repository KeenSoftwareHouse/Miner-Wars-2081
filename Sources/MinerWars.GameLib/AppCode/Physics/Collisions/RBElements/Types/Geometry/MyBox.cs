
#region Using Statements


#endregion

using MinerWarsMath;
using SysUtils.Utils;

namespace MinerWars.AppCode.Physics
{
    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Support class for the internal box collision detection
    /// </summary>
    public class MyBox
    {        

        #region public struct Edge
        /// <summary>
        /// Edge just contains indexes into the points returned by GetCornerPoints.
        /// </summary>
        public struct Edge
        {
            public BoxPointIndex Ind0;
            public BoxPointIndex Ind1;

            public Edge(BoxPointIndex ind0, BoxPointIndex ind1)
            {
                this.Ind0 = ind0;
                this.Ind1 = ind1;
            }
        }
        #endregion

        #region public enum BoxPointIndex
        /// <summary>
        /// Indices into the points returned by GetCornerPoints
        /// </summary>
        public enum BoxPointIndex
        {
            BRD, BRU, BLD, BLU,
            FRD, FRU, FLD, FLU
        }
        #endregion

        public MyTransform Transform;
        private Vector3 m_sideLengths;


        // must match with GetCornerPoints!
        private static Edge[] edges = new Edge[12]
            { 
                new Edge(BoxPointIndex.BRD,BoxPointIndex.BRU), // origin-up
                new Edge(BoxPointIndex.BRD,BoxPointIndex.BLD), // origin-left
                new Edge(BoxPointIndex.BRD,BoxPointIndex.FRD), // origin-fwd
                new Edge(BoxPointIndex.BLD,BoxPointIndex.BLU), // leftorigin-up
                new Edge(BoxPointIndex.BLD,BoxPointIndex.FLD), // leftorigin-fwd
                new Edge(BoxPointIndex.FRD,BoxPointIndex.FRU), // fwdorigin-up
                new Edge(BoxPointIndex.FRD,BoxPointIndex.FLD), // fwdorigin-left
                new Edge(BoxPointIndex.BRU,BoxPointIndex.BLU), // uporigin-left
                new Edge(BoxPointIndex.BRU,BoxPointIndex.FRU), // uporigin-fwd
                new Edge(BoxPointIndex.BLU,BoxPointIndex.FLU), // upleftorigin-fwd
                new Edge(BoxPointIndex.FRU,BoxPointIndex.FLU), // upfwdorigin-left
                new Edge(BoxPointIndex.FLD,BoxPointIndex.FLU), // fwdleftorigin-up
            };
        private Vector3[] m_outPoints = new Vector3[8];


        /// <summary>
        /// m_initialSunWindPosition/orientation are based on one corner the box. Sides are
        /// the full side lengths
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="orient"></param>
        /// <param name="m_SideLengths"></param>
        public MyBox( Matrix matrix, Vector3 m_SideLengths)
        {
            Matrix tMa = matrix;
            tMa.Translation = Vector3.Zero;
            this.Transform = new MyTransform(matrix.Translation, tMa);
            this.m_sideLengths = m_SideLengths;
        }



        /// <summary>
        /// Get/set the box corner/origin position
        /// </summary>
        public Vector3 Position
        {
            get { return Transform.Position; }
            set { Transform.Position = value; }
        }

        /// <summary>
        /// Get the box centre position
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCentre()
        {
            Vector3 result = new Vector3(
                    m_sideLengths.X * 0.5f,
                    m_sideLengths.Y * 0.5f,
                    m_sideLengths.Z * 0.5f);

            Vector3.TransformNormal(ref result, ref Transform.Orientation, out result);
            Vector3.Add(ref result, ref Transform.Position, out result);

            return result;
        }

        public void GetCentre(out Vector3 centre)
        {
            centre = new Vector3(
                m_sideLengths.X * 0.5f,
                m_sideLengths.Y * 0.5f,
                m_sideLengths.Z * 0.5f);

            Vector3.Transform(ref centre, ref Transform.Orientation, out centre);
            Vector3.Add(ref centre, ref Transform.Position, out centre);
        }

        /// <summary>
        /// Get bounding radius around the centre
        /// </summary>
        /// <returns></returns>
        public float GetBoundingRadiusAroundCentre()
        {
            return 0.5f * m_sideLengths.Length();
        }

        /// <summary>
        /// Get/Set the box orientation
        /// </summary>
        public Matrix Orientation
        {
            get { return Transform.Orientation; }
            set { Transform.Orientation = value; }
        }

        /// <summary>
        /// Get the three side lengths of the box
        /// </summary>
        /// <param name="m_SideLengths"></param>
        /// <returns></returns>
        public Vector3 SideLengths
        {
            get { return this.m_sideLengths; }
            set { this.m_sideLengths = value; }
        }

        /// <summary>
        /// Expands box by amount on each side (in both +ve and -ve directions)
        /// </summary>
        /// <param name="amount"></param>
        public void Expand(Vector3 amount)
        {
            Transform.Position -= Vector3.TransformNormal(amount, Transform.Orientation);
            m_sideLengths += m_sideLengths + 2.0f * amount;
        }

        /// <summary>
        /// Returns the half-side lengths.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetHalfSideLengths()
        {
            Vector3 result = new Vector3(
                m_sideLengths.X * 0.5f,
                m_sideLengths.Y * 0.5f,
                m_sideLengths.Z * 0.5f);

            return result;
        }

        /// <summary>
        /// Returns the vector representing the edge direction 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector3 GetSide(int i)
        {
            return MyPhysicsUtils.MyPhysicsUnsafe.Get(Transform.Orientation, i) *
                MyPhysicsUtils.MyPhysicsUnsafe.Get(ref m_sideLengths, i);
        }

        /// <summary>
        /// Returns the squared distance 
        /// todo remove this/put it in distance fns
        /// </summary>
        /// <param name="closestBoxPoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetSqDistanceToPoint(out Vector3 closestBoxPoint, Vector3 point)
        {
            closestBoxPoint = Vector3.TransformNormal(point - Transform.Position, Matrix.Transpose(Transform.Orientation));

            float sqDistance = 0.0f;
            float delta;

            if (closestBoxPoint.X < 0.0f)
            {
                sqDistance += closestBoxPoint.X * closestBoxPoint.X;
                closestBoxPoint.X = 0.0f;
            }
            else if (closestBoxPoint.X > m_sideLengths.X)
            {
                delta = closestBoxPoint.X - m_sideLengths.X;
                sqDistance += delta * delta;
                closestBoxPoint.X = m_sideLengths.X;
            }

            if (closestBoxPoint.Y < 0.0f)
            {
                sqDistance += closestBoxPoint.Y * closestBoxPoint.Y;
                closestBoxPoint.Y = 0.0f;
            }
            else if (closestBoxPoint.Y > m_sideLengths.Y)
            {
                delta = closestBoxPoint.Y - m_sideLengths.Y;
                sqDistance += delta * delta;
                closestBoxPoint.Y = m_sideLengths.Y;
            }

            if (closestBoxPoint.Z < 0.0f)
            {
                sqDistance += closestBoxPoint.Z * closestBoxPoint.Z;
                closestBoxPoint.Z = 0.0f;
            }
            else if (closestBoxPoint.Z > m_sideLengths.Z)
            {
                delta = closestBoxPoint.Z - m_sideLengths.Z;
                sqDistance += delta * delta;
                closestBoxPoint.Z = m_sideLengths.Z;
            }

            Vector3.TransformNormal(ref closestBoxPoint, ref Transform.Orientation, out closestBoxPoint);
            Vector3.Add(ref Transform.Position, ref closestBoxPoint, out closestBoxPoint);

            return sqDistance;
        }

        /// <summary>
        /// Returns the distance from the point to the box, (-ve if the
        /// point is inside the box), and optionally the closest point on
        /// the box.
        /// TODO make this actually return -ve if inside
        /// todo remove this/put it in distance fns
        /// </summary>
        /// <param name="closestBoxPoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public float GetDistanceToPoint(out Vector3 closestBoxPoint,
             Vector3 point)
        {
            return (float)System.Math.Sqrt(GetSqDistanceToPoint(out closestBoxPoint, point));
        }
        
        /// <summary>
        /// Gets the minimum and maximum extents of the box along the
        /// axis, relative to the centre of the box.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="axis"></param>
        public void GetSpan(out float min, out float max, Vector3 axis)
        {
            float s, u, d;
            Vector3 right = Transform.Orientation.Right;
            Vector3 up = Transform.Orientation.Up;
            Vector3 back = Transform.Orientation.Backward;

            Vector3.Dot(ref axis, ref right, out s);
            Vector3.Dot(ref axis, ref up, out u);
            Vector3.Dot(ref axis, ref back, out d);

            s = System.Math.Abs(s * 0.5f * m_sideLengths.X);
            u = System.Math.Abs(u * 0.5f * m_sideLengths.Y);
            d = System.Math.Abs(d * 0.5f * m_sideLengths.Z);

            float r = s + u + d;
            GetCentre(out right);
            float p;
            Vector3.Dot(ref right,ref axis,out p);
            min = p - r;
            max = p + r;
        }
        
        /// <summary>
        /// Gets the corner points, populating pts
        /// </summary>
        /// <param name="pts"></param>
        public void GetCornerPoints(out Vector3[] pts)
        {
            pts = m_outPoints;
            pts[(int)BoxPointIndex.BRD] = Transform.Position;
            pts[(int)BoxPointIndex.FRD] = Transform.Position + m_sideLengths.X * Transform.Orientation.Right;
            pts[(int)BoxPointIndex.BLD] = Transform.Position + m_sideLengths.Y * Transform.Orientation.Up;
            pts[(int)BoxPointIndex.BRU] = Transform.Position + m_sideLengths.Z * Transform.Orientation.Backward;
            pts[(int)BoxPointIndex.FLD] = pts[(int)BoxPointIndex.BLD] + m_sideLengths.X * Transform.Orientation.Right;
            pts[(int)BoxPointIndex.BLU] = pts[(int)BoxPointIndex.BRU] + m_sideLengths.Y * Transform.Orientation.Up;
            pts[(int)BoxPointIndex.FRU] = pts[(int)BoxPointIndex.FRD] + m_sideLengths.Z * Transform.Orientation.Backward;
            pts[(int)BoxPointIndex.FLU] = pts[(int)BoxPointIndex.FLD] + m_sideLengths.Z * Transform.Orientation.Backward;
        }

        /// <summary>
        /// Returns a (const) list of 12 edges - at the moment in this order:
        /// {BRD, BRU}, // origin-up
        /// {BRD, BLD}, // origin-left
        /// {BRD, FRD}, // origin-fwd
        /// {BLD, BLU}, // leftorigin-up
        /// {BLD, FLD}, // leftorigin-fwd
        /// {FRD, FRU}, // fwdorigin-up
        /// {FRD, FLD}, // fwdorigin-left
        /// {BRU, BLU}, // uporigin-left
        /// {BRU, FRU}, // uporigin-fwd
        /// {BLU, FLU}, // upleftorigin-fwd
        /// {FRU, FLU}, // upfwdorigin-left
        /// {FLD, FLU}, // fwdleftorigin-up
        /// </summary>
        /// <returns></returns>
        public void GetEdges(out Edge[] edg)
        {
            edg = edges;
        }

        /// <summary>
        /// EdgeIndices will contain indexes into the result of GetAllEdges
        /// </summary>
        /// <param name="edgeIndices"></param>
        /// <param name="pt"></param>
        public void GetEdgesAroundPoint(out int[] edgeIndices, BoxPointIndex pt)
        {
            edgeIndices = new int[3];
            int ind = 0;
            
            for (int i = 0; i < edges.Length; ++i)
            {
                if ((edges[i].Ind0 == pt) || (edges[i].Ind1 == pt))
                    edgeIndices[ind++] = i;
                if (ind == 3) return;
            }
        }

        public float GetSurfaceArea()
        {
            return 2.0f * (m_sideLengths.X * m_sideLengths.Y + m_sideLengths.X * m_sideLengths.Z + m_sideLengths.Y * m_sideLengths.Z);
        }

        public float GetVolume()
        {
            return m_sideLengths.X * m_sideLengths.Y * m_sideLengths.Z;
        }

        public bool SegmentIntersect(out float fracOut, out Vector3 posOut, out Vector3 normalOut, MySegment seg)
        {
            fracOut = float.MaxValue;
            posOut = normalOut = Vector3.Zero;

            // algo taken from p674 of realting rendering
            // needs debugging
            float min = float.MinValue;
            float max = float.MaxValue;

            Vector3 p = GetCentre() - seg.Origin;
            Vector3 h;
            h.X = m_sideLengths.X * 0.5f;
            h.Y = m_sideLengths.Y * 0.5f;
            h.Z = m_sideLengths.Z * 0.5f;

            int dirMax = 0;
            int dirMin = 0;
            int dir = 0;

            //Vector3[] matrixVec = new Vector3[3];
            MyVector3Array3 matrixVec = new MyVector3Array3();
            matrixVec[0] = Transform.Orientation.Right;
            matrixVec[1] = Transform.Orientation.Up;
            matrixVec[2] = Transform.Orientation.Backward;

            //float[] vectorFloat = new float[3];
            MyFloatArray3 vectorFloat = new MyFloatArray3(); 
            vectorFloat[0] = h.X;
            vectorFloat[1] = h.Y;
            vectorFloat[2] = h.Z;

            for (dir = 0; dir < 3; dir++)
            {
                float e = Vector3.Dot(matrixVec[dir], p);
                float f = Vector3.Dot(matrixVec[dir], seg.Delta);

                if (System.Math.Abs(f) > MyPhysicsConfig.CollisionEpsilon)
                {
                    float t1 = (e + vectorFloat[dir]) / f;
                    float t2 = (e - vectorFloat[dir]) / f;

                    if (t1 > t2) { float tmp = t1; t1 = t2; t2 = tmp; }

                    if (t1 > min)
                    {
                        min = t1;
                        dirMin = dir;
                    }
                    if (t2 < max)
                    {
                        max = t2;
                        dirMax = dir;
                    }

                    if (min > max)
                        return false;

                    if (max < 0.0f)
                        return false;
                }
                else if ((-e - vectorFloat[dir] > 0.0f) ||
                    (-e + vectorFloat[dir] < 0.0f))
                {
                    return false;
                }
            }

            if (min > 0.0f)
            {
                dir = dirMin;
                fracOut = min;
            }
            else
            {
                dir = dirMax;
                fracOut = max;
            }

            fracOut = MathHelper.Clamp(fracOut, 0.0f, 1.0f);
            posOut = seg.GetPoint(fracOut);
            if (Vector3.Dot(matrixVec[dir], seg.Delta) > 0.0f)
                normalOut = -matrixVec[dir];
            else
                normalOut = matrixVec[dir];

            return true;
        }    

 


    }

}