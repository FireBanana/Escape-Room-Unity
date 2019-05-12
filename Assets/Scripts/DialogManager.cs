using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject DialogueHolder;
    public TextMeshProUGUI TitleText, BodyText, ButtonText;
    public Button DialogueBtn;

    private bool enableBackgroundClick;
    
    private void Awake()
    {
        Instance = this;
    }

    public void EnableDialogue(string title, string body, string buttonText, bool allowBackClick, Action buttonMethod)
    {
        DialogueHolder.SetActive(true);
        enableBackgroundClick = allowBackClick;
        
        TitleText.text = title;
        BodyText.text = body;
        ButtonText.text = buttonText;

        DialogueBtn.onClick.AddListener(() => {buttonMethod.Invoke();});
    }

    public void DisableDialogue()
    {
        DialogueBtn.onClick.RemoveAllListeners();
        DialogueHolder.SetActive(false);
    }

    public void OnBackgroundClick()
    {
        if (enableBackgroundClick)
        {
            DialogueBtn.onClick.RemoveAllListeners();
            DialogueHolder.SetActive(false);
        }
    }

    public void DialogueOnClicked()
    {
        DisableDialogue();
        //add callback?
    }
}
