package com.rater193.gearforge.block;

import net.minecraft.core.BlockPos;
import net.minecraft.world.inventory.AbstractContainerMenu;
import net.minecraft.world.level.block.entity.BlockEntity;
import net.minecraft.world.level.block.state.BlockState;

/**
 * Minimal BE - actual opening of container handled via onBlockActivated in game code (omitted here).
 */
public class UpgradeStationBlockEntity extends BlockEntity {
    public UpgradeStationBlockEntity(BlockPos pos, BlockState state) {
        super(com.rater193.gearforge.ModBlockEntities.UPGRADE_STATION_BE.get(), pos, state);
    }
}