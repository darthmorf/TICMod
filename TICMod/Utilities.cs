using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;

namespace TICMod
{
    public static class Utilities
    {
        // TY Direwolf 420
        public static string GetTimeAsString(double time, bool accurate = true)
        {
            string suffix = "AM";
            if (!Main.dayTime)
            {
                time += 54000.0;
            }
            time = time / 86400.0 * 24.0;
            double val = 7.5;
            time = time - val - 12.0;
            if (time < 0.0)
            {
                time += 24.0;
            }
            if (time >= 12.0)
            {
                suffix = "PM";
            }
            int hours = (int)time;
            double doubleminutes = time - hours;
            doubleminutes = (int)(doubleminutes * 60.0);
            string minutes = string.Concat(doubleminutes);
            if (doubleminutes < 10.0)
            {
                minutes = "0" + minutes;
            }
            if (!accurate) minutes = (!(doubleminutes < 30.0)) ? "30" : "00";
            return $"{hours}:{minutes}";
        }
	}
}
