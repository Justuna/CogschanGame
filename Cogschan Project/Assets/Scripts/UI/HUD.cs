using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public Healthbar CogschanHealthbar;

    public static HUD Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && this != Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
