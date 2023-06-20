public class KeyDeposit : Interactable
{
    protected override void InteractInternal(EntityServiceLocator _)
    {
        GameStateSingleton.Instance.ClearLevel();
        _optInMessage = "";
    }
}