using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace xAudioPlayer.Services {
	/// <summary>
	/// Custom appearing of absLayout
	/// </summary>
	public class CustomAbsoluteLayout : AbsoluteLayout {
		/// <summary>
		/// Smooth appearance of layout
		/// </summary>
		/// <param name="propertyName"></param>
		protected async override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			base.OnPropertyChanged(propertyName);
			if (propertyName == "IsEnabled") {
				this.IsVisible = this.IsEnabled;
				await this.FadeTo(this.IsEnabled ? 1 : 0, 200);
			};
		}
	}
}
