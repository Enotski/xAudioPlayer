
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlaylistPage : ContentPage {
		public PlaylistPage() {
			InitializeComponent();
			BindingContext = new PlaylistViewModel(Navigation);
			NavigationPage.SetHasNavigationBar(this, false);
		}
	}
}