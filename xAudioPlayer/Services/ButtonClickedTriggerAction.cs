using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using xAudioPlayer.Interfaces;

namespace xAudioPlayer.Services {
	public class ButtonClickedTriggerAction : TriggerAction<Button> {
		ILocationFetcher service;
		public ButtonClickedTriggerAction() {
			service = DependencyService.Get<ILocationFetcher>();
		}
		public ButtonClickedTriggerAction(Action<System.Drawing.PointF> action) : this() {
			OnLocationFetched += (arg) => action(arg);
		}

		protected override async void Invoke(Button sender) {
			try {
				if (sender.Parent.Parent.GetType() == typeof(ViewCell)) {
					var location = service.GetCoordinates(sender);
					OnLocationFetched?.Invoke(location);
				}

				var actualColor = sender.TextColor;
				var randomColor = Color.OrangeRed;

				await sender.ChangeTextColorTo(randomColor, 150, Easing.CubicOut);
				await sender.ChangeTextColorTo(actualColor, 100, Easing.SinOut);
			} catch(Exception ex) { }		
		}
		public delegate void LocationFetched(System.Drawing.PointF location);
		public static event LocationFetched OnLocationFetched;
	}
}
