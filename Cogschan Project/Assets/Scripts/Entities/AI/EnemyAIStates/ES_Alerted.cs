using System;
using UnityEngine;

public class ES_Alerted : MonoBehaviour, EnemyState
{
    public float AlertTime;
    public GameObject Model;
    public Transform Player;
    public LOSCalculator LOS;

    public event Action LOSBroken, ReadyToChase;

    private float _alertTimer = 0;

    public void Behavior()
    {
        if (!LOS.CanSee)
        {
            LOSBroken?.Invoke();
            return;
        }

        Model.transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
        Model.transform.Rotate(new Vector3(0, 180, 0));

        _alertTimer -= Time.deltaTime;

        if (_alertTimer <= 0)
        {
            ReadyToChase?.Invoke();
        }
    }

    public void ResetTimer()
    {
        _alertTimer = AlertTime;
    }
}
