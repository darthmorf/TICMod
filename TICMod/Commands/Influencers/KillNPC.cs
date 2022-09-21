using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TICMod.Commands.Influencers
{
    class KillNPC : InfluencerCommand
    {
        protected List<NPC> npcs;

        private HashSet<string> aliases_ = new HashSet<string>() { "killnpc", "slay" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";

            if (args.Length < 1 || args.Length > 2)
            {
                err = "Takes 1 parameter; NPC Target";
                return false;
            }

            string playerParam = "";
            if (args.Length == 2)
            {
                playerParam = args[1];
            }

            bool valid = ParseNPCTarget(args[0], playerParam, out npcs, out int argCount, out err);
            if (!valid)
            {
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            int count = 0;
            foreach (var npc in npcs)
            {
                if (npc.life > 0)
                {
                    npc.StrikeNPCNoInteraction(npc.life + (int)(npc.defense * 0.5f) * 2, 0, 0); // Kill NPC
                    count++;
                }
            }

            return $"Killed {count} NPCs.'";
        }
    }
}
