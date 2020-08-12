using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TICMod.Commands.Influencers
{
    class Say : Influencer
    {
        protected string message = "";
        protected Color displayColor = Color.White;

        private HashSet<string> aliases_ = new HashSet<string>() { "say" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length != 4)
            {
                err = $"Takes 4 parameters; R value, G value, B value & string to print. ";
                return false;
            }

            bool valid = ParseColor(args[0], args[1], args[2], out displayColor, out err);
            if (!valid)
            {
                return false;
            }

            message = args[3];

            return true;
        }

        public override string Execute()
        {
            message.Split(new String[] { "\\n" }, StringSplitOptions.None).ToList().ForEach(line => Utils.ChatOutput(line, displayColor));
            return $"Displaying '{message}' as color {displayColor.ToString()}.";
        }
    }
}
