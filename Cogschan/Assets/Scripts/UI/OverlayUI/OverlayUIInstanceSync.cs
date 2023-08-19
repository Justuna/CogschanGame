using NaughtyAttributes;
using UnityEngine;

public class OverlayUIInstanceSync : MonoBehaviour
{
    [SerializeField]
    private bool _usePrefab;
    [ShowIf(nameof(_usePrefab))]
    [SerializeField]
    private GameObject _prefab;
    [HideIf(nameof(_usePrefab))]
    [SerializeField]
    private OverlayUIInstance _overlayUIInstance;

    private void Start()
    {
        if (_usePrefab)
            _overlayUIInstance = Instantiate(_prefab).GetComponent<OverlayUIInstance>();
        _overlayUIInstance.gameObject.SetActive(true);
        OverlayUI.Instance.AddOverlay(_overlayUIInstance);
    }

    private void LateUpdate()
    {
        _overlayUIInstance.WorldPosition = transform.position;
    }

    private void OnDestroy()
    {
        if (_overlayUIInstance != null)
            Destroy(_overlayUIInstance.gameObject);
    }
}
