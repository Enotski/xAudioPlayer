using System;
using Xamarin.Forms;
using xAudioPlayer.Interfaces;
using xAudioPlayer.Repositories;

namespace xAudioPlayer.Services {
	/// <summary>
	/// Button clicked trigger
	/// </summary>
	public class ButtonClickedTriggerAction : TriggerAction<Button> {
		ILocationFetcher service;
		/// <summary>
		/// Get service
		/// </summary>
		public ButtonClickedTriggerAction() {
			service = DependencyService.Get<ILocationFetcher>();
		}
		/// <summary>
		/// Get button location
		/// </summary>
		/// <param name="action"></param>
		public ButtonClickedTriggerAction(Action<System.Drawing.PointF> action) : this() {
			OnLocationFetched += (arg) => action(arg);
		}
		/// <summary>
		/// Change color of button when it's clicked
		/// </summary>
		/// <param name="sender"></param>
		protected override async void Invoke(Button sender) {
			try {
				if (sender.Parent.Parent.GetType() == typeof(ViewCell)) {
					var location = service.GetCoordinates(sender);
					OnLocationFetched?.Invoke(location);
				}

				var actualColor = sender.TextColor;
				var randomColor = SettingsRepository.CurrentTheme.SecondColor;

				await sender.ChangeTextColorTo(randomColor, 150, Easing.CubicOut);
				await sender.ChangeTextColorTo(actualColor, 100, Easing.SinOut);
			} catch(Exception ex) { }		
		}
		public delegate void LocationFetched(System.Drawing.PointF location);
		public static event LocationFetched OnLocationFetched;
	}
}
