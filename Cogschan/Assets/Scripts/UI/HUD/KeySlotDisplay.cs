using UnityEngine;

public class KeySlotDisplay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _keyEmptySprite;
    [SerializeField] private Sprite _keySprite;

    public void Lock()
    {
        _spriteRenderer.sprite = _keyEmptySprite;
    }

    public void Unlock()
    {
        _spriteRenderer.sprite = _keySprite;
    }
}
