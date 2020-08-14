using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.ViewModels;

namespace xAudioPlayer.Views {
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class QueuePage : ContentPage {
		public QueuePage() {
			InitializeComponent();
			this.BindingContext = new QueueViewModel(Navigation);
		}
	}
}