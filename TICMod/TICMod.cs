using System.Collections.Generic;
using IL.Terraria;
using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Point16 = Terraria.DataStructures.Point16;

namespace TICMod
{
	public class TICMod : Mod
    {
        public TICMod()
		{
        }
    }

    // ty Rartrin
    public class ExtraWireTrips : ModWorld
    {
        public Queue<Point16> updates = new Queue<Point16>();
        public override void Initialize()
        {
            updates.Clear();
        }
        public override void PostUpdate()
        {
            while (updates.Count > 0)
            {
                var cur = updates.Dequeue();
                Terraria.Wiring.TripWire(cur.X, cur.Y, 1, 1);
            }
        }
        public void AddWireUpdate(int x, int y) => updates.Enqueue(new Point16(x, y));
    }
}