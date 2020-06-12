using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TICMod
{
    public class PlayerDataStore : Dictionary<string, Player>
    {
        public void AddItem(string storename, Player item)
        {
            if (storename == null)
            {
                return;
            }

            if (!ContainsKey(storename))
            {
                Add(storename, null);
            }
            
            this[storename] = item;
        }

        public void RemoveItem(string storename)
        {
            if (storename == null)
            {
                return;
            }

            Remove(storename);
        }

        public Player GetItem(string storename)
        {
            Player player = null;
            if (this.ContainsKey(storename))
            {
                player = this[storename];
            }

            return player;
        }
    }

    public class NPCDataStore : Dictionary<string, NPC>
    {
        public void AddItem(string storename, NPC item)
        {
            if (storename == null)
            {
                return;
            }

            if (!ContainsKey(storename))
            {
                Add(storename, null);
            }

            this[storename] = item;
        }

        public void RemoveItem(string storename)
        {
            if (storename == null)
            {
                return;
            }

            Remove(storename);
        }

        public NPC GetItem(string storename)
        {
            NPC npc = null;
            if (this.ContainsKey(storename))
            {
                npc = this[storename];
            }

            return npc;
        }
    }
}
