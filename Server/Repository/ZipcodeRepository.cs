using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZipcodeLib.Entity;

namespace Server.Repository
{
    public class ZipcodeRepository
    {
        private static List<ZipcodeInfo> _zipcodes;

        public static List<ZipcodeInfo> GetRepository()
        {
            if (_zipcodes != null) return _zipcodes;
            try
            {
                var content = File.ReadAllText("../../Data/zipcodes.json");
                var zipcodes = JsonConvert.DeserializeObject<List<ZipcodeInfo>>(content);
                if (zipcodes != null)
                {
                    _zipcodes = zipcodes;
                    return _zipcodes;
                }
                throw new Exception("Can't parse josn.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
