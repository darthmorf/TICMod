using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace TICMod.Commands.Influencers
{
    class TeleportPlayer : Influencer
    {
        protected Point destination;
        protected List<Player> players;


        private HashSet<string> aliases_ = new HashSet<string>() { "teleport", "tp" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;


            if (args.Length < 3 || args.Length > 4)
            {
                err = $"Takes 3-4 parameters; X Co-ordinate, Y Co-ordinate, Player Target";
                return false;
            }

            valid = ParseCoord(args[0], args[1], out destination, out err, true);

            if (!valid)
            {
                return false;
            }

            string param = "";
            if (args.Length == 4)
            {
                param = args[3];
            }

            valid = ParsePlayerTarget(args[2], param, out players, out int argCount, out err);
            if (!valid)
            {
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            foreach (var player in players)
            {
                player.position = destination.ToVector2();
            }

            return $"Successfully teleported {GetPlayerNames(players)} to ({destination.X / 16}, {destination.Y / 16}).";
        }
    }
}
