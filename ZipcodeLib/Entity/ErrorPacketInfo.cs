using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipcodeLib.Entity
{
    public class ErrorPacketInfo : PacketInfo
    {
        public string Message { get; set; }

        public ErrorPacketInfo()
        {
            Type = "ERROR";
        }
    }
}
