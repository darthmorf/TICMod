using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Items
{
    public class Conditional : ModItem
    {
        public const string Name = "Conditional";
        public Conditional() { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Conditional");
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
            item.createTile = TileType<Tiles.Conditional>();
            item.width = 12;
            item.height = 12;
            item.value = 0;
            item.mech = true;
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