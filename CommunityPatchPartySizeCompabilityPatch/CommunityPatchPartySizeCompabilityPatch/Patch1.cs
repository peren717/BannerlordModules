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
    class Patch1
    {
        [HarmonyPostfix]
        private static void Postfix(MobileParty party, StatExplainer explanation, ref int __result)
        {
			PerkObject perk = DefaultPerks.Steward.SwordsAsTribute;
			Hero hero = party.LeaderHero;
			if (hero != null)
			{
				Clan clan = hero.Clan;
				Hero hero2;
				if (clan == null)
				{
					hero2 = null;
				}
				else
				{
					Kingdom kingdom = clan.Kingdom;
					if (kingdom == null)
					{
						hero2 = null;
					}
					else
					{
						Clan rulingClan = kingdom.RulingClan;
						hero2 = ((rulingClan != null) ? rulingClan.Leader : null);
					}
				}
				if (hero2 == hero)
				{
					if (!hero.GetPerkValue(perk))
					{
						return;
					}
					Clan clan2 = hero.Clan;
					MBReadOnlyList<Clan> mbreadOnlyList;
					if (clan2 == null)
					{
						mbreadOnlyList = null;
					}
					else
					{
						Kingdom kingdom2 = clan2.Kingdom;
						mbreadOnlyList = ((kingdom2 != null) ? kingdom2.Clans : null);
					}
					MBReadOnlyList<Clan> kingdomClans = mbreadOnlyList;
					if (kingdomClans == null)
					{
						return;
					}
					int extra = (int)Math.Max(0f, (float)(kingdomClans.Count<Clan>() - 1) * perk.PrimaryBonus);
					if (extra <= 0)
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
					explainedNumber.Add((float)extra, perk.Name);
					__result = (int)explainedNumber.ResultNumber;
					return;
				}
			}
		}
    }
}
