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
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.maxStack = 9999;
			item.consumable = true;
			item.createTile = TileType<Tiles.Influencer>();
			item.width = 12;
			item.height = 12;
			item.value = 0;
			item.mech = true;
            item.rare = 13;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
