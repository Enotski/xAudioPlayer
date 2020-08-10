using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Services;
using xAudioPlayer.Views;

namespace xAudioPlayer.ViewModels {
	/// <summary>
	/// VM of master page (RootPage)
	/// </summary>
	public class RootViewModel : BaseViewModel {
		bool _modalBgVisible;
		bool _modalNewPlaylistEnabled;
		bool _acceptModalEnabled;
		bool _errorDublicateMessageVisible;
		bool _warningModalVisible;
		string _playlistNameText;
		string _warningModalHeader;
		string _warningModalMessage;
		string _playlistNameToRemove;
		Color _playlistNameFrameColor = Color.Transparent;
		Color _errorDublicateMessageColor = Color.Transparent;
		MasterPageItem _selectedMasterItem;
		MasterPageItem _selectedPlaylistItem;
		PlaylistRepository _plRepo = PlaylistRepository.GetInstance();

		ObservableCollection<MasterPageItem> _masterItemsList = new ObservableCollection<MasterPageItem>();
		ObservableCollection<MasterPageItem> _playlistsCollection = new ObservableCollection<MasterPageItem>();

		public string CogIcon { get; } = Constants.Icons["mdi-cog-outline"];
		public string PlaylistAddIcon { get; } = Constants.Icons["mdi-playlist-plus"];
		public string PlaylistRemoveIcon { get; } = Constants.Icons["mdi-playlist-remove"];

		public RootViewModel(INavigation nav) : base(nav) {
			_plRepo.OnPlaylistsCollectionRefreshed += RefreshPlaylistCollection;
			RefreshPlaylistCollection();
			RefreshMasterItemsList();

			AddPlaylistCommand = new Command(
				execute: () => {
					PlaylistNameText = "";
					ModalBgVisible = !ModalBgVisible;
					ModalNewPlaylistEnabled = !ModalNewPlaylistEnabled;
				});
			RemovePlaylistCommand = new Command(
				execute: (object args) => {
					WarningModalVisible = !WarningModalVisible;
					ModalBgVisible = !ModalBgVisible;
					var title = (args as MasterPageItem)?.Title;
					if (title == "Default" || title == "Favorite" || title == "Queue") {
						WarningModalHeader = $"You can't remove \"{title}\" playlist";
						WarningModalMessage = "";
					} else {
						WarningModalHeader = "Are you shure?";
						WarningModalMessage = $"Playlist \"{title}\" will be removed";
						_playlistNameToRemove = title;
					}
				});
			SettingsCommand = new Command(
				execute: async () => {
					MessagingCenter.Send(EventArgs.Empty, "CloseMenu");
					await Navigation.PushModalAsync(new SettingsPage(), true);
				});
			CloseModalCommand = new Command(
				execute: (object args) => {
					if (args.ToString() == "add_ok") {
						if (_errorDublicateMessageVisible)
							return;
						_plRepo.AddPlayList(PlaylistNameText);
					}
					if (args.ToString() == "remove_ok") {
						_plRepo.RemovePlaylist(_playlistNameToRemove);
					}
					ModalBgVisible = false;
					ModalNewPlaylistEnabled = false;
					AcceptModalEnabled = false;
					WarningModalVisible = false;
					PlaylistNameText = "";
				});
			ModalBackGroundTappedCommand = new Command(
				execute: (object args) => {
					ModalBgVisible = false;
					ModalNewPlaylistEnabled = false;
					AcceptModalEnabled = false;
					WarningModalVisible = false;
					PlaylistNameText = "";
				});
			MasterItemSelectedCommand = new Command(
				execute: async (object args) => {
					var item = args as MasterPageItem;
					if (item != null) {
						MessagingCenter.Send(EventArgs.Empty, "CloseMenu");
						if (item.Parameter == "Player") {
							MessagingCenter.Send(EventArgs.Empty, "SetPlayerPage");
						} else if (item.Parameter == "Favorite") {
							MessagingCenter.Send(EventArgs.Empty, "SetPlaylistPage");
						} else if (item.Parameter == "Queue") {
							var page = (Page)Activator.CreateInstance(item.TargetPage);
							await Navigation.PushModalAsync(page, true);
						}
					}
				});
			PlaylistItemSelectedCommand = new Command(
				execute: (object args) => {
					var item = args as MasterPageItem;
					if (item != null) {
						MessagingCenter.Send(EventArgs.Empty, "CloseMenu");
						MessagingCenter.Send(EventArgs.Empty, "SetPlaylistPage");
					}
				});
		}

		/// <summary>
		/// Get add new pl modal
		/// </summary>
		public ICommand AddPlaylistCommand { get; set; }
		/// <summary>
		/// Get remove pl confirm modal
		/// </summary>
		public ICommand RemovePlaylistCommand { get; set; }
		/// <summary>
		/// Get settings page as modal
		/// </summary>
		public ICommand SettingsCommand { get; set; }
		/// <summary>
		/// Close opened modal
		/// </summary>
		public ICommand CloseModalCommand { get; set; }
		/// <summary>
		/// Modal bg tapped
		/// </summary>
		public ICommand ModalBackGroundTappedCommand { get; set; }
		/// <summary>
		/// Open page selected master item
		/// </summary>
		public ICommand MasterItemSelectedCommand { get; set; }
		/// <summary>
		/// Change current playlist to selected
		/// </summary>
		public ICommand PlaylistItemSelectedCommand { get; set; }

		public bool ModalBgVisible {
			set { SetProperty(ref _modalBgVisible, value); }
			get { return _modalBgVisible; }
		}
		public bool ModalNewPlaylistEnabled {
			set { SetProperty(ref _modalNewPlaylistEnabled, value); }
			get { return _modalNewPlaylistEnabled; }
		}
		public bool AcceptModalEnabled {
			set { SetProperty(ref _acceptModalEnabled, value); }
			get { return _acceptModalEnabled; }
		}
		public bool WarningModalVisible {
			set { SetProperty(ref _warningModalVisible, value); }
			get { return _warningModalVisible; }
		}
		public string WarningModalMessage {
			set { SetProperty(ref _warningModalMessage, value); }
			get { return _warningModalMessage; }
		}
		public string WarningModalHeader {
			set { SetProperty(ref _warningModalHeader, value); }
			get { return _warningModalHeader; }
		}
		public ObservableCollection<MasterPageItem> MasterItemsList {
			set { SetProperty(ref _masterItemsList, value); }
			get { return _masterItemsList; }
		}
		public ObservableCollection<MasterPageItem> PlaylistsCollection {
			set { SetProperty(ref _playlistsCollection, value); }
			get { return _playlistsCollection; }
		}
		public Color PlaylistNameFrameColor {
			set { SetProperty(ref _playlistNameFrameColor, value); }
			get { return _playlistNameFrameColor; }
		}
		public Color ErrorDublicateMessageColor {
			set { SetProperty(ref _errorDublicateMessageColor, value); }
			get { return _errorDublicateMessageColor; }
		}

		public string PlaylistNameText {
			set {
				SetProperty(ref _playlistNameText, value);
				// Validation
				_errorDublicateMessageVisible = _plRepo.Playlists.ContainsKey(_playlistNameText);
				PlaylistNameFrameColor = _errorDublicateMessageVisible ? Color.OrangeRed : Color.Transparent;
				ErrorDublicateMessageColor = _errorDublicateMessageVisible ? Color.OrangeRed : Color.Transparent;
			}
			get { return _playlistNameText; }
		}
		public MasterPageItem SelectedMasterItem {
			get => _selectedMasterItem;
			set {
				_selectedMasterItem = value;

				if (_selectedMasterItem == null)
					return;

				MasterItemSelectedCommand.Execute(_selectedMasterItem);

				RefreshMasterItemsList();
				SelectedMasterItem = null;
			}
		}
		public MasterPageItem SelectedPlaylistItem {
			get => _selectedPlaylistItem;
			set {
				_selectedPlaylistItem = value;

				if (_selectedPlaylistItem == null)
					return;

				PlaylistItemSelectedCommand.Execute(_selectedPlaylistItem);

				RefreshPlaylistCollection();
				SelectedPlaylistItem = null;
			}
		}
		/// <summary>
		/// Refresh master items list (little trick to avoid bug)
		/// </summary>
		public async void RefreshMasterItemsList() {
			await Task.Run(() => {
				MasterItemsList = new ObservableCollection<MasterPageItem>() {
					new MasterPageItem{ Title="Main", Icon = Constants.Icons["mdi-home-outline"], TargetPage=typeof(MainCarouselPage), Parameter="Player" },
					new MasterPageItem{ Title="Favorite", Icon = Constants.Icons["mdi-heart-outline"], TargetPage=typeof(MainCarouselPage), Parameter="Favorite" },
					new MasterPageItem{ Title="Queue", Icon = Constants.Icons["mdi-format-list-numbered"], TargetPage=typeof(QueuePage), Parameter="Queue"},
				};
			});
		}
		/// <summary>
		/// Refresh master items list (little trick to avoid bug)
		/// </summary>
		public async void RefreshPlaylistCollection() {
			await Task.Run(() => {
				PlaylistsCollection.Clear();
				foreach (var item in _plRepo.Playlists.Keys.ToList()) {
					if (item == "Queue" || item == "Favorite")
						continue;
					PlaylistsCollection.Add(new MasterPageItem { Icon = Constants.Icons["mdi-playlist-music"], Title = item });
				}
			});
		}
	}
}
