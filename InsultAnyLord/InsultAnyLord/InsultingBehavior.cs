using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace InsultAnyLord
{
    class InsultingBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
        }

        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            this.AddInsultingDialog(obj);

        }

        private void AddInsultingDialog(CampaignGameStarter obj)
        {
            obj.AddPlayerLine("insult_ID_1", "hero_main_options", "insult_seq_1", "{=my_insult_line_1}You're a piece of shit.",
                new ConversationSentence.OnConditionDelegate(this.InsultOnCondition),
                new ConversationSentence.OnConsequenceDelegate(this.InsultOnConsequence), 100, null);
            obj.AddDialogLine("insult_ID_2", "insult_seq_1", "lord_pretalk", "{=response_to_insult_1}You'll regret what you just said.", this.InsultOnCondition, null);

        }

        private bool InsultOnCondition()
        {
            return Hero.OneToOneConversationHero.IsNoble;
        }

        private void InsultOnConsequence()
        {
            int relation = (int)Hero.OneToOneConversationHero.GetRelationWithPlayer();
            MBTextManager.SetTextVariable("otherHero", Hero.OneToOneConversationHero);
            InformationManager.AddQuickInformation(new TextObject("{=insult_consequence_notify}You relation with {otherHero} is decreased by 10. {otherHero} lost 10 influence.", null));
            Hero.OneToOneConversationHero.SetPersonalRelation(Hero.MainHero, relation-10);
            Hero.OneToOneConversationHero.Clan.Influence -= 10;
        }


        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
