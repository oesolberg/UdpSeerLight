using System;
using Newtonsoft.Json;

namespace HSPI_UdpSeerLight
{
    public class JsonSplitter
    {
        public JsonSplitter() { }

        public FanData SplitJsonString(string jsonString)
        {
            if (!string.IsNullOrEmpty(jsonString))
            {
                try
                {
                    var fanData = JsonConvert.DeserializeObject<FanData>(jsonString);
                    return fanData;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    //throw;
                }
            }
            return null;
        }
    }
}