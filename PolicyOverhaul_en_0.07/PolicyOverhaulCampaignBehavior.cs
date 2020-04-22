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
        int electionCycle = 30;
        float wantToSupportProbability = 0.2f;
        float supportProbability = 0.2f;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.NewPolicyDailyTickClan));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));

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
                    }
                    if (clan.Kingdom.ActivePolicies.Contains(NewPolicies.Abdicate))
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
                }

            }
            catch (Exception e)
            {
                InformationManager.DisplayMessage(new InformationMessage("Error in NewPolicyDailyTickClan", Colors.Red));
            }
        }

        private void SupportFriend(Clan clan)
        {
            Kingdom kingdom = clan.Kingdom;
            if (MBRandom.RandomFloatRanged(0f, 1f) < wantToSupportProbability)
            {
                foreach (Clan otherClan in kingdom.Clans)
                {
                    if (clan.InfluenceChange > 0 && clan.Influence > 100)
                    {
                        if (clan.Leader.GetRelation(otherClan.Leader) > 50 && MBRandom.RandomFloatRanged(0f, 1f) < supportProbability)
                        {
                            clan.Influence -= 50;
                            otherClan.Influence += 10;
                            if (otherClan.Leader.IsHumanPlayerCharacter)
                            {
                                InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + " offers " + otherClan.Leader.ToString() + " political support.", Colors.Green));
                                InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + " offers you political support.", null));
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() +"from" + kingdom.Name.ToString() + " offers " + otherClan.Name.ToString() + " political support."));
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
