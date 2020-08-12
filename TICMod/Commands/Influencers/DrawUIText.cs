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
    class DrawUIText : Influencer
    {
        protected Color displayColor;
        protected Point pos;
        protected int duration;
        protected string message;

        private HashSet<string> aliases_ = new HashSet<string>() { "drawuitext" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length != 7)
            {
                err = $"Takes 7 parameters; R Value, G Value, B Value, Width %, Height %, Time & Message";
                return false;
            }

            valid = ParseColor(args[0], args[1], args[2], out displayColor, out err);
            if (!valid)
            {
                return false;
            }

            valid = ParseInt(args[3], out pos.Y, out err, 0, 100);
            if (!valid)
            {
                return false;
            }

            valid = ParseInt(args[4], out pos.Y, out err, 0, 100);
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
                mod.SendTextDisplayPacket(message, displayColor, duration, pos.Y, pos.Y, false);
            }
            else
            {
                mod.textDisplayer.AddText(message, displayColor, duration, pos.Y, pos.Y, false);
            }

            string timeoutText = (duration < 1) ? "until world restart." : $"for {duration} seconds.";
            return $"Displaying '{message}' as {displayColor.ToString()} at ({pos.Y}%, {pos.Y}%) {timeoutText}";

        }
    }
}
