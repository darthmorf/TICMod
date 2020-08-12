using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TICMod.Commands.Influencers
{
    class DrawWorldText : Influencer
    {
        protected Color displayColor;
        protected Point pos;
        protected int duration;
        protected string message;

        private HashSet<string> aliases_ = new HashSet<string>() { "drawworldtext" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length != 7)
            {
                err = $"Takes 7 parameters; R value, G value, B value, X Co-ordinate, Y Co-ordinate, Duration & Message";
                return false;
            }

            valid = ParseColor(args[0], args[1], args[2], out displayColor, out err);
            if (!valid)
            {
                return false;
            }

            valid = ParseCoord(args[3], args[4], out pos, out err, true);
            if (!valid)
            {
                return false;
            }
            
            valid = ParseInt(args[5], out duration, out err, 0);
            if (!valid)
            {
                return false;
            }

            message = args[6];

            return true;
        }

        public override string Execute()
        {
            if (Main.dedServ)
            {
                mod.SendTextDisplayPacket(message, displayColor, duration, pos.X, pos.Y, true);
            }
            else
            {
                mod.textDisplayer.AddText(message, displayColor, duration, pos.X, pos.Y, true);
            }

            string timeoutText = (duration < 1) ? "until world restart." : $"for {duration} seconds.";
            return $"Displaying '{message}' as {displayColor.ToString()} at ({pos.X/16}, {pos.Y/16}) {timeoutText}";
        }
    }
}
