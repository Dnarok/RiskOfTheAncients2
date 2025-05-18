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