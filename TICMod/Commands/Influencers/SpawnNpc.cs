using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace TICMod.Commands.Influencers
{
    class SpawnNpc : Influencer
    {
        protected int[] pos = new int[2];
        protected int npcID;
        protected string storeName;

        private HashSet<string> aliases_ = new HashSet<string>() { "spawnnpc" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length < 3 || args.Length > 4)
            {
                err = $"Takes 3-4 parameters; X Co-ordinate, Y Co-ordinate, NPC ID & Datastore (optional)";
                return false;
            }

            valid = ParseCoord(args[0], args[1], out pos, out err);
            if (!valid)
            {
                return false;
            }

            valid = ParseInt(args[2], out npcID, out err, -65, NPCID.Count);

            if (!valid || npcID == 0)
            {
                err += " and not 0";
                return false;
            }

            if (args.Length == 4)
            {
                storeName = args[3];
            }
            
            return true;
        }

        public override string Execute()
        {
            NPC npc = new NPC();
            npc.SetDefaults(npcID);
            int index = NPC.NewNPC(pos[0], pos[1], npc.type);
            Main.npc[index].SetDefaults(npc.netID);

            if (storeName != null)
            {
                mod.npcDataStore.AddItem(storeName, Main.npc[index]);
            }

            return $"Successfully spawned {npc.GivenOrTypeName}, ID:{npc.netID} @ {pos[0] / 16},{pos[1] / 16}.";
        }
    }
}
