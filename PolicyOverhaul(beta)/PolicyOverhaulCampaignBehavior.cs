using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Engine;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Actions;

namespace PolicyOverhaul
{
    class PolicyOverhaulCampaignBehavior : CampaignBehaviorBase
    {
        private Dictionary<string, int> timerList = new Dictionary<string, int>();
        private Dictionary<string, int> timerList2 = new Dictionary<string, int>();
        private Dictionary<string, int> timerList3 = new Dictionary<string, int>();


        int clock = 0;
        readonly int electionCycle = PolicyOverhaulSettings.Instance.ElectionCycle;
        readonly float wantToActProbability = 0.05f;
        readonly float actProbability = PolicyOverhaulSettings.Instance.ActProbablity;
        readonly bool enableAssignenPolicy = PolicyOverhaulSettings.Instance.EnablePreAssignedPolicies;
        bool debug = PolicyOverhaulSettings.Instance.debugMode;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.NewPolicyDailyTickClan));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlementEvent));


        }


        private void OnAfterNewGameCreated(CampaignGameStarter campaignGameStarter)
        {
            try
            {
                if (enableAssignenPolicy)
                {
                    foreach (Kingdom kingdom in Kingdom.All)
                    {
                        if (kingdom.Name.ToString() == "阿塞莱" || kingdom.Name.ToString() == "Aserai")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.Polygamy);
                            kingdom.ActivePolicies.Add(NewPolicies.BigCaravan);
                        }
                        else if (kingdom.Name.ToString() == "库赛特" || kingdom.Name.ToString() == "Khuzait")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.NormadicHorde);
                        }
                        else if (kingdom.Name.ToString() == "北部帝国" || kingdom.Name.ToString() == "Northern Empire")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.Republic);
                            kingdom.ActivePolicies.Add(DefaultPolicies.Senate);
                            kingdom.ActivePolicies.Add(DefaultPolicies.Citizenship);
                        }
                        else if (kingdom.Name.ToString() == "南部帝国" || kingdom.Name.ToString() == "Southern Empire")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.Centralization);
                            kingdom.ActivePolicies.Add(DefaultPolicies.Citizenship);

                        }
                        else if (kingdom.Name.ToString() == "西部帝国" || kingdom.Name.ToString() == "Western Empire")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.Tyrant);
                            kingdom.ActivePolicies.Add(DefaultPolicies.Citizenship);
                        }
                        else if (kingdom.Name.ToString() == "瓦兰迪亚" || kingdom.Name.ToString() == "Vlandia")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.Feudalism);
                            kingdom.ActivePolicies.Add(NewPolicies.Vassalism);
                        }
                        else if (kingdom.Name.ToString() == "斯特吉亚" || kingdom.Name.ToString() == "Sturgia")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.WarFury);
                        }
                        else if (kingdom.Name.ToString() == "巴旦尼亚" || kingdom.Name.ToString() == "Battania")
                        {
                            kingdom.ActivePolicies.Add(NewPolicies.TrainedCivilian);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if(debug)
                {
                    GameLog.Warn("Default Policy not set error due to:" + e.Message);
                }
            }


        }

        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            foreach (Kingdom kingdom in Kingdom.All)
            {
                if (!this.timerList.ContainsKey(kingdom.Name.ToString()))
                {
                    timerList.Add(kingdom.Name.ToString(), 0);
                }
                if (!this.timerList2.ContainsKey(kingdom.Name.ToString()))
                {
                    timerList2.Add(kingdom.Name.ToString(), 0);
                }
                if (!this.timerList3.ContainsKey(kingdom.Name.ToString()))
                {
                    timerList3.Add(kingdom.Name.ToString(), 0);
                }

            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData<int>("clock", ref this.clock);
            dataStore.SyncData<Dictionary<string, int>>("timerList", ref this.timerList);
            dataStore.SyncData<Dictionary<string, int>>("timerList2", ref this.timerList2);
            dataStore.SyncData<Dictionary<string, int>>("timerList3", ref this.timerList3);


        }

        private void DailyTickSettlementEvent(Settlement settlement)
        {
            try
            {
                if (settlement.OwnerClan != null && settlement.OwnerClan.Kingdom != null)
                {
                    Kingdom kingdom = settlement.OwnerClan.Kingdom;
                    if (kingdom.ActivePolicies.Contains(NewPolicies.Centralization))
                    {
                        if (settlement.OwnerClan != kingdom.RulingClan)
                        {
                            if (settlement.OwnerClan == Clan.PlayerClan)
                            {
                                if (settlement.IsTown)
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
                                    settlement.OwnerClan.AddRenown(50);
                                    GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, (int)settlement.Prosperity, false);
                                    ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.RulingClan.Leader, settlement);
                                }


                            }
                            else
                            {
                                if (settlement.IsTown)
                                {
                                    TextObject town_taken3 = new TextObject("{=town_taken3}{settlement_name} was confiscated by {kingdom_name}.", null);
                                    town_taken3.SetTextVariable("settlement_name", settlement.Name.ToString());
                                    town_taken3.SetTextVariable("kingdom_name", kingdom.Name.ToString());
                                    GiveGoldAction.ApplyBetweenCharacters(null, settlement.OwnerClan.Leader, (int)settlement.Prosperity, false);
                                    InformationManager.DisplayMessage(new InformationMessage(town_taken3.ToString(), Colors.Gray));
                                    ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.RulingClan.Leader, settlement);
                                    if(settlement.OwnerClan!=Clan.PlayerClan)
                                    {
                                        settlement.OwnerClan.Leader.SetPersonalRelation(kingdom.Leader, settlement.OwnerClan.Leader.GetRelation(kingdom.Leader) - 10);
                                    }
                                }

                            }


                        }
                    }


                }
            }
            catch (Exception e)
            {
                if(debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Error in DailyTickSettlementEvent" + e.Message, Colors.Red));
                }
            }


        }

        private void OnDailyTick()
        {
            clock++;
        }


        private void NewPolicyDailyTickClan(Clan clan)
        {
            try
            {
                bool clanHasKingdom = clan.Kingdom != null;
                bool isPlayer = clan.Leader.IsHumanPlayerCharacter;
                if (clanHasKingdom)
                {
                    if (!isPlayer)
                    {
                        SupportFriend(clan);
                        HarmEnemy(clan);
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Republic) && !clan.Kingdom.ActivePolicies.Contains(NewPolicies.Tyrant) && !clan.Kingdom.ActivePolicies.Contains(DefaultPolicies.FeudalInheritance))
                    {
                        if (timerList.TryGetValue(clan.Kingdom.Name.ToString(), out int t))
                        {
                            if (clock - timerList[clan.Kingdom.Name.ToString()] > electionCycle)
                            {
                                timerList[clan.Kingdom.Name.ToString()] = clock;
                                AssignNewLeader(clan.Kingdom.RulingClan);
                            }
                        }
                        else
                        {
                            timerList.Add(clan.Kingdom.Name.ToString(), clock);
                        }
                    }
                    ///////////////////////////////////////////////////////////////////////////////
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Tyrant))
                    {
                        if (timerList2.TryGetValue(clan.Kingdom.Name.ToString(), out int t))
                        {
                            if (clock - timerList2[clan.Kingdom.Name.ToString()] > electionCycle)
                            {
                                timerList2[clan.Kingdom.Name.ToString()] = clock;
                                GetNextTyrant(clan.Kingdom);
                            }
                        }
                        else
                        {
                            timerList2.Add(clan.Kingdom.Name.ToString(), clock);
                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Centralization))
                    {
                        if (timerList3.TryGetValue(clan.Kingdom.Name.ToString(), out int t))
                        {
                            if (clock - timerList3[clan.Kingdom.Name.ToString()] > electionCycle)
                            {
                                timerList3[clan.Kingdom.Name.ToString()] = clock;
                                float freeInfluence = 0;
                                foreach (Settlement settlement in clan.Kingdom.Settlements)
                                {
                                    int totalCastle = 0;
                                    if (settlement.IsCastle && !settlement.IsUnderSiege)
                                    {
                                        Campaign.Current.AddDecision(new SettlementClaimantDecision(clan.Kingdom.RulingClan, settlement, settlement.OwnerClan.Leader, clan.Kingdom.RulingClan, 24), true);
                                        totalCastle++;
                                    }
                                    freeInfluence = totalCastle * 10;

                                    foreach (Clan c in clan.Kingdom.Clans)
                                    {
                                        if (!c.IsUnderMercenaryService)
                                        {
                                            c.Influence += freeInfluence;
                                        }
                                    }
                                }
                                if (Clan.PlayerClan.Kingdom != null && !Clan.PlayerClan.IsUnderMercenaryService && clan.Kingdom == Clan.PlayerClan.Kingdom)
                                {
                                    InformationManager.AddQuickInformation(new TextObject(clan.Kingdom.Name.ToString() + "{=castle_election} starts re-assigning their castles. You get some influence to vote.", null));
                                }
                            }
                        }
                        else
                        {
                            timerList3.Add(clan.Kingdom.Name.ToString(), clock);
                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Vassalism))
                    {

                        foreach (Settlement settlement in clan.Kingdom.Settlements)
                        {
                            foreach (Hero otherHero in settlement.Notables)
                            {
                                clan.Leader.SetPersonalRelation(otherHero, 0);
                            }
                        }
                        foreach (Settlement settlement in clan.Settlements)
                        {
                            foreach (Hero otherHero in settlement.Notables)
                            {
                                clan.Leader.SetPersonalRelation(otherHero, 100);
                            }
                        }
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Centralization) && clan != clan.Kingdom.RulingClan && clan != Clan.PlayerClan)
                    {
                        if (clan.Kingdom.RulingClan != Clan.PlayerClan)
                        {
                            GiveGoldAction.ApplyBetweenCharacters(clan.Kingdom.RulingClan.Leader, clan.Leader, clan.Tier * 1000);
                            if (clan.Kingdom.RulingClan.Leader.Gold < 1000)
                            {
                                GiveGoldAction.ApplyBetweenCharacters(null, clan.Kingdom.RulingClan.Leader, 1000);
                            }
                        }
                        else
                        {
                            GiveGoldAction.ApplyBetweenCharacters(null, clan.Leader, (int)(clan.Tier * 500 + clan.Influence));

                        }

                    }



                }

            }
            catch (Exception e)
            {
                if (debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Error in NewPolicyDailyTickClan" + e.Message, Colors.Red));
                }
            }
        }
        private void HarmEnemy(Clan clan)
        {
            try
            {
                Kingdom kingdom = clan.Kingdom;
                if (MBRandom.RandomFloatRanged(0f, 1f) < wantToActProbability)
                {
                    foreach (Clan otherClan in Clan.All)
                    {
                        if (clan.Leader.GetRelation(otherClan.Leader) < -90 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                        {
                            TextObject assasin_1 = new TextObject("{=assasin_1}{otherLeader} is attacked by someone sent by {leader} of {kingdom}.", null);
                            assasin_1.SetTextVariable("leader", clan.Leader.Name.ToString());
                            assasin_1.SetTextVariable("otherLeader", otherClan.Leader.Name.ToString());
                            assasin_1.SetTextVariable("kingdom", clan.Kingdom.Name.ToString());
                            if (MBRandom.RandomFloatRanged(0f, 1f) < 0.15 * Math.Max(otherClan.Leader.GetAttributeValue(CharacterAttributesEnum.Vigor), otherClan.Leader.GetAttributeValue(CharacterAttributesEnum.Endurance)))
                            {
                                if (otherClan.Leader.IsHumanPlayerCharacter)
                                {

                                    InformationManager.DisplayMessage(new InformationMessage(assasin_1.ToString(), Colors.Red));

                                    TextObject assasin_2 = new TextObject("{=assasin_2}You are attacked by someone sent by {leader} of {kingdom}. Luckly you survived.", null);
                                    assasin_2.SetTextVariable("leader", clan.Leader.Name.ToString());
                                    assasin_2.SetTextVariable("kingdom", clan.Kingdom.Name.ToString());
                                    InformationManager.AddQuickInformation(assasin_2);
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(assasin_1.ToString(), Colors.Gray));
                                }
                            }
                            else
                            {
                                if (otherClan.Leader.IsHumanPlayerCharacter)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(assasin_1.ToString(), Colors.Red));

                                    TextObject assasin_3 = new TextObject("{=assasin_3}You are attacked by someone sent by {leader} of {kingdom}. You were gravely wounded.", null);
                                    assasin_3.SetTextVariable("leader", clan.Leader.Name.ToString());
                                    assasin_3.SetTextVariable("kingdom", clan.Kingdom.Name.ToString());
                                    InformationManager.AddQuickInformation(assasin_3);
                                    otherClan.Leader.HitPoints = 1;
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(assasin_1.ToString(), Colors.Gray));
                                }
                            }
                            return;
                        }

                        TextObject insult_1 = new TextObject("{=insult_1}{leader} spreads rumors about {otherLeader}.", null);
                        insult_1.SetTextVariable("leader", clan.Leader.Name.ToString());
                        insult_1.SetTextVariable("otherLeader", otherClan.Leader.Name.ToString());
                        if (clan.Leader.GetRelation(otherClan.Leader) < -50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                        {
                            clan.Influence -= 5;
                            otherClan.Influence -= 5;
                            if (otherClan.Leader.IsHumanPlayerCharacter)
                            {
                                InformationManager.DisplayMessage(new InformationMessage(insult_1.ToString(), Colors.Red));

                                TextObject insult_2 = new TextObject("{=insult_1}{leader} spreads rumors about you, so you lose 5 infulence.", null);
                                insult_2.SetTextVariable("leader", clan.Leader.Name.ToString());
                                InformationManager.AddQuickInformation(insult_2);
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(insult_1.ToString(), Colors.Gray));
                            }

                            return;
                        }

                        RaisePolicy(clan, otherClan, NewPolicies.ConstitutionaMonarchy);
                        RaisePolicy(clan, otherClan, NewPolicies.HouseOfLords);
                        RaisePolicy(clan, otherClan, NewPolicies.Republic);


                    }
                }
            }
            catch (Exception e)
            {
                if (debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Error in HarmEnemy" + e.Message, Colors.Red));
                }
            }


        }


        private void RaisePolicy(Clan clan, Clan otherClan, PolicyObject policy)
        {
            Kingdom kingdom = clan.Kingdom;
            if (clan.Leader.GetRelation(otherClan.Leader) < -50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
            {
                if (otherClan == kingdom.RulingClan && !kingdom.ActivePolicies.Contains(policy) && clan.Influence > 500)
                {
                    clan.Influence -= 50;
                    Campaign.Current.AddDecision(new KingdomPolicyDecision(clan, policy, false), true);
                }
                return;
            }
        }

        private void SupportFriend(Clan clan)
        {
            try
            {
                Kingdom kingdom = clan.Kingdom;
                if (MBRandom.RandomFloatRanged(0f, 1f) < wantToActProbability)
                {
                    foreach (Clan otherClan in kingdom.Clans)
                    {
                        if (clan.InfluenceChange > 0 && clan.Influence > 200)
                        {
                            if (clan == kingdom.RulingClan && kingdom.ActivePolicies.Contains(NewPolicies.Centralization) && clan.Leader.GetRelation(otherClan.Leader) > 50 && MBRandom.RandomFloatRanged(0f, 1f) < 0.8 && clan.Influence > 1000)
                            {
                                if (clan.Leader.GetRelation(otherClan.Leader) > 50)
                                {
                                    clan.Influence -= 100;
                                    otherClan.Influence += 100;
                                    if (otherClan != Clan.PlayerClan)
                                    {
                                        clan.Leader.SetPersonalRelation(otherClan.Leader, clan.Leader.GetRelation(otherClan.Leader) + 10);
                                    }
                                    TextObject support_1 = new TextObject("{=support_1}{leader} offered political support to {otherLeader}.", null);
                                    support_1.SetTextVariable("leader", clan.Leader.Name.ToString());
                                    support_1.SetTextVariable("otherLeader", otherClan.Leader.Name.ToString());
                                    if (otherClan.Leader.IsHumanPlayerCharacter)
                                    {
                                        InformationManager.DisplayMessage(new InformationMessage(support_1.ToString(), Colors.Green));

                                        TextObject support_2 = new TextObject("{=support_2}{leader} offered you political supports. You gained some influence.", null);
                                        support_2.SetTextVariable("leader", clan.Leader.Name.ToString());
                                        InformationManager.AddQuickInformation(new TextObject(support_2.ToString(), null));
                                    }
                                    else
                                    {
                                        InformationManager.DisplayMessage(new InformationMessage(support_1.ToString(), Colors.Gray));
                                    }
                                    return;
                                }
                            }
                            if (clan.Leader.GetRelation(otherClan.Leader) > 50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                            {
                                clan.Influence -= 25;
                                otherClan.Influence += 10;
                                if(otherClan != Clan.PlayerClan)
                                {
                                    clan.Leader.SetPersonalRelation(otherClan.Leader, clan.Leader.GetRelation(otherClan.Leader) + 1);
                                }
                                TextObject support_1 = new TextObject("{=support_1}{leader} offered political support to {otherLeader}.", null);
                                support_1.SetTextVariable("leader", clan.Leader.Name.ToString());
                                support_1.SetTextVariable("otherLeader", otherClan.Leader.Name.ToString());
                                if (otherClan.Leader.IsHumanPlayerCharacter)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(support_1.ToString(), Colors.Green));

                                    TextObject support_2 = new TextObject("{=support_2}{leader} offered you political supports. You gained some influence.", null);
                                    support_2.SetTextVariable("leader", clan.Leader.Name.ToString());
                                    InformationManager.AddQuickInformation(new TextObject(support_2.ToString(), null));
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(support_1.ToString(), Colors.Gray));
                                }
                                return;
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Error in SupportFriend" + e.Message, Colors.Red));
                }
            }


        }

        private void AssignNewLeader(Clan clan)
        {
            Campaign.Current.AddDecision(new NewKingSelectionKingdomDecision(clan), true);
        }

        private void GetNextTyrant(Kingdom kingdom)
        {
            try
            {
                Clan oldRuler = kingdom.RulingClan;
                float zero = 0;
                float max = -1 / zero;
                Clan result = null;
                foreach (Clan clan in kingdom.Clans)
                {
                    int totalSize = 0;
                    if (clan.Parties.Any())
                    {
                        foreach (MobileParty mobileParty in clan.Parties)
                        {
                            totalSize += mobileParty.Party.PartySizeLimit;
                        }
                    }
                    if (clan.Influence + totalSize > max)
                    {
                        result = clan;
                        max = clan.Influence + totalSize;
                    }
                }
                kingdom.RulingClan = result;
                oldRuler.Banner.ChangePrimaryColor(kingdom.PrimaryBannerColor);
                oldRuler.Banner.ChangeIconColors(kingdom.SecondaryBannerColor);
                if (oldRuler != kingdom.RulingClan)
                {
                    TextObject tyrant_1 = new TextObject("{=tyrant_1}{leader} has seized the throne of {kingdom} by military strength.", null);
                    tyrant_1.SetTextVariable("leader", kingdom.Leader.Name.ToString());
                    tyrant_1.SetTextVariable("kingdom", kingdom.Name.ToString());
                    if (kingdom.RulingClan.Leader.IsHumanPlayerCharacter)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(tyrant_1.ToString(), Colors.Green));

                        TextObject tyrant_2 = new TextObject("{=tyrant_2}You have seized the throne of {kingdom} by military strength.", null);
                        tyrant_2.SetTextVariable("kingdom", kingdom.Name.ToString());
                        InformationManager.AddQuickInformation(tyrant_2);
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage(tyrant_1.ToString()));
                    }
                }
            }
            catch (Exception e)
            {
                if (debug)
                {
                    InformationManager.DisplayMessage(new InformationMessage("Error in GetNextTyrant" + e.Message, Colors.Red));
                }
            }

        }



    }


}


