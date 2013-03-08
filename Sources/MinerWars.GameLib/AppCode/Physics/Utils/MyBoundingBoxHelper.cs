using MinerWarsMath;
using MinerWars.AppCode.Game.Utils;
using System;

/// <summary>
/// helper class for bounding box
/// </summary>
class BoundingBoxHelper
{
    static public BoundingBox InitialBox = new BoundingBox(new Vector3(float.PositiveInfinity), new Vector3(float.NegativeInfinity));

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddPoint(ref Vector3 pos, ref BoundingBox bb)
    {
        Vector3.Min(ref bb.Min, ref pos, out bb.Min);
        Vector3.Max(ref bb.Max, ref pos, out bb.Max);
    }

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddLine(ref MyLine line, ref BoundingBox bb)
    {
        //AddPoint(ref line.From, ref bb);
        //AddPoint(ref line.To, ref bb);
        Vector3.Min(ref bb.Min, ref line.From, out bb.Min);
        Vector3.Max(ref bb.Max, ref line.From, out bb.Max);
        Vector3.Min(ref bb.Min, ref line.To, out bb.Min);
        Vector3.Max(ref bb.Max, ref line.To, out bb.Max);
    }

    //  Adds three vertexes of a triangleVertexes into bounding box, changing its min/max values
    //  This method can be used for creating bounding box for a triangleVertexes
    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddTriangle(ref BoundingBox bb, ref Vector3 vertex0, ref Vector3 vertex1, ref Vector3 vertex2)
    {
        Vector3.Min(ref bb.Min, ref vertex0, out bb.Min);
        Vector3.Max(ref bb.Max, ref vertex0, out bb.Max);
        Vector3.Min(ref bb.Min, ref vertex1, out bb.Min);
        Vector3.Max(ref bb.Max, ref vertex1, out bb.Max);
        Vector3.Min(ref bb.Min, ref vertex2, out bb.Min);
        Vector3.Max(ref bb.Max, ref vertex2, out bb.Max);
    }

    //  Adds three vertexes of a triangleVertexes into bounding box, changing its min/max values
    //  This method can be used for creating bounding box for a triangleVertexes
    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddTriangle(ref BoundingBox bb, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
    {
        bb.Min = Vector3.Min(bb.Min, vertex0);
        bb.Max = Vector3.Max(bb.Max, vertex0);
        bb.Min = Vector3.Min(bb.Min, vertex1);
        bb.Max = Vector3.Max(bb.Max, vertex1);
        bb.Min = Vector3.Min(bb.Min, vertex2);
        bb.Max = Vector3.Max(bb.Max, vertex2);
    }

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddPoint(Vector3 pos, ref BoundingBox bb)
    {
        Vector3.Min(ref bb.Min, ref pos, out bb.Min);
        Vector3.Max(ref bb.Max, ref pos, out bb.Max);
    }

    [ThreadStatic]
    static Vector3[] pts = new Vector3[8];

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddFrustum(ref BoundingFrustum frustum, ref BoundingBox bb)
    {
        if (pts == null)
            pts = new Vector3[8];

        frustum.GetCorners(pts);

        AddPoint(ref pts[0], ref bb);
        AddPoint(ref pts[1], ref bb);
        AddPoint(ref pts[2], ref bb);
        AddPoint(ref pts[3], ref bb);
        AddPoint(ref pts[4], ref bb);
        AddPoint(ref pts[5], ref bb);
        AddPoint(ref pts[6], ref bb);
        AddPoint(ref pts[7], ref bb);
    }

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddBBox(BoundingBox bbox, ref BoundingBox bb)
    {
        bb.Min = Vector3.Min(bbox.Min, bb.Min);
        bb.Max = Vector3.Max(bbox.Max, bb.Max);
    }

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    //static public void AddSphere(MySphere sphere, ref BoundingBox bb)
    //{
    //    Vector3 radius = new Vector3(sphere.Radius);
    //    Vector3 minSphere = sphere.Center;
    //    Vector3 maxSphere = sphere.Center;

    //    Vector3.Subtract(ref minSphere, ref radius, out minSphere);
    //    Vector3.Add(ref maxSphere, ref radius, out maxSphere);

    //    Vector3.Min(ref bb.Min, ref minSphere, out bb.Min);
    //    Vector3.Max(ref bb.Max, ref maxSphere, out bb.Max);
    //}

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddSphere(MinerWarsMath.BoundingSphere sphere, ref BoundingBox bb)
    {
        Vector3 radius = new Vector3(sphere.Radius);
        Vector3 minSphere = sphere.Center;
        Vector3 maxSphere = sphere.Center;

        Vector3.Subtract(ref minSphere, ref radius, out minSphere);
        Vector3.Add(ref maxSphere, ref radius, out maxSphere);

        Vector3.Min(ref bb.Min, ref minSphere, out bb.Min);
        Vector3.Max(ref bb.Max, ref maxSphere, out bb.Max);
    }

    //  IMPORTANT: Bounding box you are trying to change need to have positive/negative infinite values, so initialize it with InitialBox (static member)
    static public void AddSphere(ref MinerWarsMath.BoundingSphere sphere, ref BoundingBox bb)
    {
        Vector3 radius = new Vector3(sphere.Radius);
        Vector3 minSphere = sphere.Center;
        Vector3 maxSphere = sphere.Center;

        Vector3.Subtract(ref minSphere, ref radius, out minSphere);
        Vector3.Add(ref maxSphere, ref radius, out maxSphere);

        Vector3.Min(ref bb.Min, ref minSphere, out bb.Min);
        Vector3.Max(ref bb.Max, ref maxSphere, out bb.Max);
    }


    public static bool OverlapTest(ref BoundingBox box0, ref BoundingBox box1)
    {
        return ((box0.Min.Z >= box1.Max.Z) ||
            (box0.Max.Z <= box1.Min.Z) ||
            (box0.Min.Y >= box1.Max.Y) ||
            (box0.Max.Y <= box1.Min.Y) ||
            (box0.Min.X >= box1.Max.X) ||
            (box0.Max.X <= box1.Min.X)) ? false : true;
    }

    public static bool OverlapTest(ref BoundingBox box0, ref BoundingBox box1, float tol)
    {
        return ((box0.Min.Z >= box1.Max.Z + tol) ||
            (box0.Max.Z <= box1.Min.Z - tol) ||
            (box0.Min.Y >= box1.Max.Y + tol) ||
            (box0.Max.Y <= box1.Min.Y - tol) ||
            (box0.Min.X >= box1.Max.X + tol) ||
            (box0.Max.X <= box1.Min.X - tol)) ? false : true;
    }


}
