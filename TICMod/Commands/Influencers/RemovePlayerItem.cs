using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace TICMod.Commands.Influencers
{
    class RemovePlayerItem : InfluencerCommand
    {
        protected int itemId;
        protected int count;
        protected List<Player> players;

        private HashSet<string> aliases_ = new HashSet<string>() { "removeitem" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length < 3 || args.Length > 4)
            {
                err = $"Takes 3-4 parameters; Item ID, Count, Player Target";
                return false;
            }

            valid = ParseInt(args[0], out itemId, out err, 0, ItemID.Count);
            if (!valid)
            {
                return false;
            }

            valid = ParseInt(args[1], out this.count, out err, 0, ItemID.Count);
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
                int removeCount = 0;
                foreach (var item in player.inventory)
                {
                    if (item.netID == itemId)
                    {
                        while (item.stack >= 0 && removeCount < count)
                        {
                            item.stack--;
                            removeCount++;
                        }

                        if (item.stack <= 0)
                        {
                            item.TurnToAir();
                        }

                        if (removeCount >= count)
                        {
                            break;
                        }
                    }
                }
            }

            return $"Successfully removed item ID {itemId} x{count} at from {GetPlayerNames(players)}.";
        }
    }
}
