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
    [SerializeField]
    [Tooltip("The camera that the ADS mode uses.")]
    protected CinemachineVirtualCamera aimVirtualCamera;

    private List<GameObject> _cache = new List<GameObject>();
    private PlayerInputController _ctrl;
    private Gun _currGun;
    private int _currGunIndex = 0;

    public bool IsReloading => _currGun.IsReloading;

    private void Awake()
    {
        _ctrl = GetComponent<PlayerInputController>();

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
        _currGun.gameObject.SetActive(false);
        _currGunIndex = (_currGunIndex + 1) % _cache.Count;
        SwitchWeapon();
    }

    public void PrevWeapon()
    {
        _currGun.gameObject.SetActive(false);
        _currGunIndex = (_currGunIndex - 1) % _cache.Count;
        if (_currGunIndex < 0) _currGunIndex = _cache.Count + _currGunIndex;
        SwitchWeapon();
    }

    private void SwitchWeapon()
    {
        GameObject newGun = _cache[_currGunIndex];
        newGun.SetActive(true);
        _currGun = newGun.GetComponent<Gun>();
    }

    public GameObject AddGun(GameObject prefab, bool useImmediately)
    {
        GameObject gun = Instantiate(prefab, RightHand);
        _cache.Add(gun);
        gun.GetComponent<Gun>().SetAimCamera(aimVirtualCamera);
        Debug.Log("Added " + prefab.name);

        return gun;
    }
}
