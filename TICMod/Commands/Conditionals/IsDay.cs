using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IL.Terraria;
using Main = Terraria.Main;

namespace TICMod.Commands.Conditionals
{
    class IsDay : Conditional
    {
        private HashSet<string> aliases_ = new HashSet<string>() { "day", "isday" };
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

        public override string Execute(out bool conditionMet)
        {
            conditionMet = Main.dayTime;
            return $"Day: {conditionMet}";
        }
    }
}
