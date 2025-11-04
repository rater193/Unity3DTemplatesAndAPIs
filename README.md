```markdown
# GearForge (Neoforge / Forge-style) - mod skeleton for MC 1.21.1

What this contains
- Unlock Machine: toss items onto it; it will tag them as unlockable gear (weapon vs armor).
- Weapon XP: gained when a player deals damage with an unlocked non-armor item.
- Armor XP: gained when a wearer of unlocked armor takes damage.
- Upgrade Station: right-click with an unlocked item to open server-side menu to spend gear points.
- Data-driven upgrades: JSON files included under data/gearforge/upgrades.

Key implementation details
- All gear state is stored on ItemStack NBT under tag `GearForge` (unlocked, type, level, progress_xp, gear_points, upgrades, effects).
- Leveling has no hard cap and awards gear points per level.
- Upgrades are loaded from JSON and applied by adding effect data into ItemStack NBT.

Migration notes
- Keep GearData as the single place for NBT keys; when updating to new Minecraft versions change only the tiny interaction bits (events/registrations).
- The UpgradeRegistry uses packaged JSONs for the sample; production should switch to using the game's ResourceManager + data pack loader for hot-loading user data packs.

Next steps
- Wire client GUI (buttons, rendering, previews).
- Hook upgrade effects into combat/armor math (apply flat_damage, armor_pierce, yeet effect on attacks).
- Add models, block states, and language translations.

```