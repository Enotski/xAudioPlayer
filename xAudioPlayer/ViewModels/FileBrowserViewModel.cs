using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Services;

namespace xAudioPlayer.ViewModels {
	/// <summary>
	/// VM of file browser page
	/// </summary>
	public class FileBrowserViewModel : BaseViewModel {
		DirectoryInfo _currentDirectory = new DirectoryInfo("/storage");
		DirectoryItem _selectedItem;
		string _folderName;
		string _folderPath;
		bool _allChecked;
		ObservableCollection<DirectoryItem> _dirItems;

		PlaylistRepository _plRepo { get; } = PlaylistRepository.GetInstance();

		/// <summary>
		/// All checked items
		/// </summary>
		HashSet<string> _checkedItems { get; } = new HashSet<string>();

		public string ArrowUpIcon { get; } = Constants.Icons["mdi-arrow-up"];
		public string FolderHomeIcon { get; } = Constants.Icons["mdi-folder-home-outline"];
		public string CheckMultipleIcon { get; } = Constants.Icons["mdi-checkbox-multiple-marked-outline"];
		public string CheckIcon { get; } = Constants.Icons["mdi-check-outline"];

		public FileBrowserViewModel(INavigation nav) : base(nav) {
			GetDir(_currentDirectory.GetFileSystemInfoFullName());

			DirectoryItemSelectedCommand = new Command(
				execute: (object obj) => {
					var context = (obj as DirectoryItem);
					if (context?.IsFolder ?? false)
						GetDir(context.FullPath, context.ItemChecked);
				});
			CheckAllCommand = new Command(
				execute: () => {
					try {
						if(DirectoryItems != null && DirectoryItems.Any()) {
							_allChecked = !DirectoryItems.All(x => x.ItemChecked);
							foreach (var item in DirectoryItems)
								item.ItemChecked = _allChecked;
						}
					} catch(Exception ex) { }
				});
			UpCommand = new Command(
				execute: () => {
					if (_currentDirectory.Name == "storage")
						return;
					GetDir(_currentDirectory.Parent.Name == "emulated" ? _currentDirectory.Parent.Parent.FullName : _currentDirectory.Parent.FullName);
				});
			GetRootCommand = new Command(
				execute: () => {
					GetDir("/storage");
				});
			AcceptCommand = new Command(
				execute: async() => {
					await Navigation.PopModalAsync(true);
					await Task.Run(() => {
						try {
							_plRepo.RefreshCurrentPlaylistByStorage(_checkedItems.ToList());
						} catch { }
					});
				});
		}
		/// <summary>
		/// Move to parent dir
		/// </summary>
		public ICommand UpCommand { private set; get; }
		/// <summary>
		/// Chek all itenm in current dir
		/// </summary>
		public ICommand CheckAllCommand { private set; get; }
		/// <summary>
		/// Load checked items to playlist 
		/// </summary>
		public ICommand AcceptCommand { private set; get; }
		/// <summary>
		/// Move to root dir
		/// </summary>
		public ICommand GetRootCommand { private set; get; }
		/// <summary>
		/// Move to selected child dir
		/// </summary>
		public ICommand DirectoryItemSelectedCommand { private set; get; }

		public ObservableCollection<DirectoryItem> DirectoryItems {
			set { SetProperty(ref _dirItems, value); }
			get { return _dirItems; }
		}
		public string CurrentFolderName {
			set { SetProperty(ref _folderName, value); }
			get { return _folderName; }
		}
		public string CurrentFolderPath {
			set { SetProperty(ref _folderPath, value); }
			get { return _folderPath; }
		}
		public DirectoryItem SelectedItem {
			get {
				return _selectedItem;
			}
			set {
				_selectedItem = value;

				if (_selectedItem == null)
					return;

				DirectoryItemSelectedCommand.Execute(_selectedItem);

				SelectedItem = null;
			}
		}

		/// <summary>
		/// Get directory items and set them to obs collection
		/// </summary>
		/// <param name="root">Directory full path</param>
		private async void GetDir(string root, bool itemChecked = false) {
			try {
				if (File.Exists(root)) {
					return;
				}
				_currentDirectory = new DirectoryInfo(root);
				DirectoryItems = DirectoryItems ?? new ObservableCollection<DirectoryItem>();
				DirectoryItems.Clear();
				// if name of folder is '0' set it as 'emulated/0' 
				CurrentFolderName = _currentDirectory.Name == "0" ? Path.Combine(_currentDirectory.Parent.Name, _currentDirectory.Name) : _currentDirectory.Name;
				CurrentFolderPath = _currentDirectory.GetFileSystemInfoFullName();

				await Task.Run(() => {
					FileBrowser.SetDirectoriesToList(_currentDirectory, DirectoryItems, Constants.AudioFileExtensions);
					// set right chekBox state
					foreach (var item in DirectoryItems) {
						item.PropertyChanged += ItemChecked_PropertyChanged;
						item.ItemChecked = itemChecked ? true : _checkedItems.Contains(item.FullPath);
					}
				});
			} catch (Exception ex) {
				return;
			}
		}
		/// <summary>
		/// Set or remove items from checked HashSet
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ItemChecked_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			var item = sender as DirectoryItem;
			if (item == null)
				return;
			await Task.Run(() => {
				try {
					if (item.ItemChecked) {
						if (!_checkedItems.Contains(item.FullPath))
							_checkedItems.Add(item.FullPath);
					} else {
						if (_checkedItems.Contains(item.FullPath)) {
							_checkedItems.Remove(item.FullPath);
							if (Directory.Exists(item.FullPath)) {
								foreach (var file in Directory.EnumerateFileSystemEntries(item.FullPath))
									_checkedItems.Remove(file);
							}
						}
					}
				} catch { }			
			});			
		}
	}
}
