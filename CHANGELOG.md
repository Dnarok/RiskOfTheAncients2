## 2.0.0 - Massive code overhaul, new items, and some balance adjustments.
- All assets are now loaded from Unity Addressables.
- Significant parts of the project now reside in the associated Unity project (ItemDefs, models, icons, etc.)
- Added support for Risk of Options.
- Some items received internal reworks.
- General code quality and performance improvements.
- Tranquil Boots and Spark of Courage now have a buff to indicate their "mode".
- Added Power Treads (green), Boots of Travel (red), Ex Machina (red), and Refresher (void red).
- Aeon Disk: Now correctly gives movement speed when activated.
- Aeon Disk: Now removes debuffs when activating.
- Assault Cuirass: Reduced bonus attack speed per stack from `40` -> `20`.
- Assault Cuirass: Rescaled bonus armor base/per stack from `30`/`30` -> `40`/`20`.
- Dragon Scale: Total burn damage base/per stack changed to `210%`/`210%`.
- Ghost Scepter: Cooldown increased from `30` -> `60`.
  - On-demand invulnerability is incredibly strong.
- Iron Talon: Damage is no longer amplified by Crowbars.
  - Fun as it is, it took 4 crowbars and 2 talons to one-shot every single boss.
- Orb of Blight: Fixed incorrect buff duration.
- Radiance: Total burn damage base/per stack changed to `200%`/`200%`.
- Shadow Amulet: Reduced standing still seconds requirement from `1.5` -> `1`.
- Shadow Amulet: Increased invisibility linger duration from `1` -> `1.5`.

## 1.1.7 - Bugfix, new models, and some more balance adjustments.
- Phase Boots: No longer eats OnBuffFirstStackGained and OnBuffFinalStackLost.
#### These new models are coming from item sets for different heroes.
- Radiance: Added a model and radius indicator.
- Sange, Yasha, and Sange and Yasha: Added models.
- Skull Basher: Added model.
- Assault Cuirass: Added model.
#### And some balance changes, of course.
- Bottle: Reduced cooldown from `45` -> `40`.
- Armlet of Mordiggian: Increased damage from `50%` -> `60%`.
- Armlet of Mordiggian: Increased attack speed from `50%` -> `60%`.
- Armlet of Mordiggian: Reduced cooldown from `6` -> `5`.

## 1.1.6 - New items, bug fixes, and a balance pass.
#### Reset your configs for this one...
- Added Blades of Attack (white), and Phase Boots (green).
- Added the option to all items that play a sound to disable their sound.
  - Daedalus ear destruction can now be avoided!
- Spark of Courage: Damage base/per stack increased from `10%`/`10%` -> `15%`/`15%`.
- Spark of Courage: Armor base/per stack increased from `8`/`8` -> `10`/`10`.
- Javelin: Damage base/per stack rescaled from `25%`/`15%` -> `20%`/`20%`.
- Javelin: Proc coefficient increased from `0.2` -> `0.3`.
- Dragon Scale: Burn damage base/per stack increased from `60%`/`60%` -> `70%`/`70%`.
- Shadow Amulet: Decreased wait time from `2` -> `1.5`.
- Shadow Amulet: Invisibility now lingers for `1` second after moving.
- Sange, Sange and Yasha, Kaya and Sange, and Trident: Health regeneration now scales with level.
  - This occurs in vanilla for most sources of passive health regen.
- Radiance: Radius rescaled from `25m`/`+10m` to `30m`.
- Radiance: Ignite damage base/per stack reduced from `200%`/`200%` -> `150%`/`150%`.
- Radiance: Burn duration reduced from `5` -> `3`.
- Radiance: Miss chance increased from `15%` -> `25%`.
  - These changes try to make Radiance less of a free win if you have Ignition Tank, and center its identity more on the unique type of evasion it provides.
- Black King Bar: Damage reduction increased from `25%` -> `35%`.
- Black King Bar: Cooldown reduced from `75` -> `65`.
- Tranquil Boots: Fixed out-of-danger movement speed bonus being way too high.
- Tranquil Boots: Fixed first stack having less regen per level than expected.
- Bottle: Fixed illusions being able to summon more illusions.

## 1.1.5
- Added Iron Talon (green).
- Infused Raindrops: Reworked.
  - Old behavior: White. Taking more than `5%` of your maximum health as damage will reduce the damage by `50`. Can occur `10` times per stack before the stack is consumed.
  - New behavior: Void White (Repulsion Armor Plate). Reduce all incoming damage over `60` by `15` (+`15` per stack). Cannot be reduced below `1`.
- Assault Cuirass: Aura is now global.
- Added item tags to all items so they can spawn in the category chests.

## 1.1.4
- Added Boots of Bearing (equipment), and Pirate Hat (void red).
- Enchanted Mango: Now regenerates from Consumed Mango at the start of the stage.
- Tranquil Boots: Fixed missing movement speed.
- Bottle: Icon now changes based on the contained rune.
- Bottle: Fixed Wisdom never showing up again after the first one.
- Armlet of Mordiggian: No longer two separate equipment (active/inactive).

## 1.1.3
- Added Arcane Boots (equipment), Mekansm (equipment), and Guardian Greaves (equipment).
- Fixed ROTA2 equipment activating on everyone in multiplayer lobbies.
- Shadow Amulet: Fixed invisibility portion still working after removing item.
- Tranquil Boots: Fixed Cautious Slug sound still playing after removing item.
- Black King Bar: Fixed NRE.
- Fixed tierless items being able to be removed at a scrapper.
- Code improvements.

## 1.1.2
- Infused Raindrops: Rearranged implementation again. If this fails one more time, this item is getting reworked.
- Tranquil Boots and Shadow Amulet: Added LookingGlass integration.
- Added safety checks for when items are disabled, should prevent some null derefs.

## 1.1.1
- Added Armlet of Mordiggian (Lunar Equipment), Tranquil Boots (green), and Shadow Amulet (green).
- Infused Raindrops: Fixed horrible laggy implementation, especially on Engineer.
- Infused Raindrops: Now only conusmes stacks after most damage rejection has occurred.
- Kaya, Sange, and Yasha: Combination order rearranged.
  - The items will attempt to combine in this order: Trident, Kaya and Sange, Sange and Yasha, Yasha and Kaya.
  - This should avoid something like Kaya -> Kaya -> Sange -> Yasha turning into Kaya and Sange + Kaya and Yasha. Now it will be Trident + Kaya.
- Boots of Speed: Base movement speed increased from `0.5` -> `0.7`.
  - Should now be more in line with the relative increases of other white items.
- Moon Shard: Can no longer be randomly triggered.
  - This will prevent Bottled Chaos from deleting your held equipment in exchange for the consumed moon shard item.
- Quelling Blade: Non-Boss flat damage increased from `6`/`6` -> `8`/`8`.
- Spark of Courage: Damage bonus increased from `8%`/`8%` -> `10%`/`10%`.

## 1.1.0
- Added models!
  - Bottle, Iron Branch, Quelling Blade, Boots of Speed, Healing Salve, and Enchanted Mango all received unique models.
  - All other items are now using the classic Dota 2 item box - thanks to Retorikal on Github for the idea.
- Added sounds!
  - Most items with sounds in Dota 2 now play their sound at the appropriate time.
  - Radiance exception, since it's a constant looping noise and that seems like a bit much.
- Lance of Pursuit: Fixed doubling all 0 proc coefficient damage, including fall damage. Thanks to SalTheThief on Github for the hint!

## 1.0.5
- Added Iron Branch (white).
- Nemesis Curse: Now only applies the curse from damage with a proc coefficient.
- Nemesis Curse: Recharge duration from `5` -> `7.5`.
- Nemesis Curse: Increased permanent damage stacks coefficient base/per stack from `40`/`40` -> `80`/`80`.
    - The hyperbolic nature of the reduction makes this less punishing than it seems.
    - All of these are just to make it more of a side-grade to Shaped Glass.
- Radiance: Fixed the Ignite having a proc coefficient (I think?!)
- Ghost Scepter: Fixed eating OnBuffFirstStackGained and OnBuffFinalStackLost, thanks to Snoresville for the point-out.

## 1.0.4
- Fixed newest (and any future ones if I hadn't been shown the issue) DLL being skipped, thanks to Farlier on Discord for the heads-up!
- Added Moon Shard (equipment) and Infused Raindrops (white).
- Redid all of the item icons in a way that is more a.) consistent, and b.) closer to vanilla. Some touch-ups will still need to be done, a few are a bit blocky from having to size up Dota 2 item icons from 88x64...

## 1.0.3
- Added Daedalus (green), Quicksilver Amulet (green), and Assault Cuirass (red).
- Ghost Scepter: Now only disables the Primary skill while active.
- Enchanted Mango: Health Threshold reduced from `50%` -> `40%`.
- Enchanted Mango: Now transforms into a `Consumed Mango` on use that provides `2.5%` damage per stack.

## 1.0.2
- Added Aeon Disk (red), Dragon Scale (white), Lance of Pursuit (white), and Nemesis Curse (lunar).
- Added [LookingGlass](https://thunderstore.io/package/DropPod/LookingGlass/) support.
- Kaya: Damage Base/Per Stack increased from `10%`/`10%` -> `12%`/`12%`.
- Kaya: Cooldown Reduction Base/Per Stack decreased from `8%`/`8%` -> `6%`/`6%`.
- Yasha: Attack Speed Base/Per Stack increased from `12%`/`12%` -> `15%`/`15%`.
- Yasha: Movement Speed Base/Per Stack increased from `12%`/`12%` -> `15%`/`15%`.
- Yasha and Kaya: Damage Base/Per Stack increased from `15%`/`15%` -> `18%`/`18%`.
- Yasha and Kaya: Attack Speed Base/Per Stack increased from `18%`/`18%` -> `22.5%`/`22.5%`.
- Yasha and Kaya: Movement Speed Base/Per Stack increased from `18%`/`18%` -> `22.5%`/`22.5%`.
- Yasha and Kaya: Cooldown Reduction Base/Per Stack decreased from `12%`/`12%` -> `9%`/`9%`.
- Sange and Yasha: Attack Speed Base/Per Stack increased from `18%`/`18%` -> `22.5%`/`22.5%`.
- Sange and Yasha: Movement Speed Base/Per Stack increased from `18%`/`18%` -> `22.5%`/`22.5%`.
- Kaya and Sange: Damage Base/Per Stack increased from `15%`/`15%` -> `18%`/`18%`.
- Kaya and Sange: Cooldown Reduction Base/Per Stack decreased from `12%`/`12%` -> `9%`/`9%`.
- Trident: Damage Base/Per Stack increased from `20%`/`20%` -> `24%`/`24%`.
- Trident: Attack Speed Base/Per Stack increased from `24%`/`24%` -> `30%`/`30%`.
- Trident: Movement Speed Base/Per Stack increased from `24%`/`24%` -> `30%`/`30%`.
- Trident: Cooldown Reduction Base/Per Stack decreased from `16%`/`16%` -> `12%`/`12%`.
- Healing Salve: Fixed eating OnSkillActivated, won't break Shurikens and Luminous Shot now.

## 1.0.1
- Added github link to package.

## 1.0.0
- Created.