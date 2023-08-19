using System.Linq;
using UnityEngine;

public static class RectTransformUtils
{
    private static readonly Vector3[] s_Corners = new Vector3[4];

    public static Bounds CalculateRelativeChildrenRectTransformBounds(Transform root) =>
        CalculateRelativeChildrenRectTransformBounds(root, root);

    public static Bounds CalculateRelativeChildrenRectTransformBounds(Transform root, Transform child)
    {
        RectTransform[] componentsInChildren = child.GetComponentsInChildren<RectTransform>(includeInactive: false).Where(x => x != root).ToArray();
        if (componentsInChildren.Length != 0)
        {
            Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
            int i = 0;
            for (int num = componentsInChildren.Length; i < num; i++)
            {
                componentsInChildren[i].GetWorldCorners(s_Corners);
                for (int j = 0; j < 4; j++)
                {
                    Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[j]);
                    vector = Vector3.Min(lhs, vector);
                    vector2 = Vector3.Max(lhs, vector2);
                }
            }

            Bounds result = new Bounds(vector, Vector3.zero);
            result.Encapsulate(vector2);
            return result;
        }

        return new Bounds(Vector3.zero, Vector3.zero);
    }
}
