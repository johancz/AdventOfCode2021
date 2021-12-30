namespace Shared.Extensions;

public record Vector3Long
{
    public Vector3Long(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public long X { get; set; }
    public long Y { get; set; }
    public long Z { get; set; }

    public static Vector3Long operator *(Vector3Long vector3A, Vector3Long vector3B)
    {
        return new Vector3Long(vector3A.X * vector3B.X, vector3A.Y * vector3B.Y, vector3A.Z * vector3B.Z);
    }

    public static Vector3Long operator -(Vector3Long vector3A, Vector3Long vector3B)
    {
        return new Vector3Long(vector3A.X - vector3B.X, vector3A.Y - vector3B.Y, vector3A.Z - vector3B.Z);
    }

    public static Vector3Long operator +(Vector3Long vector3A, Vector3Long vector3B)
    {
        return new Vector3Long(vector3A.X + vector3B.X, vector3A.Y + vector3B.Y, vector3A.Z + vector3B.Z);
    }
}
