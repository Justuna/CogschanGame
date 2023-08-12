using UnityEngine;
using UnityEngine.UI;

public class KeySlotDisplay : MonoBehaviour
{
    [SerializeField] private Image _keyOutlineImage;
    [SerializeField] private Image _keyFillImage;

    public KeyData KeyData { get; private set; }

    public void Init(KeyData keyData)
    {
        KeyData = keyData;
        _keyOutlineImage.color = KeyData.Color;

        Clear();
    }

    public void Clear()
    {
        var outlineColor = Color.white;
        outlineColor.a = 0.5f;
        _keyOutlineImage.color = outlineColor;
        var fillColor = KeyData.Color;
        fillColor.a = 0.5f;
        _keyFillImage.color = fillColor;
    }

    public void Collect()
    {
        _keyOutlineImage.color = Color.white;
        _keyFillImage.color = KeyData.Color;
    }
}
