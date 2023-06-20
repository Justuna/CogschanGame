using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishAI : FlyingEnemyAI
{
    protected override void Update()
    {
        base.Update();
        Debug.Log(_state);
    }

    public override void EndRangedAttack()
    {
        throw new System.NotImplementedException();
    }

    protected override void BeginRangedAttack()
    {
        throw new System.NotImplementedException();
    }
}
