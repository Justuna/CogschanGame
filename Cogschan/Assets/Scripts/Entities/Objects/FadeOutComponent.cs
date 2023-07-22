using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FadeOutComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The rate at which the transparency increases.")]
    private float _rate;

    private MeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }


    private void Update()
    {
        _renderer.material.color -= new Color(0, 0, 0, _rate * Time.deltaTime);
        if (_renderer.material.color.a <= 0)
            Destroy(gameObject);
    }
}