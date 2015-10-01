package md5fe675aca5ed38efdd8f29447692445a1;


public class Activity1
	extends md50e38c77f31dbf1ba1bcdac48fd82ebe5.AndroidGameActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("com.ipg.fastdogder.Activity1, com.ipg.fastdogder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Activity1.class, __md_methods);
	}


	public Activity1 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Activity1.class)
			mono.android.TypeManager.Activate ("com.ipg.fastdogder.Activity1, com.ipg.fastdogder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
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
