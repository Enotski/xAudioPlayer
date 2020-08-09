using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Repositories;
using xAudioPlayer.Services;

namespace xAudioPlayer.ViewModels {
	public class FileBrowserViewModel : BaseViewModel {
		DirectoryInfo _currentDirectory = new DirectoryInfo("/storage");
		DirectoryItem _selectedItem;
		string _folderName;
		string _folderPath;
		bool _allChecked;

		PlaylistRepository PlaylistRepository { get; } = PlaylistRepository.GetInstance();

		HashSet<string> checkedItems { get; } = new HashSet<string>();

		public string ArrowUpIcon { get; } = Constants.Icons["mdi-arrow-up"];
		public string FolderHomeIcon { get; } = Constants.Icons["mdi-folder-home-outline"];
		public string CheckMultipleIcon { get; } = Constants.Icons["mdi-checkbox-multiple-marked-outline"];
		public string CheckIcon { get; } = Constants.Icons["mdi-check-outline"];

		public ObservableCollection<DirectoryItem> DirectoryItems { get; set; }

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
						PlaylistRepository.RefreshCurrentPlaylistByStorage(checkedItems.ToList());
					});
				});
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
		public ICommand UpCommand { private set; get; }
		public ICommand CheckAllCommand { private set; get; }
		public ICommand AcceptCommand { private set; get; }
		public ICommand GetRootCommand { private set; get; }
		public ICommand DirectoryItemSelectedCommand { private set; get; }

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
					foreach (var item in DirectoryItems) {
						item.PropertyChanged += ItemChecked_PropertyChanged;
						item.ItemChecked = itemChecked ? true : checkedItems.Contains(item.FullPath);
					}
				});
			} catch (Exception ex) {
				return;
			}
		}
		private async void ItemChecked_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			var item = sender as DirectoryItem;
			if (item == null)
				return;
			await Task.Run(() => {
				if (item.ItemChecked) {
					if (!checkedItems.Contains(item.FullPath))
						checkedItems.Add(item.FullPath);
				} else {
					if (checkedItems.Contains(item.FullPath)) {
						checkedItems.Remove(item.FullPath);
						if (Directory.Exists(item.FullPath)) {
							foreach (var file in Directory.EnumerateFileSystemEntries(item.FullPath))
								checkedItems.Remove(file);
						}
					}
				}
			});			
		}
	}
}
