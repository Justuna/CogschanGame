using UnityEngine;

/// <summary>
/// Used to find the ground under the object. Unlike <see cref="GroundChecker"/>, we aren't looking for the ground immediately under the object.
/// </summary>
public static class GroundFinder
{
    private static readonly LayerMask s_ground;
    private static readonly LayerMask s_skybox;

    static GroundFinder()
    {
        s_ground = LayerMask.GetMask("Level");
        s_skybox = LayerMask.GetMask("Skybox");
    }

    /// <summary>
    /// Returns whether or not the given <paramref name="position"/> is over the ground.
    /// </summary>
    public static bool IsOverGround(Vector3 position)
    {
        Ray ray = new(position, Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, s_ground | s_skybox);
        return 1 << hit.collider.gameObject.layer == s_ground; // GameObject.layer returns the number rather than the bitflags, so we use left-shifting to fix that.
    }

    /// <summary>
    /// Returns the y value of the ground (or skybox) under <paramref name="position"/>.
    /// </summary>
    public static float HeightOfGround(Vector3 position)
    {
        Ray ray = new(position, Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, s_ground | s_skybox);
        return position.y - hit.distance;
    }
}