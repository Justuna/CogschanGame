using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponCache : MonoBehaviour
{
    [SerializeField]
    private GameObject StartingGun;
    [SerializeField]
    private Transform RightHand;

    private List<GameObject> _cache = new List<GameObject>();
    private PlayerController _ctrl;
    private Gun _currGun;
    private int _currGunIndex = 0;

    public bool IsReloading => _currGun.IsReloading;
    public bool CanFire => _currGun.CanFire;

    public Gun CurrGun => _currGun;

    private void Awake()
    {
        _ctrl = GetComponent<PlayerController>();

        GameObject startingGun = AddGun(StartingGun, true);
        _currGun = startingGun.GetComponent<Gun>();
    }

    void Update()
    {
        if (_ctrl.ActState == ActionState.Fire)
        {
            if (_ctrl.MoveState == MovementState.ADS) _currGun.ADSFire();
            else _currGun.HipFire();
        }
    }

    public void StartReload()
    {
        _currGun.StartReload();
    }

    public void FinishReload()
    {
        _currGun.FinishReload();
    }

    public void NextWeapon()
    {
        _currGunIndex = (_currGunIndex + 1) % _cache.Count;
        SwitchWeapon();
    }

    public void PrevWeapon()
    {
        _currGunIndex = (_currGunIndex - 1) % _cache.Count;
        if (_currGunIndex < 0) _currGunIndex = _cache.Count + _currGunIndex;
        SwitchWeapon();
    }

    public void OtherWeapon(int index)
    {
        if (_currGunIndex == index || index < 0 || index >= _cache.Count)
            return;
        _currGunIndex = index;
        SwitchWeapon();
    }

    private void SwitchWeapon()
    {
        if (_currGun != null) _currGun.gameObject.SetActive(false);

        GameObject newGun = _cache[_currGunIndex];
        newGun.SetActive(true);
        _currGun = newGun.GetComponent<Gun>();
    }

    public GameObject AddGun(GameObject prefab, bool useImmediately)
    {
        GameObject gun = Instantiate(prefab, RightHand);
        _cache.Add(gun);

        if (useImmediately)
        {
            _currGunIndex = _cache.Count - 1;
            SwitchWeapon();
        }
        else
        {
            gun.SetActive(false);
        }
        return gun;
    }

    public bool AddAmmo(AmmoType type, int amount)
    {
        foreach (GameObject gun in _cache)
        {
            if (gun.GetComponent<Gun>().AddAmmo(type, amount))
            {
                return true;
            }
        }

        return false;
    }
}
