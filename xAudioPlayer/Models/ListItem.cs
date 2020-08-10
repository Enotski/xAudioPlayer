using Xamarin.Forms;

namespace xAudioPlayer.Models {
	/// <summary>
	/// Base class of collection item 
	/// </summary>
	public class ListItem {
		public string Name { get; set; }
		public string Icon { get; set; }

		public ListItem() { }
		public ListItem(string name, string icon) {
			Name = name;
			Icon = icon;
		}
	}
}
