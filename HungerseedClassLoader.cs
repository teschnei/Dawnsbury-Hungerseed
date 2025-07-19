using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Heritages.Hungerseed;

public static class HungerseedClassLoader
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FeatAttribute : Attribute
    {
        public int Level
        {
            get; set;
        }
        public FeatAttribute(int level)
        {
            Level = level;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ItemAttribute : Attribute
    {
    }

    static IEnumerable<MethodInfo> GetFeatGenerators()
    {
        return typeof(HungerseedClassLoader).Assembly.GetTypes().Where(x => x.IsClass).SelectMany(x => x.GetMethods())
        .Where(x => x.GetCustomAttributes(typeof(FeatAttribute), false).FirstOrDefault() != null)
        .OrderBy(x => (x.GetCustomAttributes(typeof(FeatAttribute), false).First() as FeatAttribute)!.Level);
    }

    static IEnumerable<MethodInfo> GetItemGenerators()
    {
        return typeof(HungerseedClassLoader).Assembly.GetTypes().Where(x => x.IsClass).SelectMany(x => x.GetMethods())
        .Where(x => x.GetCustomAttributes(typeof(ItemAttribute), false).FirstOrDefault() != null);
    }

    [DawnsburyDaysModMainMethod]
    public static void LoadMod()
    {
        ModManager.AssertV3();

        foreach (var featGenerator in GetFeatGenerators())
        {
            var feat = (featGenerator.Invoke(null, null) as Feat)!;
            ModManager.AddFeat(feat);
        }

        foreach (var itemGenerator in GetItemGenerators())
        {
            var (name, factory) = ((string, Func<ItemName, Item>))(itemGenerator.Invoke(null, null))!;
            ModManager.RegisterNewItemIntoTheShop(name, factory);
        }
    }
}
