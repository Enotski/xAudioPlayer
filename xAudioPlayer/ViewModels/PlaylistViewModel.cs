﻿using System;
using System.Collections.Generic;
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
	public class PlaylistViewModel : BaseViewModel {
		string _playlistName;
		string _playlistInfo = "-/-";
		string _nameOfCurrentAudioFile = "CurrentTrackName - CurrentTrackAuthor";
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
		string _removeParam;
		static bool _refreshingPlaylist;
		AudioFile _selectedItem;
		ButtonClickedTriggerAction btnClickedTrigger;

		public static PlaylistRepository PlaylistRepository = PlaylistRepository.GetInstance();

		public string MenuIcon { get; } = Constants.Icons["mdi-menu"];
		public string PrevIcon { get; } = Constants.Icons["mdi-chevron-left"];
		public string PlayIcon { get; } = Constants.Icons["mdi-play-outline"];
		public string NextIcon { get; } = Constants.Icons["mdi-chevron-right"];
		public string PlusIcon { get; } = Constants.Icons["mdi-plus"];
		public string MinusIcon { get; } = Constants.Icons["mdi-minus"];
		public string FormatIcon { get; } = Constants.Icons["mdi-format-align-left"];
		public string SearchIcon { get; } = Constants.Icons["mdi-magnify"];
		public string DotsIcon { get; } = Constants.Icons["mdi-dots-vertical"];
		public string SortIcon { get; } = Constants.Icons["mdi-sort"];
		public string CheckMultipleIcon { get; } = Constants.Icons["mdi-checkbox-multiple-marked-outline"];
		public string CheckIcon { get; } = Constants.Icons["mdi-check-outline"];

		public ObservableListCollection<AudioFile> _currentPLaylist = new ObservableListCollection<AudioFile>();

		public ObservableListCollection<AudioFile> CurrentPLaylist {
			get {
				return _currentPLaylist;
			}
			set {
				SetProperty(ref _currentPLaylist, value);
			}
		}

		public PlaylistViewModel(INavigation nav) : base(nav) {
			CurrentPlaylistName = PlaylistRepository.CurrentPlaylistName;
			btnClickedTrigger = new ButtonClickedTriggerAction(SetAudioFileMenuLocation);

			PlaylistRepository.OnCurrentPlaylistRefreshing += OnCurrentPlaylistRefreshing;
			PlaylistRepository.OnCurrentPlaylistRefreshed += RefreshCurrentPlaylist;

			_currentPLaylist.OrderChanged += (sender, e) => {
				int i = 0;
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
					QueueAddBtnText = $"{(PlaylistRepository.Playlists["Queue"].Any(x => x.FullPath == _selectedItem.FullPath) ? "Remove form queue" : "Add to queue")}";
					FavoriteAddBtnText = $"{(PlaylistRepository.Playlists["Favorite"].Any(x => x.FullPath == _selectedItem.FullPath) ? "Remove form favorite" : "Add to favorite")}";
				});
			AudioFileSelectedCommand = new Command(
				execute: (object obj) => {
					var r = obj;
				});
			AddCommand = new Command(
				execute: async (object obj) => {
					if (obj == null) {
						await Navigation.PushModalAsync(new FileBrowserPage(), true);
					} else {
						await Task.Run(() => {
							if (obj.ToString() == "Queue" && PlaylistRepository.Playlists["Queue"].Any(x => x.FullPath == _selectedItem.FullPath)) {
								PlaylistRepository.RemoveFromPlayList(obj.ToString(), new List<string>() { _selectedItem.FullPath });
							} else if (obj.ToString() == "Favorite" && PlaylistRepository.Playlists["Favorite"].Any(x => x.FullPath == _selectedItem.FullPath)) {
								PlaylistRepository.RemoveFromPlayList(obj.ToString(), new List<string>() { _selectedItem.FullPath });
							} else {
								PlaylistRepository.AddToPlayList(obj.ToString(), new List<string>() { _selectedItem.FullPath });
							}
						});
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
				execute: () => {
					_allItemsChecked = !CurrentPLaylist.All(x => x.ItemChecked);
					foreach (var item in CurrentPLaylist) {
						item.ItemChecked = _allItemsChecked;
					}
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
						PlaylistRepository.RefreshCurrentPlaylistByStorage(CurrentPLaylist.Select(x => x.FullPath));
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
		}
		
		public ICommand MenuCommand { private set; get; }
		public ICommand AddCommand { private set; get; }
		public ICommand RemoveCommand { private set; get; }
		public ICommand ContextCommand { private set; get; }
		public ICommand SearchCommand { private set; get; }
		public ICommand AudioFileSelectedCommand { private set; get; }
		public ICommand PlaylistMenuCommand { private set; get; }
		public ICommand PrevNextCommand { private set; get; }
		public ICommand PlayPauseCommand { private set; get; }
		public ICommand AcceptRemoveCommand { private set; get; }
		public ICommand AudioFileMenuCommand { private set; get; }
		public ICommand ModalBackGroundTappedCommand { private set; get; }
		public ICommand CheckAllCommand { private set; get; }
		public ICommand SortCommand { private set; get; }

		public ICommand SortTypeChangedCommand { private set; get; }
		public ICommand SortTypeBtnPressedCommand { private set; get; }
		public ICommand SortByCommand { private set; get; }
		public ICommand GroupByCommand { private set; get; }
		public ICommand RefreshCommand { private set; get; }
		public ICommand ClearCommand { private set; get; }

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
		public Rectangle AudioFileMenuLocation {
			set { SetProperty(ref _audioFileMenuLocation, value); }
			get { return _audioFileMenuLocation; }
		}
		public AudioFile SelectedItem {
			get {
				return _selectedItem;
			}
			set {
				_selectedItem = value;

				if (_selectedItem == null)
					return;

				AudioFileSelectedCommand.Execute(_selectedItem);

				SelectedItem = null;
			}
		}
		void SetAudioFileMenuLocation(System.Drawing.PointF arg) {
			AudioFileMenuLocation = new Rectangle(arg.X - 200, arg.Y < 350 ? arg.Y : arg.Y - 245, 200, 245);
		}
		public async void ClearCurrentPlaylist() {
			await Task.Run(() => {
				CurrentPLaylist.Clear();
				PlaylistRepository.RemoveItems();
			});
		}
		public void OnCurrentPlaylistRefreshing() {
			_refreshingPlaylist = true;
			ModalBackgroundVisible = _refreshingPlaylist;
			ActivityIndicatorVisible = _refreshingPlaylist;
		}
		public void OnCurrentPlaylistRefreshed() {
			_refreshingPlaylist = false;
			ModalBackgroundVisible = _refreshingPlaylist;
			ActivityIndicatorVisible = _refreshingPlaylist;
		}
		public async void RefreshCurrentPlaylist() {
			await Task.Run(() => {

				Sorting(SortType, SortReverseToggled);
				CurrentPLaylist.Clear();
				foreach (var item in PlaylistRepository.Playlists[CurrentPlaylistName].Select((x, i) => { x.Num = ++i; return x; }))
					CurrentPLaylist.Add(item);

			}).ContinueWith((arg) => {
				var totalDuration = new TimeSpan(CurrentPLaylist.Sum(r => r.Duration.Ticks));
				PlaylistInfo = $"{CurrentPLaylist.Count} / {totalDuration:hh\\:mm\\:ss} / {Utilities.SizeSuffix(CurrentPLaylist.Sum(x => x.Size), 2)}";
				OnCurrentPlaylistRefreshed();
			});
		}
		public async void RemoveAudioFiles() {
			await Task.Run(() => {
				PlaylistRepository.RemoveItems(_removeParam, CurrentPLaylist.Where(x => x.ItemChecked).Select(x => x.FullPath));
				OnCurrentPlaylistRefreshing();
				RefreshCurrentPlaylist();
			});
		}
		public async void OnEntryTextChanged(string text) {
			text = text.ToLower().Trim();

			await Task.Run(() => {
				CurrentPLaylist.Clear();
				if (string.IsNullOrWhiteSpace(text)) {
					foreach (var item in PlaylistRepository.Playlists[CurrentPlaylistName].Select((x, i) => { x.Num = ++i; return x; }))
						CurrentPLaylist.Add(item);
				} else {
					foreach (var item in PlaylistRepository.Playlists[CurrentPlaylistName].Where(x => x.Name.ToLower().Trim().Contains(text)).Select((x, i) => { x.Num = ++i; return x; })) {
						CurrentPLaylist.Add(item);
					}
				}
			});
		}
		public void Sorting(string type, bool desc) {
			switch (type) {
				case "Name": {
					PlaylistRepository.Playlists[CurrentPlaylistName] = desc ?
																PlaylistRepository.Playlists[CurrentPlaylistName].OrderByDescending(x => x.Name).ToList() :
																PlaylistRepository.Playlists[CurrentPlaylistName].OrderBy(x => x.Name).ToList();
					break;
				}
				case "Duration": {
					PlaylistRepository.Playlists[CurrentPlaylistName] = desc ?
										PlaylistRepository.Playlists[CurrentPlaylistName].OrderByDescending(x => x.Duration).ToList() :
										PlaylistRepository.Playlists[CurrentPlaylistName].OrderBy(x => x.Duration).ToList();
					break;
				}
			}
		}
		public async void SortPlaylist() {
			await Task.Run(() => {
				OnCurrentPlaylistRefreshing();

				Sorting(SortType, SortReverseToggled);
				CurrentPLaylist.Clear();
				foreach (var item in PlaylistRepository.Playlists[CurrentPlaylistName].Select((x, i) => { x.Num = ++i; return x; }))
					CurrentPLaylist.Add(item);

			}).ContinueWith((arg) => {
				OnCurrentPlaylistRefreshed();
			});
		}
	}
}
