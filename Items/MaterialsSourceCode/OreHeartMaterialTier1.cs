using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Hegemony.Content.ModPlayers;
using Hegemony.Content.Rarities;

namespace Hegemony.Content.Items.Materials
{
	public class OreHeartMaterialTier1 : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(0);

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.rare = ModContent.RarityType<RarityHegemony>();
			Item.value = Item.sellPrice(0, 0, 7, 50);
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CopperBroadsword);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.TinBroadsword);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}