
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RootPage : MasterDetailPage {
		public RootPage() {
			InitializeComponent();
		}
		public void NavigateToPage(Page page) {
			Detail = new NavigationPage(page);
			IsPresented = false;
		}
	}
}