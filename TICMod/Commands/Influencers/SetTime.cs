using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Player = On.Terraria.Player;

namespace TICMod.Commands.Influencers
{
    class SetTime : InfluencerCommand
    {
        protected List<uint> time;

        private HashSet<string> aliases_ = new HashSet<string>() { "settime", "timeset" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length != 1)
            {
                err = $"Takes 1 parameter; Time to set it to in the 24 hour format hh:mm";
                return false;
            }

            bool valid = ParseTime(args[0], out time, out err);
            if (!valid)
            {
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            Main.time = Utils.TimeToMainTime(time[0], time[1], out Main.dayTime);

            return $"Set time to {time[0]}:{time[1].ToString().PadLeft(2, '0')}.";
        }
    }
}
