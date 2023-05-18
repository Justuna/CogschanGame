using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtbox
{
    /// <summary>
    /// Adds an impulse to the entity possessing this hurtbox.
    /// </summary>
    /// <param name="impulse">The impulse vector.</param>
    /// <param name="cancelOverride">Whether or not the new impulse should cancel any velocity overrides.</param>
    /// <param name="maintainMomentum">How much of the previous velocity should be maintained by momentum. Only meaningful if <c>cancelOverride</c> is true.</param>
    public void AddImpulse(Vector3 impulse, bool cancelOverride, float maintainMomentum);

    /// <summary>
    /// Change the health of the entity 
    /// </summary>
    /// <param name="amount">The amount to change health by.</param>
    public void ChangeHealth(float amount);
}
