using UnityEngine;

// TODO LATER: Make prefab for KeyDisplay
public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _keySlotPrefab;
    [SerializeField] private RectTransform _keySlotsHolder;

    private KeySlotDisplay[] _keySlotDisplays;
    private int _nextCollectableKeyIndex;

    private void Start()
    {
        _keySlotDisplays = new KeySlotDisplay[GameStateSingleton.Instance.KeysNeeded];
        for (int i = 0; i < GameStateSingleton.Instance.KeysNeeded; i++)
        {
            _keySlotDisplays[i] = Instantiate(_keySlotPrefab).GetComponent<KeySlotDisplay>();
            _keySlotDisplays[i].Lock();
        }
        _nextCollectableKeyIndex = 0;
        GameStateSingleton.Instance.KeyCollected.AddListener(() =>
        {
            _keySlotDisplays[_nextCollectableKeyIndex].Unlock();
            _nextCollectableKeyIndex++;
        });

        foreach (Transform child in _keySlotsHolder.transform)
            Destroy(child.gameObject);
    }
}