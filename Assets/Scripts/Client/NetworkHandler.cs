using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Threading;


public class NetworkHandler
{
    const int PORT = 2000;

    private string ipAddress = "192.168.1.17";
    TcpClient client;

    private CancellationTokenSource PacketListenerCancellationSource = new CancellationTokenSource();
    private Task networkTask;
    private bool isDebug;

    public delegate void AuthenticationCallback();

    public NetworkHandler(bool debug)
    {
        isDebug = debug;

        if (isDebug)
            return;

        try
        {
            client = new TcpClient(ipAddress, PORT);
            StartPacketListener(client);
        }
        catch (Exception e)
        {
            Debug.LogError("Could not connect to host\n\n" + e.ToString());
        }
    }

    public void StartPacketListener(TcpClient client)
    {
        networkTask = Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     if (PacketListenerCancellationSource.Token.IsCancellationRequested)
                     {
                         Debug.Log("Cancelling");
                         break;
                     }


                     if (client.Connected)
                     {
                         try
                         {
                             if (!client.GetStream().DataAvailable)
                                 continue;

                             var buffer = new byte[client.ReceiveBufferSize];

                             client.GetStream().Read(buffer, 0, buffer.Length);
                             try
                             {
                                 var packet = JsonConvert.DeserializeObject<Packet>(Encoding.ASCII.GetString(buffer));

                                 switch (packet.PacketId)
                                 {
                                     case "pauseGame":
                                         var pausePacket =
                                             JsonConvert.DeserializeObject<PauseGamePacket>(
                                                 Encoding.ASCII.GetString(buffer));
                                         MainGameManager.Instance.AddToCallbackQueue(() =>
                                         {
                                             MainGameManager.Instance.ToggleGame(pausePacket.IsPaused);
                                         });
                                         break;
                                     case "hintResponse":
                                         var hintResponsePacket =
                                             JsonConvert.DeserializeObject<HintResponsePacket>(
                                                 Encoding.ASCII.GetString(buffer));
                                         MainGameManager.Instance.AddToCallbackQueue(() =>
                                         {
                                             DialogManager.Instance.EnableDialogue("Hint Received!",
                                                 hintResponsePacket.Hint, "OK", true,
                                                 DialogManager.Instance.DisableDialogue);
                                         });

                                         break;

                                     case "gameEndRequest":
                                         var gameEndRequestPacket = JsonConvert.DeserializeObject<GameEndRequestPacket>(
                                                 Encoding.ASCII.GetString(buffer));
                                         MainGameManager.Instance.AddToCallbackQueue(() =>
                                         {
                                             MainGameManager.Instance.Score = gameEndRequestPacket.Score;
                                             AudioManager.Instance.PlayAudio(12);
                                             NavigationManager.Instance.ActivateGameEndScreen();
                                             DialogManager.Instance.DisableDialogue();
                                         }
                                         );
                                         break;
                                 }
                             }
                             catch (Exception e)
                             {
                                 Debug.LogError(e.ToString());
                                 break;
                             }
                         }
                         catch (Exception e)
                         {
                             Debug.LogError(e.ToString());
                             break;
                         }
                     }
                     else
                     {
                         Debug.Log("Not Connected");
                     }
                 }

                 Debug.Log("ended");
                 client.GetStream().Close();
                 client.Close();

             }, PacketListenerCancellationSource.Token, TaskCreationOptions.None,
             TaskScheduler.Default
         );
    }

    public void SendAuthentication(string teamName, AuthenticationCallback callback)
    {
        if (isDebug)
            return;

        var packet = new AuthenticationPacket(teamName);

        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);

        Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    var buffer = new byte[client.ReceiveBufferSize];
                    client.GetStream().Read(buffer, 0, buffer.Length);
                    var callbackData =
                        JsonConvert.DeserializeObject<AuthenticationResponsePacket>(Encoding.ASCII.GetString(buffer));

                    callback.Invoke();
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError("Authentication Callback Error\n\n" + e.ToString());
                    break;
                }
            }
        });
    }

    public void SendHintRequest(string teamName)
    {
        if (isDebug)
            return;

        var packet = new HintRequestPacket(teamName);

        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    public void SendHelpRequest(string teamName)
    {
        if (isDebug)
            return;

        var packet = new HelpRequestPacket(teamName);

        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    public void SendPointsUpdate(string teamName, int points, bool isHidden)
    {
        if (isDebug)
            return;

        var packet = new PointsUpdatePacket(teamName, points, isHidden);

        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    public void SendGameEnd(string teamName, string finalChoice, int finalTime, int finalScore)
    {
        if (isDebug)
            return;

        var packet = new GameEndPacket(teamName, finalChoice, finalTime, finalScore);

        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    public void SendDisconnect(string teamName)
    {
        var packet = new GameQuitPacket(teamName);
        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    public void SendTimerHeartBeat(string teamName, int time)
    {
        var packet = new ClientTimePacket(teamName, time);
        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    void SendData(byte[] data)
    {

    }

    public void Dispose()
    {
        SendDisconnect(MainGameManager.Instance.TeamName);
        PacketListenerCancellationSource.Cancel();
    }
}