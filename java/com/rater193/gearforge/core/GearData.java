package com.rater193.gearforge.core;

import net.minecraft.nbt.CompoundTag;
import net.minecraft.nbt.ListTag;
import net.minecraft.nbt.StringTag;
import net.minecraft.world.item.ItemStack;
import java.util.ArrayList;
import java.util.List;

/**
 * Utilities for storing and manipulating gear XP/level/points on ItemStack NBT.
 *
 * NBT layout under tag "GearForge":
 * - unlocked: byte (0/1)
 * - type: string ("weapon"|"armor")
 * - level: int
 * - progress_xp: double (xp toward next level)
 * - gear_points: int
 * - upgrades: ListTag of String (upgrade IDs applied)
 */
public final class GearData {
    private static final String ROOT = "GearForge";
    private static final String UNLOCKED = "unlocked";
    private static final String TYPE = "type";
    private static final String LEVEL = "level";
    private static final String PROGRESS = "progress_xp";
    private static final String GEAR_POINTS = "gear_points";
    private static final String UPGRADES = "upgrades";

    private GearData() {}

    public static boolean isUnlocked(ItemStack stack) {
        CompoundTag t = stack.getTag();
        if (t == null || !t.contains(ROOT)) return false;
        return t.getCompound(ROOT).getBoolean(UNLOCKED);
    }

    public static String getType(ItemStack stack) {
        CompoundTag t = stack.getTag();
        if (t == null || !t.contains(ROOT)) return "weapon";
        return t.getCompound(ROOT).getString(TYPE);
    }

    public static void setUnlocked(ItemStack stack, boolean unlocked, String type) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        root.putBoolean(UNLOCKED, unlocked);
        root.putString(TYPE, type);
        stack.getTag().put(ROOT, root);
    }

    public static int getLevel(ItemStack stack) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        return root.getInt(LEVEL);
    }

    public static double getProgressXp(ItemStack stack) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        return root.getDouble(PROGRESS);
    }

    public static int getGearPoints(ItemStack stack) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        return root.getInt(GEAR_POINTS);
    }

    public static List<String> getUpgrades(ItemStack stack) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        List<String> out = new ArrayList<>();
        if (root.contains(UPGRADES)) {
            ListTag list = root.getList(UPGRADES, StringTag.TYPE);
            list.forEach(t -> out.add(((StringTag) t).getAsString()));
        }
        return out;
    }

    public static void addUpgrade(ItemStack stack, String upgradeId) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        ListTag list = root.getList(UPGRADES, StringTag.TYPE);
        list.add(StringTag.valueOf(upgradeId));
        root.put(UPGRADES, list);
        stack.getTag().put(ROOT, root);
    }

    private static double xpForNextLevel(int level) {
        // Growth formula — adjust coefficients for tuning. No hard cap.
        // nextXp = base * (level + 1)^(growth)
        double base = 60.0;
        double growth = 1.5;
        return Math.floor(base * Math.pow(level + 1, growth));
    }

    private static int pointsPerLevel(int level) {
        // awarding scheme: every level gives 1 gear point, with bonus every 10 levels
        return 1 + (level % 10 == 0 ? 1 : 0);
    }

    public static void addXp(ItemStack stack, double xp) {
        if (xp <= 0) return;
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        int level = root.getInt(LEVEL);
        double progress = root.getDouble(PROGRESS);
        int gearPoints = root.getInt(GEAR_POINTS);

        progress += xp;
        boolean leveled = false;
        while (progress >= xpForNextLevel(level)) {
            double needed = xpForNextLevel(level);
            progress -= needed;
            level += 1;
            gearPoints += pointsPerLevel(level);
            leveled = true;
        }

        root.putInt(LEVEL, level);
        root.putDouble(PROGRESS, progress);
        root.putInt(GEAR_POINTS, gearPoints);
        stack.getTag().put(ROOT, root);
    }

    public static boolean spendGearPoints(ItemStack stack, int amount) {
        CompoundTag root = stack.getOrCreateTag().getCompound(ROOT);
        int gearPoints = root.getInt(GEAR_POINTS);
        if (gearPoints < amount) return false;
        root.putInt(GEAR_POINTS, gearPoints - amount);
        stack.getTag().put(ROOT, root);
        return true;
    }
}