using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private Button UnpauseButton;

    [Header("Lose Menu")]
    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private EntityServiceLocator _playerServices;
    [SerializeField] private Button _retryButton;
    [SerializeField] private Button _returnToMenuButton;
    [SerializeField] private int _mainMenuScene;
    [SerializeField] private Button _quitToDesktopButton;

    private bool _paused;

    private void Start()
    {
        CogschanInputSingleton.Instance.OnPauseButtonPressed += () => { if (_paused) Unpause(); else Pause(); };
        _playerServices.HealthTracker.OnDefeat += () => Die();
        UnpauseButton.onClick.AddListener(Unpause);
        Unpause();

        _retryButton.onClick.AddListener(Reload);
        _returnToMenuButton.onClick.AddListener(ReturnToMenu);
        _quitToDesktopButton.onClick.AddListener(QuitToDesktop);
    }

    private void Pause()
    {
        PauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _paused = true;
    }

    private void Unpause()
    {
        PauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _paused = false;
    }

    private void Die()
    {
        _gameOverMenu.gameObject.SetActive(true);
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _paused = true;
    }
    
    private void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _paused = false;
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene(_mainMenuScene);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _paused = false;
    }

    private void QuitToDesktop()
    {
        Application.Quit();
    }
}
