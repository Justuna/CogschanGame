using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler
{
    [field: SerializeField]
    public UnityEvent OnClick { get; private set; }

    [SerializeField] private Button _button;
    [SerializeField] private EventReference _selectEvent;
    [SerializeField] private EventReference _confirmEvent;

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        AudioSingleton.Instance.PlayOneShot(_confirmEvent);
        OnClick.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioSingleton.Instance.PlayOneShot(_selectEvent);
    }
}
