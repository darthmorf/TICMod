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
        private string message = "";
        private Color displayColor = Color.White;

        private HashSet<string> aliases_ = new HashSet<string>() { "say" };
        protected override HashSet<string> aliases
        {
            get
            {
                return aliases_;
            }
        }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length != 4)
            {
                err = $"Takes 4 parameters; R value, G value, B value & string to print. ";
                return false;
            }

            int[] colors = new int[3];
            for (int i = 0; i < 3; i++)
            {
                bool valid = ParseInt(args[i], out colors[i], out err, 0, 255);

                if (!valid)
                {
                    return false;
                }
            }

            displayColor = new Color(colors[0], colors[1], colors[2]);
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
