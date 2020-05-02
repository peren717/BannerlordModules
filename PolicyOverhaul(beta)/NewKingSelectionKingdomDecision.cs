using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using PolicyOverhaul;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Election
{
    [SaveableClass(8899174)]
    public class NewKingSelectionKingdomDecision : KingdomDecision
    {
        Clan proposerClan;
        bool infiniteTerm=PolicyOverhaulSettings.Instance.InfiniteTerm;

        public NewKingSelectionKingdomDecision(Clan proposerClan) : base(proposerClan)
        {
            this.proposerClan = proposerClan;
        }



        public override bool IsAllowed()
        {
            return Campaign.Current.Models.KingdomDecisionPermissionModel.IsKingSelectionDecisionAllowed(base.Kingdom);
        }

        public override int GetProposalInfluenceCost()
        {
            return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingWar(base.Kingdom);
        }

        public override TextObject GetGeneralTitle()
        {
            TextObject textObject = new TextObject("{=ZYSGp5vO}King of {KINGDOM_NAME}", null);
            textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
            return textObject;
        }

        public override TextObject GetSupportTitle()
        {
            TextObject textObject = new TextObject("{=B0uKPW9S}Vote for the next ruler of {KINGDOM_NAME}", null);
            textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
            return textObject;
        }

        public override TextObject GetChooseTitle()
        {
            TextObject textObject = new TextObject("{=L0Oxzkfw}Choose the next ruler of {KINGDOM_NAME}", null);
            textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
            return textObject;
        }

        public override TextObject GetSupportDescription()
        {
            TextObject textObject = new TextObject("{=XGuDyJMZ}{KINGDOM_NAME} will decide who will bear the crown as the next ruler. You can pick your stance regarding this decision.", null);
            textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
            return textObject;
        }

        public override TextObject GetChooseDescription()
        {
            TextObject textObject = new TextObject("{=L0Oxzkfw}Choose the next ruler of {KINGDOM_NAME}", null);
            textObject.SetTextVariable("KINGDOM_NAME", base.Kingdom.Name);
            return textObject;
        }

        public override bool IsKingsVoteAllowed
        {
            get
            {
                return false;
            }
        }

        public override float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
        {
            float num = 1f;
            foreach (Clan clan in base.Kingdom.Clans)
            {
                if (clan.Leader != Hero.MainHero)
                {
                    num += this.CalculateMeritOfOutcomeForClan(clan, candidateOutcome);
                }
            }
            return num;
        }

        public override float CalculateMeritOfOutcomeForClan(Clan clan, DecisionOutcome candidateOutcome)
        {
            float num = 0f;
            Hero king = ((KingSelectionDecisionOutcome)candidateOutcome).King;
            if (king.Clan == base.Kingdom.RulingClan)
            {
                if (clan.Leader.GetTraitLevel(DefaultTraits.Authoritarian) > 0)
                {
                    num += 3f;
                }
                else if (clan.Leader.GetTraitLevel(DefaultTraits.Oligarchic) > 0)
                {
                    num += 2f;
                }
                else
                {
                    num += 1f;
                }
            }
            List<float> source = (from t in base.Kingdom.Clans
                                  select Campaign.Current.Models.DiplomacyModel.GetClanStrength(t) into t
                                  orderby t descending
                                  select t).ToList<float>();
            int num2 = 6;
            float num3 = (float)num2 / (source.First<float>() - source.Last<float>());
            float num4 = (float)num2 / 2f - num3 * source.First<float>();
            float num5 = Campaign.Current.Models.DiplomacyModel.GetClanStrength(king.Clan) * num3 + num4;
            num += num5;
            return MathF.Clamp(num, -3f, 8f);
        }

        public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
        {
            Dictionary<Clan, float> dictionary = new Dictionary<Clan, float>();
            foreach (Clan clan in base.Kingdom.Clans)
            {
                if(infiniteTerm)
                {
                    if (!clan.IsUnderMercenaryService)
                    {
                        dictionary.Add(clan, Campaign.Current.Models.DiplomacyModel.GetClanStrength(clan));
                    }
                }
                else
                {
                    if (!clan.IsUnderMercenaryService && clan != base.Kingdom.RulingClan)
                    {
                        dictionary.Add(clan, Campaign.Current.Models.DiplomacyModel.GetClanStrength(clan));
                    }
                }

            }
            IEnumerable<KeyValuePair<Clan, float>> enumerable = (from t in dictionary
                                                                 orderby t.Value descending
                                                                 select t).Take(3);
            foreach (KeyValuePair<Clan, float> keyValuePair in enumerable)
            {
                yield return new KingSelectionDecisionOutcome(keyValuePair.Key.Leader);
            }
            IEnumerator<KeyValuePair<Clan, float>> enumerator2 = null;
            yield break;
            yield break;
        }

        public override Clan DetermineChooser()
        {
            return base.Kingdom.RulingClan;
        }

        public override IEnumerable<Supporter> DetermineSupporters()
        {
            foreach (Clan clan in base.Kingdom.Clans)
            {
                if (clan != base.Kingdom.RulingClan && !clan.IsUnderMercenaryService)
                {
                    yield return new Supporter(clan);
                }
            }
            List<Clan>.Enumerator enumerator = default(List<Clan>.Enumerator);
            yield break;
            yield break;
        }

        public override DecisionOutcome DetermineSupport(Supporter supporter, List<DecisionOutcome> possibleOutcomes, out Supporter.SupportWeights supportWeightOfSelectedOutcome, bool calculateRelationshipEffect)
        {
            supportWeightOfSelectedOutcome = Supporter.SupportWeights.SlightlyFavor;
            DecisionOutcome decisionOutcome = null;
            float num = 0f;
            foreach (DecisionOutcome decisionOutcome2 in possibleOutcomes)
            {
                float num2 = this.CalculateMeritOfOutcomeForClan(supporter.Clan, decisionOutcome2);
                if (num2 > num)
                {
                    num = num2;
                    decisionOutcome = decisionOutcome2;
                }
            }
            supportWeightOfSelectedOutcome = base.CalculateSupportLevel(decisionOutcome, supporter.Clan, calculateRelationshipEffect);
            if (supportWeightOfSelectedOutcome == Supporter.SupportWeights.StayNeutral)
            {
                return null;
            }
            return decisionOutcome;
        }

        public override void DetermineSponsors(List<DecisionOutcome> possibleOutcomes)
        {
            foreach (DecisionOutcome decisionOutcome in possibleOutcomes)
            {
                decisionOutcome.SetSponsor(((KingSelectionDecisionOutcome)decisionOutcome).King.Clan);
            }
        }

        public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
        {
            if(proposerClan==null)
            {
                proposerClan = chosenOutcome.SponsorClan;
            }
            Clan oldRulingClan = proposerClan.Kingdom.RulingClan;
            Hero king = ((KingSelectionDecisionOutcome)chosenOutcome).King;
            if (king != king.Clan.Leader)
            {
                ChangeClanLeaderAction.ApplyWithSelectedNewLeader(king.Clan, king);
            }
            base.Kingdom.RulingClan = king.Clan;
            oldRulingClan.Banner.ChangePrimaryColor(king.Clan.Kingdom.PrimaryBannerColor);
            oldRulingClan.Banner.ChangeIconColors(king.Clan.Kingdom.SecondaryBannerColor);
            CentralizationTakeOver(king.Clan.Kingdom);

        }
        bool debug = PolicyOverhaulSettings.Instance.debugMode;

        public void CentralizationTakeOver(Kingdom kingdom)
        {
            try
            {
                if (kingdom.ActivePolicies.Contains(NewPolicies.Centralization))
                {
                    foreach (Settlement settlement in kingdom.Settlements)
                    {
                        if (settlement.OwnerClan != kingdom.RulingClan && settlement.IsTown)
                        {
                            if (settlement.OwnerClan == Clan.PlayerClan)
                            {
                                TextObject town_taken1 = new TextObject("{=town_taken1}Your {settlement_name} was confiscated by {kingdom_name}.", null);
                                town_taken1.SetTextVariable("settlement_name", settlement.Name.ToString());
                                town_taken1.SetTextVariable("kingdom_name", kingdom.Name.ToString());
                                InformationManager.DisplayMessage(new InformationMessage(town_taken1.ToString(), Colors.Red));

                                TextObject town_taken2 = new TextObject("{=town_taken2}Your {settlement_name} was confiscated by {kingdom_name}. You get {settlement_prosperity} gold and 50 renown as compensation.", null);
                                town_taken2.SetTextVariable("settlement_name", settlement.Name.ToString());
                                town_taken2.SetTextVariable("kingdom_name", kingdom.Name.ToString());
                                town_taken2.SetTextVariable("settlement_prosperity", settlement.Prosperity);
                                InformationManager.AddQuickInformation(town_taken2);
                            }
                            else
                            {
                            }
                            ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.RulingClan.Leader, settlement);
                        }
                    }

                }
            }
            catch(Exception e)
            {
                if(debug)
                {
                    GameLog.Warn("CentralizationTakeOver Error: " + e.Message);
                }
            }

        }

        public override int GetInfluenceCost(DecisionOutcome decisionOutcome, Supporter.SupportWeights supportWeight, List<Tuple<DecisionOutcome, float>> winChances)
        {
            int result;
            switch (supportWeight)
            {
                case Supporter.SupportWeights.Choose:
                    {
                        Tuple<DecisionOutcome, float> tuple = winChances.MaxBy((Tuple<DecisionOutcome, float> x) => x.Item2);
                        if (tuple.Item1 == decisionOutcome)
                        {
                            result = 0;
                        }
                        else
                        {
                            float item = winChances.Find((Tuple<DecisionOutcome, float> x) => x.Item1 == decisionOutcome).Item2;
                            result = (int)(tuple.Item2 - item) * 100 * 5;
                        }
                        break;
                    }
                case Supporter.SupportWeights.StayNeutral:
                    result = 0;
                    break;
                case Supporter.SupportWeights.SlightlyFavor:
                    result = this.GetInfluenceCostOfSupport(Supporter.SupportWeights.SlightlyFavor);
                    break;
                case Supporter.SupportWeights.StronglyFavor:
                    result = this.GetInfluenceCostOfSupport(Supporter.SupportWeights.StronglyFavor);
                    break;
                case Supporter.SupportWeights.FullyPush:
                    result = this.GetInfluenceCostOfSupport(Supporter.SupportWeights.FullyPush);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("supportWeight", supportWeight, null);
            }
            return result;
        }

        public override TextObject GetSecondaryEffects()
        {
            return new TextObject("{=!}All supporters gains some relation with each other.", null);
        }

        public override void ApplySecondaryEffects(List<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
        {
        }

        public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, KingdomDecision.SupportStatus supportStatus)
        {
            TextObject textObject;
            if (supportStatus == KingdomDecision.SupportStatus.Majority)
            {
                textObject = new TextObject("{=3be1aIJh}{KINGDOM} has elected {KING.NAME} as the new ruler with majority support.", null);
            }
            else if (supportStatus == KingdomDecision.SupportStatus.Minority)
            {
                textObject = new TextObject("{=XOZcg6Sb}{KINGDOM} has elected {KING.NAME} as the new ruler despite majority support against it.", null);
            }
            else
            {
                textObject = new TextObject("{=aE6aSLgx}{KINGDOM} has elected {KING.NAME} as the new ruler with equal support on both sides.", null);
            }
            textObject.SetTextVariable("KINGDOM", base.Kingdom.Name);
            StringHelpers.SetCharacterProperties("KING", ((KingSelectionDecisionOutcome)chosenOutcome).King.CharacterObject, null, textObject);
            return textObject;
        }

        public override List<DecisionOutcome> SortDecisionOutcomes(List<DecisionOutcome> possibleOutcomes)
        {
            return (from k in possibleOutcomes
                    orderby k.Merit descending
                    select k).ToList<DecisionOutcome>();
        }

        public override DecisionOutcome GetQueriedDecisionOutcome(List<DecisionOutcome> possibleOutcomes)
        {
            return (from k in possibleOutcomes
                    orderby k.Merit descending
                    select k).ToList<DecisionOutcome>().FirstOrDefault<DecisionOutcome>();
        }

        [SaveableClass(370202)]
        internal class KingSelectionDecisionOutcome : DecisionOutcome
        {
            public KingSelectionDecisionOutcome(Hero king)
            {
                this.King = king;
            }

            public override TextObject GetDecisionTitle()
            {
                TextObject textObject = new TextObject("{=4G3Aeqna}{KING.NAME}", null);
                StringHelpers.SetCharacterProperties("KING", this.King.CharacterObject, textObject, null);
                return textObject;
            }

            public override TextObject GetDecisionDescription()
            {
                TextObject textObject = new TextObject("{=FTjKWm8s}{KING.NAME} should rule us", null);
                StringHelpers.SetCharacterProperties("KING", this.King.CharacterObject, textObject, null);
                return textObject;
            }

            public override string GetDecisionLink()
            {
                return null;
            }

            public override ImageIdentifier GetDecisionImageIdentifier()
            {
                return null;
            }

            [SaveableField(888)]
            public readonly Hero King;
        }
    }
}
