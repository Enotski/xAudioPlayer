using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Services;

namespace xAudioPlayer.ViewModels {
	public class QueueViewModel : BaseViewModel {
		string _playlistInfo;
		bool _modalBackgroundVisible;
		bool _audioFileMenuVisible;
		bool _allItemsChecked;
		Rectangle _audioFileMenuLocation;
		AudioFile _selectedItem;
		ButtonClickedTriggerAction _btnClickedTrigger;

		public ObservableListCollection<AudioFile> _queuePLaylist = new ObservableListCollection<AudioFile>();
		PlaylistRepository _plRepo = PlaylistRepository.GetInstance();
		public string DotsIcon { get; } = Constants.Icons["mdi-dots-vertical"];
		public string CheckMultipleIcon { get; } = Constants.Icons["mdi-checkbox-multiple-marked-outline"];
		public string DeleteIcon { get; } = Constants.Icons["mdi-delete-forever-outline"];
		public QueueViewModel(INavigation nav) : base(nav) {
			LoadQueuePlayListFromRepo();

			_btnClickedTrigger = new ButtonClickedTriggerAction(SetAudioFileMenuLocation);
			_queuePLaylist.CollectionChanged += (sender, e) => {
				int i = 0;
				_plRepo.ChnageAudioFilesOrder(e.OldStartingIndex, e.NewStartingIndex, "Queue");
				foreach (var item in _queuePLaylist) {
					item.Num = ++i;
				}
			};
			AudioFileMenuCommand = new Command(
				execute: (object item) => {
					AudioFileMenuVisible = !AudioFileMenuVisible;
					ModalBackgroundVisible = !ModalBackgroundVisible;
					_selectedItem = item as AudioFile;
				});
			CheckAllCommand = new Command(
				execute: async () => {
					await Task.Run(() => {
						try {
							_allItemsChecked = !QueuePLaylist.All(x => x.ItemChecked);
							foreach (var item in QueuePLaylist) {
								item.ItemChecked = _allItemsChecked;
							}
						} catch { }
					});
				});
			AcceptRemoveCommand = new Command(
				execute: () => {
					RemoveAudioFiles();
				});
			RemoveCommand = new Command(
				execute: () => {
					var item = QueuePLaylist.FirstOrDefault(x => x.FullPath == _selectedItem.FullPath);
					item.ItemChecked = true;
					RemoveAudioFiles();

					AudioFileMenuVisible = false;
					AudioFileMenuVisible = false;
				});
			ModalBackGroundTappedCommand = new Command(
				execute: () => {
					AudioFileMenuVisible = false;
					ModalBackgroundVisible = false;
				});
		}
		/// <summary>
		/// Get audio file menu
		/// </summary>
		public ICommand AudioFileMenuCommand { private set; get; }
		/// <summary>
		/// Check all audio files in pl
		/// </summary>
		public ICommand CheckAllCommand { private set; get; }
		/// <summary>
		/// Remove selected audio files from pl
		/// </summary>
		public ICommand AcceptRemoveCommand { private set; get; }
		/// <summary>
		/// Modal bg tapped
		/// </summary>
		public ICommand ModalBackGroundTappedCommand { private set; get; }
		/// <summary>
		/// Get remove mode or remove audio file form pl/dir
		/// </summary>
		public ICommand RemoveCommand { private set; get; }

		public ObservableListCollection<AudioFile> QueuePLaylist {
			get => _queuePLaylist;
			set {
				SetProperty(ref _queuePLaylist, value);
			}
		}
		public string PlaylistInfo {
			set { SetProperty(ref _playlistInfo, value); }
			get { return _playlistInfo; }
		}
		public bool ModalBackgroundVisible {
			set { SetProperty(ref _modalBackgroundVisible, value); }
			get { return _modalBackgroundVisible; }
		}
		public bool AudioFileMenuVisible {
			set { SetProperty(ref _audioFileMenuVisible, value); }
			get { return _audioFileMenuVisible; }
		}
		public Rectangle AudioFileMenuLocation {
			set { SetProperty(ref _audioFileMenuLocation, value); }
			get { return _audioFileMenuLocation; }
		}
		/// <summary>
		/// Set location of audio file menu by fetched pressed btn location
		/// </summary>
		/// <param name="arg"></param>
		void SetAudioFileMenuLocation(System.Drawing.PointF arg) {
			AudioFileMenuLocation = new Rectangle(arg.X - 200, arg.Y < 350 ? arg.Y : arg.Y - 245, 200, 245);
		}
		/// <summary>
		/// Remove audio files from pl/dir
		/// </summary>
		public async void RemoveAudioFiles() {
			try {
				await Task.Run(() => { _plRepo.RemoveFromPlayList("Queue", QueuePLaylist.Where(x => x.ItemChecked).Select(x => x.FullPath)); });
			} catch { }
		}
		private async void LoadQueuePlayListFromRepo() {
			try {
				await Task.Run(() => {
					QueuePLaylist.Clear();
					foreach (var item in _plRepo.Playlists["Queue"].Select((x, i) => { x.Num = ++i; return x; }))
						QueuePLaylist.Add(item);

					var totalDuration = new TimeSpan(QueuePLaylist.Sum(r => r.Duration.Ticks));
					PlaylistInfo = $"{QueuePLaylist.Count} / {totalDuration:hh\\:mm\\:ss} / {Utilities.SizeSuffix(QueuePLaylist.Sum(x => x.Size), 2)}";
				});
			} catch { }
		}
	}
}
