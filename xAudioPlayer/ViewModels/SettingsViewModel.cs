using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Themes;

namespace xAudioPlayer.ViewModels {
	public class SettingsViewModel : BaseViewModel {
		string _currentThemeName;
		bool _modalBgVisible;
		bool _acceptModalVisible;
		ThemeItem _selectedItem;
		ThemeItem _currentTheme;
		Color[] _currentThemeColors;

		Color _firstThemeColors;
		Color _secondThemeColors;
		Color _thirdThemeColors;
		Color _fourthThemeColors;
		Color _fifthThemeColors;
		SettingsRepository _settingsRepo = SettingsRepository.GetInstance();

		ObservableListCollection<ThemeItem> _themesList = new ObservableListCollection<ThemeItem>();
		public SettingsViewModel(INavigation nav) : base(nav) {
			ThemesList = new  ObservableListCollection<ThemeItem>(_settingsRepo.ThemesList);
			CurrentThemeColors = SettingsRepository.CurrentTheme.Colors;
			CurrentThemeName = SettingsRepository.CurrentTheme.Name;
			RefreshColors();

			CloseModalCommand = new Command(
				execute: (object args) => {
					if(args.ToString() == "set_theme")
						SetTheme();
					SetModalUi();
				});
			ThemeItemSelectedCommand = new Command(
				execute: (object args) => {
					SetModalUi();
				});
			ModalBackGroundTappedCommand = new Command(
				execute: () => {
					SetModalUi();
				});
		}
		/// <summary>
		/// Modal bg tapped
		/// </summary>
		public ICommand ModalBackGroundTappedCommand { private set; get; }
		/// <summary>
		/// 
		/// </summary>
		public ICommand SetThemeCommand { private set; get; }
		/// <summary>
		/// Close accept modal / set theme
		/// </summary>
		public ICommand CloseModalCommand { private set; get; }
		/// <summary>
		/// Theme item selected
		/// </summary>
		public ICommand ThemeItemSelectedCommand { private set; get; }

		public ObservableListCollection<ThemeItem> ThemesList {
			get => _themesList;
			set { SetProperty(ref _themesList, value); }
		}
		public ThemeItem SelectedItem {
			get => _selectedItem;
			set {
				_selectedItem = value;
				if (value == null || _currentTheme == _selectedItem)
					return;
				ThemeItemSelectedCommand.Execute(_selectedItem);
			}
		}

		public bool AcceptModalVisible {
			set { SetProperty(ref _acceptModalVisible, value); }
			get { return _acceptModalVisible; }
		}
		public bool ModalBgVisible {
			set { SetProperty(ref _modalBgVisible, value); }
			get { return _modalBgVisible; }
		}
		public string NameOfCurrentTheme {
			set { SetProperty(ref _currentThemeName, value); }
			get { return _currentThemeName; }
		}
		public string CurrentThemeName {
			set { SetProperty(ref _currentThemeName, value); }
			get { return _currentThemeName; }
		}

		public Color[] CurrentThemeColors {
			get => _currentThemeColors;
			set {
				if (value == null || value.Length < 5)
					return;

				SetProperty(ref _currentThemeColors, value);
			}
		}
		public Color FirstCurrentThemeColor {
			get => _firstThemeColors;
			set { SetProperty(ref _firstThemeColors, value); }
		}
		public Color SecondCurrentThemeColor {
			get => _secondThemeColors;
			set { SetProperty(ref _secondThemeColors, value); }
		}
		public Color ThirdCurrentThemeColor {
			get => _thirdThemeColors;
			set { SetProperty(ref _thirdThemeColors, value); }
		}
		public Color FourthCurrentThemeColor {
			get => _fourthThemeColors;
			set { SetProperty(ref _fourthThemeColors, value); }
		}
		public Color FifthCurrentThemeColor {
			get => _fifthThemeColors;
			set { SetProperty(ref _fifthThemeColors, value); }
		}
		void SetModalUi() {
			ModalBgVisible = !ModalBgVisible;
			AcceptModalVisible = !AcceptModalVisible;
		}
		void RefreshColors() {
			FirstCurrentThemeColor = CurrentThemeColors[0];
			SecondCurrentThemeColor = CurrentThemeColors[1];
			ThirdCurrentThemeColor = CurrentThemeColors[2];
			FourthCurrentThemeColor = CurrentThemeColors[3];
			FifthCurrentThemeColor = CurrentThemeColors[4];
		}
		void SetTheme() {
			ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
			if (mergedDictionaries != null) {
				mergedDictionaries.Clear();
				_currentTheme = ThemesList.FirstOrDefault(t => t.Name == SelectedItem.Name);
				switch (_currentTheme?.Name) {
					case "LightTheme":
						mergedDictionaries.Add(new LightTheme());
						break;
					case "DarkTheme":
						mergedDictionaries.Add(new DarkTheme());
						break;
					case "AestheticTheme":
						mergedDictionaries.Add(new AestheticTheme());
						break;
					case "NuanceTheme":
						mergedDictionaries.Add(new NuanceTheme());
						break;
					case "FeastTheme":
						mergedDictionaries.Add(new FeastTheme());
						break;
					default:
						mergedDictionaries.Add(new LightTheme());
						break;
				}
				_settingsRepo.SetCurrentTheme(_currentTheme.Name);
				CurrentThemeName = _currentTheme.Name;
				CurrentThemeColors = _currentTheme.Colors;
				RefreshColors();
			}
		}
	}
}
