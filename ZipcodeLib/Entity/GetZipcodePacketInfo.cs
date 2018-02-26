using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipcodeLib.Entity
{
    public class GetZipcodePacketInfo : PacketInfo
    {
        public int zipcode { get; set; }

        public GetZipcodePacketInfo()
        {
            Type = "GET_ZIPCODE";
        }
    }
}
