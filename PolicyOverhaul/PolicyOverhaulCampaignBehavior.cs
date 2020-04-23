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

namespace PolicyOverhaul
{
    class PolicyOverhaulCampaignBehavior : CampaignBehaviorBase
    {
        private Dictionary<string, int> timerList = new Dictionary<string, int>();
        int clock = 0;
        int electionCycle = PolicyOverhaulSettings.Instance.ElectionCycle;
        float wantToActProbability = 0.05f;
        float actProbability = 0.2f;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.NewPolicyDailyTickClan));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterNewGameCreated));


        }

        private void OnAfterNewGameCreated(CampaignGameStarter campaignGameStarter)
        {
            foreach (Kingdom kingdom in Kingdom.All)
            {
                if (kingdom.Name.ToString() == "阿塞莱")
                {
                    kingdom.ActivePolicies.Add(NewPolicies.Polygamy);
                }
                else if (kingdom.Name.ToString() == "库赛特")
                {
                    kingdom.ActivePolicies.Add(NewPolicies.Slavery);
                }
                else if (kingdom.Name.ToString() == "北部帝国")
                {
                    kingdom.ActivePolicies.Add(NewPolicies.Republic);
                }
                else if (kingdom.Name.ToString() == "南部帝国")
                {
                    kingdom.ActivePolicies.Add(NewPolicies.Feudalism);
                }
                else if (kingdom.Name.ToString() == "瓦兰迪亚")
                {
                    kingdom.ActivePolicies.Add(NewPolicies.Vassalism);
                }
                else if (kingdom.Name.ToString() == "斯特吉亚")
                {
                    kingdom.ActivePolicies.Add(NewPolicies.WarFury);
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
            }
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData<int>("clock", ref this.clock);
            dataStore.SyncData<Dictionary<string, int>>("timerList", ref this.timerList);
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
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Republic))
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



                }

            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage("Error in NewPolicyDailyTickClan", Colors.Red));
            }
        }
        private void HarmEnemy(Clan clan)
        {
            Kingdom kingdom = clan.Kingdom;
            if (MBRandom.RandomFloatRanged(0f, 1f) < wantToActProbability)
            {
                foreach (Clan otherClan in Clan.All)
                {

                    if (clan.Leader.GetRelation(otherClan.Leader) < -50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                    {
                        otherClan.Influence -= 1;
                        if (otherClan.Leader.IsHumanPlayerCharacter)
                        {
                            InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "在其他领主面前羞辱了" + otherClan.Leader.ToString(), Colors.Red));
                            InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "在其他领主面前羞辱了你。", null));
                        }
                        else
                        {
                            InformationManager.DisplayMessage(new InformationMessage(kingdom.Name.ToString() + "的" + clan.Leader.ToString() + "公开羞辱了" + otherClan.Name.ToString()));
                        }
                        return;
                    }

                }
            }
        }

        private void SupportFriend(Clan clan)
        {
            Kingdom kingdom = clan.Kingdom;
            if (MBRandom.RandomFloatRanged(0f, 1f) < wantToActProbability)
            {
                foreach (Clan otherClan in kingdom.Clans)
                {
                    if (clan.InfluenceChange > 0 && clan.Influence > 100)
                    {
                        if (clan.Leader.GetRelation(otherClan.Leader) > 50 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                        {
                            clan.Influence -= 50;
                            otherClan.Influence += 10;
                            if (otherClan.Leader.IsHumanPlayerCharacter)
                            {
                                InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "公开表示对" + otherClan.Leader.ToString() + "的政治支持。", Colors.Green));
                                InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "公开表示对你的政治支持。", null));
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(kingdom.Name.ToString() + "的" + clan.Leader.ToString() + "对" + otherClan.Name.ToString() + "表示政治上支持。"));
                            }
                            return;
                        }
                    }
                }
            }

        }

        private void AssignNewLeader(Clan clan)
        {
            Campaign.Current.AddDecision(new NewKingSelectionKingdomDecision(clan), true);
        }

        static Clan GetMostInfluencialClan(Kingdom kingdom)
        {
            float zero = 0;
            float max = -1 / zero;
            Clan result = null;
            foreach (Clan clan in kingdom.Clans)
            {
                if (clan.Influence > max)
                {
                    result = clan;
                    max = clan.Influence;
                }
            }
            return result;
        }


    }

}
