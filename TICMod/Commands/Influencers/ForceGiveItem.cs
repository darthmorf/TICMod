using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICMod.Commands.Influencers
{
    class ForceGiveItem : GiveItem
    {
        private HashSet<string> aliases_ = new HashSet<string>() { "forcegiveitem", "forcegive", "forcespawnitem" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override string Execute()
        {
            foreach (var player in players)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    //player.PutItemInInventory(itemId);
                    player.PutItemInInventoryFromItemUsage(itemId);
                }
            }

            string playernames = GetPlayerNames(players);


            return $"Force gave item id {itemId} x{itemCount} to {playernames}.";
        }
    }
}
