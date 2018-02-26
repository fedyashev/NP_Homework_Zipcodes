using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipcodeLib.Entity
{
    public class ZipcodeInfo
    {
        public int zipcode { get; set; }
        public List<AddressDataInfo> addressdata { get; set; }
    }
}
