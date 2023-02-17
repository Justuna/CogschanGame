using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _hud;
    [SerializeField]
    private Healthbar _playerHealthbar;
    [SerializeField]
    private GameObject _pauseMenu;

    public static UIManager Instance { get; private set; }
    public Healthbar PlayerHealthbar { get { return _playerHealthbar; } }

    private bool _paused;

    private void Awake()
    {
        if (Instance != null && this != Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Play();
    }

    // Lock the cursor upon focusing the application.
    private void OnApplicationFocus(bool hasFocus)
    {
        if (_paused) ShowMouse();
        else HideMouse();
    }

    private void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ShowMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Pause()
    {
        _paused = true;

        Time.timeScale = 0f;
        ShowMouse();
        _pauseMenu.SetActive(true);
    }

    public void Play()
    {
        _paused = false;

        Time.timeScale = 1f;
        HideMouse();
        _pauseMenu.SetActive(false);
    }

    public void TogglePause()
    {
        if (_paused) Play();
        else Pause();
    }

    public void QuitToMainMenu()
    {
        throw new System.NotImplementedException();
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
