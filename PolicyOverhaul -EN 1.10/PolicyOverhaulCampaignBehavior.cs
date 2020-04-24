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
                    if (kingdom.Name.ToString() == "Aserai")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Polygamy);
                    }
                    else if (kingdom.Name.ToString() == "Khuzait")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.NormadicHorde);
                        kingdom.ActivePolicies.Add(NewPolicies.Slavery);
                    }
                    else if (kingdom.Name.ToString() == "Northern Empire")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Republic);
                    }
                    else if (kingdom.Name.ToString() == "Southern Empire")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Republic);
                        kingdom.ActivePolicies.Add(NewPolicies.Feudalism);
                        kingdom.ActivePolicies.Add(DefaultPolicies.FeudalInheritance);

                    }
                    else if (kingdom.Name.ToString() == "Western Empire")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Republic);
                        kingdom.ActivePolicies.Add(NewPolicies.Tyrant);

                    }
                    else if (kingdom.Name.ToString() == "Vlandia")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.Vassalism);
                    }
                    else if (kingdom.Name.ToString() == "Sturgia")
                    {
                        kingdom.ActivePolicies.Add(NewPolicies.WarFury);
                    }
                    else if (kingdom.Name.ToString() == "Battania")
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
            if (settlement.OwnerClan != null && settlement.OwnerClan.Kingdom != null)
            {
                Kingdom kingdom = settlement.OwnerClan.Kingdom;
                if (kingdom.ActivePolicies.Contains(NewPolicies.Centralization))
                {
                    if (settlement.OwnerClan != kingdom.RulingClan && settlement.IsTown)
                    {
                        if (settlement.OwnerClan == Clan.PlayerClan)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Your town" + settlement.Name + "was confiscated by" + kingdom.Name, Colors.Red));
                            InformationManager.AddQuickInformation(new TextObject("Your town" + settlement.Name + "was confiscated by" + kingdom.Name + "You get" + settlement.Prosperity + "and 50 renown as compensation.", null));
                            settlement.OwnerClan.AddRenown(50);
                            GiveGoldAction.ApplyBetweenCharacters(kingdom.RulingClan.Leader, Hero.MainHero, (int)settlement.Prosperity, false);

                        }
                        else
                        {
                            InformationManager.DisplayMessage(new InformationMessage(settlement.Name + "was confiscated by" + kingdom.Name, Colors.Gray));
                        }
                        ChangeOwnerOfSettlementAction.ApplyByDefault(kingdom.RulingClan.Leader, settlement);


                    }
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
                    if (clan.Leader.GetRelation(otherClan.Leader) < -90 && MBRandom.RandomFloatRanged(0f, 1f) < actProbability)
                    {
                        if (MBRandom.RandomFloatRanged(0f, 1f) < 0.1 * Math.Max(otherClan.Leader.GetAttributeValue(CharacterAttributesEnum.Vigor), otherClan.Leader.GetAttributeValue(CharacterAttributesEnum.Endurance)))
                        {
                            if (otherClan.Leader.IsHumanPlayerCharacter)
                            {
                                InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "is attacked by an assasin sent by" + clan.Name.ToString() + "from" + kingdom.Name.ToString(), Colors.Red));
                                InformationManager.AddQuickInformation(new TextObject("You were attacked by an assasin sent by "+clan.Leader.ToString() + ". Luckly you survived", null));
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "was attacked by an assasin sent by" + clan.Leader.ToString()));
                            }
                        }
                        else
                        {
                            if (otherClan.Leader.IsHumanPlayerCharacter)
                            {
                                InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "was attacked by an assasin sent by" + clan.Leader.ToString(), Colors.Red));
                                InformationManager.AddQuickInformation(new TextObject("You were attacked by an assasin sent by"+ clan.Leader.ToString() +" and were gravely wounded.", null));
                                otherClan.Leader.HitPoints = 1;
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(otherClan.Leader.ToString() + "was attacked by an assasin sent by" + clan.Leader.ToString()));
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
                            InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "spreads rumors about" + otherClan.Leader.ToString() +".", Colors.Red));
                            InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "spreads rumors about you, so you lose 5 infulence.", null));
                        }
                        else
                        {
                            InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "from" + kingdom.Name.ToString() + "spreads rumors about" + otherClan.Name.ToString()+".", Colors.Gray));
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

        private void SupportFriend(Clan clan)
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
                                InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "offers" + otherClan.Leader.ToString() + "political support.", Colors.Green));
                                InformationManager.AddQuickInformation(new TextObject(clan.Leader.ToString() + "offers you political supports.You get 5 influence.", null));
                            }
                            else
                            {
                                InformationManager.DisplayMessage(new InformationMessage(clan.Leader.ToString() + "offers" + otherClan.Name.ToString() + "political support.", Colors.Gray));
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

        static void GetNextTyrant(Kingdom kingdom)
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
            if(oldRuler != kingdom.RulingClan)
            {
                if (kingdom.RulingClan.Leader.IsHumanPlayerCharacter)
                {
                    InformationManager.DisplayMessage(new InformationMessage(kingdom.Leader.Name+ "has seize the throne of" + kingdom.Name + "by military strength.", Colors.Green));
                    InformationManager.AddQuickInformation(new TextObject("You have seize the throne of"+ kingdom.Name + "by military strength. Congratulations, my lord!", null));
                }
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage(kingdom.RulingClan.Leader.ToString() + "has seize the throne of" + kingdom.Name + "by military strength."));
                }
            }



        }


    }

}
