using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using xAudioPlayer.Models;
using xAudioPlayer.Services;

namespace xAudioPlayer.Services {
	public static class FileBrowser {
		/// <summary>
		/// Get size of directory in b/kb/Mb etc
		/// </summary>
		/// <param name="value">Size of file in bytes</param>
		/// <param name="decimalPlaces">Precision</param>
		/// <returns>Formatted size</returns>
		public static long SizeOfDirectory(string root) {
			long result = 0;
			foreach (var file in Directory.EnumerateFiles(root)) {
				try {
					result += new FileInfo(file).Length;
				} catch {
					continue;
				}
			}
			foreach (var subDir in Directory.EnumerateDirectories(root)) {
				try {
					result += SizeOfDirectory(subDir);
				} catch (UnauthorizedAccessException ex) {
					continue;
				}
			}
			return result;
		}
		/// <summary>
		/// Search directory items by fullpath in dierectories recursively
		/// </summary>
		/// <param name="root">Root directory</param>
		/// <param name="searchTerm">Search text</param>
		/// <returns>Search result</returns>
		public static IEnumerable<string> SearchAccessibleDirectoryItemsByFullName(string root, string searchTerm) {
			var files = new List<string>();

			foreach (var file in Directory.EnumerateFiles(root).Where(m => m.ToLower().Contains(searchTerm))) {
				files.Add(file);
			}
			foreach (var subDir in Directory.EnumerateDirectories(root)) {
				try {
					if (subDir.ToLower().Contains(searchTerm))
						files.Add(subDir);
					files.AddRange(SearchAccessibleDirectoryItemsByFullName(subDir, searchTerm));
				} catch (UnauthorizedAccessException ex) { continue; }
			}
			return files;
		}
		/// <summary>
		/// Get directories items of root directory and set to Obs collection
		/// </summary>
		/// <param name="root">Root directory</param>
		/// <param name="obsList">Obs collection</param>
		public static void SetDirectoriesToList(DirectoryInfo root, ObservableCollection<DirectoryItem> obsList, HashSet<string> extensions = null) {
			obsList = obsList ?? new ObservableCollection<DirectoryItem>();
			try {
				var fileSysInfos = root.GetFileSystemInfos();
				FillDirsCollectionByItems(fileSysInfos, obsList, extensions);
			} catch { }
		}
		/// <summary>
		/// Fill obs collection with FileSystemInfos elements
		/// </summary>
		/// <param name = "fileSysInfos" > Elements </ param >
		/// < param name="obsList">Obs collections</param>
		/// <param name = "forSearch" > Set description of elements for search presentation (full path of item)</param>
		public static void FillDirsCollectionByItems(IEnumerable<FileSystemInfo> fileSysInfos, ObservableCollection<DirectoryItem> obsList, HashSet<string> extensions = null) {
			try {
				foreach (var item in fileSysInfos.OrderByDescending(f => f.Extension == "").ThenBy(f => f.Name)) {
					try {
						if (item.Name == "self")
							continue;
						if (item.Extension == "") {
							obsList.Add(new DirectoryItem {
								FullPath = item.GetFileSystemInfoFullName(),
								Name = item.GetFileSystemInfoName(),
								Icon = Constants.Icons["mdi-folder-outline"],
								ItemInfo = $"{item.FullName} | {item.LastWriteTime}",
								IconColor = Color.DeepSkyBlue,
								IsFolder = true,
								DateChange = item.LastWriteTime,
								ReadOnly = item.Attributes == FileAttributes.ReadOnly ? "Yes" : "No",
								Hidden = item.Attributes == FileAttributes.Hidden ? "Yes" : "No",
								Archive = item.Attributes == FileAttributes.Archive ? "Yes" : "No",
							});
						} else if(extensions == null || extensions.Contains(item.Extension.ToLower())) {
							var size = $"{((item as FileInfo) != null ? Utilities.SizeSuffix((item as FileInfo)?.Length ?? 0, 2) : " - ")}";
							obsList.Add(new DirectoryItem {
								FullPath = item.FullName,
								Name = item.Name,
								Icon = Constants.Icons["mdi-file-music-outline"],
								ItemInfo = $"{item.FullName} | {size} | {item.LastWriteTime}",
								FormattedSize = size,
								IconColor = Color.DeepSkyBlue,
								DateChange = item.LastWriteTime,
								ReadOnly = item.Attributes == FileAttributes.ReadOnly ? "Yes" : "No",
								Hidden = item.Attributes == FileAttributes.Hidden ? "Yes" : "No",
								Archive = item.Attributes == FileAttributes.Archive ? "Yes" : "No",
							});
						}
					} catch (Exception ex) { }
				}
			} catch (Exception ex) { }
		}
		/// <summary>
		/// Allow to skip "emulated" full path
		/// </summary>
		/// <param name="info">FileSystemInfo object</param>
		/// <returns></returns>
		public static string GetFileSystemInfoFullName(this FileSystemInfo info) {
			return info.FullName.EndsWith("emulated") ? Path.Combine(info.FullName, "0") : info.FullName;
		}
		/// <summary>
		/// Allow to skip "emulated" name
		/// </summary>
		/// <param name="info">FileSystemInfo object</param>
		/// <returns></returns>
		public static string GetFileSystemInfoName(this FileSystemInfo info) {
			return info.Name.EndsWith("emulated") ? Path.Combine(info.Name, "0") : info.Name;
		}
		/// <summary>
		/// Remove files by absolute paths
		/// </summary>
		/// <param name="list">Pats</param>
		public static void RemoveFiles(IEnumerable<string> list) {
			foreach(var item in list) {
				if (File.Exists(item))
					File.Delete(item);
			}
		}
	}
}
