using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Hegemony.Content.DamageClasses;
using Hegemony.Content.ModConfigs;

namespace Hegemony.Content.PlayerDrawLayers
{
	public class PlayerDrawLayerHeroInsignia : PlayerDrawLayer
	{
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			if (!ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaPlayerDrawLayer())
				return false;
			Player player = drawInfo.drawPlayer;
			if (player.HeldItem == null)
				return false;
			if (player.ItemAnimationActive)
				return false;
			if (ModContent.GetInstance<ModConfigServerSide>().GetHeroInsigniaValidWeapon(player.HeldItem, player) != 1)
				return false;
			if (drawInfo.drawPlayer.mount.Active)
				return false;
			return true;
		}

		public override Position GetDefaultPosition() => new BeforeParent(Terraria.DataStructures.PlayerDrawLayers.HeldItem);

		protected override void Draw(ref PlayerDrawSet drawInfo) {
			var player = drawInfo.drawPlayer;
			var frontArm = drawInfo.compFrontArmFrame;
			int bodyFrame = player.bodyFrame.Y / player.bodyFrame.Height;
			var texture = TextureAssets.Item[player.HeldItem.type];
			Rectangle? source = null;
			var v = Main.itemAnimations[player.HeldItem.type];
			if (v != null)
				source = Main.itemAnimations[player.HeldItem.type].GetFrame(texture.Value);
			Color lighting = Lighting.GetColor((int)(drawInfo.Position.X + player.width * 0.5f) / 16, (int)(drawInfo.Position.Y + player.height * 0.5f) / 16);
			var rotation = (float)Math.PI * player.direction * player.gravDir;
			// default x -8f, y 40f
            var position = new Vector2(player.Center.X, player.Center.Y) - Main.screenPosition;
            var offset = new Vector2(texture.Size().X * 0.5f - texture.Size().X * 0.5f * player.direction, texture.Size().Y * 0.5f + texture.Size().Y * 0.5f * player.gravDir);
            var x = -12f + Main.OffsetsPlayerHeadgear[bodyFrame].X;
            var y = 10f + player.HeightOffsetHitboxCenter + Main.OffsetsPlayerHeadgear[bodyFrame].Y;
            if (bodyFrame > 5) { // walk cycle
                x += frontArm.X * 2 / frontArm.Width - 4;
                y -= 6f;
                rotation *= 0.26f + 0.005f * (float)(position.X - Math.Truncate(position.X));
            }
            else if (bodyFrame == 5) { // jump
                x += 3f;
                y -= 18f;
                rotation *= 1.74f;
            }
            else { // default
                rotation *= 0.2f /* random */ + 0.1f * (float)(position.X - Math.Truncate(position.X));
            }
            position.X += x * player.direction;
            position.Y += y * player.gravDir;
            var scale = player.GetAdjustedItemScale(player.HeldItem);
            SpriteEffects spriteEffects = drawInfo.itemEffect;
	        if (player.direction == -1)
	        	spriteEffects = spriteEffects | SpriteEffects.FlipHorizontally;
	        if (player.gravDir == -1)
	            spriteEffects = spriteEffects | SpriteEffects.FlipVertically;
	        drawInfo.DrawDataCache.Add(new DrawData(
	            texture.Value,
	            position,
	            source,
	            lighting,
	            rotation,
	            offset,
	            scale,
	            spriteEffects,
	            0));
		}
	}
}