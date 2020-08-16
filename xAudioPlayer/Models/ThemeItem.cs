using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace xAudioPlayer.Models {
	public class ThemeItem : ListItem, INotifyPropertyChanged {
		Color[] _colors = new Color[5];
		public Color[] Colors {
			get => _colors;
			set {
				if (value != null) {
					_colors = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Colors"));

					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FirstColor"));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SecondColor"));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThirdColor"));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FourthColor"));
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FifthColor"));
				}
			}
		}
		public Color FirstColor {
			get => Colors[0];
			set {
				if (value != null) {
					Colors[0] = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FirstColor"));
				}
			}
		}
		public Color SecondColor {
			get => Colors[1];
			set {
				if (value != null) {
					Colors[1] = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SecondColor"));
				}
			}
		}
		public Color ThirdColor {
			get => Colors[2];
			set {
				if (value != null) {
					Colors[1] = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ThirdColor"));
				}
			}
		}
		public Color FourthColor {
			get => Colors[3];
			set {
				if (value != null) {
					Colors[3] = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FourthColor"));
				}
			}
		}
		public Color FifthColor {
			get => Colors[4];
			set {
				if (value != null) {
					Colors[4] = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FifthColor"));
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
