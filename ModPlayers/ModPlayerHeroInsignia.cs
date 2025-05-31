using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Hegemony.Content.DamageClasses;
using Hegemony.Content.GlobalItems;
using Hegemony.Content.ModConfigs;

namespace Hegemony.Content.ModPlayers
{
	public class ModPlayerHeroInsignia : ModPlayer
	{
		public bool bEnabled;

		public bool Enabled {
			get {
				return bEnabled;
			}
			set {
				bEnabled = value;
			}
		}

		public int iEffectType;

		public Vector2? oInitialVelocity;

		public List<Item> listEditedItems = new List<Item>();

		public Dictionary<int, bool> dictReuseSet = new Dictionary<int, bool>();

		public List<int[]> listAttackedNpcs = new List<int[]>();

		public bool bShoot;

		public bool Shoot {
			get {
				return bShoot;
			}
			set {
				bShoot = value;
			}
		}

		public NPC oAnimationOverride;

		public bool AnimationOverride {
			get {
				return GetNpcIsActive(oAnimationOverride);
			}
		}

		public float fAnimationRotationOverride;

		public float AnimationRotationOverride {
			get {
				return fAnimationRotationOverride;
			}
			set {
				fAnimationRotationOverride = value;
			}
		}

		public int iAnimationTimeOverride;

		public int AnimationTimeOverride {
			get {
				return iAnimationTimeOverride;
			}
			set {
				iAnimationTimeOverride = value;
			}
		}

		public int iMaxHitThreshold;

		public int iImmunityTime;

		public int ImmunityTime {
			get {
				return iImmunityTime;
			}
			set {
				iImmunityTime = value;
			}
		}

		public static void ResetDamageClass(Item item) {
			if (item.ModItem != null)
				item.DamageType = ItemLoader.GetItem(item.type).Item.DamageType;
			else {
				Item clone = new Item(item.type);
				item.DamageType = clone.DamageType;
			}
		}

		public void AddEditedItem(Item item) {
			if (!listEditedItems.Contains(item))
				listEditedItems.Add(item);
		}

		public void AddEditedReuseSet(int type, bool reuse) {
			if (!dictReuseSet.ContainsKey(type))
				dictReuseSet.Add(type, reuse);
		}		

		public void AddAttackedNpc(int npcIndex, int cooldown) {
			int[] array = new int[] { npcIndex, cooldown };
			if (!listAttackedNpcs.Contains(array)) {
				listAttackedNpcs.Add(array);
			}
		}

		public bool GetEnabled(Item item, Player player) {
			return ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaValidWeapon(item, player) == 1;
		}

		public bool GetNpcIsActive(NPC npc) {
			if (npc == null)
				return false;
			if (!npc.active)
				return false;
			int life;
			int lifeMax;
			npc.GetLifeStats(out life, out lifeMax);
			if (life == 0f) {
				return false;
			}
			return true;
		}

		public bool GetNpcValidAnimationOverride(NPC npc) {
			return npc.knockBackResist == 0f || npc.boss;
		}

		public void ResetAnimationOverride() {
			oAnimationOverride = null;
			iAnimationTimeOverride = 0;
			iMaxHitThreshold = 0;
			iImmunityTime = 0;
		}

		public override void ResetEffects() {
			bool Disabled = !Enabled;
			Enabled = false;
			for (int i = listEditedItems.Count - 1; i >= 0; i--) {
				var item = listEditedItems[i];
				if (item != Player.HeldItem || Disabled) {
					ResetDamageClass(item);
					listEditedItems.RemoveAt(i);
				}
			}
			foreach (int type in dictReuseSet.Keys) {
				if (type != Player.HeldItem.type || Disabled) {
					ItemID.Sets.ItemsThatAllowRepeatedRightClick[type] = dictReuseSet[type];
					dictReuseSet.Remove(type);
				}
			}
			for (int i = listAttackedNpcs.Count - 1; i >= 0; i--) {
				int[] array = listAttackedNpcs[i];
				Player.SetMeleeHitCooldown(array[0], array[1]);
				listAttackedNpcs.RemoveAt(i);
			}
			if (oAnimationOverride != null && !Player.ItemAnimationActive)
				ResetAnimationOverride();
		}

		public override void PreUpdateMovement() {
			if (!Enabled)
				return;
			if (AnimationOverride && Player.ItemAnimationActive) {
				if (AnimationTimeOverride != 0)
					Player.velocity = oAnimationOverride.velocity;
				return;
			}
			if (iEffectType == 2) {
				Player.velocity.X += ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaSlashDashMultiplier() * Player.maxRunSpeed * 2f * Player.direction;
			}
			else if (iEffectType == 3) {
				oInitialVelocity = Player.velocity;
				Player.velocity.X *= -1f;
			}
			else if (iEffectType == 4) {
				Player.velocity.X += ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaPierceDashMultiplier() * Player.maxRunSpeed * 2f * Player.direction + oInitialVelocity.Value.X;
				oInitialVelocity = null;
			}
			iEffectType = 0;
		}

		public override void PostItemCheck() {
			if (!Enabled)
				return;
			if (!Player.ItemAnimationActive)
				return;
			var item = Player.HeldItem;
			if (item == null)
				return;
			if (!GetEnabled(item, Player))
				return;
			if (iImmunityTime > 0)
				iImmunityTime--;
			if (oAnimationOverride != null && (Player.itemAnimation == 1 || !AnimationOverride))
				ResetAnimationOverride();
			if (AnimationOverride)
				return;
			var array = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaWeaponUseAnimationTime(item, Player);
			int weaponUseAnimation = array[0];
			int weaponTotalUseAnimation = array[1];
			int readyTime = weaponTotalUseAnimation - weaponUseAnimation * 2;
			int attackType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAttackType(item, Player);
			int animationType = ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaAnimationType(item, Player, attackType, weaponUseAnimation, weaponTotalUseAnimation);
			if (attackType == 1) {
				if (Player.itemAnimation == (int)(weaponUseAnimation + readyTime * 0.5)) {
					iEffectType = 2;
				}
			}
			else if (attackType == 2) {
				if (Player.itemAnimation == Player.itemAnimationMax) {
					iEffectType = 3;
				}
				else if (Player.itemAnimation == Player.itemAnimationMax - weaponUseAnimation) {
					iEffectType = 4;
				}
			}
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			if (ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaSpecialAnimationEnabled() && GetNpcValidAnimationOverride(target) && Player.ItemAnimationActive) {
				if (!AnimationOverride) {
					oAnimationOverride = target;
					fAnimationRotationOverride = Player.itemRotation;
				}
				iAnimationTimeOverride = -1 - iMaxHitThreshold;
				iMaxHitThreshold++;
			}
		}

		public override bool FreeDodge(Player.HurtInfo info) {
			if (!Enabled)
				return false;
			if (iImmunityTime == 0)
				return false;
			if (!info.Dodgeable)
				return false;
			if (info.DamageSource == null)
				return false;
			Entity cause;
			if (info.DamageSource.TryGetCausingEntity(out cause)) {
				if (cause is NPC) {
					if (oAnimationOverride != null) {
						return oAnimationOverride == cause;
					}
					return true;
				}
				return false;
			}
			return false;
		}
	}
}