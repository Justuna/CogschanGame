using UnityEngine;

public static class Utils
{
    public static int MaskForLayer(int layer)
    {
        int mask = 0;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(i, layer))
            {
                mask |= 1 << i;
            }
        }
        return mask;
    }
}