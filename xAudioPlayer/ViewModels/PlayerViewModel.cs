using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Services;
using MediaManager;
using MediaManager.Playback;
using MediaManager.Media;
using MediaManager.Library;

namespace xAudioPlayer.ViewModels {
	/// <summary>
	/// VM of player page
	/// </summary>
	public class PlayerViewModel : BaseViewModel {
		double _audioFileProgressValue;
		double _audioFileDurationValue = 0.1;
		string _playlistInfo = "-/-";
		string _currentAudioFileName = "-/-";
		TimeSpan _audioFileProgressTime;
		TimeSpan _audioFileDurationTime;
		string _playIcon = Constants.Icons["mdi-play-outline"];
		string _repeatIcon = Constants.Icons["mdi-repeat"];
		string _queueAddBtnText;
		string _favoriteAddBtnText;
		bool _isShuffled;
		bool _modalBackgroundVisible;
		bool _audioFileMenuVisible;
		RepeatTypeEnum _repeatType = RepeatTypeEnum.None;
		Color _shuffleBtnColor = Color.CadetBlue;
		Color _repeatBtnColor = Color.CadetBlue;
		AudioFile _currentAudioFile;

		public static PlaylistRepository _plRepo = PlaylistRepository.GetInstance();

		public string PrevIcon { get; } = Constants.Icons["mdi-chevron-left"];
		public string NextIcon { get; } = Constants.Icons["mdi-chevron-right"];
		public string DotsIcon { get; } = Constants.Icons["mdi-dots-vertical"];
		public string MenuIcon { get; } = Constants.Icons["mdi-menu"];
		public string ShuffleIcon { get; } = Constants.Icons["mdi-shuffle"];

		public Rectangle AudioFileMenuLocation { get; } = new Rectangle(10, 1, 1, 1);

		public PlayerViewModel(INavigation nav) : base(nav) {
			_plRepo.OnPlaylistsCollectionRefreshed += PlaylistsCollectionRefreshed;
			_plRepo.OnPlaylistRefreshed += PlayListRefreshed;
			_plRepo.OnPLayPauseAudioFile += PlayPauseAudioFile;

			CrossMediaManager.Current.StateChanged += MediaStateChanged;
			CrossMediaManager.Current.MediaItemFinished += MediaItemFinished;
			CrossMediaManager.Current.PositionChanged += MediaPositionChanged;

			MenuCommand = new Command(
				execute: () => {
					MessagingCenter.Send(EventArgs.Empty, "OpenMenu");
				});
			ShuffleCommand = new Command(
				execute: () => {
					_isShuffled = !_isShuffled;
					ShuffleBtnColor = _isShuffled ? Color.DeepSkyBlue : Color.CadetBlue;
					_plRepo.SetShuffle(_isShuffled);
				});
			RepeatCommand = new Command(
				execute: () => {
					_repeatType = _repeatType == RepeatTypeEnum.All ? RepeatTypeEnum.None : ++_repeatType;
					RepeatBtnColor = _repeatType == RepeatTypeEnum.None ? Color.CadetBlue : Color.DeepSkyBlue;
					RepeatIcon = _repeatType == RepeatTypeEnum.None ? Constants.Icons["mdi-repeat-off"] : _repeatType == RepeatTypeEnum.One ? Constants.Icons["mdi-repeat-once"] : Constants.Icons["mdi-repeat"];
				});
			AudioFileMenuCommand = new Command(
				execute: () => {
					QueueAddBtnText = $"{(_plRepo.Playlists["Queue"].Any(x => x.FullPath == CurrentAudioFile.FullPath) ? "Remove form queue" : "Add to queue")}";
					FavoriteAddBtnText = $"{(_plRepo.Playlists["Favorite"].Any(x => x.FullPath == CurrentAudioFile.FullPath) ? "Remove form favorite" : "Add to favorite")}";
					AudioFileMenuVisible = !AudioFileMenuVisible;
					ModalBackgroundVisible = !ModalBackgroundVisible;
				});
			PlayPauseCommand = new Command(
				execute: () => {
					PlayPauseAudioFile();
				});
			ChangeAudioFileCommand = new Command(
				execute: (object args) => {
					ChangeAudioFile(true, args.ToString() == "prev");
				});
			AudioFileProgressChangedCommand = new Command(
				execute: () => {
					CrossMediaManager.Current.SeekTo(TimeSpan.FromMinutes(AudioFileProgressValue));
				});
			AudioFileProgressChangingCommand = new Command(
				execute: (object args) => {
					AudioFileProgressTime = TimeSpan.FromMinutes((double)args);
					_plRepo.CurrentAudioProgressUpdated(AudioFileProgressTime);
				});
			ModalBackGroundTappedCommand = new Command(
				execute: () => {
					AudioFileMenuVisible = false;
					ModalBackgroundVisible = false;
				});
			AddCommand = new Command(
				execute: (object args) => {
					AddAudioFile(args.ToString());
					AudioFileMenuVisible = false;
					ModalBackgroundVisible = false;
				});
			RemoveCommand = new Command(
				execute: (object args) => {
					_plRepo.RemoveItems(args.ToString() == "Directory", new List<string>() { CurrentAudioFile?.FullPath });
					ModalBackgroundVisible = false;
					AudioFileMenuVisible = false;
				});
		}

		/// <summary>
		/// Open master page
		/// </summary>
		public ICommand MenuCommand { private set; get; }
		/// <summary>
		/// Open menu of current audio file
		/// </summary>
		public ICommand AudioFileMenuCommand { private set; get; }
		/// <summary>
		/// Shuffle current pl
		/// </summary>
		public ICommand ShuffleCommand { private set; get; }
		/// <summary>
		/// Change current audio file with prev/next 
		/// </summary>
		public ICommand ChangeAudioFileCommand { private set; get; }
		/// <summary>
		/// Play/pause
		/// </summary>
		public ICommand PlayPauseCommand { private set; get; }
		/// <summary>
		/// Change repeat mod (None/One/All)
		/// </summary>
		public ICommand RepeatCommand { private set; get; }
		/// <summary>
		/// Handle changed of audio file progress
		/// </summary>
		public ICommand AudioFileProgressChangedCommand { private set; get; }
		/// <summary>
		/// Handle changing of audio file progress
		/// </summary>
		public ICommand AudioFileProgressChangingCommand { private set; get; }
		/// <summary>
		/// Modal bg tapped
		/// </summary>
		public ICommand ModalBackGroundTappedCommand { private set; get; }
		/// <summary>
		/// Add audio file to any pl
		/// </summary>
		public ICommand AddCommand { private set; get; }
		/// <summary>
		/// Remove audio file from any pl
		/// </summary>
		public ICommand RemoveCommand { private set; get; }

		public string PlaylistInfo {
			set { SetProperty(ref _playlistInfo, value); }
			get { return _playlistInfo; }
		}
		public string CurrentAudioFileName {
			set { SetProperty(ref _currentAudioFileName, value); }
			get { return _currentAudioFileName; }
		}
		public TimeSpan AudioFileProgressTime {
			set { SetProperty(ref _audioFileProgressTime, value); }
			get { return _audioFileProgressTime; }
		}
		public TimeSpan AudioFileDurationTime {
			set { SetProperty(ref _audioFileDurationTime, value); }
			get { return _audioFileDurationTime; }
		}
		public string PlayPauseIcon {
			set { SetProperty(ref _playIcon, value); }
			get { return _playIcon; }
		}
		public string RepeatIcon {
			set { SetProperty(ref _repeatIcon, value); }
			get { return _repeatIcon; }
		}
		public bool ModalBackgroundVisible {
			set { SetProperty(ref _modalBackgroundVisible, value); }
			get { return _modalBackgroundVisible; }
		}
		public bool AudioFileMenuVisible {
			set { SetProperty(ref _audioFileMenuVisible, value); }
			get { return _audioFileMenuVisible; }
		}
		public double AudioFileDurationValue {
			set { SetProperty(ref _audioFileDurationValue, value); }
			get => _audioFileDurationValue;
		}
		public AudioFile CurrentAudioFile {
			set { SetProperty(ref _currentAudioFile, value); }
			get => _currentAudioFile;
		}
		public double AudioFileProgressValue {
			get => _audioFileProgressValue;
			set {
				SetProperty(ref _audioFileProgressValue, value);
				AudioFileProgressChangingCommand.Execute(_audioFileProgressValue);
			}
		}
		public Color ShuffleBtnColor {
			set { SetProperty(ref _shuffleBtnColor, value); }
			get { return _shuffleBtnColor; }
		}
		public Color RepeatBtnColor {
			set { SetProperty(ref _repeatBtnColor, value); }
			get { return _repeatBtnColor; }
		}
		public string QueueAddBtnText {
			set { SetProperty(ref _queueAddBtnText, value); }
			get { return _queueAddBtnText; }
		}
		public string FavoriteAddBtnText {
			set { SetProperty(ref _favoriteAddBtnText, value); }
			get { return _favoriteAddBtnText; }
		}
		private async void PlaylistsCollectionRefreshed() {
			await Task.Run(() => {
				if (!_plRepo.Playlists[_plRepo.CurrentPlaylistName].Contains(CurrentAudioFile))
					CrossMediaManager.Current.Stop();
				UpdatePlInfo();
			});
		}
		private async void PlayPauseAudioFile(AudioFile file = null) {
			if (file != null) {
				CurrentAudioFile = file;
				AudioFileDurationTime = CurrentAudioFile.Duration;
				AudioFileDurationValue = CurrentAudioFile.Duration.TotalMinutes;
				await CrossMediaManager.Current.Play(file.FullPath);
				UpdatePlInfo();
			} else if (CurrentAudioFile == null) {
				ChangeAudioFile(false);
			} else {
				await CrossMediaManager.Current.PlayPause();
			}
		}
		private async void MediaPositionChanged(object sender, MediaManager.Playback.PositionChangedEventArgs e) {
			await Task.Run(() => {
				AudioFileProgressValue = e.Position.TotalMinutes;
				if (AudioFileProgressTime > CurrentAudioFile.Duration)
					MediaItemFinished(this, new MediaItemEventArgs(new MediaItem()));
			});
		}
		private async void MediaItemFinished(object sender, MediaItemEventArgs e) {
			if (Math.Truncate(CrossMediaManager.Current.Position.TotalMinutes) >= Math.Truncate(CurrentAudioFile.Duration.TotalMinutes)) {
				if (_repeatType == RepeatTypeEnum.One) {
					await CrossMediaManager.Current.SeekTo(TimeSpan.FromMinutes(0));
				} else if (_repeatType == RepeatTypeEnum.None && (_plRepo.Playlists[_plRepo.CurrentPlaylistName].IndexOf(CurrentAudioFile) + 1 == _plRepo.Playlists[_plRepo.CurrentPlaylistName].Count())) {
					await CrossMediaManager.Current.SeekToStart().ContinueWith((args) => { CrossMediaManager.Current.Stop(); });
				} else {
					ChangeAudioFile(false);
				}
			}
		}
		private async void MediaStateChanged(object sender, StateChangedEventArgs e) {
			await Task.Run(() => {
				PlayPauseIcon = CrossMediaManager.Current.State == MediaManager.Player.MediaPlayerState.Playing ? Constants.Icons["mdi-pause"] : Constants.Icons["mdi-play-outline"];
				_plRepo.UpdateMediaPLayerState(CrossMediaManager.Current.State);
			});
		}
		private async void PlayListRefreshed() {
			await Task.Run(() => {
				if (!_plRepo.Playlists[_plRepo.CurrentPlaylistName].Contains(CurrentAudioFile)) {
					CrossMediaManager.Current.Stop();
				}
				UpdatePlInfo();
			});
		}
		private async void UpdatePlInfo() {
			await Task.Run(() => {
				CurrentAudioFileName = CurrentAudioFile?.Name ?? "-/-";
				PlaylistInfo = $"{_plRepo.CurrentPlaylistName} ({ (CurrentAudioFile == null ? 0 : _plRepo.Playlists[_plRepo.CurrentPlaylistName].IndexOf(CurrentAudioFile) + 1)} / {_plRepo.Playlists[_plRepo.CurrentPlaylistName].Count()})";
			});
		}
		private async void ChangeAudioFile(bool isHandle, bool prev = false) {
			try {
				await Task.Run(() => {
					_plRepo.ChangeAudioFile(CurrentAudioFile?.FullPath ?? "", prev, isHandle);
				});
			} catch { }
		}
		async void AddAudioFile(string args) {
			try {
				await Task.Run(() => {
					if (args == "Queue" && _plRepo.Playlists["Queue"].Any(x => x.FullPath == CurrentAudioFile.FullPath)) {
						_plRepo.RemoveFromPlayList(args, new List<string>() { CurrentAudioFile.FullPath });
					} else if (args == "Favorite" && _plRepo.Playlists["Favorite"].Any(x => x.FullPath == CurrentAudioFile.FullPath)) {
						_plRepo.RemoveFromPlayList(args, new List<string>() { CurrentAudioFile.FullPath });
					} else {
						_plRepo.AddToPlayList(args, new List<string>() { CurrentAudioFile.FullPath });
					}
				});
			} catch { }
		}
	}
}
