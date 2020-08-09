using System;
using System.Collections.Generic;
using System.Text;

namespace xAudioPlayer.Interfaces {
	/// <summary>
	/// Used by bound collections to expose ordering methods/events
	/// </summary>
	public interface IOrderable {
		/// <summary>
		/// Event fired when the items in the collection are re-ordered.
		/// </summary>
		event EventHandler OrderChanged;
		/// <summary>
		/// Used to change the item orders in an enumerable
		/// </summary>
		/// <param name="oldIndex">
		/// The old index of the item.
		/// </param>
		/// <param name="newIndex">
		/// The new index of the item.
		/// </param>
		void ChangeOrdinal(int oldIndex, int newIndex);
	}
}
