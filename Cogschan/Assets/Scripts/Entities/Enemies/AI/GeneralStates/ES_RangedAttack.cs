using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Numerics = System.Numerics;

public class ES_RangedAttack : MonoBehaviour, IEnemyState
{
    [SerializeField]
    [Tooltip("The projectile fired by the object.")]
    private GameObject _projectilePrefab;
    [SerializeField]
    [Tooltip("The speed at which the projectile is fired.")]
    private float _launchSpeed;
    [SerializeField]
    [Tooltip("The service locator for the enemy.")]
    private EntityServiceLocator _services;

    public CogschanSimpleEvent AttackTerminated;

    public void Behavior()
    {
        Vector3 playerDisplacement = _services.LOSChecker.LastSeenPosition - transform.position;
        Vector3 playerDir = playerDisplacement;
        playerDir.y = 0;
        _services.Model.transform.rotation = Quaternion.RotateTowards(_services.Model.transform.rotation,
            Quaternion.LookRotation(playerDir), Time.deltaTime * _services.GroundedAI.TurnSpeed);
        if (Vector3.Angle(_services.Model.transform.forward, playerDir) < _services.GroundedAI.TurnSpeed * Time.deltaTime)
        {
            float? angleNullable = GetAngle(new(playerDir.magnitude, playerDir.y));
            if (angleNullable is not null)
            {
                float angle = angleNullable.Value;
                Vector3 launchVel = _services.Model.transform.forward * Mathf.Cos(angle)
                    + Vector3.up * Mathf.Sin(angle);
                launchVel *= _launchSpeed;
                GameObject proj = Instantiate(_projectilePrefab, transform.position + launchVel.normalized,
                    Quaternion.identity);
                proj.GetComponent<Rigidbody>().velocity = launchVel;
            }
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
}