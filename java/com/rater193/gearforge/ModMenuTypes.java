package com.rater193.gearforge;

import net.minecraft.world.inventory.MenuType;
import net.minecraftforge.fml.RegistryObject;
import net.minecraftforge.registries.DeferredRegister;
import net.minecraftforge.registries.ForgeRegistries;
import net.minecraftforge.eventbus.api.IEventBus;

public class ModMenuTypes {
    public static final DeferredRegister<MenuType<?>> MENUS = DeferredRegister.create(ForgeRegistries.MENU_TYPES, GearForgeMod.MODID);

    public static final RegistryObject<MenuType<UpgradeStationMenu>> UPGRADE_STATION_MENU =
            MENUS.register("upgrade_station_menu", () -> new MenuType<>(UpgradeStationMenu::fromNetwork));

    public static void register(IEventBus bus) {
        MENUS.register(bus);
    }
}