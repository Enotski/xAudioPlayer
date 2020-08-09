using MediaManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.Models;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainCarouselPage : CarouselPage {
		public MainCarouselPage() {
			InitializeComponent();
			PlayerContent.BindingContext = new PlayerViewModel(Navigation);
			PlaylistContent.BindingContext = new PlaylistViewModel(Navigation);
			NavigationPage.SetHasNavigationBar(this, false);
		}
	}
}