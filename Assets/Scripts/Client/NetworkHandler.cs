﻿using System;
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
using EscapeRoomServer.PacketCommands;


public class NetworkHandler : IDisposable
{
    const int PORT = 2000;

    private string ipAddress = "192.168.1.4";
    TcpClient client;

    private CancellationTokenSource PacketListenerCancellationSource = new CancellationTokenSource();

    public delegate void AuthenticationCallback();

    public NetworkHandler()
    {
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
        var task = Task.Factory.StartNew(() =>
            {
                PacketListenerCancellationSource.Token.ThrowIfCancellationRequested();
                while (true)
                {
                    if (PacketListenerCancellationSource.Token.IsCancellationRequested)
                    {
                        Debug.Log("break requested");
                        break;
                    }
                    
                    if (client.Connected)
                    {
                        try
                        {
                            var buffer = new byte[client.ReceiveBufferSize];
                            
                            if(!client.GetStream().DataAvailable)
                                continue;
                            
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
                                                hintResponsePacket.Hint, "OK", null);
                                        });

                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                File.WriteAllText(@"C:\Users\Owais\Desktop\log.txt", e.ToString());
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            File.WriteAllText(@"C:\Users\Owais\Desktop\log.txt", e.ToString());
                            break;
                        }
                    }
                }
                Debug.Log("ended");
            }, PacketListenerCancellationSource.Token
        );
    }

    public void SendAuthentication(string teamName, AuthenticationCallback callback)
    {
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

    public void SendPointsUpdate(string teamName, int points)
    {
        var packet = new PointsUpdatePacket(teamName, points);

        var serializedPacket = JsonConvert.SerializeObject(packet);
        var buff = Encoding.ASCII.GetBytes(serializedPacket);
        client.GetStream().Write(buff, 0, buff.Length);
    }

    public void Dispose()
    {
        PacketListenerCancellationSource.Cancel();
    }
}