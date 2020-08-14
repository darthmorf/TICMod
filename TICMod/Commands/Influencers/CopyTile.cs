using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace TICMod.Commands.Influencers
{
    class CopyTile : Influencer
    {
        protected Point start;
        protected Point end;
        protected Point destination;

        private HashSet<string> aliases_ = new HashSet<string>() { "copytile" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length != 6)
            {
                err = $"Takes 6 parameters; Start X Co-ordinate, Start Y Co-ordinate, End X Co-ordinate, End Y Co-ordinate, Destination X & Destination Y";
                return false;
            }

            valid = ParseCoord(args[0], args[1], out start, out err);
            if (!valid)
            {
                return false;
            }

            valid = ParseCoord(args[2], args[3], out end, out err);
            if (!valid)
            {
                return false;
            }

            valid = ParseCoord(args[4], args[5], out destination, out err);
            if (!valid)
            {
                return false;
            }

            return true;
        }

        public override string Execute()
        {
            Point startpos = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point endpos = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            for (int x = 0; x < endpos.X - startpos.X + 1; x++)
            {
                for (int y = 0; y < endpos.Y - startpos.Y + 1; y++)
                {
                    Main.tile[destination.X + x, destination.Y + y].CopyFrom(Main.tile[startpos.X + x, startpos.Y + y]);
                }
            }

            return $"Successfully copied tiles @ {startpos.X},{startpos.Y} - {endpos.X},{endpos.Y} to {destination.X}, {destination.Y}.";
        }
    }
}
