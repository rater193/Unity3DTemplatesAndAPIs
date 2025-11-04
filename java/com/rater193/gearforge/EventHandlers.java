package com.rater193.gearforge;

import com.rater193.gearforge.core.GearData;
import net.minecraft.world.entity.item.ItemEntity;
import net.minecraft.world.entity.player.Player;
import net.minecraft.world.item.ArmorItem;
import net.minecraft.world.item.ItemStack;
import net.minecraftforge.common.MinecraftForge;
import net.minecraftforge.event.entity.living.LivingHurtEvent;
import net.minecraftforge.eventbus.api.SubscribeEvent;

/**
 * Hooks to award XP for weapons (on-deal) and armor (on-receive).
 */
public class EventHandlers {
    public static void register() {
        MinecraftForge.EVENT_BUS.register(new EventHandlers());
    }

    @SubscribeEvent
    public void onLivingHurt(LivingHurtEvent event) {
        if (event.getSource() == null) return;

        // if a player dealt damage to target -> award weapon XP to the player's held item
        if (event.getSource().getEntity() instanceof Player) {
            Player p = (Player) event.getSource().getEntity();
            ItemStack held = p.getMainHandItem();
            if (!held.isEmpty() && GearData.isUnlocked(held) && !"armor".equals(GearData.getType(held))) {
                double xp = Math.max(1.0, event.getAmount() * 2.0); // tuned: damage -> xp
                GearData.addXp(held, xp);
            }
        }

        // if target is a player and they took damage -> award armor XP to worn armor pieces
        if (event.getEntity() instanceof Player) {
            Player target = (Player) event.getEntity();
            double damage = event.getAmount();
            if (damage <= 0) return;
            for (ItemStack armor : target.getArmorSlots()) {
                if (armor.isEmpty()) continue;
                if (GearData.isUnlocked(armor) && "armor".equals(GearData.getType(armor))) {
                    double xp = Math.max(1.0, damage * 1.5); // tuned: damage taken -> xp for armor
                    GearData.addXp(armor, xp);
                }
            }
        }
    }
}