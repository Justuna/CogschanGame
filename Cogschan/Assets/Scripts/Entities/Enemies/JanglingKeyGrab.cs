using UnityEngine;
using UnityEngine.VFX;

public class JanglingKeyGrab : Interactable
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private VisualEffect _grabVisualEffect;
    [SerializeField] private FXController _grabFXController;

    private KeyData _keyData;

    public void Init(KeyData keyData)
    {
        _keyData = keyData;
        _grabVisualEffect.SetVector4(Shader.PropertyToID("Color"), keyData.Color);
    }

    // Quick note: the service locator passed in is of the interacting entity (the player)
    // We don't need to operate on the player objects, so it does not get used
    protected override void InteractInternal(EntityServiceLocator _)
    {
        _grabFXController.Play();
        GameStateSingleton.Instance.CollectKey(_keyData);

        // Potentially preceded by some sort of an animation in the future
        Destroy(_services.Jangling.gameObject);
    }
}
