using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace xAudioPlayer.Models {
	/// <summary>
	/// Directory item
	/// </summary>
	public class DirectoryItem : ListItem, INotifyPropertyChanged {

		private bool _itemChecked = false;

		public string FullPath { get; set; }
		public string ItemInfo { get; set; }
		public bool IsFolder { get; set; }
		public Color IconColor { get; set; }
		public string FormattedSize { get; set; }
		public DateTime DateChange { get; set; }
		public string ReadOnly { get; set; }
		public string Hidden { get; set; }
		public string Archive { get; set; }

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

		public DirectoryItem() { }
		public DirectoryItem(string name, string path, string icon) : base(name, icon) {
			FullPath = path;
		}
	}
}
