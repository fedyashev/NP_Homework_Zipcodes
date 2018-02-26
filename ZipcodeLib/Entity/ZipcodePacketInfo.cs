using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipcodeLib.Entity
{
    public class ZipcodePacketInfo : PacketInfo
    {
        public ZipcodeInfo Zipcode { get; set; }

        public ZipcodePacketInfo()
        {
            Type = "ZIPCODE";
        }
    }
}
