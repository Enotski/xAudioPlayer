using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace xAudioPlayer.Services {
	public class CustomAbsoluteLayout : AbsoluteLayout {
		protected async override void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			base.OnPropertyChanged(propertyName);
			if (propertyName == "IsEnabled") {
				this.IsVisible = this.IsEnabled;
				await this.FadeTo(this.IsEnabled ? 1 : 0, 200);
			};
		}
	}
}
