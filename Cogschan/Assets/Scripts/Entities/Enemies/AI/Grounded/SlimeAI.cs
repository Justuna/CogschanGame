using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : GroundedEnemyAI
{
    protected override void Update()
    {
        base.Update();
        Debug.Log(_state);
    }

    public override void EndMeleeAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void EndRangedAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void BeginMeleeAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void BeginRangedAttack()
    {
        throw new System.NotImplementedException();
    }
}
