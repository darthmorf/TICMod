using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TICMod.Commands.Influencers
{
    class RespawnPlayer : Influencer
    {
        protected List<Player> players;

        private HashSet<string> aliases_ = new HashSet<string>() { "respawn", "respawnplayer" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length < 1 || args.Length > 2)
            {
                err = "Takes 1-2 parameters; Player Target";
                return false;
            }

            string playerParam = "";
            if (args.Length == 2)
            {
                playerParam = args[1];
            }

            bool valid = ParsePlayerTarget(args[0], playerParam, out players, out int argCount, out err);
            if (!valid)
            {
                return false;
            }
            
            return true;
        }

        public override string Execute()
        { 
            List<Player> respawnedPlayers = new List<Player>();
            foreach (var player in players)
            {
                if (player.respawnTimer > 0)
                {
                    player.respawnTimer = 0;
                    respawnedPlayers.Add(player);
                }
            }

            string playernames = GetPlayerNames(respawnedPlayers);
            return $"Respawned players {playernames}.";
        }
    }
}
