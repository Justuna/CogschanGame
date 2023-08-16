using UnityEngine;

public class OverlayArrow : MonoBehaviour
{
    [Tooltip("Degrees to offset the angle by. Use this to correct for the rotation of the arrow.")]
    [SerializeField]
    private float _angleOffset;
    [SerializeField]
    private OverlayUIInstance _overlayUIInstance;
    [SerializeField]
    private bool _hideWhenVisible = false;
    [SerializeField]
    private GameObject _arrow;

    private void LateUpdate()
    {
        if (_hideWhenVisible)
            _arrow.SetActive(_overlayUIInstance.IsOffscreen);
        if (_overlayUIInstance.IsOffscreen)
            transform.eulerAngles = new Vector3(0, 0, _overlayUIInstance.AngleToWorldPosition + _angleOffset);
        else
            transform.eulerAngles = new Vector3(0, 0, -90 + _angleOffset);
    }
}
