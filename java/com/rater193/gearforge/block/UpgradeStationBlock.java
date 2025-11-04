package com.rater193.gearforge.block;

import net.minecraft.core.BlockPos;
import net.minecraft.world.level.block.Block;
import net.minecraft.world.level.block.entity.BlockEntity;
import net.minecraft.world.level.block.state.BlockState;
import net.minecraft.world.level.block.state.BlockBehaviour;

/**
 * Block to open upgrade UI when right-clicked with an unlocked item.
 */
public class UpgradeStationBlock extends Block {
    public UpgradeStationBlock(BlockBehaviour.Properties props) {
        super(props);
    }

    @Override
    public BlockEntity newBlockEntity(BlockPos pos, BlockState state) {
        return new UpgradeStationBlockEntity(pos, state);
    }
}