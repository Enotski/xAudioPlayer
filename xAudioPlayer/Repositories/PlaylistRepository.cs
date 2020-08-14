using MediaManager.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using xAudioPlayer.Models;
using xAudioPlayer.Services;

namespace xAudioPlayer.Repositories {
	/// <summary>
	/// Repository for working with playlists
	/// </summary>
	public class PlaylistRepository {
		RepeatTypeEnum _repeatType = RepeatTypeEnum.None;
		bool _isShuffle;
		int _playlistAudioFileIndx = 0;
		string _lastSettedPlaylist;

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
		/// <summary>
		/// Occures when current playlist collection start changing
		/// </summary>
		public delegate void CurrentPlaylistRefreshing();
		public event CurrentPlaylistRefreshing OnCurrentPlaylistRefreshing;

		/// <summary>
		/// Occures when current playlist collection changed
		/// </summary>
		public delegate void CurrentPlaylistRefreshed();
		public event CurrentPlaylistRefreshed OnCurrentPlaylistRefreshed;

		/// <summary>
		/// Occures when playlist was add/removed from dictionary
		/// </summary>
		public delegate void PlaylistsCollectionRefreshed();
		public event PlaylistsCollectionRefreshed OnPlaylistsCollectionRefreshed;

		/// <summary>
		/// Occures when current audio file changed
		/// </summary>
		public delegate void AudioFileChanged(AudioFile file);
		public event AudioFileChanged OnAudioFileChanged;

		/// <summary>
		/// Occures when playing paused/played
		/// </summary>
		public delegate void PLayPauseAudioFile(AudioFile file = null);
		public event PLayPauseAudioFile OnPLayPauseAudioFile;

		/// <summary>
		/// Occures when MediaState updatet 
		/// </summary>
		public delegate void MediaStateUpdated(MediaPlayerState state);
		public event MediaStateUpdated OnMediaStateUpdated;

		/// <summary>
		/// Occures when MediaState updatet 
		/// </summary>
		public delegate void MediaProgressUpdated(TimeSpan progress);
		public event MediaProgressUpdated OnMediaProgressUpdated;

		public string CurrentPlaylistName { get; set; }
		public Dictionary<string, List<AudioFile>> Playlists { get; } = new Dictionary<string, List<AudioFile>>();

		/// <summary>
		/// Get data from xml file
		/// </summary>
		public async void GetDataFromXml() {
			try {

			} catch { }
		}
		/// <summary>
		/// Save data to xml file
		/// </summary>
		public async void SetDataToXml() {
			try {

			} catch { }
		}
		/// <summary>
		/// Chnage current playlist
		/// </summary>
		public void SetCurrentPlaylist(string name) {
			name = name == "PlayList" ? _lastSettedPlaylist : name;
			name = !string.IsNullOrWhiteSpace(name) && Playlists.ContainsKey(name) ? name : "Default";

			_lastSettedPlaylist = name != "Favorite" ? name : _lastSettedPlaylist;

			OnCurrentPlaylistRefreshing?.Invoke();
			CurrentPlaylistName = name;
			_playlistAudioFileIndx = 0;
			OnCurrentPlaylistRefreshed?.Invoke();

		}
		public void CurrentAudioProgressUpdated(TimeSpan progress) {
			OnMediaProgressUpdated?.Invoke(progress);
		}
		/// <summary>
		/// Refresh playlist by storage
		/// </summary>
		/// <param name="itemsPaths"></param>
		public void RefreshCurrentPlaylistByStorage(IEnumerable<string> itemsPaths) {
			try {
				OnCurrentPlaylistRefreshing?.Invoke();
				RefreshPlaylist(itemsPaths);
				OnCurrentPlaylistRefreshed?.Invoke();
			} catch (Exception ex) { OnCurrentPlaylistRefreshed?.Invoke(); }
		}
		/// <summary>
		/// Remove items from playlist/directory
		/// </summary>
		/// <param name="type">Directory</param>
		/// <param name="items">if items is null, then clear collection</param>
		/// <param name="plName">Playlist name</param>
		public void RemoveItems(bool fromDir, IEnumerable<string> items = null) {
			try {
				if (Playlists.ContainsKey(CurrentPlaylistName)) {
					OnCurrentPlaylistRefreshing?.Invoke();
					if (items == null)
						Playlists[CurrentPlaylistName].Clear();
					else {
						Playlists[CurrentPlaylistName].RemoveAll(x => items.Contains(x.FullPath));

						if (fromDir)
							FileBrowser.RemoveFiles(items);
					}
					OnCurrentPlaylistRefreshed?.Invoke();
				}
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Add audio files from current playlist to other
		/// </summary>
		/// <param name="playlistName">Specified playlist</param>
		/// <param name="itemsPaths">Files paths</param>
		public void AddToPlayList(string playlistName, IEnumerable<string> itemsPaths) {
			try {
				if (Playlists.ContainsKey(playlistName)) {
					foreach (var itemPath in itemsPaths) {
						try {
							if (File.Exists(itemPath) && !Playlists[playlistName].Any(x => x.FullPath == itemPath) && Playlists[CurrentPlaylistName].Any(x => x.FullPath == itemPath))
								Playlists[playlistName].Add(Playlists[CurrentPlaylistName].First(x => x.FullPath == itemPath));
						} catch (Exception ex) { }
					}
				}
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Remove audio file from specified playlist
		/// </summary>
		/// <param name="playlistName">Specified playlist</param>
		/// <param name="itemsPaths">Files paths</param>
		public void RemoveFromPlayList(string playlistName, IEnumerable<string> itemsPaths) {
			try {
				OnCurrentPlaylistRefreshing?.Invoke();
				if (Playlists.ContainsKey(playlistName)) {
					foreach (var itemPath in itemsPaths) {
						try {
							if (Playlists[playlistName].Any(x => x.FullPath == itemPath))
								Playlists[playlistName].RemoveAll(x => x.FullPath == itemPath);
						} catch (Exception ex) { }
					}
				}
				OnCurrentPlaylistRefreshed?.Invoke();
			} catch (Exception ex) { OnCurrentPlaylistRefreshed?.Invoke(); }
		}
		/// <summary>
		/// Refresh current playlist
		/// </summary>
		/// <param name="itemsPaths">Files paths</param>
		private void RefreshPlaylist(IEnumerable<string> itemsPaths) {
			try {
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
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Add new playlist to dict
		/// </summary>
		/// <param name="name">Name of pl</param>
		public void AddPlayList(string name) {
			try {
				if (!string.IsNullOrWhiteSpace(name) && !Playlists.ContainsKey(name)) {
					Playlists.Add(name, new List<AudioFile>());
					OnPlaylistsCollectionRefreshed?.Invoke();
				}
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Remove existing playlist from dict
		/// </summary>
		/// <param name="name">Name of pl</param>
		public void RemovePlaylist(string name) {
			try {
				if (!string.IsNullOrWhiteSpace(name) && Playlists.ContainsKey(name) && (name != "Default" || name != "Favorite" || name != "Queue")) {
					Playlists.Remove(name);
					SetCurrentPlaylist("Default");
					OnPlaylistsCollectionRefreshed?.Invoke();
				}
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Sort current pl
		/// </summary>
		/// <param name="type">Name/duration</param>
		/// <param name="desc">By descending</param>
		public void SortPlayList(string type, bool desc) {
			OnCurrentPlaylistRefreshing?.Invoke();
			try {
				switch (type) {
					case "Name": {
						Playlists[CurrentPlaylistName] = desc ?
													Playlists[CurrentPlaylistName].OrderByDescending(x => x.Name).ToList() :
													Playlists[CurrentPlaylistName].OrderBy(x => x.Name).ToList();
						break;
					}
					case "Duration": {
						Playlists[CurrentPlaylistName] = desc ?
													Playlists[CurrentPlaylistName].OrderByDescending(x => x.Duration).ToList() :
													Playlists[CurrentPlaylistName].OrderBy(x => x.Duration).ToList();
						break;
					}
				}
			} catch { }
			OnCurrentPlaylistRefreshed?.Invoke();
		}
		/// <summary>
		/// Change order of items
		/// </summary>
		/// <param name="oldIndex"></param>
		/// <param name="newIndex"></param>
		public void ChnageAudioFilesOrder(int oldIndex, int newIndex, string plName = "") {
			try {
				var currName = string.IsNullOrWhiteSpace(plName) ? CurrentPlaylistName : plName;

				var tmp = Playlists[currName].ElementAt(oldIndex);
				Playlists[currName].RemoveAt(oldIndex);
				Playlists[currName].Insert(newIndex, tmp);
			} catch { }
		}
		public void PlayPauseAudioFile(string path = null) {
			try {
				if (string.IsNullOrWhiteSpace(path))
					OnPLayPauseAudioFile?.Invoke();
				else if (Playlists[CurrentPlaylistName].Any(x => x.FullPath == path))
					OnPLayPauseAudioFile?.Invoke(Playlists[CurrentPlaylistName].FirstOrDefault(x => x.FullPath == path));
			} catch { }
		}

		public void UpdateMediaPLayerState(MediaPlayerState state) {
			OnMediaStateUpdated?.Invoke(state);
		}
		public void SetShuffle(bool isShuffle) {
			_isShuffle = isShuffle;
		}
		public void ChangeAudioFile(string filePath, bool prev, bool isHandle = false) {
			try {
				if (Playlists["Queue"].Any() && !isHandle) {

					var file = Playlists["Queue"].ElementAt(0);
					OnPLayPauseAudioFile?.Invoke(file);
					OnAudioFileChanged?.Invoke(file);
					Playlists["Queue"].RemoveAt(0);
				} else {
					if (!string.IsNullOrWhiteSpace(CurrentPlaylistName) && Playlists.ContainsKey(CurrentPlaylistName)) {
						var file = Playlists[CurrentPlaylistName].FirstOrDefault(x => x.FullPath == filePath);

						_playlistAudioFileIndx = Playlists[CurrentPlaylistName].IndexOf(file);

						if (isHandle || !_isShuffle) {
							_playlistAudioFileIndx = prev ? (_playlistAudioFileIndx == -1 || _playlistAudioFileIndx <= 0 ? Playlists[CurrentPlaylistName].Count - 1 : --_playlistAudioFileIndx) :
								(_playlistAudioFileIndx == -1 || _playlistAudioFileIndx >= Playlists[CurrentPlaylistName].Count - 1 ? 0 : ++_playlistAudioFileIndx);
						} else {
							var rnd = new Random();
							int result = 0;
							do {
								_playlistAudioFileIndx = rnd.Next(0, Playlists[CurrentPlaylistName].Count);
							} while (_playlistAudioFileIndx == result);
						}
					}
					if (Playlists[CurrentPlaylistName].Any() && Playlists[CurrentPlaylistName].Count > _playlistAudioFileIndx) {
						OnAudioFileChanged?.Invoke(Playlists[CurrentPlaylistName].ElementAt(_playlistAudioFileIndx));
					}
				}
			} catch { }
		}
	}
}
