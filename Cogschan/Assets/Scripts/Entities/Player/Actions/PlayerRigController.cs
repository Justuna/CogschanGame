using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRigController : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private Rig _generalAimRig;
    [SerializeField] private Rig _leftHandAimRig;
    [SerializeField] private Rig _leftHandIKRig;
    [SerializeField] private Transform _leftHandTarget;

    private int _generalAimRigLocks = 0;
    private int _leftHandAimRigLocks = 0;
    private int _leftHandIKRigLocks = 0;

    public void AddGeneralAimLock()
    {
        _generalAimRigLocks++;
    }

    public void RemoveGeneralAimLock() 
    {
        if (_generalAimRigLocks == 0) return;

        _generalAimRigLocks--;
    }

    public void AddLeftHandAimLock()
    {
        _leftHandAimRigLocks++;
    }

    public void RemoveLeftHandAimLock()
    {
        if (_leftHandAimRigLocks == 0) return;

        _leftHandAimRigLocks--;
    }

    public void AddLeftHandIKLock()
    {
        _leftHandIKRigLocks++;
    }

    public void RemoveLeftHandIKLock()
    {
        if (_leftHandIKRigLocks == 0) return;

        _leftHandIKRigLocks--;
    }

    private void Update()
    {
        if (_generalAimRigLocks > 0) _generalAimRig.weight = 1;
        else _generalAimRig.weight = 0;

        if (_generalAimRigLocks > 0) _leftHandAimRig.weight = 1;
        else _leftHandAimRig.weight = 0;

        if (_leftHandIKRigLocks > 0) _leftHandIKRig.weight = 1;
        else _leftHandIKRig.weight = 0;

        _leftHandTarget.position = _services.WeaponCache.CurrentWeapon.GetLeftHandTransform().position;
        _leftHandTarget.rotation = _services.WeaponCache.CurrentWeapon.GetLeftHandTransform().rotation;
    }
}
