using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class CustomContentSizeFitter : MonoBehaviour
{
    [SerializeField] private bool _autoFitWidth = false;
    [SerializeField] private bool _autoFitHeight = false;
    [SerializeField] private bool _autoUpdate = true;

    private void Update()
    {
        if (_autoUpdate || !Application.isPlaying)
            UpdateSize();
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

    public void UpdateSize()
    {
        var rectTransform = GetComponent<RectTransform>();
        var bounds = RectTransformUtils.CalculateRelativeChildrenRectTransformBounds(this.transform);
        var sizeDelta = rectTransform.sizeDelta;
        if (_autoFitWidth)
            sizeDelta.x = bounds.size.x;
        if (_autoFitHeight)
            sizeDelta.y = bounds.size.y;
        rectTransform.sizeDelta = sizeDelta;
    }
}
