using UnityEngine;

public class TestInteractable : Interactable
{
    protected override void InteractInternal(EntityServiceLocator services)
    {
        Debug.Log("Greetings, " + services.name + "! It's me, your friend " + name + "!");
    }
}