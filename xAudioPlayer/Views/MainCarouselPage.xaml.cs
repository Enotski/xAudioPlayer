
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainCarouselPage : CarouselPage {
		public MainCarouselPage() {
			InitializeComponent();
			PlayerContent.BindingContext = new PlayerViewModel(Navigation);
			PlaylistContent.BindingContext = new PlaylistViewModel(Navigation);
			NavigationPage.SetHasNavigationBar(this, false);

			MessagingCenter.Subscribe<EventArgs>(this, "SetPlayerPage", args => {
				if (this.CurrentPage != this.PlayerContent)
					this.CurrentPage = this.PlayerContent;
			});
			MessagingCenter.Subscribe<EventArgs>(this, "SetPlaylistPage", args => {
				if (this.CurrentPage != this.PlaylistContent)
					this.CurrentPage = this.PlaylistContent;
			});
		}
	}
}