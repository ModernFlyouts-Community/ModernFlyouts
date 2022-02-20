using ModernFlyouts.Standard.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernFlyouts.Standard.Helpers
{
    class JsonHelper
    {
        public async Task<Settings> DeserializeSettings(string JSON)
        {
            return await Task.Run<Settings>(() =>
            {
                return JsonConvert.DeserializeObject<Settings>(JSON);
            });
        }
        public async Task<string> SerializeSettings(Settings JSONObj)
        {
            return await Task.Run<string>(() =>
            {
                return JsonConvert.SerializeObject(JSONObj);
            });
        }
    }
}
