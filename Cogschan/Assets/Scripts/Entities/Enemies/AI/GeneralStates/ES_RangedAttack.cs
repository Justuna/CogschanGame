using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Numerics = System.Numerics;

public class ES_RangedAttack : MonoBehaviour, IEnemyState
{
    [SerializeField]
    [Tooltip("The type of attack this enemy uses.")]
    private AttackType _attackType;
    [SerializeField]
    [Tooltip("The GameObject fired by the object.")]
    private GameObject _gameObject;
    [SerializeField]
    [Tooltip("The speed at which the projectile is fired. Ignored if the attack is a raycast.")]
    private float _launchSpeed;
    [SerializeField]
    [Tooltip("The amount of damage the player takes on hit. Ignored unless the attack is a raycast.")]
    private int _damage;
    [SerializeField]
    [Tooltip("The service locator for the enemy.")]
    private EntityServiceLocator _services;

    public CogschanSimpleEvent AttackTerminated;
    private bool _hasAttacked = false;

    public void Behavior()
    {
        if (_hasAttacked) return;
        Vector3 playerDisplacement = _services.LOSChecker.LastSeenPosition - transform.position;
        Vector3 playerDir = playerDisplacement;
        playerDir.y = 0;
        Vector3 rotationVal = _services.FlyingAI == null ? playerDir : playerDisplacement;
        float turnSpeed = _services.FlyingAI == null ? _services.GroundedAI.TurnSpeed : _services.FlyingAI.TurnSpeed;
        _services.Model.transform.rotation = Quaternion.RotateTowards(_services.Model.transform.rotation,
            Quaternion.LookRotation(rotationVal), Time.deltaTime * turnSpeed);
        if (Vector3.Angle(_services.Model.transform.forward, rotationVal) < turnSpeed * Time.deltaTime)
        {
            switch (_attackType)
            {
                case AttackType.ProjectileGravity:
                    float? angleNullable = GetAngle(new(playerDir.magnitude, playerDir.y));
                    if (angleNullable is not null)
                    {
                        float angle = angleNullable.Value;
                        Vector3 launchVel = _services.Model.transform.forward * Mathf.Cos(angle)
                            + Vector3.up * Mathf.Sin(angle);
                        launchVel *= _launchSpeed;
                        GameObject proj = Instantiate(_gameObject, transform.position + launchVel.normalized,
                            Quaternion.identity);
                        proj.GetComponent<Rigidbody>().velocity = launchVel;
                    }
                    break;
                case AttackType.RayCast:
                    print("attack!!");
                    Physics.Raycast(transform.position, _services.Model.transform.forward, out RaycastHit hitInfo, Mathf.Infinity,
                        LayerMask.GetMask("Level", "Skybox", "Player Hurtbox"));
                    print(hitInfo.collider.gameObject);
                    Hurtbox hurt = hitInfo.collider.gameObject.GetComponent<Hurtbox>();
                    if (hurt != null)
                        hurt.Services.HealthTracker.Damage(_damage);
                    break;
            }
            _hasAttacked = true;
            AttackTerminated?.Invoke();
        }
    }

    // Calculates the angle of the trajectory based on the relative position of the target.
    private float? GetAngle(Vector2 relPos)
    {
        float epsilon = 1e-3f;
        float tolerance = Mathf.PI / 180;

        float C = -Physics.gravity.y * Mathf.Pow(relPos.x, 2) /
            (2 * Mathf.Pow(_launchSpeed, 2));
        Numerics::Complex[] rootsOfTaylorApprox = PolynomialSolver.QuarticSolver(
            relPos.y / 4, relPos.x / 2, -relPos.y, -relPos.x, relPos.y + C);

        bool isNan = false;
        foreach (Numerics::Complex root in rootsOfTaylorApprox)
        {
            if (float.IsNaN((float)root.Real) || float.IsNaN((float)root.Imaginary))
            {
                isNan = true;
                break;
            }
        }
        if (isNan)
        {
            rootsOfTaylorApprox = PolynomialSolver.CubicSolver(
                relPos.x / 2, 0, -relPos.x, C);

            isNan = false;
            foreach (Numerics::Complex root in rootsOfTaylorApprox)
            {
                if (float.IsNaN((float)root.Real) || float.IsNaN((float)root.Imaginary))
                {
                    isNan = true;
                    break;
                }
            }
            if (isNan)
            {
                return Mathf.PI / 2; // If target is close enough, just fire straight up.
            }
        };

        List<float> validRoots =
            (from value in rootsOfTaylorApprox.ToList()
                where Mathf.Abs((float)value.Imaginary) <= epsilon && value.Real <= Mathf.PI && value.Real >= 0
                select (float)value.Real).ToList();

        if (validRoots.Count == 0)
            return null;

        validRoots.Sort();
        validRoots.Reverse();

        RootFinder.Root angle = RootFinder.NewtonsMethod(
            (float x) => relPos.y * Mathf.Pow(Mathf.Cos(x), 2)
            - relPos.x * Mathf.Sin(x) * Mathf.Cos(x) + C,
            validRoots[0], tolerance, epsilon);
        return angle.IsAccurate ? angle : null;
    }

    private enum AttackType
    {
        ProjectileGravity,
        RayCast
    }
}