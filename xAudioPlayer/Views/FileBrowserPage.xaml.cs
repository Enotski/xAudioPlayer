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
	public partial class FileBrowserPage : ContentPage {
		public FileBrowserPage() {
			InitializeComponent();
			BindingContext = new FileBrowserViewModel(Navigation);
		}
	}
}