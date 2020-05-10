using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace DiplomacyVote
{
    public class CustomSaveDefiner : SaveableTypeDefiner
    {
        public CustomSaveDefiner() : base(5412188) { }
        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(NewDeclareWarKingdomDecision), 8899174);
            AddClassDefinition(typeof(NewDeclareWarKingdomDecision.DeclareWarDecisionOutcome), 370202);
        }

    }
}
