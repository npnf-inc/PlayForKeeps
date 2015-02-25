/**
 * Platform - This class is for initialization of the platform 
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;

using NPNF.Core;
using NPNF.Core.Utils;
using NPNF.Core.Users.Tracking;

#if !UNITY_WEBPLAYER
using System.Net.NetworkInformation;
#endif

namespace NPNF
{
    internal enum NPNFPlatformInitState
    {
        NotInitialize,
        Initializing,
        HasInitialized
    }
   
    public delegate void PlatformInitCallback(NPNFError error);

    public partial class Platform
    {
        private static NPNFPlatformInitState initState = NPNFPlatformInitState.NotInitialize;

        private static NPNFSettings Settings { get; set; }

        #if UNITY_EDITOR || UNITY_IPHONE || UNITY_ANDROID
        public static UserTrackingController utController;
        #endif

        private static PlatformInitCallback InitCallback = null;
        private static Queue<PlatformInitCallback> CallbackQueue = new Queue<PlatformInitCallback>();

        /** 
         * Call this method to initialize the NPNF platform
         **/
        public static void Init(PlatformInitCallback callback = null)
        {
            switch(initState)
            {
                case NPNFPlatformInitState.NotInitialize:
                    ProcessInit(callback);
                    break;
                case NPNFPlatformInitState.Initializing:
                    QueueInit(callback);
                    break;
                case NPNFPlatformInitState.HasInitialized:
                    SkipInit(callback);
                    break;
            }
        }

        private static void ProcessInit(PlatformInitCallback callback = null)
        {
            initState = NPNFPlatformInitState.Initializing;

            InitCallback = callback;
            UnityEngine.Debug.Log("NPNF Platform Init");
            Settings = NPNFSettings.Instance;
            NPNFMain.Settings = Settings;
            if (NPNFMain.Settings != null)
            {
                NPNFMain.OnInitProfile += InitProfile;
                NPNFMain.OnLoadComplete += LoadComplete;
            }
            CustomFeaturesInitialization();

            NPNFMain.Init();
            #if !UNITY_WEBPLAYER
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
            #endif
        }

        private static void QueueInit(PlatformInitCallback callback = null)
        {
            CallbackQueue.Enqueue((callback != null)? callback : InternalInitCallback);
        }

        private static void SkipInit(PlatformInitCallback callback = null)
        {
            UnityEngine.Debug.Log("Platform already Initialized");
            if (callback != null)
            {
                callback(null);
            }
        }

        private static void InternalInitCallback(NPNFError error)
        {
            // queue position holder
        }

        private static void CustomFeaturesInitialization()
        {
            Type type = typeof(Platform);
            MethodInfo[] infos = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            
            for (int i = 0; i < infos.Length; i++)
            {
                MethodInfo info = infos [i];
                
                if (info.Name.StartsWith("PlatformInit"))
                {
                    info.Invoke(null, null);
                }
            }
        }
        
        private static void LoadComplete(object sender, LoadCompleteArgs args)
        {
            initState = NPNFPlatformInitState.HasInitialized;

            if (InitCallback != null)
            {
                InitCallback(args.error);
                InitCallback = null;
            }

            foreach (PlatformInitCallback callback in CallbackQueue)
            {
                callback(args.error);
            }
            CallbackQueue.Clear();
        }

        private static void InitProfile(object sender, EventArgs args)
        {
#if !UNITY_EDITOR
            NPNFSettings.Instance.RefreshDeviceProfile();
#endif
        }
        
    }
}
