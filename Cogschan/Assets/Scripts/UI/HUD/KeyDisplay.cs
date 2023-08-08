using TMPro;
using UnityEngine;

public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _keyDisplay;

    private void Update()
    {
        _keyDisplay.text = GameStateSingleton.Instance.KeyCount.ToString();
    }
}