package com.rater193.gearforge.upgrade;

import com.google.gson.*;
import net.minecraft.server.packs.resources.ResourceManager;
import net.minecraft.resources.ResourceLocation;
import net.minecraftforge.fml.loading.FMLPaths;

import java.io.InputStreamReader;
import java.nio.file.Files;
import java.util.*;

/**
 * A tiny JSON loader for upgrades. In a full mod you'd use the game's data pack system — but this
 * keeps the sample simple and portable. Place JSON files under resources/data/gearforge/upgrades/.
 */
public class UpgradeRegistry {
    private static final Map<String, Upgrade> UPGRADES = new LinkedHashMap<>();

    public static void init() {
        // For simplicity here we'll load packaged sample JSON files from the jar resource folder.
        // In production use Minecraft's ResourceManager & data pack system.
        try {
            // Look for resources in data/gearforge/upgrades inside the mod JAR
            var cl = UpgradeRegistry.class.getClassLoader();
            var names = List.of(
                    "data/gearforge/upgrades/add_damage_2.json",
                    "data/gearforge/upgrades/add_armor_3.json",
                    "data/gearforge/upgrades/armor_pierce_5.json",
                    "data/gearforge/upgrades/yeet_to_moon.json",
                    "data/gearforge/upgrades/crit_boost_1.json"
            );
            Gson gson = new Gson();
            for (String name : names) {
                try (var is = cl.getResourceAsStream(name)) {
                    if (is == null) continue;
                    var reader = new InputStreamReader(is);
                    JsonObject obj = gson.fromJson(reader, JsonObject.class);
                    String id = obj.get("id").getAsString();
                    String nm = obj.get("name").getAsString();
                    String desc = obj.get("description").getAsString();
                    int tier = obj.get("tier").getAsInt();
                    int cost = obj.get("cost").getAsInt();
                    String effect = obj.get("effect").getAsString();
                    Upgrade u = new Upgrade(id, nm, desc, tier, cost, effect);
                    UPGRADES.put(id, u);
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static Collection<Upgrade> all() {
        return UPGRADES.values();
    }

    public static Optional<Upgrade> get(String id) {
        return Optional.ofNullable(UPGRADES.get(id));
    }

    /**
     * Choose random upgrade choices based on requested point budget.
     *  - 1 point -> choose 3 random low-tier upgrades
     *  - 2 points -> choose 4 higher-tier upgrades
     * This is simplistic and tunable.
     */
    public static List<Upgrade> chooseForCost(int cost) {
        Random rng = new Random();
        List<Upgrade> pool = new ArrayList<>(UPGRADES.values());
        if (cost <= 1) {
            // choose from tier 1 and 2 mostly
            pool.removeIf(u -> u.tier > 2);
            Collections.shuffle(pool, rng);
            return pool.subList(0, Math.min(3, pool.size()));
        } else {
            // higher-tier choices
            pool.removeIf(u -> u.tier > 4);
            Collections.shuffle(pool, rng);
            return pool.subList(0, Math.min(4, pool.size()));
        }
    }
}