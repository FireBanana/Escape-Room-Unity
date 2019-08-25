using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject DialogueHolder;
    public TextMeshProUGUI TitleText, BodyText, ButtonText;
    public Button DialogueBtn;
    public Button SecondDialogueBtn;
    public TextMeshProUGUI SecondButtonText;

    private bool enableBackgroundClick;
    
    private void Awake()
    {
        Instance = this;
    }

    public void EnableDialogue(string title, string body, string buttonText, bool allowBackClick, Action buttonMethod)
    {
        if (DialogueHolder.activeInHierarchy)
        {
            print("Dialogue active, but tried to open: \n\n" + title);
            return;
        }
        
        DialogueHolder.SetActive(true);
        enableBackgroundClick = allowBackClick;
        
        TitleText.text = title;
        BodyText.text = body;
        ButtonText.text = buttonText;

        DialogueBtn.onClick.AddListener(() => {buttonMethod.Invoke();});
    }

    public void EnableDoubleDialogue(string title, string body, string buttonText, string button2Text, bool allowBackClick, Action buttonMethod1, Action buttonMethod2)
    {
        if (DialogueHolder.activeInHierarchy)
        {
            print("Dialogue active, but tried to open: \n\n" + title);
            return;
        }
        
        SecondDialogueBtn.gameObject.SetActive(true);
        
        DialogueHolder.SetActive(true);
        enableBackgroundClick = allowBackClick;
        
        TitleText.text = title;
        BodyText.text = body;
        ButtonText.text = buttonText;
        SecondButtonText.text = button2Text;

        DialogueBtn.onClick.AddListener(() => {buttonMethod1.Invoke();});
        SecondDialogueBtn.onClick.AddListener(() => {buttonMethod2.Invoke();});
    }

    public void DisableDialogue()
    {
        AudioManager.Instance.StopAudio();
        SecondDialogueBtn.onClick.RemoveAllListeners();
        SecondDialogueBtn.gameObject.SetActive(false);
        DialogueBtn.onClick.RemoveAllListeners();
        DialogueHolder.SetActive(false);
    }

    public void OnBackgroundClick()
    {
        if (enableBackgroundClick)
        {
            AudioManager.Instance.StopAudio();
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
