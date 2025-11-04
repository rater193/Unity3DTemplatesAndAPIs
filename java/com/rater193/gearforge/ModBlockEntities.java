package com.rater193.gearforge;

import net.minecraft.world.level.block.entity.BlockEntityType;
import net.minecraftforge.fml.RegistryObject;
import net.minecraftforge.registries.DeferredRegister;
import net.minecraftforge.registries.ForgeRegistries;
import net.minecraftforge.eventbus.api.IEventBus;

public class ModBlockEntities {
    public static final DeferredRegister<BlockEntityType<?>> BLOCK_ENTITY_TYPES = DeferredRegister.create(ForgeRegistries.BLOCK_ENTITY_TYPES, GearForgeMod.MODID);

    public static final RegistryObject<BlockEntityType<UnlockMachineBlockEntity>> UNLOCK_MACHINE_BE =
            BLOCK_ENTITY_TYPES.register("unlock_machine_be",
                    () -> BlockEntityType.Builder.of(UnlockMachineBlockEntity::new, ModBlocks.UNLOCK_MACHINE.get()).build(null));

    public static final RegistryObject<BlockEntityType<UpgradeStationBlockEntity>> UPGRADE_STATION_BE =
            BLOCK_ENTITY_TYPES.register("upgrade_station_be",
                    () -> BlockEntityType.Builder.of(UpgradeStationBlockEntity::new, ModBlocks.UPGRADE_STATION.get()).build(null));

    public static void register(IEventBus bus) {
        BLOCK_ENTITY_TYPES.register(bus);
    }
}