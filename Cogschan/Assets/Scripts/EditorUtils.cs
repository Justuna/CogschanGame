#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

public static class EditorUtils
{
    public static class Urbanist
    {
        public const string Black = "Assets/Art/Fonts/Urbanist/Urbanist-Black.ttf";
        public const string ExtraBold = "Assets/Art/Fonts/Urbanist/Urbanist-ExtraBold.ttf";
        public const string Bold = "Assets/Art/Fonts/Urbanist/Urbanist-Bold.ttf";
        public const string SemiBold = "Assets/Art/Fonts/Urbanist/Urbanist-SemiBold.ttf";
        public const string Medium = "Assets/Art/Fonts/Urbanist/Urbanist-Medium.ttf";
        public const string Regular = "Assets/Art/Fonts/Urbanist/Urbanist-Regular.ttf";
        public const string Thin = "Assets/Art/Fonts/Urbanist/Urbanist-Thin.ttf";
        public const string Light = "Assets/Art/Fonts/Urbanist/Urbanist-Light.ttf";
        public const string ExtraLight = "Assets/Art/Fonts/Urbanist/Urbanist-ExtraLight.ttf";
    }

    public static void EditorDrawTextPretty(this MonoBehaviour monoBehaviour, Vector3 localPos, string text, Color color = default, int fontSize = 24, string fontPath = Urbanist.Medium, Color backgroundColor = default, int padding = 4, Vector2 anchor = default)
        => DrawTextPretty(monoBehaviour.transform.TransformPoint(localPos), text, color, fontSize, fontPath, backgroundColor, padding, anchor);

    public static void EditorDrawText(this MonoBehaviour monoBehaviour, Vector3 localPos, string text, GUIStyle guiStyle = null, Vector2 anchor = default)
        => DrawText(monoBehaviour.transform.TransformPoint(localPos), text, guiStyle, anchor);

    public static void DrawTextPretty(Vector3 worldPos, string text, Color color = default, int fontSize = 24, string fontPath = Urbanist.Medium, Color backgroundColor = default, int padding = 4, Vector2 anchor = default)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label)
        {
            font = AssetDatabase.LoadAssetAtPath<Font>(fontPath),
            fontSize = fontSize,
            padding = new RectOffset() { bottom = padding, left = padding, right = padding, top = padding, },
            normal = new GUIStyleState() { textColor = color, background = MakeColorTexture(1, 1, backgroundColor) },
        };
        DrawText(worldPos, text, style, anchor);
    }

    public static void DrawText(Vector3 worldPos, string text, GUIStyle guiStyle, Vector2 anchor = default)
    {
#if UNITY_EDITOR    
        var view = SceneView.currentDrawingSceneView;
        if (!view)
            return;
        Vector3 screenPosition = view.camera.WorldToScreenPoint(worldPos);
        if (screenPosition.y < 0 || screenPosition.y > view.camera.pixelHeight || screenPosition.x < 0 || screenPosition.x > view.camera.pixelWidth || screenPosition.z < 0)
            return;
        var pixelRatio = EditorGUIUtility.pixelsPerPoint;
        Handles.BeginGUI();
        var style = guiStyle ?? new GUIStyle(GUI.skin.label);
        Vector2 size = style.CalcSize(new GUIContent(text)) * pixelRatio;
        var alignedPosition =
        ((Vector2)screenPosition +
            size * ((anchor + Vector2.left + Vector2.up) / 2f)) * (Vector2.right + Vector2.down) +
            Vector2.up * view.camera.pixelHeight;
        GUI.Label(new Rect(alignedPosition / pixelRatio, size / pixelRatio), text, style);
        Handles.EndGUI();
#endif
    }

    public static Texture2D MakeColorTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
        var texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}