using UnityEngine;

public class TestInteractable : Interactable
{
    public GameObject Prefab; 

    protected override void InteractInternal(EntityServiceLocator services)
    {
        Instantiate(Prefab, transform.position, Quaternion.identity);
    }
}