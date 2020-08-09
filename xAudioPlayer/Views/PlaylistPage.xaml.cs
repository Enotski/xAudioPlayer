using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.Models;
using xAudioPlayer.Services;
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