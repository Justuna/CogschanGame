using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : GroundedEnemyAI
{
    [SerializeField]
    [Tooltip("The melee hitbox of the enemy.")]
    private GameObject _hitbox;

    public override void EndMeleeAttack()
    {
        _hitbox.GetComponent<MeleeHitbox>().Deactivate();
        _hitbox.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void EndRangedAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void BeginMeleeAttack()
    {
        _hitbox.GetComponent<MeleeHitbox>().Activate(); 
        _hitbox.GetComponent<MeshRenderer>().enabled = true;
    }

    public override void BeginRangedAttack()
    {
        throw new System.NotImplementedException();
    }
}
