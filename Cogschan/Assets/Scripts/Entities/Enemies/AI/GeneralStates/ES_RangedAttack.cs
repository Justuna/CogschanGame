using TMPro;
using UnityEngine;

public class ES_RangedAttack : MonoBehaviour, IEnemyState
{
    [SerializeField]
    [Tooltip("The projectile fired by the object.")]
    private GameObject _projectilePrefab;
    /*[SerializeField]
    [Tooltip("The speed at which the projectile is fired.")]
    private float _launchSpeed;*/
    [SerializeField]
    [Tooltip("The service locator for the enemy.")]
    private EntityServiceLocator _services;

    public CogschanSimpleEvent AttackTerminated;

    [SerializeField]
    private Vector2 _launchVector;

    public void Behavior()
    {
        Vector3 playerDir = (_services.LOSChecker.LastSeenPosition - transform.position).normalized;
        playerDir.y = 0;
        _services.Model.transform.rotation = Quaternion.RotateTowards(_services.Model.transform.rotation,
            Quaternion.LookRotation(playerDir), Time.deltaTime * _services.GroundedAI.TurnSpeed);
        if (Vector3.Angle(_services.Model.transform.forward, playerDir) < _services.GroundedAI.TurnSpeed * Time.deltaTime)
        {
            Vector3 launchVel;
            launchVel = _services.Model.transform.forward * _launchVector.x + Vector3.up * _launchVector.y;
            GameObject proj = Instantiate(_projectilePrefab, transform.position + launchVel * .125f, Quaternion.identity);
            proj.GetComponent<Rigidbody>().velocity = launchVel;
        }
        AttackTerminated?.Invoke();
    }
}