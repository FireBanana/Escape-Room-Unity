using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QrManager : MonoBehaviour
{
    public static QrManager Instance;
    public QRCodeDecodeController QrDecoder;
    public Action<string> QrCallback;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        QrDecoder.onQRScanFinished += QrScanned;
    }
    
    //Set a callback before scanning QR

    private void QrScanned(string value)
    {
        if (QrCallback == null)
        {
            print("No callback defined for QR");
        }
        else
        {
            QrCallback.Invoke(value);
            QrCallback = null;
        }
    }

    
}