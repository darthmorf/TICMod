using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace TICMod.Commands.Influencers
{
    class GiveItem : Influencer
    {
        protected List<Player> players = new List<Player>();
        protected int itemId;
        protected int itemCount;

        private HashSet<string> aliases_ = new HashSet<string>() { "giveitem", "give" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length < 3 || args.Length > 4)
            {
                err = "Takes 3-4 parameters; Item ID, Item Count & Player target";
                return false;
            }

            valid = ParseInt(args[0], out itemId, out err, 1, ItemID.Count);
            if (!valid) // TODO: Allow for negative item IDs (wait for 1.4)
            {
                return false;
            }

            valid = ParseInt(args[1], out itemCount, out err, 1);
            if (!valid)
            {
                return false;
            }

            string playerParam = "";
            if (args.Length == 4)
            {
                playerParam = args[3];
            }

            valid = ParsePlayerTarget(args[2], playerParam, out players, out int argCount, out err);
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
                player.QuickSpawnItem(null, itemId, itemCount); // TODO: Look into creating a custom entity source for Influencer Blocks
            }

            string playernames = GetPlayerNames(players);

            
            return $"Gave item id {itemId} x{itemCount} to {playernames}.";
        }
    }
}
