using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Gun : MonoBehaviour, IWeapon
{
    [field: SerializeField]
    public UnityEvent Fired { get; private set; }

    [Header("Gun Attributes")]
    [Tooltip("The particle system responsible for any effects that should be spawned when the gun fires.")]
    [SerializeField] protected ParticleSystem _muzzleFlash;
    [Tooltip("The recoil pattern to call when this gun fires.")]
    [SerializeField] protected RecoilPattern _recoilPattern;
    [Tooltip("The spread pattern to call when this gun fires.")]
    [SerializeField] protected SpreadPattern _spreadPattern;
    [Tooltip("The spread pattern to call when this gun fires accurately.")]
    [SerializeField] protected SpreadPattern _spreadPatternAccutate;
    [Tooltip("How long is required to wait after firing the gun to fire it again.")]
    [SerializeField] protected float _fireRate = 0.5f;
    [Tooltip("The unique name that identifies this gun prefab.")]
    [SerializeField] protected string _name;
    [Tooltip("Icon for this weapon")]
    [SerializeField] protected Sprite _icon;
    [Tooltip("The transform from which the bullet will originate.")]
    [SerializeField] protected Transform _muzzle;

    [Header("Ammo Attributes")]
    [Tooltip("Whether or not this gun requires ammo to use.")]
    [SerializeField] protected bool _requiresAmmo;
    [Tooltip("The kind of ammo this gun uses.")]
    [SerializeField] protected AmmoType _ammoType;
    [Tooltip("The maximum amount of ammo that can be readied at once.")]
    [SerializeField] protected int _clipSize;
    [Tooltip("The maximum amount of clips that can be in reserve at once.")]
    [SerializeField] protected int _maxClips;
    [Tooltip("The number of bullets fired in one shot.")]
    [SerializeField] protected int _count = 1;
    [Tooltip("The number of ammo fired in one shot when firing accurately")]
    [SerializeField] private int _countAccurate;
    [Tooltip("The amount of time the gun takes to reload")]
    [SerializeField] private float _reloadTime;



    protected EntityServiceLocator _services;
    protected float _fireRateTimer = 0;
    protected int _loadedAmmo = 0;
    protected int _reserveAmmo = 0;
    /// <summary>
    /// The <see cref="SpreadEvent"/> associated with the gun.
    /// </summary>
    protected SpreadEvent _spreadEvent;
    /// <summary>
    /// The <see cref="SpreadEvent"/> associated with the gun's accurate firing mode.
    /// </summary>
    protected SpreadEvent _spreadEventAccurate;

    private bool _contRecoilActive = false;

    protected void Start()
    {
        if (_requiresAmmo)
        {
            _loadedAmmo = _clipSize;
            _reserveAmmo = _clipSize;
        }
        if (_spreadPattern is not null)
            _spreadEvent = new(_spreadPattern);
        if (_spreadPatternAccutate is not null)
            _spreadEventAccurate = new(_spreadPatternAccutate);
    }

    public void Init(EntityServiceLocator services)
    {
        _services = services;
    }

    protected void Update()
    {
        if (_fireRateTimer > 0)
        {
            _fireRateTimer -= Time.deltaTime;
        }

        _spreadEvent?.StepTime();
        _spreadEventAccurate?.StepTime();
    }

    public string GetName()
    {
        return _name;
    }

    public Sprite GetIcon()
    {
        return _icon;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Use()
    {
        if (_fireRateTimer > 0 || InUse() || !SufficientAmmo()) return;

        // TODO: Wait for animation to play before doing all fire setup.
        // TODO: Move next part to a delayed function callback for when animation finishes.

        PreFireSetup();

        if (_services.MovementController.IsAiming) FireAccurate(_services.CameraController.TargetPosition.Value);
        else Fire(_services.CameraController.TargetPosition.Value);
    }

    /// <summary>
    /// Does all of the setup necessary right before the gun is fired.
    /// </summary>
    private void PreFireSetup()
    {
        _fireRateTimer = _fireRate;
        _loadedAmmo--;

        if (_muzzleFlash != null) _muzzleFlash.Play();
        if (_recoilPattern != null)
        {
            switch (_recoilPattern)
            {
                case SimpleRecoilPattern simple:
                    _services.CameraController.AddRecoil(simple);
                    break;
                case ContinuousRecoilPattern cont:
                    if (!_contRecoilActive)
                    {
                        Func<bool> endCondition = () =>
                        {
                            bool condition = !_services.ActionController.IsFiring || !SufficientAmmo();
                            _contRecoilActive = !condition;
                            return condition;
                        };
                        _services.CameraController.AddRecoil(cont, endCondition);
                    }
                    break;
            }
        }
        _spreadEvent?.IncrementSpread();

        Fired.Invoke();
        _spreadEventAccurate?.IncrementSpread();
    }

    /// <summary>
    /// Fires the gun from the hip.
    /// </summary>
    /// <param name="targetPosition">The world position to be firing at.</param>
    protected abstract void Fire(Vector3 targetPosition);

    /// <summary>
    /// Fires the gun accurately.
    /// </summary>
    /// <param name="targetPosition">The world position to be firing at.</param>
    protected abstract void FireAccurate(Vector3 targetPosition);



    /// <summary>
    /// Whether or not Cogschan is in a firing animation.
    /// </summary>
    /// <returns>
    /// Returns true if the Cogschan is still in the middle of the firing animation. Returns false otherwise.
    /// </returns>
    public bool InUse()
    {
        // TODO: Implement animation stuff.

        return false;
    }

    /// <summary>
    /// Cancels the fire animation.
    /// </summary>
    public void CancelUse()
    {
        // TODO: Implement animation stuff.
    }

    public bool SufficientAmmo()
    {
        return !_requiresAmmo || _loadedAmmo > 0;
    }

    public bool CanReload()
    {
        return _requiresAmmo && _reserveAmmo > 0 && _loadedAmmo < _clipSize;
    }

    public void Reload()
    {
        if (!_requiresAmmo) return;

        int missingAmmo = _clipSize - _loadedAmmo;
        if (_reserveAmmo >= missingAmmo)
        {
            _loadedAmmo = _clipSize;
            _reserveAmmo -= missingAmmo;
        }
        else
        {
            _loadedAmmo = _reserveAmmo;
            _reserveAmmo = 0;
        }
    }

    public bool CanLoadClip()
    {
        return _requiresAmmo && _reserveAmmo < _clipSize * _maxClips;
    }

    public void LoadClip()
    {
        if (!_requiresAmmo) return;

        _reserveAmmo = Mathf.Min(_clipSize * _maxClips, _reserveAmmo + _clipSize);
    }

    public AmmoType GetAmmoType()
    {
        if (!_requiresAmmo)
        {
            return null;
        }

        return _ammoType;
    }

    public int? GetLoadedAmmoCount()
    {
        if (!_requiresAmmo) return null;
        else return _loadedAmmo;
    }

    public int? GetReserveAmmoCount()
    {
        if (!_requiresAmmo) return null;
        else return _reserveAmmo;
    }

    float IWeapon.GetReloadTime()
    {
        return _reloadTime;
    }
}