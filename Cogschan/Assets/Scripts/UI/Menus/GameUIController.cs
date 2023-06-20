using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private Button UnpauseButton;

    private bool _paused;

    private void Start()
    {
        CogschanInputSingleton.Instance.OnPauseButtonPressed += () => { if (_paused) Unpause(); else Pause(); };
        UnpauseButton.onClick.AddListener(Unpause);
        Unpause();
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

    private void ReturnToMenu()
    {
        // Fill
    }

    private void QuitToDesktop()
    {
        // Fill
    }
}
