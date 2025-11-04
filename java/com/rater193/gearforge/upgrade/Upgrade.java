package com.rater193.gearforge.upgrade;

import net.minecraft.nbt.CompoundTag;
import net.minecraft.world.item.ItemStack;

/**
 * Simple Upgrade abstraction. Implementations apply changes to ItemStack NBT.
 */
public class Upgrade {
    public final String id;
    public final String name;
    public final String description;
    public final int tier;
    public final int cost;
    public final String effect; // freeform effect id that code will interpret

    public Upgrade(String id, String name, String description, int tier, int cost, String effect) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.tier = tier;
        this.cost = cost;
        this.effect = effect;
    }

    /**
     * Apply this upgrade to the stack by writing NBT flags/data. Implement effect interpretation.
     */
    public void applyTo(ItemStack stack) {
        CompoundTag root = stack.getOrCreateTag().getCompound("GearForge");
        // store applied upgrades list
        var list = root.getList("upgrades", net.minecraft.nbt.StringTag.TYPE);
        list.add(net.minecraft.nbt.StringTag.valueOf(id));
        root.put("upgrades", list);

        // Basic built-in effects: simple stats
        CompoundTag effects = root.getCompound("effects");
        // add a numeric value for a few recognized effects
        switch (effect) {
            case "add_damage_2":
                effects.putInt("flat_damage", effects.getInt("flat_damage") + 2);
                break;
            case "add_armor_3":
                effects.putInt("flat_armor", effects.getInt("flat_armor") + 3);
                break;
            case "armor_pierce_5":
                effects.putInt("armor_pierce", effects.getInt("armor_pierce") + 5);
                break;
            case "yeet_to_moon":
                // flag a wacky effect to be triggered during runtime by the combat code when relevant
                effects.putBoolean("yeet_to_moon", true);
                break;
            // add more effects here
            default:
                // unknown effect: store it so data persists
                effects.putString("custom:" + effect, "1");
                break;
        }
        root.put("effects", effects);
        stack.getTag().put("GearForge", root);
    }
}