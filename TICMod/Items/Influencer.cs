using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Items
{
    public class Influencer : ModItem
    {
        public const string Name = "Influencer";
		public Influencer() { }

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Influencer");
			Tooltip.SetDefault("When given a signal, activates internal command.");
		}

		public override void SetDefaults()
		{
			Item.useStyle = 1;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.createTile = TileType<Tiles.Influencer>();
			Item.width = 12;
			Item.height = 12;
			Item.value = 0;
			Item.mech = true;
            Item.rare = ItemRarityID.Cyan;
		}
    }
}
