using UnityEngine;

public static class VectorUtils
{
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1,
        Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2)
                    / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public static bool LineLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 p)
    {
        const float EPSILON = 1E-6f;
        p = new Vector2(float.NaN, float.NaN);

        var cc = (a1.y - a2.y) * (b2.x - b1.x) - (a2.x - a1.x) * (b1.y - b2.y);
        if (System.Math.Abs(cc) < EPSILON) return false; // lines are parallel or congruent

        p = (1f / cc) * new Vector2(
          (a2.x - a1.x) * (b1.x * b2.y - b1.y * b2.x) - (a1.x * a2.y - a1.y * a2.x) * (b2.x - b1.x),
          (a1.x * a2.y - a1.y * a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x * b2.y - b1.y * b2.x)
        );

        return true;
    }
}