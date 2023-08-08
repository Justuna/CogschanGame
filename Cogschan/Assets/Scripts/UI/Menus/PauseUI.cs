using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private Button _unpauseButton;
    [SerializeField] private Button _quitButton;

    private bool _paused;

    private void Start()
    {
        CogschanInputSingleton.Instance.OnPauseButtonPressed += () => { if (_paused) Unpause(); else Pause(); };
        _unpauseButton.onClick.AddListener(Unpause);
        _quitButton.onClick.AddListener(Quit);
        Unpause();
    }

    private void Pause()
    {
        _pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _paused = true;
    }

    private void Unpause()
    {
        _pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _paused = false;
    }

    private void Quit()
    {
        Application.Quit();
    }
}
