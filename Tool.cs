using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodcraft_Re
{
    public static class Tool
    {
        public static void LogInfo(this object message)
        {
            Plugin.Log.LogInfo(message);
        }

    }
}
