using MinerWarsMath;


#region public struct Segment
/// <summary>
/// A Segment is a line that starts at origin and goes only as far as
/// (origin + delta).
/// </summary>
public struct MySegment
{
    public Vector3 Origin;
    public Vector3 Delta;

    public MySegment(Vector3 origin, Vector3 delta)
    {
        this.Origin = origin;
        this.Delta = delta;
    }

    public void GetPoint(float t, out Vector3 point)
    {
        point = new Vector3(
            t * Delta.X,
            t * Delta.Y,
            t * Delta.Z);

        point.X += Origin.X;
        point.Y += Origin.Y;
        point.Z += Origin.Z;
    }

    public Vector3 GetPoint(float t)
    {
        Vector3 result = new Vector3(
            t * Delta.X,
            t * Delta.Y,
            t * Delta.Z);

        result.X += Origin.X;
        result.Y += Origin.Y;
        result.Z += Origin.Z;

        return result;
    }

    public Vector3 GetEnd()
    {
        return new Vector3(
            Delta.X + Origin.X,
            Delta.Y + Origin.Y,
            Delta.Z + Origin.Z);
        //return Origin + Delta;
    }

}
#endregion