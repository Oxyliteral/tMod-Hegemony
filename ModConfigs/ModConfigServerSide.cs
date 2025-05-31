using System;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Hegemony.Content.DamageClasses;

namespace Hegemony.Content.ModConfigs
{
	public class ModConfigServerSide : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Header("WeaponOut")]
		[DefaultValue(true)]
		public bool CanDrawLayer;

		[Header("HeroInsignia")]
		[RangeAttribute(150, 1000)]
		[DefaultValue(200)]
		[IncrementAttribute(25)]
		[SliderAttribute]
		public int DamageMultiplier;

		[RangeAttribute(100, 400)]
		[DefaultValue(200)]
		[IncrementAttribute(25)]
		[SliderAttribute]
		public int UseTimeMultiplier;

		[DefaultValue(true)]
		public bool SpecialAttackAnimation;

		[RangeAttribute(0, 10)]
		[DefaultValue(3)]
		[IncrementAttribute(1)]
		[SliderAttribute]
		public int SpecialAttackAnimationHitThreshold;

		[RangeAttribute(0, 400)]
		[DefaultValue(100)]
		[IncrementAttribute(25)]
		[SliderAttribute]
		public int SpecialAttackAnimationImmunityMultiplier;

		[Header("HeroInsigniaSlash")]
		[RangeAttribute(1, 100)]
		[DefaultValue(21)]
		[IncrementAttribute(1)]
		public int AttackTypeSlash;

		[RangeAttribute(0, 400)]
		[DefaultValue(100)]
		[IncrementAttribute(25)]
		[SliderAttribute]
		public int AttackTypeSlashDashMultiplier;

		[Header("HeroInsigniaPierce")]
		[RangeAttribute(1, 100)]
		[DefaultValue(10)]
		[IncrementAttribute(1)]
		public int AttackTypePierce;

		[RangeAttribute(0, 400)]
		[DefaultValue(100)]
		[IncrementAttribute(25)]
		[SliderAttribute]
		public int AttackTypePierceDashMultiplier;

		[RangeAttribute(0, 400)]
		[DefaultValue(100)]
		[IncrementAttribute(25)]
		[SliderAttribute]
		public int AttackTypePierceImmunityMultiplier;

		[Header("HeroInsigniaBlunt")]
		[RangeAttribute(1, 100)]
		[DefaultValue(26)]
		[IncrementAttribute(1)]
		public int AttackTypeBlunt;

		[Header("BonusWeapon")]
		[DefaultValue(true)]
		public bool BonusWeaponEnabled;

		[Header("BonusShortcut")]
		[DefaultValue(true)]
		public bool BonusShortcutEnabled;

		public float GetHeroInsigniaDamageMultiplier() {
			return DamageMultiplier / 100f;
		}

		public float GetHeroInsigniaUseSpeedMultiplier() {
			return 100f / (float)(UseTimeMultiplier + 200);
		}

		public bool GetHeroInsigniaSpecialAnimationEnabled() {
			return SpecialAttackAnimation;
		}

		public int GetHeroInsigniaSpecialAnimationHitThreshold() {
			return SpecialAttackAnimationHitThreshold;
		}

		public float GetHeroInsigniaaSpecialAnimationImmunityMultiplier() {
			return SpecialAttackAnimationImmunityMultiplier / 100f;
		}

		public float GetHeroInsigniaSlashDashMultiplier() {
			return AttackTypeSlashDashMultiplier / 100f;
		}

		public float GetHeroInsigniaPierceDashMultiplier() {
			return AttackTypePierceDashMultiplier / 100f;
		}

		public float GetHeroInsigniaPierceImmunityMulitplier() {
			return AttackTypePierceImmunityMultiplier / 100f;
		}

		public int[] GetHeroInsigniaWeaponUseAnimationTime(Item item, Player player) {
			var modded = item.useAnimation / ItemLoader.UseSpeedMultiplier(item, player);
			var total = modded / player.GetWeaponAttackSpeed(item);
			return new int[] { Math.Max(1, (int)(item.useAnimation + (total - modded) * 0.5f)), (int)total };
		}

		public bool GetHeroInsigniaPlayerDrawLayer() {
			return CanDrawLayer;
		}

		public bool GetBonusWeaponEnabled() {
			return BonusWeaponEnabled;
		}

		public bool GetBonusShortcutEnabled() {
			return BonusShortcutEnabled;
		}

		public int GetHeroInsigniaAttackType(Item item, Player player) {
			if (item.useAnimation >= AttackTypeBlunt)
				return 3;
			if (item.useAnimation >= AttackTypeSlash)
				return 1;
			if (item.useAnimation >= AttackTypePierce)
				return 2;
			return 0;
		}

		public int GetHeroInsigniaAnimationType(Item item, Player player, int attackType, int weaponUseAnimation, int weaponTotalUseAnimation) {
			var readyTime = weaponTotalUseAnimation - weaponUseAnimation * 2;
			if (attackType == 1) {
				if (player.itemAnimation <= weaponUseAnimation) {
					return 4;
				}
				if (player.itemAnimation <= weaponUseAnimation + readyTime * 0.5) {
					return 3;
				}
				if (player.itemAnimation <= weaponUseAnimation * 2 + readyTime * 0.5) {
					return 2;
				}
				return 1;
			}
			else if (attackType == 2) {
				if (player.itemAnimation <= weaponUseAnimation) {
					return 3;
				}
			}
			else if (attackType == 3) {
				if (player.itemAnimation <= weaponUseAnimation + readyTime * 0.5) {
					return 3;
				}
			}
			if (player.itemAnimation >= weaponUseAnimation + readyTime) {
				return 1;
			}
			return 2;
		}

		// 0 = nothing, 1 = full gas, 2 = elegible, 3 = altUse
		public int GetHeroInsigniaValidWeapon(Item item, Player player) {
			if (item.noMelee || item.noUseGraphic)
				return 0;
			if (item.axe != 0 || item.pick != 0 || item.hammer != 0)
				return 0;
			if (item.createTile != -1)
				return 0;
			if (item.channel)
				return 0;
			if (GetHeroInsigniaAttackType(item, player) == 0)
				return 0;
			int type = 0;
			if (item.DamageType.CountsAsClass(ModContent.GetInstance<DamageClassTrueMelee>()))
				type = 1;
			else if (item.DamageType.CountsAsClass(DamageClass.Melee))
				type = 2;
			if (player.altFunctionUse != 0 && type != 0)
				type = 3;
			return type;
		}
	}
}