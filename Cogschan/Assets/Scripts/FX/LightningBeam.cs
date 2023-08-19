using UnityEngine;
using UnityEngine.VFX;

public class LightningBeam : MonoBehaviour, IBeamEffectPlayer
{
    [SerializeField] private VisualEffect _lightningEffect;
    [SerializeField] private float _playoutTime;

    private float _playoutTimer;

    private void Start()
    {
        _playoutTimer = _playoutTime;
    }

    public void Fire(Vector3 start, Vector3 end)
    {
        _lightningEffect.SetVector3("Start", start);
        _lightningEffect.SetVector3("End", end);
        _lightningEffect.Play();
    }

    private void Update()
    {
        _playoutTimer -= Time.deltaTime;
        if (_playoutTimer <= 0) Destroy(gameObject);
    }
}