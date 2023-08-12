using System.Linq;
using UnityEngine;

// TODO LATER: Make prefab for KeyDisplay
public class KeyDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _keySlotPrefab;
    [SerializeField] private RectTransform _keySlotsHolder;

    private KeySlotDisplay[] _keySlotDisplays;

    private void Start()
    {
        foreach (Transform child in _keySlotsHolder.transform)
            Destroy(child.gameObject);

        _keySlotDisplays = new KeySlotDisplay[GameStateSingleton.Instance.KeysNeeded];
        for (int i = 0; i < GameStateSingleton.Instance.Keys.Length; i++)
        {
            _keySlotDisplays[i] = Instantiate(_keySlotPrefab, _keySlotsHolder).GetComponent<KeySlotDisplay>();
            _keySlotDisplays[i].Init(GameStateSingleton.Instance.Keys[i].KeyData);
        }

        GameStateSingleton.Instance.KeyCollected.AddListener((keyData) =>
        {
            var display = _keySlotDisplays.FirstOrDefault(x => x.KeyData == keyData);
            if (display == null)
            {
                Debug.LogError("Collected Jangling could not be found! Is the Jangling registered to the GameStateSingleton?");
                return;
            }
            display.Collect();
        });
    }
}