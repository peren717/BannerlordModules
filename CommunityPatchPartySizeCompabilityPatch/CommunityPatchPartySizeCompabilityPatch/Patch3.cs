using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CommunityPatchPartySizeCompabilityPatch
{
    [HarmonyPatch(typeof(DefaultPartySizeLimitModel), "CalculateMobilePartyMemberSizeLimit")]
    class Patch3
    {
        [HarmonyPostfix]
        private static void Postfix(MobileParty party, StatExplainer explanation, ref int __result)
        {
            PerkObject perk = DefaultPerks.Steward.ManAtArms;
			Hero leaderHero = party.LeaderHero;
			if (leaderHero == null || !leaderHero.GetPerkValue(perk))
			{
				return;
			}
			if ((float)party.LeaderHero.Clan.Settlements.Count<Settlement>() * perk.PrimaryBonus < 1.401298E-45f)
			{
				return;
			}
			ExplainedNumber explainedNumber = new ExplainedNumber((float)__result, explanation, null);
			StatExplainer.ExplanationLine explanationLine;
			if (explanation == null)
			{
				explanationLine = null;
			}
			else
			{
				explanationLine = explanation.Lines.Find((StatExplainer.ExplanationLine x) => x.Name == "Base");
			}
			StatExplainer.ExplanationLine baseLine = explanationLine;
			if (baseLine != null)
			{
				explanation.Lines.Remove(baseLine);
			}
			explainedNumber.Add((float)party.LeaderHero.Clan.Settlements.Count<Settlement>() * perk.PrimaryBonus, perk.Name);
			__result = (int)explainedNumber.ResultNumber;

		}
	}
}
