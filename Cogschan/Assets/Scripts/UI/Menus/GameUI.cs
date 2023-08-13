using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("External Dependencies")]
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private PlayerCameraController _playerCameraController;

    [Header("Local Dependencies")]
    [SerializeField] private HealthDisplay _healthDisplay;
    [SerializeField] private WeaponDisplay _weaponDisplay;
    [SerializeField] private InteractionMessage _interactionMessage;
    [SerializeField] private Transform _targetReticle;
    [SerializeField] private PauseUI _pauseUI;

    private void Awake()
    {
        _playerCameraController.Init(_targetReticle);
        _healthDisplay.Init(_services);
        _weaponDisplay.Init(_services);
        _interactionMessage.Init(_services);
        _pauseUI.Init(_services);
    }
}
