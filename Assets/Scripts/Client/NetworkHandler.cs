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
using Unity_Escape_Room_Server_WPF.PacketCommands;


public class NetworkHandler : IDisposable
{
    const int PORT = 2000;

    private string ipAddress = "192.168.1.4";
    TcpClient client;

    public delegate void AuthenticationCallback();
    
    public NetworkHandler()
    {
        try
        {
            client = new TcpClient(ipAddress, PORT);
        }
        catch(Exception e)
        {
            Debug.LogError("Could not connect to host\n\n" + e.ToString());
        }
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
                    var callbackData = JsonConvert.DeserializeObject<AuthenticationResponsePacket>(Encoding.ASCII.GetString(buffer));
                    
                    callback.Invoke();
                    return;
                }
                catch(Exception e)
                {
                    Debug.LogError("Authentication Callback Error\n\n" + e.ToString());
                    break;
                }
            }
        });
    }

    public void Dispose()
    {
    }
}