using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Hegemony.Content.Projectiles;

namespace Hegemony.Content.Items.Swords
{
	public class HegemonyPreHardmode : ModItem
	{
		public override void SetDefaults()
		{	
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = false;
			Item.autoReuse = true;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.reuseDelay = 10;
			Item.rare = ItemRarityID.Pink;
			Item.width = 40;
			Item.height = 40;
			Item.UseSound = SoundID.Item1;
			Item.damage = 72;
			Item.knockBack = 4.5f;
			Item.width = 40;
			Item.height = 40;
			Item.scale = 1f;
			Item.damage = 20;
			Item.knockBack = 1f;
			Item.shoot = ModContent.ProjectileType<HegemonyPreHardmodeProjectile>();
			Item.shootSpeed = 0f;
			Item.DamageType = DamageClass.Melee;
			Item.crit = 1;
			Item.shootsEveryUse = true;	
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<NatureWrath>());
			recipe.AddIngredient(ModContent.ItemType<OreHeart>());
			recipe.AddTile(TileID.Furnaces);
			recipe.AddTile(TileID.Anvils);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}


	}
}
