
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using xAudioPlayer.Models;

namespace xAudioPlayer.Repositories {
	/// <summary>
	/// Repository of custom settings
	/// </summary>
	public class SettingsRepository {
		static SettingsRepository _instance;
		public List<ThemeItem> ThemesList { get; } = new List<ThemeItem>();
		public static ThemeItem CurrentTheme { get; private set; }

		public static SettingsRepository GetInstance() {
			if (_instance == null)
				_instance = new SettingsRepository();
			return _instance;
		}
		private SettingsRepository() {
			ThemesList.AddRange(new List<ThemeItem> {
				new ThemeItem{ Name = "LightTheme", Colors = new Color[5] {Color.FromHex("#f2f2f0"), Color.FromHex("#88b0bf"), Color.FromHex("#131a40"), Color.FromHex("#344059"), Color.FromHex("#5c8fed") } },
				new ThemeItem{ Name = "DarkTheme", Colors = new Color[5] {Color.FromHex("#1e2532"), Color.FromHex("#262f40"), Color.FromHex("#738ebf"), Color.FromHex("#4d5e80"), Color.FromHex("#5c8fed") } },
				new ThemeItem{ Name = "AestheticTheme", Colors = new Color[5] {Color.FromHex("#5679A6"), Color.FromHex("#2d63a8"), Color.FromHex("#263f73"), Color.FromHex("#222059"), Color.FromHex("#D97398") } },
				new ThemeItem{ Name = "NuanceTheme", Colors = new Color[5] {Color.FromHex("#a65d85"), Color.FromHex("#f29c6b"), Color.FromHex("#f2786d"), Color.FromHex("#d96c80"), Color.FromHex("#f2e4bb") } },
				new ThemeItem{ Name = "FeastTheme", Colors = new Color[5] {Color.FromHex("#020659"), Color.FromHex("#22ddf2"), Color.FromHex("#f249a6"), Color.FromHex("#f2d541"), Color.FromHex("#12ff55") } },
			});
			CurrentTheme = ThemesList[0];
		}
		public void SetCurrentTheme(string name) {
			if(!string.IsNullOrWhiteSpace(name) && ThemesList.Any(x => x.Name == name)) {
				CurrentTheme = ThemesList.FirstOrDefault(x => x.Name == name);
			}
		}
	}
}
