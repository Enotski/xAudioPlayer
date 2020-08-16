using System;
using Xamarin.Forms;
using xAudioPlayer.Repositories;
using xAudioPlayer.Views;

namespace xAudioPlayer {
	public partial class App : Application {
		public App() {
			InitializeComponent();
			Device.SetFlags(new[] {
				"SwipeView_Experimental",
				"CarouselView_Experimental",
				"RadioButton_Experimental",
				"IndicatorView_Experimental",
			});
			var settings = SettingsRepository.GetInstance();
			var rootPage = new RootPage();
			MainPage = rootPage;
			MessagingCenter.Subscribe<EventArgs>(this, "OpenMenu", args =>
			{
				rootPage.IsPresented = true;
			});
			MessagingCenter.Subscribe<EventArgs>(this, "CloseMenu", args => {
				rootPage.IsPresented = false;
			});
		}

		protected override void OnStart() {

		}

		protected override void OnSleep() {
		}

		protected override void OnResume() {
		}
	}
}
