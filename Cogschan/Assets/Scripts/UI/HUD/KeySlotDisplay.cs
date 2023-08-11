using UnityEngine;
using UnityEngine.UI;

public class KeySlotDisplay : MonoBehaviour
{
    [SerializeField] private Image _keyOutlineImage;
    [SerializeField] private Image _keyFillImage;
    [SerializeField] private Color _defaultKeyColor = new Color32(92, 87, 100, 255);

    public void Lock()
    {
        var color = Color.white;
        color.a = 0.5f;
        _keyOutlineImage.color = color;
        _keyFillImage.enabled = false;
    }

    public void Unlock()
        => Unlock(_defaultKeyColor);

    public void Unlock(Color color)
    {
        _keyOutlineImage.color = Color.white;
        _keyFillImage.enabled = true;
        _keyOutlineImage.color = color;
    }
}
