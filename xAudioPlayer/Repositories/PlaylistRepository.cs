using MediaManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using xAudioPlayer.Models;
using xAudioPlayer.Services;

namespace xAudioPlayer.Repositories {
	public class PlaylistRepository {
		private static PlaylistRepository _instance;
		private PlaylistRepository() {
			if (!Playlists.Any()) {
				Playlists.Add("Default", new List<AudioFile>());
				Playlists.Add("Favorite", new List<AudioFile>());
				Playlists.Add("Queue", new List<AudioFile>());
				CurrentPlaylistName = "Default";
			}
		}
		public static PlaylistRepository GetInstance() {
			if (_instance == null)
				_instance = new PlaylistRepository();
			return _instance;
		}
		public delegate void CurrentPlaylistRefreshing();
		public event CurrentPlaylistRefreshing OnCurrentPlaylistRefreshing;

		public delegate void CurrentPlaylistRefreshed();
		public event CurrentPlaylistRefreshed OnCurrentPlaylistRefreshed;

		public delegate void PlaylistsCollectionRefreshed();
		public event PlaylistsCollectionRefreshed OnPlaylistsCollectionRefreshed;

		public string CurrentPlaylistName { get; set; }
		public Dictionary<string, List<AudioFile>> Playlists { get; } = new Dictionary<string, List<AudioFile>>();

		public async void GetDataFromXml() {

		}
		public async void SetDataToXml() {

		}
		public void SetCurrentPlaylist() {

		}
		public void RefreshCurrentPlaylistByStorage(IEnumerable<string> itemsPaths) {
			try {
				OnCurrentPlaylistRefreshing?.Invoke();
				RefreshPlaylist(itemsPaths);
				OnCurrentPlaylistRefreshed?.Invoke();
			} catch (Exception ex) { OnCurrentPlaylistRefreshed?.Invoke(); }
		}
		public void RemoveItems(string type = null, IEnumerable<string> items = null) {
			if (Playlists.ContainsKey(CurrentPlaylistName)) {
				if (items == null)
					Playlists[CurrentPlaylistName].Clear();
				else {
					Playlists[CurrentPlaylistName].RemoveAll(x => items.Contains(x.FullPath));

					if (type == "Directory")
						FileBrowser.RemoveFiles(items);
				}
			}
		}
		public void AddToPlayList(string playlistName, IEnumerable<string> itemsPaths) {
			if (Playlists.ContainsKey(playlistName)) {
				foreach (var itemPath in itemsPaths) {
					if (File.Exists(itemPath) && !Playlists[playlistName].Any(x => x.FullPath == itemPath) && Playlists[CurrentPlaylistName].Any(x => x.FullPath == itemPath))
						Playlists[playlistName].Add(Playlists[CurrentPlaylistName].First(x => x.FullPath == itemPath));
				}
			}
		}
		public void RemoveFromPlayList(string playlistName, IEnumerable<string> itemsPaths) {
			if (Playlists.ContainsKey(playlistName)) {
				foreach (var itemPath in itemsPaths) {
					if (Playlists[playlistName].Any(x => x.FullPath == itemPath))
						Playlists[playlistName].RemoveAll(x => x.FullPath == itemPath);
				}
			}
		}
		private void RefreshPlaylist(IEnumerable<string> itemsPaths) {
			if (Playlists.ContainsKey(CurrentPlaylistName)) {
				if (itemsPaths?.Any() ?? false) {
					foreach (var itemPath in itemsPaths) {
						try {
							if (Directory.Exists(itemPath))
								RefreshPlaylist(new DirectoryInfo(itemPath).GetFileSystemInfos().Select(f => f.FullName));

							if (!File.Exists(itemPath)) {
								if (Playlists[CurrentPlaylistName].Any(x => x.FullPath == itemPath)) {
									Playlists[CurrentPlaylistName].RemoveAll(x => x.FullPath == itemPath);
								}
								continue;

							} else if (!Playlists[CurrentPlaylistName].Any(x => x.FullPath == itemPath)) {
								var file = new FileInfo(itemPath);

								if (!Constants.AudioFileExtensions.Contains(file.Extension.ToLower()))
									continue;

								var tfile = TagLib.File.Create(itemPath);
								Playlists[CurrentPlaylistName].Add(new AudioFile() {
									Name = file.Name.Replace(file.Extension, ""),
									FullPath = file.FullName,
									FolderName = file.DirectoryName,
									BitRate = tfile.Properties.AudioBitrate,
									ExtensionName = file.Extension,
									SampleRate = tfile.Properties.AudioSampleRate,
									Channels = tfile.Properties.AudioChannels,
									Duration = tfile.Properties.Duration,
									Size = file.Length
								});

							}
						} catch { continue; }
					}
				}
			}
		}
		public void AddPlayList(string name) {
			if (!string.IsNullOrWhiteSpace(name) && !Playlists.ContainsKey(name)) {
				Playlists.Add(name, new List<AudioFile>());
				OnPlaylistsCollectionRefreshed?.Invoke();
			}
		}
		public void RemovePlaylist(string name) {
			if (!string.IsNullOrWhiteSpace(name) &&  Playlists.ContainsKey(name) && (name != "Default" || name != "Favorite" || name != "Queue")) {
				Playlists.Remove(name);
				OnPlaylistsCollectionRefreshed?.Invoke();
			}
		}
	}
}
