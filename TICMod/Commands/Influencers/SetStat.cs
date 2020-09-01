using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TICMod.Commands.Influencers
{
    class SetStat : Influencer
    {
        protected int value;
        protected List<Player> players;

        protected virtual string valueType { get;}
        protected virtual int minValue { get; }

        protected override HashSet<string> aliases { get; }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length != 2)
            {
                err = $"Takes 2 parameters; {valueType} value, player";
                return false;
            }

            valid = ParseInt(args[0], out value, out err, minValue);
            if (!valid)
            {
                return false;
            }

            string param = "";
            if (args.Length == 3)
            {
                param = args[2];
            }

            valid = ParsePlayerTarget(args[1], param, out players, out int argCount, out err);
            if (!valid)
            {
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            return $"Successfully set {valueType} of {GetPlayerNames(players)} to {value}.";
        }
    }
}
