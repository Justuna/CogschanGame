using TMPro;
using UnityEngine;

public class InteractionMessage : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private TextMeshProUGUI _textbox;

    private void Update()
    {
        Interactable optIn = _services.InteractionChecker.OptIn;
        if (optIn != null )
        {
            _textbox.enabled = true;
            _textbox.text = optIn.OptInMessage;
        }
        else
        {
            _textbox.enabled = false;
            _textbox.text = "";
        }
    }
}