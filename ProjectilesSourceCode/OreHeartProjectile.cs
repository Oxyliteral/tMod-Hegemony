using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Hegemony.Content.Projectiles
{
	public class OreHeartProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			Main.projFrames[Type] = 4;
		}

		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 3;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.ownerHitCheck = true;
			Projectile.ownerHitCheckDistance = 300f; // The maximum range that the projectile can hit a target. 300 pixels is 18.75 tiles.
			Projectile.usesOwnerMeleeHitCD = true;Projectile.stopsDealingDamageAfterPenetrateHits = true;

			// We will be using custom AI for this projectile. The original Excalibur uses aiStyle 190.
			Projectile.aiStyle = -1;
			Projectile.noEnchantmentVisuals = true;
		}

		public override void AI() {
		}
	}
}