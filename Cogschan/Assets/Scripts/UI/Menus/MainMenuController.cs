using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SceneReference _startScene;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    void Start()
    {
        _playButton.onClick.AddListener(PlayGame);
        _quitButton.onClick.AddListener(QuitGame);
    }

    private void PlayGame()
    {
        SceneManager.LoadScene(_startScene.BuildIndex);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
