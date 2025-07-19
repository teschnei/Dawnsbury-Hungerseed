using Dawnsbury.Core.Mechanics;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Heritages.Hungerseed.RegisteredComponents
{
    public static class HungerseedQEffects
    {
        public static readonly QEffectId OniForm = ModManager.RegisterEnumMember<QEffectId>("OniForm");
        public static readonly QEffectId OniFormUsed = ModManager.RegisterEnumMember<QEffectId>("OniFormUsed");
        public static readonly QEffectId GreaterOniForm = ModManager.RegisterEnumMember<QEffectId>("GreaterOniForm");
        public static readonly QEffectId StormingGazeCD = ModManager.RegisterEnumMember<QEffectId>("StormingGazeCooldown");
    }
}
