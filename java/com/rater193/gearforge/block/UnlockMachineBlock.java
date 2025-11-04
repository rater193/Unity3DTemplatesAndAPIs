package com.rater193.gearforge.block;

import net.minecraft.core.BlockPos;
import net.minecraft.server.level.ServerLevel;
import net.minecraft.world.entity.item.ItemEntity;
import net.minecraft.world.level.block.Block;
import net.minecraft.world.level.block.entity.BlockEntity;
import net.minecraft.world.level.block.state.BlockState;
import net.minecraft.world.level.Level;
import net.minecraft.world.level.block.state.BlockBehaviour;

/**
 * Simple block whose BlockEntity will scan for ItemEntity on top and unlock them.
 */
public class UnlockMachineBlock extends Block {
    public UnlockMachineBlock(BlockBehaviour.Properties props) {
        super(props);
    }

    @Override
    public void onPlace(BlockState state, Level world, BlockPos pos, BlockState oldState, boolean p_220082_) {
        super.onPlace(state, world, pos, oldState, p_220082_);
    }

    @Override
    public void tick(BlockState state, ServerLevel world, BlockPos pos, java.util.Random random) {
        // tick is handled by BlockEntity; this method intentionally left light.
        super.tick(state, world, pos, random);
    }

    @Override
    public BlockEntity newBlockEntity(BlockPos pos, BlockState state) {
        return new UnlockMachineBlockEntity(pos, state);
    }
}