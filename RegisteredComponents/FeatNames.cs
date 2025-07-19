using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Modding;

namespace Dawnsbury.Mods.Heritages.Hungerseed.RegisteredComponents
{
    public static class HungerseedFeat
    {
        public static readonly FeatName Hungerseed = ModManager.RegisterFeatName("HungerseedHeritage", "Hungerseed");

        //Level 1
        public static readonly FeatName HungryEyes = ModManager.RegisterFeatName("HungryEyes", "Hungry Eyes");
        public static readonly FeatName OniWeaponFamiliarity = ModManager.RegisterFeatName("OniWeaponFamiliarity", "Oni Weapon Familiarity");
        public static readonly FeatName OniForm = ModManager.RegisterFeatName("OniForm", "Oni Form");

        //Level 5
        public static readonly FeatName BloodsoakedDash = ModManager.RegisterFeatName("BloodsoakedDash", "Bloodsoaked Dash");
        public static readonly FeatName OnisMask = ModManager.RegisterFeatName("OnisMask", "Oni's Mask");
        public static readonly FeatName StormingGaze = ModManager.RegisterFeatName("StormingGaze", "Storming Gaze");

        //Level 9
        public static readonly FeatName BloodMustHaveBlood = ModManager.RegisterFeatName("BloodMustHaveBlood", "Blood Must Have Blood");
        public static readonly FeatName GreaterTransformation = ModManager.RegisterFeatName("GreaterTransformation", "Greater Transformation");
    }
}
