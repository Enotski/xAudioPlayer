using System;
using System.ComponentModel;
using xAudioPlayer.Services;

namespace xAudioPlayer.Models {
	/// <summary>
	/// Audio file
	/// </summary>
	public class AudioFile : ListItem, INotifyPropertyChanged {
		bool _itemChecked;
		int _num;

		public string NumFormatted { get => Num > 9 ? Num.ToString() : "0" + Num.ToString(); }
		public string FolderName { get; set; }
		public string FullPath { get; set; }
		public string ExtensionName { get; set; }
		public int SampleRate { get; set; }
		public double BitRate { get; set; }
		public long Size { get; set; }
		public string CoverPath { get; set; }
		public int Channels { get; set; }
		public TimeSpan Duration { get; set; }
		public int Num {
			get => _num;
			set {
				if (_num != value) {
					_num = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Num"));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FullNameFormatted"));
				}
			}
		}
		public bool ItemChecked {
			set {
				if (_itemChecked != value) {
					_itemChecked = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ItemChecked"));
				}
			}
			get {
				return _itemChecked;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string FullNameFormatted {
			get => $"{NumFormatted}. {Name}";
		}
		public string AudioFilePropertiesFormatted {
			get => $"{ExtensionName?.TrimStart(new char[] { '.' }).ToUpper()} :: {Utilities.RateSampleSuffix(SampleRate, 0)} :: {BitRate} kbps :: {(Channels == 2 ? "Stereo" : Channels == 4 ? "Quadro" : Channels == 6 ? "5.1" : Channels == 8 ? "7.1" : "-/-")} :: {Utilities.SizeSuffix(Size, 2)}";
		}
		public AudioFile() { }
	}
}
