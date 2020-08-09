using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RootPageMaster : ContentPage {
		public RootPageMaster() {
			InitializeComponent();
			BindingContext = new RootViewModel(Navigation);
			MenuListView.ItemSelected += MenuListView_ItemSelected;
		}

		private void MenuListView_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
			var selected = sender;
		}
	}
}