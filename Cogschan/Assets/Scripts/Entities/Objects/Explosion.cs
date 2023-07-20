using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The rate at which the size of teh explosion increases.")]
    private float _rate;
    [SerializeField]
    [Tooltip("The time the explosion lasts.")]
    private float _time;

    private void Start()
    {
        Destroy(this, _time);
    }

    private void Update()
    {
        float scale = transform.localScale.magnitude;
        transform.localScale *= (scale + _rate * Time.deltaTime) / scale;
    }
}