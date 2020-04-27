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
            if (enableAssignenPolicy)
            {
                foreach (Kingdom kingdom in Kingdom.All)
                {
                    if (kingdom.Name.ToString() == "阿塞莱")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Polygamy);
                    }
                    else if (kingdom.Name.ToString() == "库赛特")
                    {
                        //kingdom.ActivePolicies.Add(NewPolicies.NormadicHorde);
                        kingdom.ActivePolicies.Add(NewPolicies.Slavery);
                    }
                    else if (kingdom.Name.ToString() == "北部帝国")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Republic);
                    }
                    else if (kingdom.Name.ToString() == "南部帝国")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Republic);
                        kingdom.ActivePolicies.Add(NewPolicies.Feudalism);
                        kingdom.ActivePolicies.Add(DefaultPolicies.FeudalInheritance);

                    }
                    else if (kingdom.Name.ToString() == "西部帝国")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Republic);
                        kingdom.ActivePolicies.Add(NewPolicies.Tyrant);

                    }
                    else if (kingdom.Name.ToString() == "瓦兰迪亚")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Vassalism);
                    }
                    else if (kingdom.Name.ToString() == "斯特吉亚")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.WarFury);
                    }
                    else if (kingdom.Name.ToString() == "巴旦尼亚")
                    {
                    }


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

            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData<int>("clock", ref this.clock);
            dataStore.SyncData<Dictionary<string, int>>("timerList", ref this.timerList);
            dataStore.SyncData<Dictionary<string, int>>("timerList2", ref this.timerList2);

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
                        if (settlement.OwnerClan != kingdom.RulingClan && settlement.IsTown)
                        {
                            if (settlement.OwnerClan == Clan.PlayerClan)
                            {
                                InformationManager.DisplayMessage(new InformationMessage("你的" + settlement.Name + "已被收归" + kingdom.Name + "所有", Colors.Red));
                                InformationManager.AddQuickInformation(new TextObject("你的" + settlement.Name + "已被收归" + kingdom.Name + "所有。作为补偿你获得" + settlement.Prosperity + "金币和50点声望。", null));
                                settlement.OwnerClan.AddRenown(50);
                                GiveGoldAction.ApplyBetweenCharacters(kingdom.RulingClan.Leader, Hero.MainHero, (int)settlement.Prosperity, false);

                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(settlement.Name + "已被收归" + kingdom.Name + "所有", Colors.Gray));
                            }
                            ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.RulingClan.Leader, settlement);


                        }
                    }

                }
            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage("Error in DailyTickSettlementEvent" + e.Message, Colors.Red));
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
                                AssignNewLeader(clan);
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
                            if (MBRandom.RandomFloatRanged(0f, 1f) < 0.1 * Math.Max(otherClan.Leader.GetAttributeValue(CharacterAttributesEnum.Vigor), otherClan.Leader.GetAttributeValue(CharacterAttributesEnum.Endurance)))
                            {
                                if (otherClan.Leader.IsHumanPlayerCharacter)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "被不明人士袭击。经过拷问，袭击者受雇于" + kingdom.Name.ToString() + "的" + clan.Name.ToString(), Colors.Red));
                                    InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "的人趁你身边护卫不在时袭击了你。经过搏斗你索性生还。", null));
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "被不明人士袭击。经过拷问，袭击者受雇于" + kingdom.Name.ToString() + "的" + clan.Name.ToString()));
                                }
                            }
                            else
                            {
                                if (otherClan.Leader.IsHumanPlayerCharacter)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "被不明人士袭击。经过拷问，袭击者受雇于" + kingdom.Name.ToString() + "的" + clan.Name.ToString(), Colors.Red));
                                    InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "的人趁你身边护卫不在时袭击了你。经过搏斗你身受重伤。", null));
                                    otherClan.Leader.HitPoints = 1;
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "被不明人士袭击。经过拷问，袭击者受雇于" + kingdom.Name.ToString() + "的" + clan.Name.ToString()));
                                }
                            }
                            return;
                        }

                        if (clan.Leader.GetRelation(otherClan.Leader) < -50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                        {
                            clan.Influence -= 5;
                            otherClan.Influence -= 5;
                            if (otherClan.Leader.IsHumanPlayerCharacter)
                            {
                                InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "在其他领主面前羞辱了" + otherClan.Leader.ToString(), Colors.Red));
                                InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "在其他领主面前羞辱了你。你的影响力减5。", null));
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(kingdom.Name.ToString() + "的" + clan.Leader.ToString() + "公开羞辱了" + otherClan.Name.ToString(), Colors.Gray));
                            }

                            return;
                        }

                        if (clan.Leader.GetRelation(otherClan.Leader) < -50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                        {
                            if (otherClan == kingdom.RulingClan && !kingdom.ActivePolicies.Contains(NewPolicies.ConstitutionaMonarchy) && clan.Influence > 200)
                            {
                                clan.Influence -= 50;
                                Campaign.Current.AddDecision(new KingdomPolicyDecision(clan, NewPolicies.ConstitutionaMonarchy, false), true);
                            }
                            return;
                        }

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
                            if (clan.Leader.GetRelation(otherClan.Leader) > 50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                            {
                                clan.Influence -= 50;
                                otherClan.Influence += 10;
                                if (otherClan.Leader.IsHumanPlayerCharacter)
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "公开表示对" + otherClan.Leader.ToString() + "的政治支持。", Colors.Green));
                                    InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "公开表示对你的政治支持。你的影响力加10。", null));
                                }
                                else
                                {
                                    InformationManager.DisplayMessage(new InformationMessage(kingdom.Name.ToString() + "的" + clan.Leader.ToString() + "对" + otherClan.Name.ToString() + "表示政治上支持。", Colors.Gray));
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
                    if (kingdom.RulingClan.Leader.IsHumanPlayerCharacter)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(kingdom.Leader.Name + "借用军事力量夺取了" + kingdom.Name + "的政权，成为新任僭主。", Colors.Green));
                        InformationManager.AddQuickInformation(new TextObject("你借用军事力量夺取了" + kingdom.Name + "的政权，成为新任僭主。", null));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage(kingdom.RulingClan.Leader.ToString() + "借用军事力量夺取了" + kingdom.Name + "的政权，成为新任僭主。"));
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


