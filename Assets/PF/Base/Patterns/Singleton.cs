//The MIT License (MIT)

// Copyright (c) 2013 Tim Tregubov 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons 
// to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR 
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
  
using UnityEngine;
using System.Collections;
using System;

namespace PF.Base
{
	/// <summary>
	/// Prefab attribute. Use this on child classes
	/// to define if they have a prefab associated or not
	/// By default will attempt to load a prefab
	/// that has the same name as the class,
	/// otherwise [Prefab("path/to/prefab")] to define it specifically. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class PrefabAttribute : Attribute
	{
		string _name;
		public string Name 
	  	{ 
	    	get 
	    	{ 
	      		return this._name; 
	    	} 
	  	}

	 	public PrefabAttribute() 
	  	{ 
	    	this._name = ""; 
	  	}

	  	public PrefabAttribute(string name) 
	  	{ 
	    	this._name = name; 
	  	}
	}

	/// <summary>
	/// MONOBEHAVIOR PSEUDO SINGLETON ABSTRACT CLASS
	/// usage        : can be attached to a gameobject and if not
	///              : this will create one on first access
	/// example      : public sealed class MyClass : Singleton<MyClass> {
	/// references   : http://tinyurl.com/d498g8c
	///              : http://tinyurl.com/cc73a9h
	///              : http://unifycommunity.com/wiki/index.php?title=Singleton
	/// </summary>
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
	  	private static T _instance = null;
		private static object _lock = new object();
		private static bool applicationIsQuitting = false;
		public bool isAwakeOnAwake = false;
	  	public static bool IsAwake 
	  	{ 
	    	get 
	    	{ 
	      		return (_instance != null); 
	    	} 
	  	}
	          
		/// <summary>
		/// gets the instance of this Singleton
		/// use this for all instance calls:
		/// MyClass.Instance.MyMethod();
		/// or make your public methods static
		/// and have them use Instance internally
		/// for a nice clean interface
		/// </summary>
	  	public static T Instance 
	  	{
			get 
		    {
		      	if (applicationIsQuitting) 
		      	{
		        	Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
		          	"' already destroyed on application quit." +
		          	" Won't create again - returning null.");
		        	return null;
		      	}

		      	lock(_lock)
		      	{
		        	if (_instance == null) 
		        	{
		          		string singletonObjName = typeof(T).Name.ToString ();
		          		GameObject singletonObj = GameObject.Find (singletonObjName);

		          		if (singletonObj == null) //if still not found try prefab or create
		          		{
		            		// checks if the [Prefab] attribute is set and pulls that if it can
		            		bool hasPrefab = Attribute.IsDefined(typeof(T), typeof(PrefabAttribute));
		            		if (hasPrefab)
		            		{
		                		PrefabAttribute attr = (PrefabAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(PrefabAttribute));
		                		string prefabName = attr.Name;
		                		try
		                		{
		                  			if (prefabName != "")
		                  			{
		                      			singletonObj = (GameObject)Instantiate(Resources.Load(prefabName, typeof(GameObject)));
		                  			}
		                  			else
		                  			{
		                      			singletonObj = (GameObject)Instantiate(Resources.Load(singletonObjName, typeof(GameObject)));
		                  			}
		                		} 
		                		catch (Exception e)
		                		{
		                  			Debug.LogError("could not instantiate prefab even though prefab attribute was set: " + e.Message + "\n" + e.StackTrace);
		                		}
		            		}

		            		if (singletonObj == null)
		            		{
				              	singletonObj = new GameObject ();
		            		}

							DontDestroyOnLoad(singletonObj);
		            		singletonObj.name = singletonObjName;
						}

		          		_instance = singletonObj.GetComponent<T>();
		          		if (_instance == null)
		         		{
		            		_instance = singletonObj.AddComponent<T>();
		          		}
		        	}
						          
		        	return _instance;
		      	}
	    	}
	  	}

		// in your child class you can implement Awake()
		// and add any initialization code you want.


		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		/// it will create a buggy ghost object that will stay on the Editor scene
		/// even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		public void OnDestroy () 
		{
			applicationIsQuitting = true;
		}

		// Helper function to initialize the Singleton object.
		public void Load (GameObject parentObj = null) 
		{
			SetParent(parentObj);
		}


		/// <summary>
		/// Implement details in child class if it needs to be reset at some point.
		/// </summary>
		protected virtual void Reset () {}
	 

		/// <summary>
		/// Parent this to another gameobject by string
		/// call from Awake if you so desire
		/// </summary>
		protected void SetParent (GameObject parentGO)
		{
			if (parentGO != null) 
			{
				this.transform.parent = parentGO.transform;
			}
		}
	}
}