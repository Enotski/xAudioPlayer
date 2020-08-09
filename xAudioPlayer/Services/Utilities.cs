using System;
using System.Collections.Generic;
using System.Text;

namespace xAudioPlayer.Services {
	public static class Utilities {
		public static readonly string[] SizeSuffixes =
		   { "bytes", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

		public static readonly string[] RateSampleSuffixes =
			{ "Hz", "kHz", "MHz", "GHz", "THz", "PHz", "EHz", "ZHz", "YHz" };
		/// <summary>
		/// Get size of file in b/kb/Mb etc
		/// </summary>
		/// <param name="value">Size of file in bytes</param>
		/// <param name="decimalPlaces">Precision</param>
		/// <returns>Formatted size</returns>
		public static string SizeSuffix(long value, int decimalPlaces = 1) {
			if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
			if (value < 0) { return $"-{SizeSuffix(-value)}"; }
			if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

			int mag = (int)Math.Log(value, 1024);
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			if (Math.Round(adjustedSize, decimalPlaces) >= 1000) {
				mag += 1;
				adjustedSize /= 1024;
			}
			return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
		}

		/// <summary>
		/// Get AudioSampleRate of file in b/kb/Mb etc
		/// </summary>
		/// <param name="value">Size of file in bytes</param>
		/// <param name="decimalPlaces">Precision</param>
		/// <returns>Formatted size</returns>
		public static string RateSampleSuffix(int value, int decimalPlaces = 1) {
			if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
			if (value < 0) { return $"-{SizeSuffix(-value)}"; }
			if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

			int mag = (int)Math.Log(value, 1000);
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			if (Math.Round(adjustedSize, decimalPlaces) >= 1000) {
				mag += 1;
				adjustedSize /= 1000;
			}
			return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, RateSampleSuffixes[mag]);
		}
	}
}
