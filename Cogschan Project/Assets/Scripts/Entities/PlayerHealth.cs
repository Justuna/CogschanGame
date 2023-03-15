using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Entity
{
    void Start()
    {
        _healthbar = UIManager.Instance.PlayerHealthbar;
        _healthbar.UpdateValue(1);
    }

    protected override void Kill()
    {
        // Ragdoll? Probably shouldnt just delete the player
        Debug.Log("Nothing implemented");
    }
}
