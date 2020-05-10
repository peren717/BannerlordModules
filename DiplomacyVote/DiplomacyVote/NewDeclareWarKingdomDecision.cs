using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace DiplomacyVote
{
	[SaveableClass(8899174)]
	public class NewDeclareWarKingdomDecision : KingdomDecision
	{
		public NewDeclareWarKingdomDecision(Clan proposerClan, Kingdom kingdomToDeclareWarOn) : base(proposerClan)
		{
			this.AggressorKingdom = proposerClan.Kingdom;
			this.KingdomToDeclareWarOn = kingdomToDeclareWarOn;
		}

		public override bool IsAllowed()
		{
			return Campaign.Current.Models.KingdomDecisionPermissionModel.IsWarDecisionAllowedBetweenKingdoms(this.AggressorKingdom, this.KingdomToDeclareWarOn);
		}

		public override int GetProposalInfluenceCost()
		{
			return Campaign.Current.Models.DiplomacyModel.GetInfluenceCostOfProposingWar(base.Kingdom);
		}

		public override TextObject GetGeneralTitle()
		{
			TextObject textObject = new TextObject("{=rtfoywJl}Declare war on {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", this.KingdomToDeclareWarOn.Name);
			return textObject;
		}

		public override TextObject GetSupportTitle()
		{
			TextObject textObject = new TextObject("{=xM97H0oR}Vote for declaring war on {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", this.KingdomToDeclareWarOn.Name);
			return textObject;
		}

		public override TextObject GetChooseTitle()
		{
			TextObject textObject = new TextObject("{=aQAI99d4}Declaring War On {KINGDOM_NAME}", null);
			textObject.SetTextVariable("KINGDOM_NAME", this.KingdomToDeclareWarOn.Name);
			return textObject;
		}

		public override TextObject GetSupportDescription()
		{
			TextObject textObject = new TextObject("{=KSrNutEO}{FACTION_LEADER} will decide if war will be declared on {KINGDOM_NAME}. You can pick your stance regarding this decision.", null);
			textObject.SetTextVariable("FACTION_LEADER", this.DetermineChooser().Leader.Name);
			textObject.SetTextVariable("KINGDOM_NAME", this.KingdomToDeclareWarOn.Name);
			return textObject;
		}

		public override TextObject GetChooseDescription()
		{
			TextObject textObject = new TextObject("{=4JSzHkpt}As {?IS_FEMALE}queen{?}king{\\?} you must decide if war will be declared on {KINGDOM_NAME}", null);
			textObject.SetTextVariable("IS_FEMALE", this.DetermineChooser().Leader.IsFemale ? 1 : 0);
			textObject.SetTextVariable("KINGDOM_NAME", this.KingdomToDeclareWarOn.Name);
			return textObject;
		}

		public override float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
		{
			return 1f;
		}

		public override float CalculateMeritOfOutcomeForClan(Clan clan, DecisionOutcome candidateOutcome)
		{
			return 0f;
		}

		public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
		{
			yield return new NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome(true);
			yield return new NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome(false);
			yield break;
		}

		public override Clan DetermineChooser()
		{
			return this.AggressorKingdom.RulingClan;
		}

		public override IEnumerable<Supporter> DetermineSupporters()
		{
			foreach (Clan clan in this.AggressorKingdom.Clans)
			{
				if (clan != this.AggressorKingdom.RulingClan && !clan.IsUnderMercenaryService)
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
				if (((NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome)decisionOutcome).ShouldWarBeDeclared)
				{
					decisionOutcome.SetSponsor(base.ProposerClan);
				}
				else
				{
					base.AssignDefaultSponsor(decisionOutcome);
				}
			}
		}

		public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
		{
			if (((NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome)chosenOutcome).ShouldWarBeDeclared)
			{
				DeclareWarAction.Apply(this.AggressorKingdom, this.KingdomToDeclareWarOn);
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
			if (((NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome)chosenOutcome).ShouldWarBeDeclared)
			{
				if (supportStatus == KingdomDecision.SupportStatus.Majority)
				{
					textObject = new TextObject("{=cRpsjvOM}{AGGRESSOR_KINGDOM} has declared war on {KINGDOM} with majority support.", null);
				}
				else if (supportStatus == KingdomDecision.SupportStatus.Minority)
				{
					textObject = new TextObject("{=DphlPaF9}{AGGRESSOR_KINGDOM} has declared war on {KINGDOM} despite majority support against it.", null);
				}
				else
				{
					textObject = new TextObject("{=RH8mgLEk}{AGGRESSOR_KINGDOM} has declared war on {KINGDOM} with equal support on both sides.", null);
				}
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Majority)
			{
				textObject = new TextObject("{=8RBZaQKk}The vote to declare war on {KINGDOM} has failed with majority support.", null);
			}
			else if (supportStatus == KingdomDecision.SupportStatus.Minority)
			{
				textObject = new TextObject("{=XaZyU3Cn}The vote to declare war on {KINGDOM} has failed despite majority support against it.", null);
			}
			else
			{
				textObject = new TextObject("{=5L0wDGhR}The vote to declare war on {KINGDOM} has failed with equal support on both sides.", null);
			}
			textObject.SetTextVariable("AGGRESSOR_KINGDOM", this.AggressorKingdom.Name);
			textObject.SetTextVariable("KINGDOM", this.KingdomToDeclareWarOn.Name);
			return textObject;
		}

		public override List<DecisionOutcome> SortDecisionOutcomes(List<DecisionOutcome> possibleOutcomes)
		{
			return (from k in possibleOutcomes
					orderby ((NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome)k).ShouldWarBeDeclared descending
					select k).ToList<DecisionOutcome>();
		}

		public override DecisionOutcome GetQueriedDecisionOutcome(List<DecisionOutcome> possibleOutcomes)
		{
			return possibleOutcomes.FirstOrDefault((DecisionOutcome t) => ((NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome)t).ShouldWarBeDeclared);
		}

		[SaveableField(100)]
		public readonly Kingdom AggressorKingdom;

		[SaveableField(101)]
		public readonly Kingdom KingdomToDeclareWarOn;

		[SaveableClass(370202)]
		internal class DeclareWarDecisionOutcome : DecisionOutcome
		{
			public DeclareWarDecisionOutcome(bool shouldWarBeDeclared)
			{
				this.ShouldWarBeDeclared = shouldWarBeDeclared;
			}

			public override TextObject GetDecisionTitle()
			{
				TextObject textObject = new TextObject("{=kakxnaN5}{?SUPPORT}Yes{?}No{\\?}", null);
				textObject.SetTextVariable("SUPPORT", this.ShouldWarBeDeclared ? 1 : 0);
				return textObject;
			}

			public override TextObject GetDecisionDescription()
			{
				if (this.ShouldWarBeDeclared)
				{
					return new TextObject("{=w9olmhv0}It is time to declare war", null);
				}
				return new TextObject("{=epnk9qIt}We oppose a declaration of war", null);
			}

			public override string GetDecisionLink()
			{
				return null;
			}

			public override ImageIdentifier GetDecisionImageIdentifier()
			{
				return null;
			}

			[SaveableField(100)]
			public readonly bool ShouldWarBeDeclared;
		}
	}
}
