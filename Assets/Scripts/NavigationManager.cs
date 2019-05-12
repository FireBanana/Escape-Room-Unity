using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    [Header("UI Screens")] public GameObject LoginScreen;
    public GameObject InitialScreen;
    public GameObject MainScreen;

    public GameObject MainBackground;

    private DialogManager dialogManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dialogManager = DialogManager.Instance;
    }

    public void ActivateInitalScreen()
    {
        LoginScreen.SetActive(false);
        MainScreen.SetActive(false);
        InitialScreen.SetActive(true);
    }

    public void ActivateMainScreen()
    {
        InitialScreen.SetActive(false);
        MainBackground.SetActive(true);
        MainScreen.SetActive(true);

        dialogManager.EnableDialogue("Great Job!",
            "You have successfully calibrated the functional aspects of the game.\nPlease select the QR tab on the top right and scan the code located on the door or press the button below to go back if you haven't opened the door!",
            "Oops, we didn't open the door!", true, () =>
            {
                ActivateInitalScreen();
                dialogManager.DisableDialogue();
            });
    }

    public void ActivateQr()
    {
        MainBackground.SetActive(false);
        LevelManager.Instance.SelectQr();
    }
}