using MyYamlParser;

namespace Service.Backoffice.Settings
{
	public class ServerKeyValueKeysSettingsModel
	{
		[YamlProperty("EducationProgressKey")]
		public string EducationProgressKey { get; set; }
		
		[YamlProperty("AchievementKeys")]
		public string AchievementKeys { get; set; }

		[YamlProperty("StatusKeys")]
		public string StatusKeys { get; set; }

		[YamlProperty("HabitKeys")]
		public string HabitKeys { get; set; }

		[YamlProperty("SkillKeys")]
		public string SkillKeys { get; set; }

		[YamlProperty("KnowledgeKeys")]
		public string KnowledgeKeys { get; set; }

		[YamlProperty("UserTimeKeys")]
		public string UserTimeKeys { get; set; }

		[YamlProperty("RetryKeys")]
		public string RetryKeys { get; set; }
	}
}