using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class OverlayUIDistanceFade : MonoBehaviour
{
    [SerializeField]
    private OverlayUIInstance _overlayUIInstance;
    [SerializeField]
    private Image[] _images;
    [SerializeField]
    private float _fadeDistance;
    [SerializeField]
    private AnimationCurve _fadeCurve;
    [SerializeField]
    private bool _showWhenLineOfSightBlocked;
    [ShowIf(nameof(_showWhenLineOfSightBlocked))]
    [SerializeField]
    private LayerMask _lineOfSightLayerMask;

    private void LateUpdate()
    {
        float value = 0;
        var cameraToOverlayDistance = Vector3.Distance(Camera.main.transform.position, _overlayUIInstance.WorldPosition);
        if (_showWhenLineOfSightBlocked && Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(_overlayUIInstance.WorldPosition)), cameraToOverlayDistance, _lineOfSightLayerMask))
        {
            value = 1;
        }
        else
            value = _fadeCurve.Evaluate(cameraToOverlayDistance / _fadeDistance);

        foreach (var image in _images)
        {
            var color = image.color;
            color.a = value;
            image.color = color;
        }
    }
}
