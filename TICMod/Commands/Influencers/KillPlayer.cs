using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace TICMod.Commands.Influencers
{
    class KillPlayer : Influencer
    {
        protected List<Player> players;
        protected string message;

        private HashSet<string> aliases_ = new HashSet<string>() { "kill", "killplayer" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length < 2 || args.Length > 3)
            {
                err = "Takes 2-3 parameters; Player Target & Message";
                return false;
            }

            string playerParam = "";
            if (args.Length == 3)
            {
                playerParam = args[1];
            }

            bool valid = ParsePlayerTarget(args[0], playerParam, out players, out int argCount, out err);
            if (!valid)
            {
                return false;
            }

            message = args[1];
            if (args.Length == 3)
            {
                message = args[2];
            }

            return true;
        }

        public override string Execute()
        {
            foreach (var player in players)
            {
                string thisreason = message.Replace("#name", player.name);
                player.KillMe(PlayerDeathReason.ByCustomReason($"{thisreason}"), Double.MaxValue, 0, false);
            }

            string playernames = GetPlayerNames(players);
            return $"Killed players {playernames}.";
        }
    }
}
