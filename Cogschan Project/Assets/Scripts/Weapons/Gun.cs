using UnityEngine;
using StarterAssets;
using Cinemachine;
using TMPro;

/// <summary>
/// Abstract class for all guns.
/// </summary>
public abstract class Gun : MonoBehaviour
{
    [Header("Generic Gun Attributes")]
    /// <summary>
    /// Whether or not the gun has infinite ammo. If true, all other ammo settings are obsolete.
    /// </summary>
    [SerializeField]
    [Tooltip("Whether or not the gun has infinite ammo. If true, all other ammo settings are obsolete.")]
    protected bool InfiniteAmmo;
    /// <summary>
    /// The maximum ammo a gun can have in one magazine.
    /// </summary>
    [SerializeField]
    [Tooltip("The maximum ammo a gun can have in one magizine.")]
    protected int MaxAmmo;
    /// <summary>
    /// The maximum ammo a gun can have in reserve.
    /// </summary>
    [SerializeField]
    [Tooltip("THe maximum ammo a gun can have in reserve")]
    protected int MaxReserveAmmo;
    /// <summary>
    /// The amount of time it takes to reload
    /// </summary>
    [SerializeField]
    [Tooltip("The amount of time it takes to reload.")]
    protected float ReloadTime;
    [SerializeField]
    [Tooltip("The amount of time between shots.")]
    protected float _fireRate;
    [SerializeField]
    [Tooltip("The kind of ammo this gun uses.")]
    protected AmmoType ammoType;
    protected PlayerController playerController;
    private float fireClock;
    private float reloadClock;
    private TextMeshProUGUI ammoText;

    /// <summary>
    /// The ammo count of the gun.
    /// </summary>
    public int Ammo { get; protected set; }
    /// <summary>
    /// The amount of ammo in reserve to be used when reloading..
    /// </summary>
    public int ReserveAmmo { get; protected set; }
    /// <summary>
    /// The amount of time between shots.
    /// </summary>
    public float FireRate { get { return _fireRate; } protected set { _fireRate = value; } }
    /// <summary>
    /// Whether or not the gun can fire.
    /// </summary>
    public bool CanFire => fireClock <= 0;
    /// <summary>
    /// Whether or not the gun has ammo to fire.
    /// </summary>
    public bool HasAmmo => Ammo > 0 || InfiniteAmmo;
    /// <summary>
    /// Whether or not the gun is reloading.
    /// </summary>
    public bool IsReloading => reloadClock > 0;

    /// <summary>
    /// Fire without aiming down sights. Returns false if unable to fire.
    /// </summary>
    /// <param name="hitTransform">The transform the crosshair is currently pointed at.</param>
    public virtual bool HipFire()
    {
        if (!CanFire || !HasAmmo)
            return false;
        Ammo -= 1;
        fireClock = FireRate;
        return true;
    }
    /// <summary>
    /// Fire while aiming down sights. Returns false if unable to fire.
    /// </summary>
    /// <param name="hitTransform">The transform the crosshair is currently pointed at.</param>
    public virtual bool ADSFire()
    {
        if (!CanFire || !HasAmmo)
            return false;
        Ammo -= 1;
        fireClock = FireRate;
        return true;
    }
    /// <summary>
    /// Begin reloading the gun.
    /// </summary>
    public virtual void StartReload()
    {
        if (InfiniteAmmo) return;
        reloadClock = ReloadTime;
    }
    /// <summary>
    /// Finish reloading the gun.
    /// </summary>
    public virtual void FinishReload()
    {
        int neededAmmo = MaxAmmo - Ammo;
        int ammoReloaded = neededAmmo < ReserveAmmo ? neededAmmo : ReserveAmmo;
        Ammo += ammoReloaded;
        ReserveAmmo -= ammoReloaded;
    }
    /// <summary>
    /// Add ammo clip.
    /// </summary>
    public virtual bool AddAmmo(AmmoType type, int amount)
    {
        if (InfiniteAmmo || type.Identifier != ammoType.Identifier) return false;

        if (ReserveAmmo < MaxReserveAmmo)
        {
            ReserveAmmo = Mathf.Min(ReserveAmmo + amount, MaxReserveAmmo);
            return true;
        }

        return false;
    }
    /// <summary>
    /// Grab a reference to the player movement script.
    /// </summary>
    protected virtual void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    /// <summary>
    /// Set the initial Ammo and ReserveAmmo counts, as well as ammoText.
    /// TODO: Move the ammoText to a GUIManager.
    /// </summary>
    protected virtual void Start()
    {
        Ammo = MaxAmmo;
        ReserveAmmo = MaxReserveAmmo;
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Check if the gun is reloading or able to fire and manage the appropiate timers. Also manages GUI.
    /// TODO: I feel like ADS camera stuff and camera stuff in general should be with the movement controller,
    /// but that's currently under construction, so I placed it here. Also, I feel like there's a better way
    /// to rotate the camera than do a raycast every update? Maybe just add a toggle-able "lock model face to camera"
    /// setting in the movement controls.
    /// </summary>
    protected virtual void Update()
    {
        if (IsReloading)
            reloadClock -= Time.deltaTime;
        if (!CanFire)
            fireClock -= Time.deltaTime;

        if (InfiniteAmmo) ammoText.text = "\u221E";
        else ammoText.text = $"{Ammo}|{ReserveAmmo}";
    }
}