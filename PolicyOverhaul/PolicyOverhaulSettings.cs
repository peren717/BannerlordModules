using ModLib;
using ModLib.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PolicyOverhaul
{
    public class PolicyOverhaulSettings : SettingsBase
    {
        public override string ModName => "PolicyOverhual/政策大修";
        public override string ModuleFolderName => "PolicyOverhaul";

        [XmlElement]
        public override string ID { get; set; } = "PolicyOverhaulSettings";

        public const string InstanceID = "PolicyOverhaulSettings";
        public static PolicyOverhaulSettings Instance
        {
            get
            {
                return (PolicyOverhaulSettings)SettingsDatabase.GetSettings("PolicyOverhaulSettings");
            }
        }

        [XmlElement]
        [SettingProperty("Election Cycle/选举间隔", 5, 500, "Election Cycle. Changing this parameter in-game may result in unpredicatable behaviors./选举间隔,游戏进行时不可更改。")]
        [SettingPropertyGroup("General")]
        public int ElectionCycle { get; set; } = 30;

        [XmlElement]
        [SettingProperty("Reelection/领袖可否连任", 5, 500, "Is the leader reelectable? Becareful: A selfish lord may always reelect himself. Changing this parameter in-game may result in unpredicatable behaviors./领袖可否连任,游戏进行时不可更改。")]
        [SettingPropertyGroup("General")]
        public bool InfiniteTerm { get; set; } = false;

        [XmlElement]
        [SettingProperty("Interaction frequency/互动频率", 0f, 1f, "How often should lord interact with one another? Recommended: 0.2. Changing this parameter in-game may result in unpredicatable behaviors./领主之间互动频率，0为无互动，1为必定互动（会导致刷屏）,游戏进行时不可更改。")]
        [SettingPropertyGroup("General")]
        public float ActProbablity { get; set; } = 0.2f;

        [XmlElement]
        [SettingProperty("Pre-assigned policies/新游戏国家自带政策", 5, 500, "By enabling this, every nation has some policies pre-assigned to them according to lore.Changing this parameter in-game may result in unpredicatable behaviors./开启后，开始新游戏时，每个国家将会根据背景自带国策,游戏进行时不可更改。")]
        [SettingPropertyGroup("General")]
        public bool EnablePreAssignedPolicies { get; set; } = true;

        [XmlElement]
        [SettingProperty("Clan rebel/家族背叛", "Enbale clan rebel behavior/是否启用家族背叛")]
        [SettingPropertyGroup("Separitism")]
        public bool EnableRebel { get; set; } = true;

        [XmlElement]
        [SettingProperty("Rebel chance per day/每日叛变几率", 0f, 1f, "How often should a unloyal lord rebel. Changing this parameter in-game may result in unpredicatable behaviors./不忠领主每日有多少几率叛变。游戏进行时不可更改。")]
        [SettingPropertyGroup("Separitism")]
        public float RebelProbability { get; set; } = 0.05f;


        [XmlElement]
        [SettingProperty("Debug Mode", "Show debug message when enabled")]
        [SettingPropertyGroup("Debug")]
        public bool debugMode { get; set; } = false;
    }


}
