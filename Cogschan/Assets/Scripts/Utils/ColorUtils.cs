using UnityEngine;

public static class ColorUtils
{
    public static bool IsDark(this Color color)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        return s <= 0.5f;
    }

    public static Color GetNeutralContrastColor(this Color color)
    {
        return color.IsDark() ? Color.white : Color.black;
    }
}
