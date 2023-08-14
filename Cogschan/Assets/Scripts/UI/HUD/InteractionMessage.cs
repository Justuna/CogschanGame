using TMPro;
using UnityEngine;

public class InteractionMessage : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private TextMeshProUGUI _textbox;
    [SerializeField] private GameObject _interactionPanel;

    public void Init(EntityServiceLocator services)
    {
        _services = services;
    }

    private void Update()
    {
        Interactable optIn = _services.InteractionChecker.OptIn;
        if (optIn != null && optIn.OptInMessage != "")
        {
            _interactionPanel.SetActive(true);
            _textbox.text = optIn.OptInMessage;
        }
        else
        {
            _interactionPanel.SetActive(false);
            _textbox.text = "";
        }
    }
}