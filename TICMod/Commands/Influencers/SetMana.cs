using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICMod.Commands.Influencers
{
    class SetMana : SetStat
    {
        protected override string valueType { get { return "mana"; } }
        protected override int minValue { get { return 0; } }

        private HashSet<string> aliases_ = new HashSet<string>() { "setmana" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override string Execute()
        {
            foreach (var player in players)
            {
                player.statMana = value;
            }

            return base.Execute();
        }
    }
}