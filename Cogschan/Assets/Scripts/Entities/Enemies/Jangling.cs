using UnityEngine;

[ExecuteAlways]
public class Jangling : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField]
    public KeyData KeyData { get; set; }

    [field: Header("Dependencies")]
    [field: SerializeField]
    public EntityServiceLocator ServiceLocator { get; private set; }
    [SerializeField]
    private HealthDisplay _healthDisplay;
    [SerializeField]
    private MeshRenderer[] _colorMeshRenderers;
    private MaterialPropertyBlock _propertyBlock;

    public void Awake()
    {
        KeyData.Jangling = this;
        UpdateKeyDataVisuals();
    }

    public void Update()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
            UpdateKeyDataVisuals();
#endif
    }

    private void UpdateKeyDataVisuals()
    {
        if (_propertyBlock == null)
            _propertyBlock = new MaterialPropertyBlock();
        _propertyBlock.SetColor("_BaseColor", KeyData.Color);

        if (_healthDisplay != null)
            _healthDisplay.SetSingleFillColor(KeyData.Color);
        foreach (var meshRenderer in _colorMeshRenderers)
        {
            if (meshRenderer != null)
                meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        if (ServiceLocator.PointGraph != null)
        {
            var edgeColor = KeyData.Color;
            edgeColor.a = 0.5f;
            ServiceLocator.PointGraph.DebugNodeColor = KeyData.Color;
            ServiceLocator.PointGraph.DebugEdgeColor = edgeColor;
        }
    }
}
