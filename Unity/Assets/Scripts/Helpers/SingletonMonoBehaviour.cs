using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
	where T : SingletonMonoBehaviour<T>
{
	private static T mInstance = null;
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static T Instance
	{
		get
		{
			if (mInstance == null)
			{
				string singletonName = "_" + typeof(T).ToString();
				GameObject singleton = GameObject.Find(singletonName);
				
				if (singleton == null)
				{
					singleton = new GameObject(singletonName);
				}
				
				if (singleton != null)
				{
					mInstance = singleton.GetComponent<T>();
					
					if (mInstance == null)
					{
						mInstance = singleton.AddComponent<T>();
					}
					
					mInstance.Init();
					DontDestroyOnLoad(singleton);
				}
			}
			
			return mInstance;
		}
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	protected virtual void Start()
	{
		if (mInstance == null)
		{
			mInstance = (T)this;
			mInstance.Init();
			DontDestroyOnLoad(gameObject);
		}
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected virtual void OnDestroy()
	{
		mInstance = null;
	}
	
	/// <summary>
	/// Init this instance.
	/// </summary>
	protected abstract void Init();
}
