using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace TICMod.Commands.Influencers
{
    class SpawnItem : Influencer
    {
        protected Point position;
        protected int itemId;
        protected int count;

        private HashSet<string> aliases_ = new HashSet<string>() { "spawnitem" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length != 4)
            {
                err = $"Takes 4 parameters; X Co-ordinate, Y Co-ordinate, Item ID & Item Count";
                return false;
            }

            valid = ParseCoord(args[0], args[1], out position, out err, true);
            if (!valid)
            {
                return false;
            }

            valid = ParseInt(args[2], out itemId, out err, 0, ItemID.Count);
            if (!valid)
            {
                return false;
            }

            valid = ParseInt(args[3], out count, out err, 0);
            if (!valid)
            {
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            Item.NewItem(new Vector2(position.X, position.Y), itemId, Stack: count);
            return $"Successfully spawned item ID {itemId} x{count} at ({position.X / 16}, {position.Y / 16}).";
        }
    }
}
