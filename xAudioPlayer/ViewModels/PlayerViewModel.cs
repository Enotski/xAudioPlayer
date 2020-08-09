using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Services;

namespace xAudioPlayer.ViewModels {
	public class PlayerViewModel : BaseViewModel {
		enum RepeatTypeEnum {
			None,
			One,
			All
		}
		double _audioFileProgressValue;
		double _audioFileDurationValue = 124;
		string _playlistCount = "100 / 200";
		string _currentAudioFileName = "CurrentTrackName - CurrentAuthorName";
		TimeSpan _audioFileProgress;
		TimeSpan _audioFileDuration = TimeSpan.FromSeconds(124);
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
		public static PlaylistRepository PlaylistRepository = PlaylistRepository.GetInstance();
		AudioFile _currentAudioFile;

		public string PrevIcon { get; } = Constants.Icons["mdi-chevron-left"];
		public string NextIcon { get; } = Constants.Icons["mdi-chevron-right"];
		public string DotsIcon { get; } = Constants.Icons["mdi-dots-vertical"];
		public string MenuIcon { get; } = Constants.Icons["mdi-menu"];
		public string ShuffleIcon { get; } = Constants.Icons["mdi-shuffle"];

		public Rectangle AudioFileMenuLocation { get; } = new Rectangle(10, 1, 1, 1);

		public PlayerViewModel(INavigation nav) : base(nav) {
			MenuCommand = new Command(
				execute: () => {
					MessagingCenter.Send(EventArgs.Empty, "OpenMenu");
				});
			ShuffleCommand = new Command(
				execute: () => {
					_isShuffled = !_isShuffled;
					ShuffleBtnColor = _isShuffled ? Color.DeepSkyBlue : Color.CadetBlue;
				});
			RepeatCommand = new Command(
				execute: () => {
					_repeatType = _repeatType == RepeatTypeEnum.All ? RepeatTypeEnum.None : ++_repeatType;
					RepeatBtnColor = _repeatType == RepeatTypeEnum.None ? Color.CadetBlue : Color.DeepSkyBlue;
					RepeatIcon = _repeatType == RepeatTypeEnum.None ? Constants.Icons["mdi-repeat-off"] : _repeatType == RepeatTypeEnum.One ? Constants.Icons["mdi-repeat-once"] : Constants.Icons["mdi-repeat"];
				});
			PlayPauseCommand = new Command(
				execute: () => {
				});
			AudioFileMenuCommand = new Command(
				execute: () => {
					QueueAddBtnText = $"{(PlaylistRepository.Playlists["Queue"].Any(x => x.FullPath == CurrentAudioFile.FullPath) ? "Remove form queue" : "Add to queue")}";
					FavoriteAddBtnText = $"{(PlaylistRepository.Playlists["Favorite"].Any(x => x.FullPath == CurrentAudioFile.FullPath) ? "Remove form favorite" : "Add to favorite")}";
					AudioFileMenuVisible = !AudioFileMenuVisible;
					ModalBackgroundVisible = !ModalBackgroundVisible;
				});
			ChangeAudioFileCommand = new Command(
				execute: (object args) => {
				});
			AudioFileProgressChangedCommand = new Command(
				execute: () => {
				});
			AudioFileProgressChangingCommand = new Command(
				execute: (object args) => {
					AudioFileProgress = TimeSpan.FromSeconds((double)args);
				});
			ModalBackGroundTappedCommand = new Command(
				execute: () => {
					AudioFileMenuVisible = false;
					ModalBackgroundVisible = false;
				});

			AddCommand = new Command(
				execute: async (object args) => {
					//await Task.Run(() => {
					//	if (args.ToString() == "Queue" && PlaylistRepository.Playlists["Queue"].Any(x => x.FullPath == CurrentAudioFile.FullPath)) {
					//		PlaylistRepository.RemoveFromPlayList(args.ToString(), new List<string>() { CurrentAudioFile.FullPath });
					//	} else if (args.ToString() == "Favorite" && PlaylistRepository.Playlists["Favorite"].Any(x => x.FullPath == CurrentAudioFile.FullPath)) {
					//		PlaylistRepository.RemoveFromPlayList(args.ToString(), new List<string>() { CurrentAudioFile.FullPath });
					//	} else {
					//		PlaylistRepository.AddToPlayList(args.ToString(), new List<string>() { CurrentAudioFile.FullPath });
					//	}
					//});
					AudioFileMenuVisible = false;
					ModalBackgroundVisible = false;
				});
			RemoveCommand = new Command(
				execute: (object args) => {
					//PlaylistRepository.RemoveItems(args.ToString(), new List<string>() { CurrentAudioFile.FullPath });
					ModalBackgroundVisible = false;
					AudioFileMenuVisible = false;
				});
		}

		public ICommand MenuCommand { private set; get; }
		public ICommand AudioFileMenuCommand { private set; get; }
		public ICommand ShuffleCommand { private set; get; }
		public ICommand ChangeAudioFileCommand { private set; get; }
		public ICommand PlayPauseCommand { private set; get; }
		public ICommand RepeatCommand { private set; get; }
		public ICommand AudioFileProgressChangedCommand { private set; get; }
		public ICommand ModalBackGroundTappedCommand { private set; get; }
		public ICommand AddCommand { private set; get; }
		public ICommand RemoveCommand { private set; get; }
		public ICommand AudioFileProgressChangingCommand { private set; get; }


		public string PlaylistCount {
			set { SetProperty(ref _playlistCount, value); }
			get { return _playlistCount; }
		}
		public string CurrentAudioFileName {
			set { SetProperty(ref _currentAudioFileName, value); }
			get { return _currentAudioFileName; }
		}
		public TimeSpan AudioFileProgress {
			set { SetProperty(ref _audioFileProgress, value); }
			get { return _audioFileProgress; }
		}
		public TimeSpan AudioFileDuration {
			set { SetProperty(ref _audioFileDuration, value); }
			get { return _audioFileDuration; }
		}
		public string PlayIcon {
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
	}
}
