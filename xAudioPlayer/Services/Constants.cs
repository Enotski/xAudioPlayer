using System.Collections.Generic;

namespace xAudioPlayer.Services {
	/// <summary>
	/// Repeat types
	/// </summary>
	public enum RepeatTypeEnum {
		None,
		One,
		All
	}
	/// <summary>
	/// Class of constants
	/// </summary>
	public static class Constants {
		public static Dictionary<string, string> Icons = new Dictionary<string, string>() {
			{"mdi-menu", "\U000F035C"},
			{"mdi-dots-vertical", "\U000F01D9"},
			{"mdi-shuffle", "\U000F049D"},
			{"mdi-chevron-left", "\U000F0141"},
			{"mdi-play-outline", "\U000F0F1B"},
			{"mdi-pause", "\U000F03E4"},
			{"mdi-chevron-right", "\U000F0142"},
			{"mdi-repeat", "\U000F0456"},
			{"mdi-repeat-off", "\U000F0457"},
			{"mdi-repeat-once", "\U000F0458"},
			{"mdi-playlist-plus", "\U000F0412"},
			{"mdi-playlist-remove", "\U000F0413"},
			{"mdi-cog-outline", "\U000F08BB"},
			{"mdi-drag", "\U000F01DB"},
			{"mdi-plus", "\U000F0415"},
			{"mdi-minus", "\U000F0374"},
			{"mdi-format-align-left", "\U000F0262"},
			{"mdi-magnify", "\U000F0349"},
			{"mdi-folder-outline", "\U000F0256"},
			{"mdi-file-music-outline", "\U000F0E2A"},
			{"mdi-arrow-up", "\U000F005D"},
			{"mdi-folder-home-outline", "\U000F10B6"},
			{"mdi-checkbox-multiple-marked-outline", "\U000F0139"},
			{"mdi-check-outline", "\U000F0855"},
			{"mdi-home-outline", "\U000F06A1"},
			{"mdi-sort-descending", "\U000F04BD"},
			{"mdi-sort-ascending", "\U000F04BC"},
			{"mdi-sort", "\U000F04BA"},
			{"mdi-heart-outline", "\U000F02D5"},
			{"mdi-playlist-music", "\U000F0CB8"},
			{"mdi-format-list-numbered", "\U000F027B"},
			{"mdi-delete-forever-outline", "\U000F0B89"},
		};

		public static HashSet<string> AudioFileExtensions = new HashSet<string>() {
			".mp3", ".flac", ".wav", ".aif", ".mid", ".sdt", ".flp", ".wow", ".rns", ".4pm", ".pcm", ".wav", ".ogg", ".opus", ".wave", ".midi", ".wma", ".alac", ".aac", ".aiff", ".f32"
		};
	}
}
