using TMPro;
using UnityEngine;

public class AmmoCounterDisplay : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _maxAmmoText;

    public void Construct(EntityServiceLocator services)
    {
        _services = services;
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
            _maxAmmoText.text = " / " + reserve.Value;
        }
        // This could happen if a gun has infinite ammo, but a limited clip size (overwatch style)
        else if (!reserve.HasValue)
        {
            _ammoText.text = loaded.Value.ToString();
            _maxAmmoText.text = " / \u221E";
        }
        // Print ammo like normal
        else
        {
            _ammoText.text = loaded.Value.ToString();
            _maxAmmoText.text = " / " + reserve.Value;
        }
    }
}
