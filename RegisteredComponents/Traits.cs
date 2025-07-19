using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Heritages.Hungerseed.RegisteredComponents
{
    public static class HungerseedTrait
    {
        public static readonly Trait Hungerseed = ModManager.RegisterTrait("Hungerseed", new TraitProperties("Hungerseed", true) { IsAncestryTrait = true });
        public static readonly Trait Oni = ModManager.RegisterTrait("Oni", new TraitProperties("Oni", true));

        public static readonly Trait OniWeapon = ModManager.RegisterTrait("OniWeapon", new TraitProperties("OniWeapon", false));
        public static readonly Trait Brace = ModManager.RegisterTrait("Brace", new TraitProperties("Brace", true, "When you Ready to Strike an opponent that moves within your reach, until the start of your next turn Strikes made as part of a reaction with the brace weapon deal an additional 2 precision damage for each weapon damage die it has."));
    }
}
