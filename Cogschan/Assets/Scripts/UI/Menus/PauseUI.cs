using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EntityServiceLocator _playerServices;

    [Header("Pause Menu")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _unpauseButton;
    [SerializeField] private Button _quitToDesktopButton;

    [Header("Lose Menu")]
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _returnToMenuButton;
    [SerializeField] private int _mainMenuScene;

    private bool _paused;

    public void Init(EntityServiceLocator playerServices)
    {
        _playerServices = playerServices;
    }

    private void Start()
    {
        CogschanInputSingleton.Instance.OnPauseButtonPressed += () => { if (_paused) ClosePauseMenu(); else OpenPauseMenu(); };
        _unpauseButton.onClick.AddListener(ClosePauseMenu);
        _playerServices.HealthTracker.OnDefeat += OnDefeat;
        ClosePauseMenu();

        _retryButton.onClick.AddListener(Reload);
        _returnToMenuButton.onClick.AddListener(ReturnToMenu);
        _quitToDesktopButton.onClick.AddListener(QuitToDesktop);
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
        _pauseMenu.gameObject.SetActive(true);
        Pause(true);
    }

    private void ClosePauseMenu()
    {
        _pauseMenu.gameObject.SetActive(false);
        Pause(false);
    }

    private void OnDefeat()
    {
        _pauseMenu.gameObject.SetActive(false);
        _gameOverMenu.gameObject.SetActive(true);
        Pause(true);
    }

    private void Reload()
    {
        Pause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ReturnToMenu()
    {
        Pause(false);
        SceneManager.LoadScene(_mainMenuScene);
    }

    private void QuitToDesktop()
    {
        Application.Quit();
    }
}
