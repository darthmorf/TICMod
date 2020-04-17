using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Tiles
{
	public class Conditional : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileSolidTop[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.addTile(Type);
		}

		public override bool Dangersense(int i, int j, Player player) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 48, ItemType<Items.Conditional>());
        }

		public override void HitWire(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int style = tile.frameY / 18;
			Vector2 spawnPosition;
			// This logic here corresponds to the orientation of the sprites in the spritesheet, change it if your tile is different in design.
			int horizontalDirection = (tile.frameX == 0) ? -1 : ((tile.frameX == 18) ? 1 : 0);
			int verticalDirection = (tile.frameX < 36) ? 0 : ((tile.frameX < 72) ? -1 : 1);
			// Each trap style within this Tile shoots different projectiles.
			// Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer number here for less frequent projectile spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
			if (Wiring.CheckMech(i, j, 60))
			{
				spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here help center the projectile spawn position if you need to.

				// In reality you should be spawning projectiles that are both hostile and friendly to do damage to both players and NPC.
				// Make sure to change velocity, projectile, damage, and knockback.
				Projectile.NewProjectile(spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 6f, ProjectileID.IchorBullet, 20, 2f, Main.myPlayer);
			}
		}
	}
}
