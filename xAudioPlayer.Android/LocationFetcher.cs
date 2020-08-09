using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using xAudioPlayer.Droid;
using xAudioPlayer.Interfaces;

[assembly: Dependency(typeof(LocationFetcher))]
namespace xAudioPlayer.Droid {
	public class LocationFetcher : ILocationFetcher {
        public System.Drawing.PointF GetCoordinates(global::Xamarin.Forms.VisualElement element) {
            var renderer = Platform.GetRenderer(element);
            var nativeView = renderer.View;
            var location = new int[2];
            var density = nativeView.Context.Resources.DisplayMetrics.Density;

            nativeView.GetLocationOnScreen(location);
            return new System.Drawing.PointF(location[0] / density, location[1] / density);
        }
    }
}