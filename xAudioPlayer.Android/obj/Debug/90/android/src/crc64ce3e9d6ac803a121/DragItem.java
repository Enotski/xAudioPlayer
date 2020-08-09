package crc64ce3e9d6ac803a121;


public class DragItem
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("xAudioPlayer.Droid.Effects.DragItem, xAudioPlayer.Android", DragItem.class, __md_methods);
	}


	public DragItem ()
	{
		super ();
		if (getClass () == DragItem.class)
			mono.android.TypeManager.Activate ("xAudioPlayer.Droid.Effects.DragItem, xAudioPlayer.Android", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
