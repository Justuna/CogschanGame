using System.Collections;
using System.Collections.Generic;

public interface IMovementState
{
    public void Behavior();

    public void OnDash();

    public void OnJump();
}
