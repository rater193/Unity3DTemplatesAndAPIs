package com.rater193.gearforge.block;

import com.rater193.gearforge.core.GearData;
import net.minecraft.core.BlockPos;
import net.minecraft.server.level.ServerLevel;
import net.minecraft.world.entity.item.ItemEntity;
import net.minecraft.world.item.ArmorItem;
import net.minecraft.world.level.block.entity.BlockEntity;
import net.minecraft.world.level.block.state.BlockState;

/**
 * On server tick, finds ItemEntity instances on the block and tags their ItemStack unlocked.
 */
public class UnlockMachineBlockEntity extends BlockEntity {
    private int tickCooldown = 0;

    public UnlockMachineBlockEntity(BlockPos pos, BlockState state) {
        super(com.rater193.gearforge.ModBlockEntities.UNLOCK_MACHINE_BE.get(), pos, state);
    }

    public static void serverTick(ServerLevel level, BlockPos pos, BlockState state, UnlockMachineBlockEntity be) {
        if (++be.tickCooldown < 10) return;
        be.tickCooldown = 0;

        var items = level.getEntitiesOfClass(ItemEntity.class, state.getShape(level, pos).bounds().move(pos));
        for (ItemEntity itemEntity : items) {
            if (itemEntity.isRemoved()) continue;
            var stack = itemEntity.getItem();
            if (stack.isEmpty()) continue;

            // Determine type: if armor item -> armor, else weapon
            String t = (stack.getItem() instanceof ArmorItem) ? "armor" : "weapon";
            GearData.setUnlocked(stack, true, t);

            // Put the modified stack back into the item entity so players can pick up an unlocked item
            itemEntity.setItem(stack);

            // Optionally send a little XP unlock particle/notification — omitted for brevity
        }
    }
}