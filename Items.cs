
using System;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Mods.Heritages.Hungerseed.RegisteredComponents;
using static Dawnsbury.Mods.Heritages.Hungerseed.HungerseedClassLoader;

namespace Dawnsbury.Mods.Heritages.Hungerseed;

public static class Items
{
    [Item]
    public static (string, Func<ItemName, Item>) Khakkhara()
    {
        return ("Khakkhara", itemName =>
        {
            //TODO: Two-Handed D10 (needs another version of the item? or dynamically add two-handed trait? plus actions to change grip)
            return new Item(itemName, HungerseedIllustrations.Khakkhara, "khakkhara", 0, 2, [HungerseedTrait.OniWeapon, Trait.Uncommon, Trait.MonkWeapon, Trait.Shove, Trait.Weapon, Trait.VersatileP, Trait.Martial, Trait.Club])
                .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning));
        }
        );
    }

    [Item]
    public static (string, Func<ItemName, Item>) Nodachi()
    {
        return ("Nodachi", itemName =>
        {
            return new Item(itemName, HungerseedIllustrations.Nodachi, "nodachi", 0, 6, [HungerseedTrait.OniWeapon, Trait.Reach, Trait.DeadlyD12, HungerseedTrait.Brace, Trait.Weapon, Trait.TwoHanded, Trait.Advanced, Trait.Sword])
                .WithWeaponProperties(new WeaponProperties("1d8", DamageKind.Slashing));
        }
        );
    }

    [Item]
    public static (string, Func<ItemName, Item>) OgreHook()
    {
        return ("Ogre Hook", itemName =>
        {
            return new Item(itemName, IllustrationName.Greatpick, "ogre hook", 0, 1, [HungerseedTrait.OniWeapon, Trait.Uncommon, Trait.Martial, Trait.Weapon, Trait.TwoHanded, Trait.Pick, Trait.DeadlyD10, Trait.Trip])
                .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Piercing));
        }
        );
    }

    [Item]
    public static (string, Func<ItemName, Item>) Tetsubo()
    {
        return ("Tetsubo", itemName =>
        {
            return new Item(itemName, HungerseedIllustrations.Tetsubo, "tetsubo", 0, 3, [HungerseedTrait.OniWeapon, Trait.Uncommon, Trait.Martial, Trait.Weapon, Trait.TwoHanded, Trait.Club, Trait.Razing, Trait.Shove, Trait.Sweep])
                .WithWeaponProperties(new WeaponProperties("1d10", DamageKind.Bludgeoning));
        }
        );
    }
}
