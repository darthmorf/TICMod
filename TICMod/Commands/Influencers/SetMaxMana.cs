using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICMod.Commands.Influencers
{
    class SetMaxMana : SetStat
    {
        protected override string valueType { get { return "max mana"; } }
        protected override int minValue { get { return 0; } }

        private HashSet<string> aliases_ = new HashSet<string>() { "setmaxmana" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override string Execute()
        {
            foreach (var player in players)
            {
                player.statManaMax = value;
            }

            return base.Execute();
        }
    }
}