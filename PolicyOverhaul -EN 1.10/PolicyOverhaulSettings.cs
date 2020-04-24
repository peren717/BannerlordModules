using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBOptionScreen.Attributes;
using MBOptionScreen.Settings;

namespace PolicyOverhaul
{
    class PolicyOverhaulSettings : AttributeSettings<PolicyOverhaulSettings>
    {
        public override string ModName => "Policy Overhaul";
        public override string ModuleFolderName => "PolicyOverhaul";
        public override string Id { get; set; } = "PolicyOverhaul";

        [SettingProperty("Election Cycle", 5,500 , false, "Election Cycle. Changing this parameter in-game may result in unpredicatable behaviors.")]
        [SettingPropertyGroup("General")]
        public int ElectionCycle { get; set; } = 30;

        [SettingProperty("Reelection", 5, 500, false, "Is the leader reelectable? Becareful: A selfish lord may always reelect himself. Changing this parameter in-game may result in unpredicatable behaviors.")]
        [SettingPropertyGroup("General")]
        public bool InfiniteTerm { get; set; } = false;

        [SettingProperty("Interaction frequency", 0f, 1f, false, "How often should lord interact with one another? Recommended: 0.2. Changing this parameter in-game may result in unpredicatable behaviors.")]
        [SettingPropertyGroup("General")]
        public float ActProbablity { get; set; } = 0.2f;

        [SettingProperty("Pre-assigned policies", 5, 500, false, "By enabling this, every nation has some policies pre-assigned to them according to lore.")]
        [SettingPropertyGroup("General")]
        public bool EnablePreAssignedPolicies { get; set; } = true;
    }


}
