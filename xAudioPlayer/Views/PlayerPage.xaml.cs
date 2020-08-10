
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayerPage : ContentPage {
		public PlayerPage() {
			InitializeComponent();
			BindingContext = new PlayerViewModel(Navigation);
			NavigationPage.SetHasNavigationBar(this, false);
		}
	}
}