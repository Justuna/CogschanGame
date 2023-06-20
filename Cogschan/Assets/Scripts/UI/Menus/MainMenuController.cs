using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string _startSceneName;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    void Start()
    {
        _playButton.onClick.AddListener(PlayGame);
        _quitButton.onClick.AddListener(QuitGame);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(_startSceneName);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
