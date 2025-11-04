package com.rater193.gearforge.gui;

import com.rater193.gearforge.core.GearData;
import com.rater193.gearforge.upgrade.Upgrade;
import com.rater193.gearforge.upgrade.UpgradeRegistry;
import net.minecraft.server.level.ServerPlayer;
import net.minecraft.world.entity.player.Inventory;
import net.minecraft.world.entity.player.Player;
import net.minecraft.world.inventory.AbstractContainerMenu;
import net.minecraft.world.inventory.ContainerLevelAccess;
import net.minecraft.world.inventory.SimpleContainerData;
import net.minecraft.world.item.ItemStack;
import net.minecraft.network.FriendlyByteBuf;

import java.util.List;

/**
 * Minimal server-side menu for the Upgrade Station.
 * UI logic is intentionally minimal; this demonstrates selecting upgrades server-side.
 */
public class UpgradeStationMenu extends AbstractContainerMenu {
    private final Player player;
    private final ItemStack stack;

    protected UpgradeStationMenu(int id, Inventory playerInventory, FriendlyByteBuf buf) {
        super(com.rater193.gearforge.ModMenuTypes.UPGRADE_STATION_MENU.get(), id);
        this.player = playerInventory.player;
        this.stack = player.getMainHandItem();
    }

    public static UpgradeStationMenu fromNetwork(int windowId, Inventory inv, FriendlyByteBuf buf) {
        return new UpgradeStationMenu(windowId, inv, buf);
    }

    @Override
    public boolean stillValid(Player pPlayer) {
        return true;
    }

    // Example RPC from client: request choices for spending `cost`
    public List<Upgrade> requestChoices(int cost) {
        return UpgradeRegistry.chooseForCost(cost);
    }

    // Example: apply selected upgrade id (server-side)
    public boolean applyUpgrade(String upgradeId) {
        if (!(player instanceof ServerPlayer)) return false;
        if (!GearData.isUnlocked(stack)) return false;
        var opt = UpgradeRegistry.get(upgradeId);
        if (opt.isEmpty()) return false;
        var up = opt.get();
        if (!GearData.spendGearPoints(stack, up.cost)) return false;
        up.applyTo(stack);
        return true;
    }
}