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
            
            if (this[storename] == null)
            {
                this[storename] = item;
            }
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
            Player player = new Player();
            if (this.ContainsKey(storename))
            {
                player = this[storename];
            }

            return player;
        }
    }
}
