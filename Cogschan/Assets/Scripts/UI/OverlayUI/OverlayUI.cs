using UnityEngine;

public class OverlayUI : MonoBehaviour
{
    public static OverlayUI Instance { get; private set; }

    [SerializeField] private Canvas _canvas;
    [SerializeField] private float _windowPadding = 64;
    [SerializeField] private RectTransform _overlayHolder;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LateUpdate()
    {
        foreach (RectTransform overlayTransform in _overlayHolder)
        {
            OverlayUIInstance overlayInstance = overlayTransform.GetComponent<OverlayUIInstance>();
            if (overlayInstance == null) continue;

            var targetScreenPosition = Camera.main.WorldToScreenPoint(overlayInstance.WorldPosition);
            var screenPosition = Vector3.zero;
            bool isTargetVisible = IsTargetVisible(targetScreenPosition);

            if (isTargetVisible)
            {
                targetScreenPosition = Camera.main.WorldToScreenPoint(overlayInstance.WorldPosition + overlayInstance.VisibleWorldPositionOffset);
                screenPosition = targetScreenPosition + (Vector3)overlayInstance.VisibleScreenPositionOffset;
                overlayInstance.AngleToWorldPosition = 0;
                overlayInstance.IsOffscreen = false;
                overlayTransform.gameObject.SetActive(!overlayInstance.HideWhenVisible);
            }
            else if (overlayInstance.WindowClamped)
            {
                var screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
                var screenPaddingScaled = new Vector2(_windowPadding + overlayTransform.sizeDelta.x / 2f, _windowPadding + overlayTransform.sizeDelta.y / 2f) * _canvas.scaleFactor;
                var screenBounds = new Vector3(screenCenter.x - screenPaddingScaled.x, screenCenter.y - screenPaddingScaled.y, 0);

                float angle = 0;
                GetOffscreenOverlayUIPositionAndAngle(ref targetScreenPosition, ref angle, screenCenter, screenBounds);
                screenPosition = targetScreenPosition;
                overlayInstance.AngleToWorldPosition = angle * Mathf.Rad2Deg;
                overlayInstance.IsOffscreen = true;
                overlayTransform.gameObject.SetActive(true);
            }
            else
            {
                overlayInstance.IsOffscreen = true;
                overlayTransform.gameObject.SetActive(false);
            }
            overlayTransform.position = screenPosition;
        }
    }

    private static bool IsTargetVisible(Vector3 screenPosition)
    {
        bool isTargetVisible = screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < Camera.main.pixelWidth && screenPosition.y > 0 && screenPosition.y < Camera.main.pixelHeight;
        return isTargetVisible;
    }

    private static void GetOffscreenOverlayUIPositionAndAngle(ref Vector3 screenPosition, ref float angle, Vector3 screenCentre, Vector3 screenBounds)
    {
        // Our screenPosition's origin is screen's bottom-left corner.
        // But we have to get the arrow's screenPosition and rotation with respect to screenCentre.
        screenPosition -= screenCentre;

        // When the targets are behind the camera their projections on the screen (WorldToScreenPoint) are inverted,
        // so just invert them.
        if (screenPosition.z < 0)
        {
            screenPosition *= -1;
        }

        // Angle between the x-axis (bottom of screen) and a vector starting at zero(bottom-left corner of screen) and terminating at screenPosition.
        angle = Mathf.Atan2(screenPosition.y, screenPosition.x);
        // Slope of the line starting from zero and terminating at screenPosition.
        float slope = Mathf.Tan(angle);

        // Two point's line's form is (y2 - y1) = m (x2 - x1) + c,
        // starting point (x1, y1) is screen botton-left (0, 0),
        // ending point (x2, y2) is one of the screenBounds,
        // m is the slope
        // c is y intercept which will be 0, as line is passing through origin.
        // Final equation will be y = mx.
        if (screenPosition.x > 0)
        {
            // Keep the x screen position to the maximum x bounds and
            // find the y screen position using y = mx.
            screenPosition = new Vector3(screenBounds.x, screenBounds.x * slope, 0);
        }
        else
        {
            screenPosition = new Vector3(-screenBounds.x, -screenBounds.x * slope, 0);
        }
        // Incase the y ScreenPosition exceeds the y screenBounds
        if (screenPosition.y > screenBounds.y)
        {
            // Keep the y screen position to the maximum y bounds and
            // find the x screen position using x = y/m.
            screenPosition = new Vector3(screenBounds.y / slope, screenBounds.y, 0);
        }
        else if (screenPosition.y < -screenBounds.y)
        {
            screenPosition = new Vector3(-screenBounds.y / slope, -screenBounds.y, 0);
        }
        // Bring the ScreenPosition back to its original reference.
        screenPosition += screenCentre;
    }

    public void AddOverlay(OverlayUIInstance overlayTransform)
    {
        overlayTransform.transform.SetParent(_overlayHolder);
        overlayTransform.transform.localScale = Vector3.one;
    }
}
