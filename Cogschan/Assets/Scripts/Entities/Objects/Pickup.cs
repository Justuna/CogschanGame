using UnityEngine;

public abstract class Pickup : Interactable
{
    [Tooltip("The EntityServiceLocator for this entity.")]
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _timeBeforeLive = 0.5f;
    [SerializeField] private float _timeBeforeDespawn = 10.0f;

    // Just in case the Unity garbage collector does not delete this immediately.
    private bool _deleted;

    private float _liveTimer;
    private float _despawnTimer;

    private void Start()
    {
        _liveTimer = _timeBeforeLive;
        _despawnTimer = _timeBeforeDespawn;
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (_liveTimer > 0) _liveTimer -= Time.deltaTime;

        if (_despawnTimer > 0) _despawnTimer -= Time.deltaTime;
        
        if (_despawnTimer <= 0) Destroy(_services.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "VoidPlane")  // Assuming the void plane has the tag "VoidPlane"
        {
            Destroy(_services.gameObject);
        }
    }

    protected override void InteractInternal(EntityServiceLocator services)
    {
        if (_deleted || _liveTimer > 0) return;

        if (PickupAction(services))
        {
            _deleted = true;
            Destroy(_services.gameObject);
        }
    }

    protected abstract bool PickupAction(EntityServiceLocator services);
}
