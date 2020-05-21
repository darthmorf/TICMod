using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TICMod
{
    public class PlayerDataStore : Dictionary<string, List<Player>>
    {
        public void AddItem(string storename, Player item)
        {
            if (storename == null)
            {
                return;
            }

            if (!ContainsKey(storename))
            {
                Add(storename, new List<Player>());
            }
            
            if (!this[storename].Contains(item))
            {
                this[storename].Add(item);
            }
        }

        public void RemoveItem(string storename, Player item)
        {
            if (storename == null)
            {
                return;
            }

            if (ContainsKey(storename) && this[storename].Contains(item))
            {
                this[storename].Remove(item);

                if (this[storename].Count == 0)
                {
                    Remove(storename);
                }
            }
        }

        public List<Player> GetItem(string storename)
        {
            List<Player> players = new List<Player>();
            if (this.ContainsKey(storename) && this[storename] != null)
            {
                players = this[storename];
            }

            return players;
        }
    }
}
