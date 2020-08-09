using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace xAudioPlayer.ViewModels {
	/// <summary>
	/// Base class of view-model
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged {

		protected INavigation Navigation { get; set; }

		public BaseViewModel(INavigation nav) {
			Navigation = nav;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
			if (Object.Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
