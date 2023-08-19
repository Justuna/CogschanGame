using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FadeComponent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The amount of time it takes to finish the fade in.\nNegative values indicate a fade out.")]
    private float _time;

    private float _rate;

    private MeshRenderer _renderer;

    private void Start()
    {
        _rate = 1 / _time;
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.color = new Color(
            _renderer.material.color.r,
            _renderer.material.color.g,
            _renderer.material.color.b,
            _rate > 0 ? 0 : 1);
    }


    private void Update()
    {
        _renderer.material.color += new Color(0, 0, 0, _rate * Time.deltaTime);
        if (_renderer.material.color.a <= 0 || _renderer.material.color.a >= 1)
            Destroy(gameObject);
    }

    /// <summary>
    /// Instantiates a game object and assigns it's time field.
    /// </summary>
    /// <returns> The instantiated <see cref="GameObject"/> </returns>
    /// <exception cref="ArgumentException"> Thrown if the prefab doesn't have a <see cref="MeshRenderer"/> component. </exception>
    public static GameObject Create(GameObject prefab, Transform parent, float time)
    {
        if (prefab.GetComponent<MeshRenderer>() == null)
            throw new ArgumentException("The game object to be instantiated must have a MeshRenderer component.", nameof(prefab));
        GameObject go = Instantiate(prefab, parent);
        go.AddComponent<FadeComponent>();
        go.GetComponent<FadeComponent>()._time = time;
        return go;
    }
}