using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using Hegemony.Content.ModPlayers;
using Hegemony.Content.DamageClasses;
using Hegemony.Content.ModConfigs;

namespace Hegemony.Content.GlobalItems
{
	public class GlobalItemHeroInsignia : GlobalItem
	{
		public static float Lerp(int mid, int currentTime, bool clamped, float rotation, float lerpStart, float lerpEnd, float rotationStart, float rotationEnd) {
			var lerp = Utils.GetLerpValue(mid * lerpStart, mid * lerpEnd, currentTime, clamped);
			return Single.Lerp(rotation * rotationStart, rotation * rotationEnd, lerp);
		}

		public bool GetEnabled(Item item, Player player) {
			return ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaValidWeapon(item, player) == 1;
		}

		public override void HoldItem(Item item, Player player) {
			if (player.GetModPlayer<ModPlayerHeroInsignia>().Enabled && ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaValidWeapon(item, player) == 2) {
				item.DamageType = ModContent.GetInstance<DamageClassTrueMelee>();
				player.GetModPlayer<ModPlayerHeroInsignia>().AddEditedItem(item);
			}
		}

		public override float UseSpeedMultiplier(Item item, Player player) {
			if (GetEnabled(item, player))
				return ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaUseSpeedMultiplier();
			return 1f;
		}

		public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) {
			if (GetEnabled(item, player)) {
				damage += ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaDamageMultiplier();
			}
		}

		public override bool AltFunctionUse(Item item, Player player) {
			if (ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaValidWeapon(item, player) != 0) {
				if (player.CanAutoReuseItem(item)) {
					player.GetModPlayer<ModPlayerHeroInsignia>().AddEditedReuseSet(item.type, ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type]);
					ItemID.Sets.ItemsThatAllowRepeatedRightClick[item.type] = true;
				}
				return true;
			}
			return false;
		}

		public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			if (GetEnabled(item, player)) {
				return player.GetModPlayer<ModPlayerHeroInsignia>().Shoot;
			}
			return true;
		}

		public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers) {
			if (!GetEnabled(item, player))
				return;
			TryShoot(item, player, target);
			var array = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaWeaponUseAnimationTime(item, player);
			player.GetModPlayer<ModPlayerHeroInsignia>().AddAttackedNpc(target.whoAmI, array[0]);
			int attackType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAttackType(item, player);
			if (attackType == 2) {
				player.GetModPlayer<ModPlayerHeroInsignia>().ImmunityTime = (int)(array[1] * 0.5 + ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaPierceImmunityMulitplier());
			}
		}

		public override void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers) {
			if (!GetEnabled(item, player))
				return;
			TryShoot(item, player, target);
		}

		public void TryShoot(Item sItem, Player player, Entity target) {
			if (sItem.shoot == 0)
				return;
			if (!ItemLoader.CanShoot(sItem, player))
				return;
			int projToShoot = sItem.shoot;
			float speed = sItem.shootSpeed;
			int Damage = sItem.damage;
			float KnockBack = sItem.knockBack;
			KnockBack = player.GetWeaponKnockback(sItem, KnockBack);
			IEntitySource projectileSource_Item_WithPotentialAmmo = player.GetSource_ItemUse_WithPotentialAmmo(sItem, 0);
			Vector2 pointPoisition = player.RotatedRelativePoint(player.MountedCenter);
			float num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
			float num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
			if (player.gravDir == -1f)
			{
				num3 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - pointPoisition.Y;
			}
			float num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
			float num5 = num4;
			if ((float.IsNaN(num2) && float.IsNaN(num3)) || (num2 == 0f && num3 == 0f))
			{
				num2 = player.direction;
				num3 = 0f;
				num4 = speed;
			}
			else
			{
				num4 = speed / num4;
			}
			num2 *= num4;
			num3 *= num4;
			Vector2 velocity = new Vector2(num2, num3);
			ItemLoader.ModifyShootStats(sItem, player, ref pointPoisition, ref velocity, ref projToShoot, ref Damage, ref KnockBack);
			num2 = velocity.X;
			num3 = velocity.Y;
			player.GetModPlayer<ModPlayerHeroInsignia>().Shoot = true;
			if (ItemLoader.Shoot(sItem, player, (EntitySource_ItemUse_WithAmmo)projectileSource_Item_WithPotentialAmmo, pointPoisition, velocity, projToShoot, Damage, KnockBack))
			{
				int num192 = Projectile.NewProjectile(projectileSource_Item_WithPotentialAmmo, pointPoisition.X, pointPoisition.Y, num2, num3, projToShoot, Damage, KnockBack, player.whoAmI);
				NetMessage.SendData(13, -1, -1, null, player.whoAmI);
			}
			player.GetModPlayer<ModPlayerHeroInsignia>().Shoot = false;
		}

		public override bool? CanMeleeAttackCollideWithNPC(Item item, Rectangle meleeAttackHitbox, Player player, NPC target) {
			if (!GetEnabled(item, player))
				return null;
			Rectangle rectangle = new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height);
			var scale = player.GetAdjustedItemScale(item);
			this.GetPointOnSwungItemPath(player, player.itemWidth, player.itemHeight, 0f, scale, out var location, out var outwardDirection);
			this.GetPointOnSwungItemPath(player, player.itemWidth, player.itemHeight, 1f, scale, out var location2, out outwardDirection);
			return Utils.LineRectangleDistance(rectangle, location, location2) <= player.itemWidth * 0.3f;
		}

		public void GetPointOnSwungItemPath(Player player, float spriteWidth, float spriteHeight, float normalizedPointOnPath, float itemScale, out Vector2 location, out Vector2 outwardDirection) {
			float num = (float)Math.Sqrt(spriteWidth * spriteWidth + spriteHeight * spriteHeight);
			float num2 = (float)(player.direction == 1).ToInt() * ((float)Math.PI / 2f);
			if (player.gravDir == -1f)
			{
				num2 += (float)Math.PI / 2f * (float)player.direction;
			}
			outwardDirection = player.itemRotation.ToRotationVector2().RotatedBy(3.926991f + num2);
			location = player.RotatedRelativePoint(player.itemLocation + outwardDirection * num * normalizedPointOnPath * itemScale);
		}

		public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox) {
			if (!GetEnabled(item, player))
				return;
			var array = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaWeaponUseAnimationTime(item, player);
			int attackType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAttackType(item, player);
			int animationType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAnimationType(item, player, attackType, array[0], array[1]);
			if (player.GetModPlayer<ModPlayerHeroInsignia>().AnimationOverride) {
				if (attackType == 3 && animationType == 3)
					noHitbox = true;
				return;
			}
			if (attackType == 1) {
				if (animationType == 2 || animationType == 4) {
					noHitbox = true;
					return;
				}
			}
			else if (animationType == 1 || animationType == 3) {
				noHitbox = true;
				return;
			}
			hitbox.X = (int)player.itemLocation.X;
			hitbox.Y = (int)player.itemLocation.Y;
			hitbox.Width = (int)player.itemWidth;
			hitbox.Height = (int)player.itemHeight;
			if (player.gravDir == 1)
				hitbox.Y += hitbox.Height;
			if (player.direction == -1)
				hitbox.X -= hitbox.Width;
			/*
			if (attackType == 1) {
				hitbox.Width = (int)(hitbox.Width * 0.7);
				hitbox.Height = (int)(hitbox.Height * 1.6);
				hitbox.Y -= hitbox.Height / 2;
			}
			else if (attackType == 3) {
				hitbox.Width = (int)(hitbox.Width * 1.1);
				hitbox.Height = (int)(hitbox.Height * 1.5);
				hitbox.Y -= hitbox.Height / 2;
			}
			*/
		}

		public override void UseStyle(Item item, Player player, Rectangle heldItemFrame) {
			if (!GetEnabled(item, player))
				return;
			player.itemRotation = (float)Math.PI * player.direction;
			var rotation = player.itemRotation;
			Vector2? offset = null;
			var array = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaWeaponUseAnimationTime(item, player);
			int weaponUseAnimation = array[0];
			int weaponTotalUseAnimation = array[1];
			int readyTime = weaponTotalUseAnimation - weaponUseAnimation * 2;
			int attackType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAttackType(item, player);
			int animationType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAnimationType(item, player, attackType, weaponUseAnimation, weaponTotalUseAnimation);
			int currentTime = player.itemAnimationMax - player.itemAnimation - weaponUseAnimation;
			var rot = rotation;
			if (player.attackCD > weaponUseAnimation)
				player.attackCD = weaponUseAnimation;
			var modPlayer = player.GetModPlayer<ModPlayerHeroInsignia>();
			if (modPlayer.AnimationOverride) {
				if (modPlayer.AnimationTimeOverride <= -1) {
					int timeOffset = 0;
					if (modPlayer.AnimationTimeOverride <= -ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaSpecialAnimationHitThreshold())
						timeOffset = 10;
					modPlayer.AnimationTimeOverride = weaponUseAnimation - 1 - timeOffset;
					if (attackType == 2) {
						modPlayer.AnimationTimeOverride = 1;
					}
					modPlayer.ImmunityTime = (int)(modPlayer.AnimationTimeOverride * ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaaSpecialAnimationImmunityMultiplier());
				}
				if (modPlayer.AnimationTimeOverride > 0) {
					modPlayer.AnimationTimeOverride = modPlayer.AnimationTimeOverride - 1;
					if (player.itemAnimation < player.itemAnimationMax)
						player.itemAnimation++;
				}
				else {
					if (attackType == 1) {
						if (animationType == 4) {
							if (modPlayer.AnimationRotationOverride == 100f)
								player.itemAnimation = 1;
							modPlayer.AnimationRotationOverride = 100f;
						}
					}
					else if (attackType == 3) {
						if (animationType == 3) {
							if (modPlayer.AnimationRotationOverride == 100f)
								player.itemAnimation = 1;
							modPlayer.AnimationRotationOverride = 100f;
						}	
					}
				}
				if (attackType == 2) {
					attackType = 0;
					rotation = player.GetModPlayer<ModPlayerHeroInsignia>().AnimationRotationOverride;
					player.itemRotation = rotation;
				}
			}
			// this chain sucks and you should be ashamed of yourself, me. Rework it if you ever feel like it (which will be never).
			if (attackType == 1) {
				/*
				if (animationType == 1) {
					offset = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation) - player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, 0f);
					var off = 0.25f * player.direction * Lerp(item.useTime, currentTime + item.useTime, true, 1f, 0f, 1f, 0f, 1f);
					offset *= new Vector2(off * -4f, off);
				}
				*/
				if (animationType == 4) {
				}
				else {
					if (animationType != 3) {
						var mid = (int)(weaponUseAnimation + readyTime * 0.5f);
						currentTime += weaponUseAnimation;
						if (currentTime >= mid * 0.7f) {
							rot = Lerp(mid, currentTime, true, rot, 0.7f, 1f, -0.75f, -0.625f);
						}
						else if (currentTime >= mid * 0.6f) {
							rot *= -0.75f;
						}
						else if (currentTime >= mid * 0.45f) {
							rot = Lerp(mid, currentTime, true, rot, 0.45f, 0.6f, -0.625f, -0.75f);
						}
						else if (currentTime >= mid * 0.35f) {
							rot = Lerp(mid, currentTime, true, rot, 0.35f, 0.45f, -0.5f, -0.625f);
						}
						else if (currentTime >= mid * 0.225f) {
							rot = Lerp(mid, currentTime, true, rot, 0.225f, 0.35f, -0.25f, -0.5f);
						}
						else if (currentTime >= mid * 0.15f) {
							rot = Lerp(mid, currentTime, true, rot, 0.15f, 0.225f, 0f, -0.25f);
						}
						else {
							rot = Lerp(mid, currentTime, true, rot, 0f, 0.15f, 1f, 0f);
						}
					}
					else {
						var mid = weaponUseAnimation;
						currentTime -= weaponUseAnimation;
						if (currentTime >= mid * 0.55f) {
							rot = Lerp(mid, currentTime, true, rot, 0.55f, 1f, -0.25f, 1f);
						}
						else if (currentTime >= mid * 0.35f) {
							rot = Lerp(mid, currentTime, true, rot, 0.35f, 0.55f, -0.5f, -0.25f);
						}
						else {
							rot = Lerp(mid, currentTime, true, rot, 0f, 0.35f, -0.625f, -0.5f);
						}
					}
					player.itemRotation = rot;
					rotation = rot;
				}
			}
			else if (attackType == 2) {
				if (animationType != 2) {
					player.itemRotation *= 0.22f;
					rotation *= 0.2f;
					offset = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation) - player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, 0f);
					if (animationType == 1)
						offset *= new Vector2(-0.25f * player.direction * Lerp(weaponUseAnimation, currentTime + weaponUseAnimation, true, offset.Value.X, 0f, 1f, 0f, 1f), 0f);
					rotation += (float)Math.PI * 0.75f * player.direction;
				}
				else {
					player.itemRotation *= 0.25f;
					rotation *= 0.25f;
				}
			}
			else if (attackType == 3) {
				if (animationType == 1) {
					player.itemRotation *= -0.25f;
					rotation *= -0.25f;
					offset = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, 0f) - player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation);
					offset *= new Vector2(0f, player.direction * Lerp(weaponUseAnimation, currentTime + weaponUseAnimation, true, offset.Value.X, 0f, 1f, 1f, 0f));
				}
				else if (animationType == 2) {
					var end = (int)(weaponUseAnimation + readyTime * 0.5f);
					var mid = end - weaponUseAnimation;
					if (currentTime >= mid * 0.98f) {
						rot = Lerp(mid, currentTime, true, rot, 0.98f, 1f, 2.25f, 2.5f);
					}
					else if (currentTime >= mid * 0.95f) {
						rot = Lerp(mid, currentTime, true, rot, 0.95f, 0.98f, 2f, 2.25f);
					}
					else if (currentTime >= mid * 0.9f) {
						rot = Lerp(mid, currentTime, true, rot, 0.9f, 0.95f, 1.75f, 2f);
					}
					else if (currentTime >= mid * 0.8f) {
						rot = Lerp(mid, currentTime, true, rot, 0.8f, 0.9f, 1.5f, 1.75f);
					}
					else if (currentTime >= mid * 0.68f) {
						rot = Lerp(mid, currentTime, true, rot, 0.68f, 0.8f, 1.25f, 1.5f);
					}
					else if (currentTime >= mid * 0.55f) {
						rot = Lerp(mid, currentTime, true, rot, 0.55f, 0.68f, 1.125f, 1.25f);
					}
					else if (currentTime >= mid * 0.4f) {
						rot = Lerp(mid, currentTime, true, rot, 0.4f, 0.55f, 1f, 1.125f);
					}
					else if (currentTime >= mid * 0.3f) {
						rot = Lerp(mid, currentTime, true, rot, 0.3f, 0.4f, 0.875f, 1f);
					}
					else if (currentTime >= mid * 0.25f) {
						rot = Lerp(mid, currentTime, true, rot, 0.25f, 0.3f, 0.75f, 0.875f);
					}
					else if (currentTime >= mid * 0.175f) {
						rot = Lerp(mid, currentTime, true, rot, 0.175f, 0.25f, 0.5f, 0.75f);
					}
					else if (currentTime >= mid * 0.125f) {
						rot = Lerp(mid, currentTime, true, rot, 0.125f, 0.175f, 0.25f, 0.5f);
					}
					else if (currentTime >= mid * 0.075f) {
						rot = Lerp(mid, currentTime, true, rot, 0.075f, 0.125f, 0f, 0.25f);
					}
					else {
						rot = Lerp(mid, currentTime, true, rot, 0f, 0.075f, -0.25f, 0f);
					}
					player.itemRotation = rot;
					rotation = rot;
				}
				else {
					player.itemRotation *= 2.5f;
					rotation *= 2.5f;
				}
			}
			rotation -= (float)Math.PI * 0.75f * player.direction;
			var hitbox = Item.GetDrawHitbox(item.type, player);
			player.SetCompositeArmFront(enabled: true, Player.CompositeArmStretchAmount.Full, rotation);
			var location = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation);
			hitbox.X = (int)location.X;
			hitbox.Y = (int)location.Y;
			var scale = player.GetAdjustedItemScale(item);
			hitbox.Width = (int)(hitbox.Width * scale); /* * 1.5 */
			hitbox.Height = (int)(hitbox.Height * scale); /* * 1.5 */
			heldItemFrame = hitbox;
			player.itemLocation.X = heldItemFrame.X;
			player.itemLocation.Y = heldItemFrame.Y;
			player.itemWidth = heldItemFrame.Width;
			player.itemHeight = heldItemFrame.Height;
			if (offset != null) {
				player.itemLocation.X += offset.Value.X;
				player.itemLocation.Y -= offset.Value.Y;
			}
			player.FlipItemLocationAndRotationForGravity();
		}
	}
}