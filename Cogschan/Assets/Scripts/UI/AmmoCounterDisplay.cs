using TMPro;
using UnityEngine;

public class AmmoCounterDisplay : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _playerServices;
    [SerializeField] private TextMeshProUGUI _textBox;

    private void Update()
    {
        int? loaded = _playerServices.WeaponCache.CurrentWeapon.GetLoadedAmmoCount();
        int? reserve = _playerServices.WeaponCache.CurrentWeapon.GetReserveAmmoCount();

        // Weapon has no ammo, so just print "infinity"
        if (!loaded.HasValue || !reserve.HasValue)
        {
            _textBox.text = "\u221E";
        }
        // This will probably never happen (unless someone royally screws up)
        else if (!loaded.HasValue)
        {
            _textBox.text = "\u221E / " + reserve.Value;
        }
        // This could happen if a gun has infinite ammo, but a limited clip size (overwatch style)
        else if (!reserve.HasValue)
        {
            _textBox.text = loaded.Value + " / \u221E";
        }
        // Print ammo like normal
        else
        {
            _textBox.text = loaded.Value + " / " + reserve.Value;
        }
    }
}
