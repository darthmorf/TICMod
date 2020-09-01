using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TICMod.Commands.Influencers
{
    class SetMaxHealth : SetStat
    {
        protected override string valueType { get { return "max health"; }  }
        protected override int minValue { get { return 20; } }

        private HashSet<string> aliases_ = new HashSet<string>() { "setmaxhealth" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override string Execute()
        {
            foreach (var player in players)
            {
                player.statLifeMax = value;
            }

            return base.Execute();
        }
    }
}
