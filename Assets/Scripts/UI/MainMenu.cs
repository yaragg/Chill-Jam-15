using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsPopup;
    public List<Button> mainButtons;
    public Button backButton;

    public void Start()
    {
        creditsPopup.transform.position += Screen.height * Vector3.down;
        creditsPopup.SetActive(false);
    }

    public void HandleExitButton()
    {
        mainButtons.ForEach(button => button.interactable = false);
        Application.Quit();
    }

    public void HandleStartGame()
    {
        mainButtons.ForEach(button => button.interactable = false);
        SceneManager.LoadScene("Gameplay");
    }

    public void HandleCreditsButton()
    {
        creditsPopup.SetActive(true);

        backButton.interactable = true;

        creditsPopup.transform.DOMoveY(creditsPopup.transform.position.y + Screen.height, 0.4f).SetEase(Ease.InCubic).OnComplete(() => mainButtons.ForEach(button => button.interactable = false));
    }

    public void HandleBackButton()
    {
        creditsPopup.transform.DOMoveY(creditsPopup.transform.position.y - Screen.height, 0.4f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            creditsPopup.SetActive(false);
            backButton.interactable = false;
        });

        mainButtons.ForEach(button => button.interactable = true);

    }
}