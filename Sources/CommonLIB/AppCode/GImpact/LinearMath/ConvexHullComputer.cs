//#define DEBUG_CONVEX_HULL
///*
//Copyright (c) 2011 Ole Kniemeyer, MAXON, www.maxon.net

//This software is provided 'as-is', without any express or implied warranty.
//In no event will the authors be held liable for any damages arising from the use of this software.
//Permission is granted to anyone to use this software for any purpose, 
//including commercial applications, and to alter it and redistribute it freely, 
//subject to the following restrictions:

//1. The origin of this software must not be misrepresented; you must not claim that you wrote the original software. If you use this software in a product, an acknowledgment in the product documentation would be appreciated but is not required.
//2. Altered source versions must be plainly marked as such, and must not be misrepresented as being the original software.
//3. This notice may not be removed or altered from any source distribution.
//*/

//using System;
//using System.Collections.Generic;
//using System.Text;
//using MinerWarsMath;
//using System.Diagnostics;


//namespace BulletXNA.LinearMath
//{
///// Convex hull implementation based on Preparata and Hong
///// See http://code.google.com/p/bullet/issues/detail?id=275
///// Ole Kniemeyer, MAXON Computer GmbH
//    public class ConvexHullComputer
//    {
//        // Vertices of the output hull
//        public ObjectArray<IndexedVector3> vertices = new ObjectArray<IndexedVector3>();

//        // Edges of the output hull
//        public ObjectArray<Edge> edges = new ObjectArray<Edge>();

//        // Faces of the convex hull. Each entry is an index into the "edges" array pointing to an edge of the face. Faces are planar n-gons
//        public ObjectArray<int> faces = new ObjectArray<int>();

//        /*
//        Compute convex hull of "count" vertices stored in "coords". "stride" is the difference in bytes
//        between the addresses of consecutive vertices. If "shrink" is positive, the convex hull is shrunken
//        by that amount (each face is moved by "shrink" length units towards the center along its normal).
//        If "shrinkClamp" is positive, "shrink" is clamped to not exceed "shrinkClamp * innerRadius", where "innerRadius"
//        is the minimum distance of a face to the center of the convex hull.

//        The returned value is the amount by which the hull has been shrunken. If it is negative, the amount was so large
//        that the resulting convex hull is empty.

//        The output convex hull can be found in the member variables "vertices", "edges", "faces".
//        */
//        public float Compute(float[] coords, int stride, int count, float shrink, float shrinkClamp)
//        {
//            return Compute(coords, false, stride, count, shrink, shrinkClamp);
//        }

//        private float Compute(float[] coords, bool doubleCoords, int stride, int count, float shrink, float shrinkClamp)
//        {
//            return 0.0f;
//        }

//}

//    public struct Point32
//    {
//        public int X;
//        public int Y;
//        public int Z;
//        public int index;
//    }




//        public class Edge
//        {
//            private int next;
//            private int reverse;
//            private int targetVertex;


//                public int GetSourceVertex() 
//                {
//                    return (this + reverse).targetVertex;
//                }

//                public int GetTargetVertex() 
//                {
//                    return targetVertex;
//                }

//                public Edge GetNextEdgeOfVertex() // counter-clockwise list of all edges of a vertex
//                {
//                    return this + next;
//                }

//                public Edge GetNextEdgeOfFace() // clockwise list of all edges of a face
//                {
//                    return (this + reverse).GetNextEdgeOfVertex();
//                }

//                public Edge GetReverseEdge()
//                {
//                    return this + reverse;
//                }
//        }


//    private class ConvexHullInternal
//    {
//    }

//        public class Vertex
//        {
//                public Vertex next;
//                public Vertex prev;
//                ObjectArray<Edge> edges;
//                public Face firstNearbyFace;
//                public Face lastNearbyFace;
//                //PointR128 point128;
//                Point32 point;
//                int copy;
				
//                public Vertex()
//                {
//                    copy = -1;
//                }

//#if DEBUG_CONVEX_HULL
//                void Print()
//                {
//                    System.Console.WriteLine("V{0} ({1})", point.index, point);
//                }

//                void printGraph();
//#endif

//                public static Point32 operator -(Point32 value1, Vertex value2)
//                {
//                    return new Point32(value1.X - value2.point.X,value1.Y - value2.point.Y,value1.Z - value2.point.Z);
//                }


//                public float xvalue()
//                {
//                    return (point.index >= 0) ? float(point.x) : point128.xvalue();
//                }

//                public float yvalue()
//                {
//                    return (point.index >= 0) ? float(point.y) : point128.yvalue();
//                }

//                public float zvalue()
//                {
//                    return (point.index >= 0) ? float(point.z) : point128.zvalue();
//                }

//                void receiveNearbyFaces(Vertex* src)
//                {
//                    if (lastNearbyFace)
//                    {
//                        lastNearbyFace.nextWithSameNearbyVertex = src.firstNearbyFace;
//                    }
//                    else
//                    {
//                        firstNearbyFace = src.firstNearbyFace;
//                    }
//                    if (src.lastNearbyFace)
//                    {
//                        lastNearbyFace = src.lastNearbyFace;
//                    }
//                    for (Face* f = src.firstNearbyFace; f; f = f.nextWithSameNearbyVertex)
//                    {
//                        btAssert(f.nearbyVertex == src);
//                        f.nearbyVertex = this;
//                    }
//                    src.firstNearbyFace = NULL;
//                    src.lastNearbyFace = NULL;
//                }
//        }


//        public class Edge
//        {
//                public Edge next;
//                public Edge prev;
//                public Edge reverse;
//                public Vertex target;
//                public Face face;
//                int copy;

//                //~Edge()
//                //{
//                //    next = NULL;
//                //    prev = NULL;
//                //    reverse = NULL;
//                //    target = NULL;
//                //    face = NULL;
//                //}

//                public void Link(Edge n)
//                {
//                    Debug.Assert(reverse.target == n.reverse.target);
//                    next = n;
//                    n.prev = this;
//                }

//#if DEBUG_CONVEX_HULL
//                void print()
//                {
//                    System.Console.WriteLine("E{0} : {1} . {2},  n={3} p={4}   (0 {5}) . ({6})", this, reverse.target.point.index, target.point.index, next, prev,
//                                 reverse.target.point, target.point);
//                }
//#endif
//        }

//        public class Face
//        {
//                public Face next;
//                public Vertex nearbyVertex;
//                public Face nextWithSameNearbyVertex;
//                Point32 origin;
//                Point32 dir0;
//                Point32 dir1;

//                public Face()
//                {
//                }

//                public void Init(Vertex a, Vertex b, Vertex c)
//                {
//                    nearbyVertex = a;
//                    origin = a.point;
//                    dir0 = *b - *a;
//                    dir1 = *c - *a;
//                    if (a.lastNearbyFace)
//                    {
//                        a.lastNearbyFace.nextWithSameNearbyVertex = this;
//                    }
//                    else
//                    {
//                        a.firstNearbyFace = this;
//                    }
//                    a.lastNearbyFace = this;
//                }

//        }

//}
