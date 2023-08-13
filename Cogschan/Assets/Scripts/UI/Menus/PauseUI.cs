using Cysharp.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EntityServiceLocator _playerServices;
    [SerializeField] private SceneReference _mainMenuScene;

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _unpauseButton;
    [SerializeField] private Button _pauseMainMenuButton;

    [Header("Lose Menu")]
    [SerializeField] private float _waitBeforeGameOverMenuDuration = 5f;
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _loseMainMenuButton;

    [Header("Win Menu")]
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private Button _winMainMenuButton;

    private bool _paused;

    public void Init(EntityServiceLocator playerServices)
    {
        _playerServices = playerServices;
    }

    private void Start()
    {
        CogschanInputSingleton.Instance.OnPauseButtonPressed += () =>
        {
            if (_paused)
                ClosePauseMenu();
            else OpenPauseMenu();
        };
        _unpauseButton.onClick.AddListener(ClosePauseMenu);
        _playerServices.HealthTracker.OnDefeat.AddListener(OnDefeat);

        _retryButton.onClick.AddListener(Reload);
        _pauseMainMenuButton.onClick.AddListener(ReturnToMenu);
        _loseMainMenuButton.onClick.AddListener(ReturnToMenu);
        _winMainMenuButton.onClick.AddListener(ReturnToMenu);

        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(false);
        _winMenu.SetActive(false);
        Pause(false);

        GameStateSingleton.Instance.LevelClear.AddListener(OnWin);
    }

    private void Pause(bool paused = true)
    {
        _paused = paused;
        if (_paused)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OpenPauseMenu()
    {
        _pauseMenu.SetActive(true);
        Pause(true);
    }

    private void ClosePauseMenu()
    {
        _pauseMenu.SetActive(false);
        Pause(false);
    }

    private async void OnDefeat()
    {
        await UniTask.WaitForSeconds(_waitBeforeGameOverMenuDuration);
        _pauseMenu.SetActive(false);
        _gameOverMenu.SetActive(true);
        Pause(true);
    }

    private void OnWin()
    {
        _pauseMenu.SetActive(false);
        _winMenu.SetActive(true);
        Pause(true);
    }

    private void Reload()
    {
        Pause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(_mainMenuScene.BuildIndex);
    }
}
