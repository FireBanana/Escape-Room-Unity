using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HelpRequestPacket : Packet
{
    public HelpRequestPacket(string teamName)
    {
        PacketId = "helpRequest";
        TeamName = teamName;
    }
}