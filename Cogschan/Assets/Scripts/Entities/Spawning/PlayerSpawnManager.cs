public class PlayerSpawnManager : SpawnManager
{
    protected override bool FirstSpawn()
    {
        base.FirstSpawn();
        //Destroy(this);
        return false; // Don't spawn more stuff
    }
}