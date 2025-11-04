package com.rater193.gearforge;

import net.minecraft.world.level.block.Block;
import net.minecraft.world.level.block.SoundType;
import net.minecraft.world.level.block.state.BlockBehaviour;
import net.minecraft.world.level.material.Material;
import net.minecraftforge.fml.RegistryObject;
import net.minecraftforge.registries.DeferredRegister;
import net.minecraftforge.registries.ForgeRegistries;
import net.minecraftforge.eventbus.api.IEventBus;

public class ModBlocks {
    public static final DeferredRegister<Block> BLOCKS = DeferredRegister.create(ForgeRegistries.BLOCKS, GearForgeMod.MODID);

    public static final RegistryObject<Block> UNLOCK_MACHINE = BLOCKS.register("unlock_machine",
            () -> new UnlockMachineBlock(BlockBehaviour.Properties.of(Material.METAL).strength(3.5F).sound(SoundType.METAL)));

    public static final RegistryObject<Block> UPGRADE_STATION = BLOCKS.register("upgrade_station",
            () -> new UpgradeStationBlock(BlockBehaviour.Properties.of(Material.WOOD).strength(2.5F).sound(SoundType.WOOD)));

    public static void register(IEventBus bus) {
        BLOCKS.register(bus);
    }
}