using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponDisplay : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _maxAmmoText;
    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [SerializeField] private Image _weaponIconImage;

    public void Init(EntityServiceLocator services)
    {
        _services = services;
        _services.WeaponCache.WeaponChanged.AddListener(OnWeaponChanged);
    }

    private void OnWeaponChanged(IWeapon newWeapon)
    {
        _weaponNameText.text = newWeapon.GetName();
        _weaponIconImage.sprite = newWeapon.GetIcon();
    }

    private void Update()
    {
        int? loaded = _services.WeaponCache.CurrentWeapon.GetLoadedAmmoCount();
        int? reserve = _services.WeaponCache.CurrentWeapon.GetReserveAmmoCount();

        // Weapon has no ammo, so just print "infinity"
        if (!loaded.HasValue || !reserve.HasValue)
        {
            _ammoText.text = "\u221E";
            _maxAmmoText.text = "";
        }
        // This will probably never happen (unless someone royally screws up)
        else if (!loaded.HasValue)
        {
            _ammoText.text = "\u221E";
            _maxAmmoText.text = reserve.Value.ToString();
        }
        // This could happen if a gun has infinite ammo, but a limited clip size (overwatch style)
        else if (!reserve.HasValue)
        {
            _ammoText.text = loaded.Value.ToString();
            _maxAmmoText.text = "\u221E";
        }
        // Print ammo like normal
        else
        {
            _ammoText.text = loaded.Value.ToString();
            _maxAmmoText.text = reserve.Value.ToString();
        }
    }
}
