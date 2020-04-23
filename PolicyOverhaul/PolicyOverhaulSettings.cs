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
        public override string ModName => "政策大修";
        public override string ModuleFolderName => "PolicyOverhaul";
        public override string Id { get; set; } = "PolicyOverhaul";

        [SettingProperty("选举间隔", 5,500 , false, "选举间隔")]
        [SettingPropertyGroup("通用")]
        public int ElectionCycle { get; set; } = 30;

        [SettingProperty("领袖可否连任", 5, 500, false, "领袖可否连任")]
        [SettingPropertyGroup("通用")]
        public bool InfiniteTerm { get; set; } = false;
    }


}
