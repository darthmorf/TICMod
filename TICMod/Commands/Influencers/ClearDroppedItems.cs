using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TICMod.Commands.Influencers
{
    class ClearDroppedItems : Influencer
    {

        private HashSet<string> aliases_ = new HashSet<string>() { "cleardroppeditems" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length != 0)
            {
                err = $"Takes 0 parameters.";
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            foreach (var item in Main.item)
            {
                item.TurnToAir();
            }

            return $"Successfully cleared dropped items.";
        }
    }
}
