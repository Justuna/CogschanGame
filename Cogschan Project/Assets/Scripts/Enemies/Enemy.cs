using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public Healthbar Healthbar;

    void Start()
    {
        _healthbar = Healthbar;
        _healthbar.UpdateValue(1);
    }
}
