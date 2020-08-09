using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using xAudioPlayer.Models;

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