using UnityEngine;
using TMPro;

public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _keyDisplay;

    private void Update()
    {
        _keyDisplay.text = "" + GameStateSingleton.Instance.KeyCount;
    }
}