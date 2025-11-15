using System.Collections.Generic;
using Dawnsbury.Auxiliary;
using Dawnsbury.Core;
using Dawnsbury.Core.Animations;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Spellbook;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Creatures.Parts;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Display.Text;
using Dawnsbury.Mods.Heritages.Hungerseed.RegisteredComponents;
using Humanizer;
using Microsoft.Xna.Framework;
using static Dawnsbury.Mods.Heritages.Hungerseed.HungerseedClassLoader;

namespace Dawnsbury.Mods.Heritages.Hungerseed;

public static class Hungerseed
{
    [Feat(0)]
    public static Feat HungerseedHeritage()
    {
        return new HeritageSelectionFeat(HungerseedFeat.Hungerseed, "One of your parents was an oni or a hungerseed. You possess a pair of horns, ranging in size from conical nubs to lengthy protrusions. You might have other signs of your parentage, such as colorful skin, fangs and claws, or a third eye in your forehead.", "You gain the oni trait.\nYou gain a horns unarmed attack that deals 1d6 piercing damage and is in the brawling group.\nYou can select from hungerseed feats and feats from your other parent's ancestry whenever you gain an ancestry feat.", [Trait.VersatileHeritage, HungerseedTrait.Hungerseed])
            .WithOnSheet(sheet =>
            {
                sheet.Ancestries.Add(HungerseedTrait.Hungerseed);
                sheet.Ancestries.Add(HungerseedTrait.Oni);
            })
            .WithOnCreature(cr =>
            {
                cr.Traits.Add(HungerseedTrait.Hungerseed);
                cr.Traits.Add(HungerseedTrait.Oni);
            })
            .WithPermanentQEffect(null, q =>
            {
                q.AdditionalUnarmedStrike = new Item(IllustrationName.Horn, "horns", [Trait.Unarmed, Trait.Brawling, HungerseedTrait.OniWeapon])
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing));
            });
    }

    [Feat(1)]
    public static Feat HungryEyes()
    {
        return new TrueFeat(HungerseedFeat.HungryEyes, 1, "Your eyes can see through darkness with an oni's visual acuity.", "You gain a +2 circumstance bonus to Seek against targets within 30 feet of you. In addition, your miss chance when targeting a concealed creature is only 10% instead of 20%, and your miss chance when targeting an invisible creature is only 40% instead of 50%.", [HungerseedTrait.Hungerseed, Trait.Homebrew])
            .WithPermanentQEffectAndSameRulesText(q =>
            {
                q.Id = QEffectId.KeenEyes;
                q.BonusToAttackRolls = (QEffect qf, CombatAction seek, Creature? defender) =>
                {
                    if (defender == null)
                    {
                        return null;
                    }
                    return (seek.ActionId != ActionId.Seek || defender.DistanceTo(qf.Owner) > 6) ? null : new Bonus(2, BonusType.Circumstance, "Hungry Eyes");
                };
            });
    }

    [Feat(1)]
    public static Feat OniWeaponFamiliarity()
    {
        return new TrueFeat(HungerseedFeat.OniWeaponFamiliarity, 1, "Oni prefer large, cruel weapons for smashing their foes to pieces, and so do you.",
                "For the purposes of proficiency, you treat the khakkhara, nodachi, ogre hook, and tetsubo as simple weapons. At 5th level, whenever you get a critical hit with one of these weapons, or with your horns unarmed strike, you get its critical specialization effect.",
                [HungerseedTrait.Hungerseed])
            .WithOnSheet(sheet =>
            {
                sheet.Proficiencies.AddProficiencyAdjustment((List<Trait> item) => item.Contains(HungerseedTrait.OniWeapon), Trait.Simple);
            })
            .WithPermanentQEffectAndSameRulesText(q =>
            {
                q.YouHaveCriticalSpecialization = (qe, item, action, creature) => qe.Owner.Level >= 5 && item.HasTrait(HungerseedTrait.OniWeapon);
            });
    }

    [Feat(1)]
    public static Feat OniForm()
    {
        return new TrueFeat(HungerseedFeat.OniForm, 1, "Your horns flash briefly as you grow in size and ferocity.", "Your size increases to Large, and you're clumsy 1. You can Sustain your Oni Form for up to 10 minutes.", [HungerseedTrait.Hungerseed])
            .WithActionCost(1)
            .WithPermanentQEffectAndSameRulesText(q =>
            {
                q.ProvideMainAction = q => 
                {
                    var greater = q.Owner.HasEffect(HungerseedQEffects.GreaterOniForm);
                    return q.Owner.HasEffect(HungerseedQEffects.OniFormUsed) || q.Owner.Space.Size >= Size.Large ? null :
                        new ActionPossibility(new CombatAction(q.Owner,
                                    IllustrationName.DemonMask,
                                    greater ? "Greater Oni Form" : "Oni Form",
                                    [Trait.Concentrate, HungerseedTrait.Hungerseed, Trait.Polymorph, Trait.Primal, Trait.Basic],
                                    greater ? "Unleash your Oni Form to increase your size to Large and reach by 5 feet, and become clumsy 1 until the end of the encounter." : "Unleash your Oni Form to increase your size to Large and become clumsy 1. You can Sustain your Oni Form for up to 10 minutes.",
                                    Target.Self())
                            .WithActionCost(1)
                            .WithEffectOnChosenTargets(async (action, cr, targets) =>
                            {
                                var oldSize = cr.Space.Size;
                                var oldLong = cr.Space.Long;
                                if (await cr.Space.GrowTo(Size.Large))
                                {
                                    cr.Space.Long = greater ? false : true;
                                    cr.AddQEffect(new QEffect()
                                    {
                                        Id = HungerseedQEffects.OniFormUsed,
                                        StateCheckWithVisibleChanges = async q =>
                                        {
                                            if (q.Tag is bool tag && tag == true)
                                            {
                                                await q.Owner.Space.GrowTo(oldSize);
                                                cr.Space.Long = oldLong;
                                            }
                                        }
                                    });
                                    cr.AddQEffect(new QEffect("Oni Form", "Your Oni Form is unleashed, increasing your size but making you clumsy 1.",
                                            greater ? ExpirationCondition.Never : ExpirationCondition.ExpiresAtEndOfSourcesTurn,
                                            cr, IllustrationName.DemonMask)
                                    {
                                        Id = HungerseedQEffects.OniForm,
                                        CannotExpireThisTurn = true,
                                        StateCheck = q => q.Owner.AddQEffect(QEffect.Clumsy(1).WithExpirationEphemeral()),
                                        WhenExpires = q => 
                                        {
                                            var qe = q.Owner.FindQEffect(HungerseedQEffects.OniFormUsed);
                                            if (qe != null)
                                            {
                                                qe.Tag = true;
                                            }
                                        },
                                        ProvideContextualAction = q => q.CannotExpireThisTurn || greater ? null : new ActionPossibility(new CombatAction(
                                                q.Owner,
                                                IllustrationName.DemonMask,
                                                "Sustain Oni Form",
                                                [Trait.Concentrate, Trait.SustainASpell, Trait.Basic, Trait.DoesNotBreakStealth],
                                                "The duration of Oni Form continues until the end of your next turn.", Target.Self()
                                            )
                                            .WithReferencedQEffect(q)
                                            .WithEffectOnSelf(async (action, cr) =>
                                            {
                                                q.CannotExpireThisTurn = true;
                                            })
                                        ).WithPossibilityGroup("Maintain an activity")
                                    });
                                }
                                else
                                {
					                cr.Overhead("nowhere to grow", Color.Red, cr.ToColoredBoldedName() + " cannot be enlarged because there is no space to fit the creature's enlarged form.");
					                action.RevertRequested = true;
                                }
                            })
                        );
                };
            });
    }

    [Feat(5)]
    public static Feat BloodsoakedDash()
    {
        return new TrueFeat(HungerseedFeat.BloodsoakedDash, 5, "Injuries don't bother you anymore - in fact, the pain invigorates you.", "If you're at or below half of your maximum Hit Points, you gain a 10-foot status bonus to all your speeds.", [HungerseedTrait.Hungerseed])
            .WithPermanentQEffectAndSameRulesText(q =>
            {
                q.BonusToAllSpeeds = qe => qe.Owner.HP <= qe.Owner.MaxHP / 2 ? new Bonus(2, BonusType.Status, "Bloodsoaked Dash") : null;
            });
    }

    /*
     * Need a better idea for this one in DD
    [Feat(5)]
    public static Feat OnisMask()
    {
        return new TrueFeat(HungerseedFeat.OnisMask, 5, "", "", [HungerseedTrait.Hungerseed])
            .WithOnSheet(sheet =>
            {
                sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
                sheet.InnateSpells.GetOrCreate(HungerseedTrait.Hungerseed, () => new InnateSpells(Trait.Primal));
                sheet.InnateSpells[HungerseedTrait.Hungerseed].SpellsKnown.Add(AllSpells.CreateModernSpellTemplate(SpellId.SeeInvisibility, HungerseedTrait.Hungerseed));
            });
    }
    */

    [Feat(5)]
    public static Feat StormingGaze()
    {
        ActionPossibility StormingGaze(Creature caster, DamageKind kind)
        {
            return new ActionPossibility(new CombatAction(
                    caster,
                    IllustrationName.Seek,
                    $"Storming Gaze ({kind.Humanize()})",
                    [HungerseedTrait.Hungerseed, Trait.Primal, DamageKindExtensions.DamageKindToTrait(kind), Trait.Basic],
                    $"You deal {S.HeightenedVariable((caster.Level + 1) / 2, 3)}d4 {kind.Humanize(LetterCasing.LowerCase)} damage in a 15-foot cone. Each creature in the area must attempt a basic Reflex saving throw against your spell DC. You can't use this ability again for 1d4 rounds.",
                    Target.FifteenFootCone().WithAdditionalRequirementOnCaster(cr => cr.HasEffect(HungerseedQEffects.StormingGazeCD) ? Usability.NotUsable("Storming Gaze was used recently and must recover before being usable again.") : Usability.Usable))
                .WithHeighteningNumerical(caster.Level, 5, true, 2, "The damage increases by 1d4.")
                .WithActionCost(2)
                .WithSavingThrow(new SavingThrow(Defense.Reflex, cr => cr?.ClassOrSpellDC() ?? 0))
                .WithProjectileCone(IllustrationName.LightningBolt, 15, ProjectileKind.Cone)
                .WithSoundEffect(Audio.SfxName.ElectricBlast)
                .WithEffectOnSelf(self =>
                {
                    self.AddQEffect(new QEffect("Storming Gaze", "Your third eye is recovering its power. You can't use Storming Gaze until this effect expires.", ExpirationCondition.CountsDownAtStartOfSourcesTurn, self, IllustrationName.Seek)
                    {
                        Id = HungerseedQEffects.StormingGazeCD
                    }.WithExpirationAtStartOfSourcesTurn(self, R.Next(1, 5)));
                })
                .WithEffectOnEachTarget(async (action, self, target, checkResult) =>
                {
                    await CommonSpellEffects.DealBasicDamage(action, self, target, checkResult, $"{(caster.Level + 1 / 2)}d4", kind);
                }));
        }
        return new TrueFeat(HungerseedFeat.StormingGaze, 5, "You can open or energize a notable third eye on your forehead to strike with storming power.",
                "You deal 3d4 electricity or sonic damage in a 15-foot cone; Storming Gaze gains this trait. Each creature in the area must attempt a basic Reflex saving throw against your class DC or spell DC. You can't use this ability again for 1d4 rounds.\n\nAt 7th level and every 2 levels thereafter, the damage increases by 1d4.",
                [HungerseedTrait.Hungerseed, Trait.Primal])
            .WithActionCost(2)
            .WithPermanentQEffect(null, q =>
            {
                q.ProvideMainAction = qe => new SubmenuPossibility(IllustrationName.Seek, "Storming Gaze")
                {
                    Subsections =
                    [
                        new PossibilitySection("Storming Gaze")
                        {
                            Possibilities =
                            [
                                StormingGaze(qe.Owner, DamageKind.Electricity),
                                StormingGaze(qe.Owner, DamageKind.Sonic)
                            ]
                        },
                        new PossibilitySection("Storming Gaze Sheet Description")
                        {
                            Visible = false,
                            Possibilities =
                            [
                                new ActionPossibility(new CombatAction(qe.Owner, IllustrationName.Seek, "Storming Gaze",
                                    [],
                                    $"You deal {S.HeightenedVariable((qe.Owner.Level+1) / 2, 3)}d4 electricity or sonic damage in a 15-foot cone; Storming Gaze gains this trait. Each creature in the area must attempt a basic Reflex saving throw against your class DC or spell DC. You can't use this ability again for 1d4 rounds.",
                                    Target.FifteenFootCone()).WithActionCost(2))
                            ]
                        }
                    ]
                };
            });
    }

    [Feat(9)]
    public static Feat BloodMustHaveBlood()
    {
        return new TrueFeat(HungerseedFeat.BloodMustHaveBlood, 9, "You tap into a bottomless hunger for carnage.",
                $"You can cast {AllSpells.CreateModernSpellTemplate(SpellId.BloodVendetta, HungerseedTrait.Hungerseed, 2).ToSpellLink()} and {AllSpells.CreateModernSpellTemplate(SpellId.FalseLife, HungerseedTrait.Hungerseed, 2).ToSpellLink()} as 2nd-rank primal spells, each once per day.",
                [HungerseedTrait.Hungerseed])
            .WithOnSheet(sheet =>
            {
                sheet.SetProficiency(Trait.Spell, Proficiency.Trained);
                sheet.InnateSpells.GetOrCreate(HungerseedTrait.Hungerseed, () => new InnateSpells(Trait.Primal));
                sheet.InnateSpells[HungerseedTrait.Hungerseed].SpellsKnown.Add(AllSpells.CreateModernSpellTemplate(SpellId.BloodVendetta, HungerseedTrait.Hungerseed, 2));
                sheet.InnateSpells[HungerseedTrait.Hungerseed].SpellsKnown.Add(AllSpells.CreateModernSpellTemplate(SpellId.FalseLife, HungerseedTrait.Hungerseed, 2));
            });
    }

    [Feat(9)]
    public static Feat GreaterTransformation()
    {
        return new TrueFeat(HungerseedFeat.GreaterTransformation, 9, "You can assume your oni form more easily, and with greater benefits.",
                "Your oni form lasts for 10 minutes without the need to Sustain it. While in your oni form, your reach increases by 5 feet.",
                [HungerseedTrait.Hungerseed])
            .WithPrerequisite(HungerseedFeat.OniForm, "Oni Form")
            .WithPermanentQEffectAndSameRulesText(q =>
            {
                q.Id = HungerseedQEffects.GreaterOniForm;
            });
    }
}
