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
    class PlaceTile : InfluencerCommand
    {
        protected Point start;
        protected Point end;
        protected int tileId;
        protected int tileStyle;

        private HashSet<string> aliases_ = new HashSet<string>() { "placetile", "placeblock" };
        protected override HashSet<string> aliases { get { return aliases_; } }

        public override bool ParseArguments(string[] args, out string err)
        {
            err = "";
            bool valid;

            if (args.Length < 5 || args.Length > 6)
            {
                err = $"Takes 5-6 parameters; Start X Co-ordinate, Start Y Co-ordinate, End X Co-ordinate, End Y Co-ordinate, Tile ID & Tile Style(optional)";
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

            valid = ParseInt(args[4], out tileId, out err, 0, TileID.Count);
            if (!valid)
            {
                return false;
            }

            if (args.Length == 4)
            {
                valid = ParseInt(args[5], out tileStyle, out err, 0);
                if (!valid)
                {
                    return false;
                }
            }

            return true;
        }

        public override string Execute()
        {
            Point startpos = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
            Point endpos = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));

            for (int x = startpos.X; x < endpos.X + 1; x++)
            {
                for (int y = startpos.Y; y < endpos.Y + 1; y++)
                {
                    WorldGen.PlaceTile(x, y, tileId, style: tileStyle, forced: true);
                }
            }

            return $"Successfully spawned tile ID:{tileId} @ {startpos.X},{startpos.Y} - {endpos.X},{endpos.Y}.";
        }
    }
}
