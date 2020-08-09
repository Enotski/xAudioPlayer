using System;

namespace xAudioPlayer.Models {
	/// <summary>
	/// Master page item
	/// </summary>
	public class MasterPageItem {
		public string Title { get; set; }
		public string Icon { get; set; }
		public Type TargetPage { get; set; }
		public string Parameter { get; internal set; }
	}
}
