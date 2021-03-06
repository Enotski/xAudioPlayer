﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MediaManager;
using Android;

namespace xAudioPlayer.Droid {
	[Activity(Label = "xAudioPlayer", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
		protected override void OnCreate(Bundle savedInstanceState) {

			TryToGetPermissions();
			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			CrossMediaManager.Current.Init();
			LoadApplication(new App());
		}
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults) {
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			switch (requestCode) {
				case RequestLocationId: {
					if (grantResults[0] == (int)Android.Content.PM.Permission.Granted) {
						Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();

					} else {
						//Permission Denied :(
						Toast.MakeText(this, "Special permissions denied", ToastLength.Short).Show();

					}
				}
				break;
			}

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
		void TryToGetPermissions() {
			if ((int)Build.VERSION.SdkInt >= 23) {
				GetPermissionsAsync();
				return;
			}


		}
		const int RequestLocationId = 0;

		readonly string[] PermissionsGroupLocation = {
                            //TODO add more permissions
                            Manifest.Permission.ReadExternalStorage,
							Manifest.Permission.WriteExternalStorage,
			 };
		void GetPermissionsAsync() {
			const string permission = Manifest.Permission.AccessCoarseLocation;

			if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted) {
				//TODO change the message to show the permissions name
				//Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
				return;
			}

			if (ShouldShowRequestPermissionRationale(permission)) {
				//set alert for executing the task
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("Permissions Needed");
				alert.SetMessage("The application requires permission to access the data storage");
				alert.SetPositiveButton("Request Permissions", (senderAlert, args) => {
					RequestPermissions(PermissionsGroupLocation, RequestLocationId);
				});

				alert.SetNegativeButton("Cancel", (senderAlert, args) => {
					Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
				});

				Dialog dialog = alert.Create();
				dialog.Show();


				return;
			}

			RequestPermissions(PermissionsGroupLocation, RequestLocationId);

		}
	}
}