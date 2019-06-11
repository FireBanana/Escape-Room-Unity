using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QrManager : MonoBehaviour
{
    public static QrManager Instance;
    public QRCodeDecodeController QrDecoder;

    private Dictionary<string, int> qrDictionary = new Dictionary<string, int>();
    Dictionary<string, bool> qrScannedDictionary = new Dictionary<string, bool>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        QrDecoder.onQRScanFinished += QrScanned;
        InstantiateQrData();
        PopulateScannedDictionary();
    }


    private void QrScanned(string value)
    {
        var formattedString = value.Split('/');
        try
        {
            if (!qrScannedDictionary["Entrance"] && formattedString[0] != "Entrance")
            {
                DialogManager.Instance.EnableDialogue("Thats the incorrect Qr Code", "You need to play the correct tone on the keyboard to open the door.", "OK", false,
                    () =>
                    {
                        LevelManager.Instance.SelectLevel(-2);
                        NavigationManager.Instance.ActivateInitalScreen();
                        DialogManager.Instance.DisableDialogue();
                    });
                return;
            }
            
            if (formattedString[0] == "Entrance" && !qrScannedDictionary["Entrance"])
            {
                LevelManager.Instance.SelectLevel(-2);
                DialogManager.Instance.EnableDialogue("Excellent!", "You just scored 100 points for scanning your first QR code!\nAnytime you find a QR, click on the \"Scan Qr\" tab again to adjust your score. Now click the level 1 tab to begin collecting data.", "OK", false,
                    () =>
                    {
                        qrScannedDictionary["Entrance"] = true;
                        LevelManager.Instance.UnlockLevel(0);
                        MainGameManager.Instance.UpdatePoints(qrDictionary["Entrance"]);
                        DialogManager.Instance.DisableDialogue();
                    });
                
                return;
            }
            
            if (formattedString[0] == "Exit")
            {
                MainGameManager.Instance.UpdatePoints(qrDictionary["Exit"]);
                LevelManager.Instance.SelectLevel(-2);
                DialogManager.Instance.DisableDialogue();
                qrScannedDictionary["Exit"] = true;
                
                if (!AnswerManager.Instance.FinalQuestionAnswered)
                {
                    DialogManager.Instance.DisableDialogue();
                    DialogManager.Instance.EnableDialogue("Exit Code Success",
                        "You are awarded points for solving the exit code. Do you wish to exit now and end the simulation or do you wish to continue collecting data to add to your score? Click outside this dialog to continue or click the end button to end simulation.\nNOTE: Each minute your remain in the simulation costs 25 points from your score. But you can gain many more points by continuing!",
                        "End", true, () =>
                        {
                            NavigationManager.Instance.ActivateGameEndScreen();
                            print("GAME END");
                            DialogManager.Instance.DisableDialogue();
                        });
                }
                else
                {
                    DialogManager.Instance.EnableDialogue("Simulation Terminated",
                        "You have made your decision and succesafully unlocked the escape door. Please exit the simulation arena and await final instructions.",
                        "End", false, () =>
                        {
                            NavigationManager.Instance.ActivateGameEndScreen();
                            print("GAME END");
                            DialogManager.Instance.DisableDialogue();
                        });
                }
                
                return;
            }
            
            var pair = CheckScan(formattedString[0]);
            print(formattedString[0] + " " + pair.Key + " " + pair.Value);

            if (!pair.Key)
            {
                //First time scanning
                if (pair.Value > 0)
                    DialogManager.Instance.EnableDialogue("Scan Success!", "You have been awarded points", "OK", false,
                        () =>
                        {
                            MainGameManager.Instance.UpdatePoints(pair.Value);
                            LevelManager.Instance.SelectLevel(-2);
                            DialogManager.Instance.DisableDialogue();
                        });
                else
                    DialogManager.Instance.EnableDialogue("Code Scanned", "You have scanned a QR code", "OK", false,
                        () =>
                        {
                            MainGameManager.Instance.UpdatePoints(pair.Value);
                            LevelManager.Instance.SelectLevel(-2);
                            DialogManager.Instance.DisableDialogue();
                        });
            }
            else
            {
                //Did not exist
                if (pair.Value == -69)
                {
                }
                else
                {
                    DialogManager.Instance.EnableDialogue("Code Already Scanned",
                        "You have already scanned this QR code", "OK", false, () =>
                        {
                            LevelManager.Instance.SelectLevel(-2);
                            DialogManager.Instance.DisableDialogue();
                        });
                }
            }
        }
        catch (Exception e)
        {
            print("Error: \n" + e.StackTrace + "\n" + e.ToString());
        }
    }

    KeyValuePair<bool, int> CheckScan(string value)
    {
        if (qrDictionary.ContainsKey(value))
        {
            if (!qrScannedDictionary[value])
            {
                qrScannedDictionary[value] = true;
                return new KeyValuePair<bool, int>(false, qrDictionary[value]);
            }
        }
        else
        {
            return new KeyValuePair<bool, int>(true, -69);
        }

        return new KeyValuePair<bool, int>(true, qrDictionary[value]);
    }

    public bool IsExitCodeScanned()
    {
        return qrScannedDictionary["Exit"];
    }

    void InstantiateQrData()
    {
        qrDictionary.Add("bees", 100);
        qrDictionary.Add("conditioning.01", 100);
        qrDictionary.Add("conditioning.02", 100);
        qrDictionary.Add("conditioning.03", 100);
        qrDictionary.Add("Entrance", 300);
        qrDictionary.Add("Exit", 1000);
        qrDictionary.Add("filing.cabinet", 100);
        qrDictionary.Add("gijoe", 100);
        qrDictionary.Add("helpus", 100);
        qrDictionary.Add("key.lockbox", 100);
        qrDictionary.Add("locker.1492", 100);
        qrDictionary.Add("locker.1493", 100);
        qrDictionary.Add("locker.1494", 100);
        qrDictionary.Add("locker.1495", 100);
        qrDictionary.Add("pac-man", 100);
        qrDictionary.Add("shock.01", -100);
        qrDictionary.Add("shock.02", -200);
        qrDictionary.Add("shock.03", -400);
        qrDictionary.Add("society", 500);
        qrDictionary.Add("suitcase", 100);
        qrDictionary.Add("tear.gas", 100);
        qrDictionary.Add("test.fifth.wall", 65534);
        qrDictionary.Add("tigers.01", 50);
        qrDictionary.Add("tigers.02", 50);
        qrDictionary.Add("tigers.03", 50);
        qrDictionary.Add("tigers.04", 50);
        qrDictionary.Add("tigers.05", 50);
        qrDictionary.Add("tigers.06", 50);
        qrDictionary.Add("tigers.07", 50);
        qrDictionary.Add("tigers.08", 50);
        qrDictionary.Add("tigers.09", 50);
        qrDictionary.Add("tigers.10", 50);
        qrDictionary.Add("tigers.11", 50);
        qrDictionary.Add("tigers.12", 50);
        qrDictionary.Add("tigers.13", 50);
        qrDictionary.Add("tigers.14", 50);
        qrDictionary.Add("tigers.15", 50);
        qrDictionary.Add("tigers.16", 50);
        qrDictionary.Add("tigers.17", 50);
        qrDictionary.Add("tigers.18", 50);
        qrDictionary.Add("tigers.19", 50);
        qrDictionary.Add("tigers.20", 50);
        qrDictionary.Add("understairs", 100);
        qrDictionary.Add("upstairs", 100);
        qrDictionary.Add("upstairs.door", 100);
        qrDictionary.Add("videotape", 100);
        qrDictionary.Add("viewmaster", 100);
        qrDictionary.Add("wallpaper", 2000);
    }

    void PopulateScannedDictionary()
    {
        foreach (var item in qrDictionary)
        {
            qrScannedDictionary.Add(item.Key, false);
        }
    }
}