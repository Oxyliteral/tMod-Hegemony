using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Hegemony.Content.ModPlayers;
using Hegemony.Content.Rarities;

namespace Hegemony.Content.Items.Materials
{
	public class DwellerSightMaterial : ModItem
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
			recipe.AddIngredient(ItemID.GreenMoss);
			recipe.AddIngredient(ItemID.Daybloom);
			recipe.AddIngredient(ItemID.Blinkroot);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BrownMoss);
			recipe.AddIngredient(ItemID.Daybloom);
			recipe.AddIngredient(ItemID.Blinkroot);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.RedMoss);
			recipe.AddIngredient(ItemID.Daybloom);
			recipe.AddIngredient(ItemID.Blinkroot);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BlueMoss);
			recipe.AddIngredient(ItemID.Daybloom);
			recipe.AddIngredient(ItemID.Blinkroot);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PurpleMoss);
			recipe.AddIngredient(ItemID.Daybloom);
			recipe.AddIngredient(ItemID.Blinkroot);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.LavaMoss);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.KryptonMoss);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.XenonMoss);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.ArgonMoss);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.VioletMoss);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
			recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.RainbowMoss);
			recipe.AddTile(TileID.Furnaces);
			recipe.Register();
		}
	}
}