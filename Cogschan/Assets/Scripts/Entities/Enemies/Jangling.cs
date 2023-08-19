using UnityEngine;
using UnityEngine.UI;

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
    private MeshRenderer[] _colorMeshRenderers;
    [SerializeField]
    private Image[] _colorImages;
    private MaterialPropertyBlock _propertyBlock;
    [SerializeField]
    private JanglingKeyGrab _janglingKeyGrab;

    public void Awake()
    {
        KeyData.Jangling = this;
        UpdateKeyDataVisuals();
        _janglingKeyGrab.Init(KeyData);
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

        foreach (var meshRenderer in _colorMeshRenderers)
            if (meshRenderer != null)
                meshRenderer.SetPropertyBlock(_propertyBlock);

        foreach (var colorImage in _colorImages)
            if (colorImage != null)
            {
                // Preserve intiial alpha
                var color = KeyData.Color;
                color.a = colorImage.color.a;
                colorImage.color = color;
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
