using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using Helpers;
using HarmonyLib;
using TaleWorlds.ObjectSystem;

namespace PolicyOverhaul
{
    public static class GameLog
    {
        public static void Info(string text)
        {
            GameLog.Print(text, Color.FromUint(16777215U));
        }

        public static void Warn(string text)
        {
            GameLog.Print(text, Color.FromUint(16711680U));
        }

        private static void Print(string text, Color color)
        {
            InformationManager.DisplayMessage(new InformationMessage(text, color));
        }
    }

    public class SeparateBehaviour : CampaignBehaviorBase
    {
        private static Random rng = new Random();
        static float RebelProbability = PolicyOverhaulSettings.Instance.RebelProbability;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, OnClanTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnClanTick(Clan clan)
        {
            var kingdom = clan.Kingdom;
            if (kingdom == null
                || clan == Clan.PlayerClan
                || !clan.Leader.IsAlive
                || clan.Leader.IsPrisoner)
            {
                return;
            }
            var ruler = kingdom.Ruler;

            if (clan.Leader != ruler)
            {
                var rulerIsEnemy = clan.Leader.GetRelation(clan.Kingdom.Ruler) < -80;
                var rulerIsFriend = clan.Leader.IsFriend(ruler);
                var rulerIsDifferentCulture = clan.Culture.GetCultureCode() != ruler.Culture.GetCultureCode();
                var hasReason = rulerIsEnemy || (!rulerIsFriend && rulerIsDifferentCulture);

                var kingdomFiefs = kingdom.Settlements.Sum(x => x.IsTown ? 2 : x.IsCastle ? 1 : 0);
                var clanFiefs = clan.Settlements.Sum(x => x.IsTown ? 2 : x.IsCastle ? 1 : 0);
                var hasEnoughFiefs = (double)clanFiefs / kingdomFiefs >= 0.1;

                if (hasReason && hasEnoughFiefs)
                {
                    if(MBRandom.RandomFloatRanged(0f,1f)< RebelProbability)
                    {
                        var colors = BannerManager.ColorPalette.Values.Select(x => x.Color).ToList();
                        uint color1 = TakeColor(colors);
                        uint color2 = color1;
                        while (colors.Count > 0 && ColorDiff(color1, color2) < 0.3)
                        {
                            color2 = TakeColor(colors);
                        }

                        clan.Banner.ChangePrimaryColor(color1);
                        clan.Banner.ChangeIconColors(color2);
                        clan.Color = color1;
                        clan.Color2 = color2;
                        var rebelKingdom = GoRebelKingdom(clan);
                        
                        TextObject rebel_1 = new TextObject("{=rebel_1}{clan} left {kingdom}, and founded their own Kingdom.", null);
                        rebel_1.SetTextVariable("kingdom", kingdom.Name.ToString());
                        rebel_1.SetTextVariable("clan", clan.Name.ToString());
                        InformationManager.DisplayMessage(new InformationMessage(rebel_1.ToString()));
                    }else if((double)MBRandom.RandomFloatRanged(0f, 1f) < (double)Clan.PlayerClan.Leader.GetAttributeValue(CharacterAttributesEnum.Cunning) * 0.1 && Clan.PlayerClan.Kingdom != null && clan.Kingdom == Clan.PlayerClan.Kingdom)
                    {
                        TextObject rebel_2 = new TextObject("{=rebel_2}You heard {clan} is plotting a rebellion agaist {kingdom}.", null);
                        rebel_2.SetTextVariable("kingdom", kingdom.Name.ToString());
                        rebel_2.SetTextVariable("clan", clan.Name.ToString());
                        InformationManager.DisplayMessage(new InformationMessage(rebel_2.ToString(), Colors.Red));

                    }

                }
            }
            else
            {
                var noClans = kingdom.Clans.Where(x => x.Leader.IsAlive).Count() == 1;
                var noFiefs = clan.Settlements.Count() == 0;

                if (noClans && noFiefs)
                {
                    ClanChangeKingdom(clan, null);
                    TextObject dead_kingdom = new TextObject("{=dead_kingdom}{kingdom} does not exist anymore, so {clan} has left it.", null);
                    dead_kingdom.SetTextVariable("kingdom", kingdom.Name.ToString());
                    dead_kingdom.SetTextVariable("clan", clan.Name.ToString());
                    GameLog.Warn(dead_kingdom.ToString());
                }
            }
        }

        private uint TakeColor(List<uint> colors)
        {
            int index = rng.Next(colors.Count);
            uint color = colors[index];
            colors.RemoveAt(index);

            return color;
        }

        private double ColorDiff(uint color1, uint color2)
        {
            var gray1 = (0.2126 * (color1 >> 16 & 0xFF) + 0.7152 * (color1 >> 8 & 0xFF) + 0.0722 * (color1 & 0xFF)) / 255;
            var gray2 = (0.2126 * (color2 >> 16 & 0xFF) + 0.7152 * (color2 >> 8 & 0xFF) + 0.0722 * (color2 & 0xFF)) / 255;

            return Math.Abs(gray1 - gray2);
        }

        private string GetClanKingdomId(Clan clan)
        {
            return $"{clan.Name.ToString().ToLower()}_kingdom";
        }

        private Kingdom GoRebelKingdom(Clan clan)
        {
            string kingdomId = GetClanKingdomId(clan);
            var kingdom = Kingdom.All.SingleOrDefault(x => x.StringId == kingdomId);

            if (kingdom == null)
            {
                kingdom = MBObjectManager.Instance.CreateObject<Kingdom>(kingdomId);
                TextObject textObject = new TextObject("{=72pbZgQL}{CLAN_NAME}", null);
                textObject.SetTextVariable("CLAN_NAME", clan.Name);
                var kingdomName = "{CLAN_NAME}";
                TextObject textObject2 = new TextObject("{=EXp18CLD}" + kingdomName, null);
                textObject2.SetTextVariable("CLAN_NAME", clan.Name);
                kingdom.InitializeKingdom(textObject2, textObject, clan.Culture, clan.Banner, clan.Color, clan.Color2, clan.InitialPosition);
                kingdom.RulingClan = clan;
            }

            ClanChangeKingdom(clan, kingdom);
            if (!Kingdom.All.Contains(kingdom))
            {
                ModifyKingdomList(kingdoms => kingdoms.Add(kingdom));
            }

            return kingdom;
        }

        private void ModifyKingdomList(Action<List<Kingdom>> modificator)
        {
            List<Kingdom> kingdoms = new List<Kingdom>(Campaign.Current.Kingdoms.ToList());
            modificator(kingdoms);
            AccessTools.Field(Campaign.Current.GetType(), "_kingdoms").SetValue(Campaign.Current, new MBReadOnlyList<Kingdom>(kingdoms));
        }

        private void ClanChangeKingdom(Clan clan, Kingdom newKingdom)
        {
            Kingdom oldKingdom = clan.Kingdom;

            if (newKingdom != null)
            {
                foreach (Kingdom k in Kingdom.All)
                {
                    if (k == newKingdom || !newKingdom.IsAtWarWith(k))
                    {
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(clan, k);
                        FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(k, clan);
                    }
                }
                foreach (Clan c in Clan.All)
                {
                    if (c != clan && c.Kingdom == null && !newKingdom.IsAtWarWith(c))
                    {
                        FactionHelper.FinishAllRelatedHostileActions(clan, c);
                    }
                }
            }

            StatisticsDataLogHelper.AddLog(StatisticsDataLogHelper.LogAction.ChangeKingdomAction, new object[]
            {
                clan,
                oldKingdom,
                newKingdom,
                newKingdom == null
            });
            clan.IsUnderMercenaryService = false;
            clan.ClanLeaveKingdom(false);
            if (newKingdom != null)
            {
                clan.ClanJoinFaction(newKingdom);
                foreach (Clan c in oldKingdom.Clans)
                {
                    int relationChange = 0;
                    if (c.Leader == oldKingdom.Leader)
                    {
                        relationChange = -20;
                    }
                    else if (c.Leader.IsFriend(oldKingdom.Leader))
                    {
                        relationChange = -10;
                    }
                    else if (c.Leader.IsEnemy(oldKingdom.Leader))
                    {
                        relationChange = +10;
                    }

                    ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan.Leader, c.Leader, relationChange, true);
                }
                DeclareWarAction.Apply(oldKingdom, newKingdom);
            }
            else 
            {
                DestroyKingdomAction.Apply(oldKingdom);
                ModifyKingdomList(kingdoms => kingdoms.RemoveAll(x => x == oldKingdom));
            }

            CheckIfPartyIconIsDirty(clan, oldKingdom);
        }

        private void CheckIfPartyIconIsDirty(Clan clan, Kingdom oldKingdom)
        {
            IFaction faction;
            if (clan.Kingdom == null)
            {
                faction = clan;
            }
            else
            {
                IFaction kingdom = clan.Kingdom;
                faction = kingdom;
            }
            IFaction faction2 = faction;
            IFaction faction3 = (IFaction)oldKingdom ?? clan;
            foreach (MobileParty mobileParty in MobileParty.All)
            {
                if (mobileParty.IsVisible && ((mobileParty.Party.Owner != null && mobileParty.Party.Owner.Clan == clan) || (clan == Clan.PlayerClan && ((!FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2) && FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction3)) || (FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2) && !FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction3))))))
                {
                    mobileParty.Party.Visuals.SetMapIconAsDirty();
                }
            }
            foreach (Settlement settlement in clan.Settlements)
            {
                settlement.Party.Visuals.SetMapIconAsDirty();
            }
        }
    }
}
