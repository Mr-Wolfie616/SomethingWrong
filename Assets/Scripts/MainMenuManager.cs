using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Elements")]
    [SerializeField] GameObject titleText;
    [SerializeField] GameObject subtitleText;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject startButtonText;
    [SerializeField] GameObject optionsButton;
    [SerializeField] GameObject optionsButtonText;
    [SerializeField] GameObject creditsButton;
    [SerializeField] GameObject creditsButtonText;
    [SerializeField] GameObject leaveGameButton;

    [SerializeField] GameObject leaveConfirmPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] GameObject creditsPanel;

    [SerializeField] GameObject mainMenuElements;
    [SerializeField] GameObject pauseMenuElements;

    [Header("Configurables")]
    [SerializeField] string sceneToLoad;
    [SerializeField] bool isPauseMenu;
    private void Start()
    {
        SetActive(true);
        leaveConfirmPanel.SetActive(false);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        if (isPauseMenu)
        {
            pauseMenuElements.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenuElements.SetActive(true);
        }
    }

    void SetActive(bool toggle)
    {
        titleText.SetActive(toggle);
        subtitleText.SetActive(toggle);
        startButton.SetActive(toggle);
        startButtonText.SetActive(toggle);
        optionsButton.SetActive(toggle);
        optionsButtonText.SetActive(toggle);
        creditsButton.SetActive(toggle);
        creditsButtonText.SetActive(toggle);
        leaveGameButton.SetActive(toggle);
    }

    public void ResumeGame()
    {
        pauseMenuElements.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Options()
    {
        optionsPanel.SetActive(true);
        mainMenuElements.SetActive(false);
    }

    public void Credits()
    {
        creditsPanel.SetActive(true);
        mainMenuElements.SetActive(false);
    }

    public void LeaveGame()
    {
        leaveConfirmPanel.SetActive(true);
    }

    public void LeaveGameYes()
    {
        Application.Quit();
    }

    public void LeaveGameNo()
    {
        leaveConfirmPanel.SetActive(false);
    }

    public void Back()
    {
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        mainMenuElements.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}