
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage {
		public SettingsPage() {
			InitializeComponent();
			this.BindingContext = new SettingsViewModel(Navigation);
		}
	}
}