using MediaManager.Player;
using System;
using System.Collections.Generic;
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
	/// VM of playlist page
	/// </summary>
	public class PlaylistViewModel : BaseViewModel {
		string _playlistName;
		string _playlistInfo = "-/-";
		string _nameOfCurrentAudioFile = "-/-";
		string _searchEntryText;
		string _queueAddBtnText;
		string _favoriteAddBtnText;
		Rectangle _audioFileMenuLocation = new Rectangle(100, 100, 200, 245);
		bool _playlistMenuVisible;
		bool _audioFileMenuVisible;
		bool _modalBackgroundVisible;
		bool _activityIndicatorVisible;
		bool _itemCheckBoxVisible;
		bool _contextSortingEnabled;
		bool _allItemsChecked;
		bool _standartMenuButtonsVisible = true;
		bool _removeMenuButtonVisible = true;
		bool _dragMenuButtonVisible = true;
		bool _headerInfoVisible = true;
		bool _searchMenuButtonVisible = true;
		bool _searchEntryVisible;
		bool _sortByPanelVisible;
		bool _sortReverseToggled;
		string _sortAscDescIcon = Constants.Icons["mdi-sort-ascending"];
		string _sortType = "Name";
		string _playPauseIcon = Constants.Icons["mdi-play-outline"];
		string _removeParam;
		static bool _refreshingPlaylist;
		double _audioFileProgressValue = 0;
		AudioFile _selectedItem;
		ButtonClickedTriggerAction _btnClickedTrigger;
		ObservableListCollection<AudioFile> _currentPLaylist = new ObservableListCollection<AudioFile>();

		PlaylistRepository _plRepo = PlaylistRepository.GetInstance();

		public string MenuIcon { get; } = Constants.Icons["mdi-menu"];
		public string PrevIcon { get; } = Constants.Icons["mdi-chevron-left"];
		public string NextIcon { get; } = Constants.Icons["mdi-chevron-right"];
		public string PlusIcon { get; } = Constants.Icons["mdi-plus"];
		public string MinusIcon { get; } = Constants.Icons["mdi-minus"];
		public string FormatIcon { get; } = Constants.Icons["mdi-format-align-left"];
		public string SearchIcon { get; } = Constants.Icons["mdi-magnify"];
		public string DotsIcon { get; } = Constants.Icons["mdi-dots-vertical"];
		public string SortIcon { get; } = Constants.Icons["mdi-sort"];
		public string CheckMultipleIcon { get; } = Constants.Icons["mdi-checkbox-multiple-marked-outline"];
		public string CheckIcon { get; } = Constants.Icons["mdi-check-outline"];

		public PlaylistViewModel(INavigation nav) : base(nav) {
			CurrentPlaylistName = _plRepo.CurrentPlaylistName;
			_btnClickedTrigger = new ButtonClickedTriggerAction(SetAudioFileMenuLocation);

			_plRepo.OnPlaylistRefreshing += CurrentPlaylistRefreshing;
			_plRepo.OnPlaylistRefreshed += CurrentPlaylistRefreshed;
			_plRepo.OnAudioFileChanged += AudioFileChanged;
			_plRepo.OnMediaStateUpdated += MediaStateUpdated;
			_plRepo.OnMediaProgressUpdated += MediaProgressChanged;

			_currentPLaylist.CollectionChanged += (sender, e) => {
				int i = 0;
				_plRepo.ChnageAudioFilesOrder(e.OldStartingIndex, e.NewStartingIndex);
				foreach (var item in _currentPLaylist) {
					item.Num = ++i;
				}
			};


			PlaylistMenuCommand = new Command(
				execute: () => {
					PlaylistMenuVisible = !PlaylistMenuVisible;
					ModalBackgroundVisible = !ModalBackgroundVisible;
				});
			AudioFileMenuCommand = new Command(
				execute: (object item) => {
					AudioFileMenuVisible = !AudioFileMenuVisible;
					ModalBackgroundVisible = !ModalBackgroundVisible;
					_selectedItem = item as AudioFile;
					QueueAddBtnText = $"{(_plRepo.Playlists["Queue"].Any(x => x.FullPath == _selectedItem.FullPath) ? "Remove form queue" : "Add to queue")}";
					FavoriteAddBtnText = $"{(_plRepo.Playlists["Favorite"].Any(x => x.FullPath == _selectedItem.FullPath) ? "Remove form favorite" : "Add to favorite")}";
				});
			AudioFileSelectedCommand = new Command(
				execute: (object obj) => {
					UpdateCurentAudioFileInfo();

					_plRepo.PlayPauseAudioFile((obj as AudioFile)?.FullPath);
				});
			AddCommand = new Command(
				execute: async (object obj) => {
					if (obj == null) {
						await Navigation.PushModalAsync(new FileBrowserPage(), true);
					} else {
						AddAudioFile(obj.ToString());
						AudioFileMenuVisible = false;
						ModalBackgroundVisible = false;
					}
				});
			RemoveCommand = new Command(
				execute: (object obj) => {
					_removeParam = obj.ToString();
					if (_removeParam == "Global") {
						ItemCheckBoxVisible = !ItemCheckBoxVisible;
						StandartMenuButtonsVisible = !StandartMenuButtonsVisible;
						SearchMenuButtonVisible = !SearchMenuButtonVisible;
						DragMenuButtonVisible = !DragMenuButtonVisible;

						_allItemsChecked = false;
						foreach (var item in CurrentPLaylist) {
							item.ItemChecked = _allItemsChecked;
						}
					} else if (_removeParam == "Playlist" || _removeParam == "Directory") {
						var item = CurrentPLaylist.FirstOrDefault(x => x.FullPath == _selectedItem.FullPath);
						item.ItemChecked = true;
						RemoveAudioFiles();
					}
					PlaylistMenuVisible = false;
					AudioFileMenuVisible = false;
				});
			ModalBackGroundTappedCommand = new Command(
				execute: () => {
					if (!_refreshingPlaylist) {
						AudioFileMenuVisible = false;
						PlaylistMenuVisible = false;
						ModalBackgroundVisible = false;
						SortByPanelVisible = false;
					}
				});
			ClearCommand = new Command(
				execute: () => {
					ClearCurrentPlaylist();
					PlaylistInfo = $"-/-";
					PlaylistMenuVisible = false;
					ModalBackgroundVisible = false;
				});
			ContextCommand = new Command(
				execute: () => {
					ContextSortingEnabled = !ContextSortingEnabled;
					StandartMenuButtonsVisible = !StandartMenuButtonsVisible;
					SearchMenuButtonVisible = !SearchMenuButtonVisible;
					RemoveMenuButtonVisible = !RemoveMenuButtonVisible;
				});
			AcceptRemoveCommand = new Command(
				execute: () => {
					RemoveAudioFiles();
				});
			CheckAllCommand = new Command(
				execute: async () => {
					await Task.Run(() => {
						try {
							_allItemsChecked = !CurrentPLaylist.All(x => x.ItemChecked);
							foreach (var item in CurrentPLaylist) {
								item.ItemChecked = _allItemsChecked;
							}
						} catch { }
					});
				});
			SortCommand = new Command(
				execute: () => {
					SortReverseToggled = !SortReverseToggled;
					SortAscDescIcon = SortReverseToggled ? Constants.Icons["mdi-sort-descending"] : Constants.Icons["mdi-sort-ascending"];
					SortPlaylist();
				});
			SearchCommand = new Command(
				execute: (object obj) => {
					SearchEntryText = "";
					SearchEntryVisible = !SearchEntryVisible;
					HeaderInfoVisible = !HeaderInfoVisible;
					DragMenuButtonVisible = !DragMenuButtonVisible;
					RemoveMenuButtonVisible = !RemoveMenuButtonVisible;
					StandartMenuButtonsVisible = !StandartMenuButtonsVisible;
					if (!SearchEntryVisible)
						OnEntryTextChanged("");
				});
			RefreshCommand = new Command(
				execute: async (object obj) => {
					PlaylistMenuVisible = false;
					await Task.Run(() => {
						_plRepo.RefreshCurrentPlaylistByStorage(CurrentPLaylist.Select(x => x.FullPath));
					});
				});
			SortByCommand = new Command(
				execute: (object obj) => {
					SortByPanelVisible = !SortByPanelVisible;
					if (obj.ToString() == "ContextMenu")
						ModalBackgroundVisible = true;
					PlaylistMenuVisible = false;
				});
			SortTypeChangedCommand = new Command(
				execute: (object obj) => {
					SortType = obj.ToString();
				});
			SortTypeBtnPressedCommand = new Command(
				execute: (object obj) => {
					SortByPanelVisible = false;
					ModalBackgroundVisible = false;
					if (obj.ToString() == "Ok") {
						SortPlaylist();
						SortAscDescIcon = SortReverseToggled ? Constants.Icons["mdi-sort-descending"] : Constants.Icons["mdi-sort-ascending"];
					}
				});
			MenuCommand = new Command(
				execute: () => {
					MessagingCenter.Send(EventArgs.Empty, "OpenMenu");
				});

			PlayPauseCommand = new Command(
				execute: () => {
					if (SelectedItem == null)
						ChangeAudioFile(false);
					else
						_plRepo.PlayPauseAudioFile();
				});
			PrevNextCommand = new Command(
				execute: (object args) => {
					ChangeAudioFile(true, args.ToString() == "prev");
				});
		}

		/// <summary>
		/// Open master page
		/// </summary>
		public ICommand MenuCommand { private set; get; }
		/// <summary>
		/// Get file browser modal window or add audio file to any pl
		/// </summary>
		public ICommand AddCommand { private set; get; }
		/// <summary>
		/// Get remove mode or remove audio file form pl/dir
		/// </summary>
		public ICommand RemoveCommand { private set; get; }
		/// <summary>
		/// Get sorting mode
		/// </summary>
		public ICommand ContextCommand { private set; get; }
		/// <summary>
		/// Get search mode
		/// </summary>
		public ICommand SearchCommand { private set; get; }
		/// <summary>
		/// Play/pause selected audio file
		/// </summary>
		public ICommand AudioFileSelectedCommand { private set; get; }
		/// <summary>
		/// Get pl menu
		/// </summary>
		public ICommand PlaylistMenuCommand { private set; get; }
		/// <summary>
		/// Change current audio file with prev/next 
		/// </summary>
		public ICommand PrevNextCommand { private set; get; }
		/// <summary>
		/// Play/pause audio file
		/// </summary>
		public ICommand PlayPauseCommand { private set; get; }
		/// <summary>
		/// Remove selected audio files from pl
		/// </summary>
		public ICommand AcceptRemoveCommand { private set; get; }
		/// <summary>
		/// Get audio file menu
		/// </summary>
		public ICommand AudioFileMenuCommand { private set; get; }
		/// <summary>
		/// Modal bg tapped
		/// </summary>
		public ICommand ModalBackGroundTappedCommand { private set; get; }
		/// <summary>
		/// Check all audio files in pl
		/// </summary>
		public ICommand CheckAllCommand { private set; get; }
		/// <summary>
		/// Sort current pl by asc/desc
		/// </summary>
		public ICommand SortCommand { private set; get; }

		/// <summary>
		/// Sort type changed (name/duration)
		/// </summary>
		public ICommand SortTypeChangedCommand { private set; get; }
		/// <summary>
		/// Sort btn on modal window pressed
		/// </summary>
		public ICommand SortTypeBtnPressedCommand { private set; get; }
		/// <summary>
		/// Get sort modal window
		/// </summary>
		public ICommand SortByCommand { private set; get; }
		/// <summary>
		/// Refresh current pl
		/// </summary>
		public ICommand RefreshCommand { private set; get; }
		/// <summary>
		/// Clear current pl
		/// </summary>
		public ICommand ClearCommand { private set; get; }

		public ObservableListCollection<AudioFile> CurrentPLaylist {
			get => _currentPLaylist;
			set {
				SetProperty(ref _currentPLaylist, value);
			}
		}
		public double AudioFileProgressValue {
			set { SetProperty(ref _audioFileProgressValue, value); }
			get { return _audioFileProgressValue; }
		}
		public string CurrentPlaylistName {
			set { SetProperty(ref _playlistName, value); }
			get { return _playlistName; }
		}
		public string PlaylistInfo {
			set { SetProperty(ref _playlistInfo, value); }
			get { return _playlistInfo; }
		}
		public string NameOfCurrentAudioFile {
			set { SetProperty(ref _nameOfCurrentAudioFile, value); }
			get { return _nameOfCurrentAudioFile; }
		}
		public bool PlaylistMenuVisible {
			set { SetProperty(ref _playlistMenuVisible, value); }
			get { return _playlistMenuVisible; }
		}
		public string QueueAddBtnText {
			set { SetProperty(ref _queueAddBtnText, value); }
			get { return _queueAddBtnText; }
		}
		public string FavoriteAddBtnText {
			set { SetProperty(ref _favoriteAddBtnText, value); }
			get { return _favoriteAddBtnText; }
		}
		public bool AudioFileMenuVisible {
			set { SetProperty(ref _audioFileMenuVisible, value); }
			get { return _audioFileMenuVisible; }
		}
		public bool ModalBackgroundVisible {
			set { SetProperty(ref _modalBackgroundVisible, value); }
			get { return _modalBackgroundVisible; }
		}
		public bool ActivityIndicatorVisible {
			set { SetProperty(ref _activityIndicatorVisible, value); }
			get { return _activityIndicatorVisible; }
		}
		public bool ItemCheckBoxVisible {
			set { SetProperty(ref _itemCheckBoxVisible, value); }
			get { return _itemCheckBoxVisible; }
		}
		public bool ContextSortingEnabled {
			set { SetProperty(ref _contextSortingEnabled, value); }
			get { return _contextSortingEnabled; }
		}
		public bool StandartMenuButtonsVisible {
			set { SetProperty(ref _standartMenuButtonsVisible, value); }
			get { return _standartMenuButtonsVisible; }
		}
		public bool RemoveMenuButtonVisible {
			set { SetProperty(ref _removeMenuButtonVisible, value); }
			get { return _removeMenuButtonVisible; }
		}
		public bool DragMenuButtonVisible {
			set { SetProperty(ref _dragMenuButtonVisible, value); }
			get { return _dragMenuButtonVisible; }
		}
		public bool SearchMenuButtonVisible {
			set { SetProperty(ref _searchMenuButtonVisible, value); }
			get { return _searchMenuButtonVisible; }
		}
		public bool SearchEntryVisible {
			set { SetProperty(ref _searchEntryVisible, value); }
			get { return _searchEntryVisible; }
		}
		public bool HeaderInfoVisible {
			set { SetProperty(ref _headerInfoVisible, value); }
			get { return _headerInfoVisible; }
		}
		public bool SortByPanelVisible {
			set { SetProperty(ref _sortByPanelVisible, value); }
			get { return _sortByPanelVisible; }
		}
		public string SearchEntryText {
			set {
				if (value.Trim() != _searchEntryText)
					OnEntryTextChanged(value);
				SetProperty(ref _searchEntryText, value);
			}
			get { return _searchEntryText; }
		}
		public bool SortReverseToggled {
			set { SetProperty(ref _sortReverseToggled, value); }
			get { return _sortReverseToggled; }
		}
		public string SortType {
			set { SetProperty(ref _sortType, value); }
			get { return _sortType; }
		}
		public string SortAscDescIcon {
			get => _sortAscDescIcon;
			set { SetProperty(ref _sortAscDescIcon, value); }
		}
		public string PlayPauseIcon {
			get => _playPauseIcon;
			set { SetProperty(ref _playPauseIcon, value); }
		}
		public Rectangle AudioFileMenuLocation {
			set { SetProperty(ref _audioFileMenuLocation, value); }
			get { return _audioFileMenuLocation; }
		}
		public AudioFile SelectedItem {
			get => _selectedItem;
			set {
				_selectedItem = value;

				if (_selectedItem == null)
					return;
				AudioFileSelectedCommand.Execute(_selectedItem);
			}
		}
		/// <summary>
		/// Set location of audio file menu by fetched pressed btn location
		/// </summary>
		/// <param name="arg"></param>
		void SetAudioFileMenuLocation(System.Drawing.PointF arg) {
			AudioFileMenuLocation = new Rectangle(arg.X - 200, arg.Y < 350 ? arg.Y : arg.Y - 245, 200, 245);
		}
		/// <summary>
		/// Clear current pl
		/// </summary>
		public async void ClearCurrentPlaylist() {
			await Task.Run(() => {
				_plRepo.RemoveItems(false);
			});
		}
		/// <summary>
		/// Show modal bg and busy indicator
		/// </summary>
		public async void CurrentPlaylistRefreshing() {
			await Task.Run(() => {
				_refreshingPlaylist = true;
				ModalBackgroundVisible = _refreshingPlaylist;
				ActivityIndicatorVisible = _refreshingPlaylist;
			});
		}
		/// <summary>
		/// Hide modal bg and busy indicator
		/// </summary>
		public async void CurrentPlaylistRefreshed() {
			await Task.Run(() => {
				RefreshCurrentPlaylist();
			}).ContinueWith((args) => {
				_refreshingPlaylist = false;
				ModalBackgroundVisible = _refreshingPlaylist;
				ActivityIndicatorVisible = _refreshingPlaylist;
			});
		}
		/// <summary>
		/// Refresh current playlist from plRepo
		/// </summary>
		public void RefreshCurrentPlaylist() {
			try {
				CurrentPlaylistName = _plRepo.CurrentPlaylistName;
				CurrentPLaylist.Clear();
				foreach (var item in _plRepo.Playlists[CurrentPlaylistName].Select((x, i) => { x.Num = ++i; return x; }))
					CurrentPLaylist.Add(item);

				var totalDuration = new TimeSpan(CurrentPLaylist.Sum(r => r.Duration.Ticks));
				PlaylistInfo = $"{CurrentPLaylist.Count} / {totalDuration:hh\\:mm\\:ss} / {Utilities.SizeSuffix(CurrentPLaylist.Sum(x => x.Size), 2)}";
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Remove audio files from pl/dir
		/// </summary>
		public async void RemoveAudioFiles() {
			try {
				await Task.Run(() => { _plRepo.RemoveItems(_removeParam == "Directory", CurrentPLaylist.Where(x => x.ItemChecked).Select(x => x.FullPath)); });
			} catch { }
		}
		/// <summary>
		/// Refresh pl by search text
		/// </summary>
		/// <param name="text"></param>
		public async void OnEntryTextChanged(string text) {
			text = text.ToLower().Trim();
			try {
				await Task.Run(() => {
					CurrentPLaylist.Clear();
					if (string.IsNullOrWhiteSpace(text)) {
						foreach (var item in _plRepo.Playlists[CurrentPlaylistName].Select((x, i) => { x.Num = ++i; return x; }))
							CurrentPLaylist.Add(item);
					} else {
						foreach (var item in _plRepo.Playlists[CurrentPlaylistName].Where(x => x.Name.ToLower().Trim().Contains(text)).Select((x, i) => { x.Num = ++i; return x; })) {
							CurrentPLaylist.Add(item);
						}
					}
				});
			} catch { }
		}
		/// <summary>
		/// Sort and refresh current playlist
		/// </summary>
		public async void SortPlaylist() {
			try {
				await Task.Run(() => { _plRepo.SortPlayList(SortType, SortReverseToggled); });
			} catch { }
		}
		private void MediaProgressChanged(TimeSpan progress) {
			if (SelectedItem != null)
				AudioFileProgressValue = progress.TotalMinutes / SelectedItem.Duration.TotalMinutes;
		}
		void AudioFileChanged(AudioFile audioFile) {
			SelectedItem = audioFile;
		}
		void UpdateCurentAudioFileInfo() {
			//foreach (var item in CurrentPLaylist)
			//	item.BgColor = Color.Transparent;
			//SelectedItem.BgColor = Color.LightGray;
			NameOfCurrentAudioFile = SelectedItem.Name;
		}
		async void AddAudioFile(string args) {
			try {
				await Task.Run(() => {
					if (args == "Queue" && _plRepo.Playlists["Queue"].Any(x => x.FullPath == _selectedItem.FullPath)) {
						_plRepo.RemoveFromPlayList(args, new List<string>() { _selectedItem.FullPath });
					} else if (args == "Favorite" && _plRepo.Playlists["Favorite"].Any(x => x.FullPath == _selectedItem.FullPath)) {
						_plRepo.RemoveFromPlayList(args, new List<string>() { _selectedItem.FullPath });
					} else {
						_plRepo.AddToPlayList(args, new List<string>() { _selectedItem.FullPath });
					}
				});
			} catch { }
		}
		async void ChangeAudioFile(bool isHandle, bool prev = false) {
			try {
				await Task.Run(() => {
					_plRepo.ChangeAudioFile(SelectedItem?.FullPath ?? "", prev, isHandle);
				});
			} catch { }
		}
		private void MediaStateUpdated(MediaPlayerState state) {
			PlayPauseIcon = state == MediaPlayerState.Playing ? Constants.Icons["mdi-pause"] : Constants.Icons["mdi-play-outline"];
		}
	}
}
