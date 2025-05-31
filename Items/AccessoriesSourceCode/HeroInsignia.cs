using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Hegemony.Content.ModPlayers;
using Hegemony.Content.Rarities;
using Hegemony.Content.Items.Materials;

namespace Hegemony.Content.Items.Accessories
{
	public class HeroInsignia : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(0);

		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
			Item.accessory = true;
			Item.defense = 1;
			Item.rare = ModContent.RarityType<RarityHegemony>();
			Item.value = Item.sellPrice(0, 0, 7, 50);
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<HeroInsigniaMaterial>());
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		// add shimmer to copper shortsword

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<ModPlayerHeroInsignia>().Enabled = true;
		}
	}
}