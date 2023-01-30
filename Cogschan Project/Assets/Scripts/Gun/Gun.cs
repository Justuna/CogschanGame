using UnityEngine;
using TMPro;

/// <summary>
/// Abstract class for all guns.
/// </summary>
public abstract class Gun : MonoBehaviour
{
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
    private float fireClock;
    [SerializeField]
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
    public bool CanFire => fireClock <= 0 && (Ammo > 0 || InfiniteAmmo);
    /// <summary>
    /// Whether or not the gun is reloading.
    /// </summary>
    public bool IsReloading => reloadClock > 0;

    /// <summary>
    /// Fire without aiming down sights. Returns false if unable to fire.
    /// </summary>
    /// <param name="hitTransform">The transform the crosshair is currently pointed at.</param>
    public virtual bool HipFire(Transform hitTransform)
    {
        if (!CanFire)
            return false;
        Ammo -= 1;
        fireClock = FireRate;
        return true;
    }
    /// <summary>
    /// Fire while aiming down sights. Returns false if unable to fire.
    /// </summary>
    /// <param name="hitTransform">The transform the crosshair is currently pointed at.</param>
    public virtual bool ADSFire(Transform hitTransform)
    {
        if (!CanFire)
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