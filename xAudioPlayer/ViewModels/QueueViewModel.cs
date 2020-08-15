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
		bool _allItemsChecked;
		Rectangle _audioFileMenuLocation;

		public ObservableListCollection<AudioFile> _queuePLaylist = new ObservableListCollection<AudioFile>();
		PlaylistRepository _plRepo = PlaylistRepository.GetInstance();
		public string CloseIcon { get; } = Constants.Icons["mdi-close"];
		public string CheckMultipleIcon { get; } = Constants.Icons["mdi-checkbox-multiple-marked-outline"];
		public string DeleteIcon { get; } = Constants.Icons["mdi-delete-forever-outline"];
		public QueueViewModel(INavigation nav) : base(nav) {
			LoadQueuePlayListFromRepo();
			_plRepo.OnPlaylistRefreshed += LoadQueuePlayListFromRepo;

			_queuePLaylist.CollectionChanged += (sender, e) => {
				int i = 0;
				_plRepo.ChnageAudioFilesOrder(e.OldStartingIndex, e.NewStartingIndex, "Queue");
				foreach (var item in _queuePLaylist) {
					item.Num = ++i;
				}
			};
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
				execute: (object args) => {
					RemoveAudioFiles(args as AudioFile);
				});
		}
		/// <summary>
		/// Check all audio files in pl
		/// </summary>
		public ICommand CheckAllCommand { private set; get; }
		/// <summary>
		/// Remove selected audio files from pl
		/// </summary>
		public ICommand AcceptRemoveCommand { private set; get; }
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
		public Rectangle AudioFileMenuLocation {
			set { SetProperty(ref _audioFileMenuLocation, value); }
			get { return _audioFileMenuLocation; }
		}
		/// <summary>
		/// Remove audio files from pl/dir
		/// </summary>
		public async void RemoveAudioFiles(AudioFile file = null ) {
			try {
				await Task.Run(() => { _plRepo.RemoveFromPlayList("Queue", file != null ? new List<string>() { file.FullPath } : QueuePLaylist.Where(x => x.ItemChecked).Select(x => x.FullPath)); });
			} catch { }
		}
		private async void LoadQueuePlayListFromRepo() {
			try {
				await Task.Run(() => {
					QueuePLaylist.Clear();
					foreach (var item in _plRepo.Playlists["Queue"].Select((x, i) => { x.Num = ++i; x.ItemChecked = false; return x; }))
						QueuePLaylist.Add(item);

					UpdateUi();
				});
			} catch { }
		}
		private void UpdateUi() {
			var totalDuration = new TimeSpan(QueuePLaylist.Sum(r => r.Duration.Ticks));
			PlaylistInfo = $"{QueuePLaylist.Count} / {totalDuration:hh\\:mm\\:ss} / {Utilities.SizeSuffix(QueuePLaylist.Sum(x => x.Size), 2)}";
		}
	}
}
