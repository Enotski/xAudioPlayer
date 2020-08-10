
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FileBrowserPage : ContentPage {
		public FileBrowserPage() {
			InitializeComponent();
			BindingContext = new FileBrowserViewModel(Navigation);
		}
	}
}