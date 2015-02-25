using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.NPNFEditor;
using NPNF.Core;

[CustomEditor (typeof(NPNFSettings))]
public class NPNFSettingsEditor : Editor
{
	private GUIContent appVersionLabel = new GUIContent("App Version:", "Set the version for your app");
	private GUIContent appIdLabel = new GUIContent("App ID:");
	private GUIContent appSecretLabel = new GUIContent("App Secret:");
	private GUIContent sdkVersionLabel = new GUIContent("SDK Version", "This is Unity NPNF SDK version. If you have problems or compliments, please include this so we know which version to look out for.");
    private GUIContent adminIdLabel = new GUIContent("Admin ID:");
    private GUIContent adminSecretLabel = new GUIContent("Admin Secret:");
    private GUIContent getVersionsLabel = new GUIContent("Get Version(s)");
    private GUIContent reloadVersionsLabel = new GUIContent("Refresh");
    private string verifyStatus = "";
	private NPNFSettings instance;

    private bool isAppSettingsValid = true;
    private bool isAdminSettingsValid = true;
    private bool isVersionSettingsValid = true;

    private bool isClientVersionExist = false;

    public override void OnInspectorGUI()
	{
        instance = (NPNFSettings)target;

        EditorGUILayout.Space();
		AppKeyGUI();
		EditorGUILayout.Space();
        AdminKeyGUI();
//        EditorGUILayout.Space();
//        PushNotificationsGUI ();
		EditorGUILayout.Space();
		AboutGUI();
        EditorGUILayout.Space();
        VerifySettingsGUI();
        // SSDK-1154 - force OnInspectorGUI to be called on every frame
        EditorUtility.SetDirty(target);
        instance.Update();
	}

	private void AppKeyGUI()
	{
		EditorGUILayout.HelpBox("Add the NPNF Platform keys associated with this game", MessageType.None);
        string newAppId = EditableField(appIdLabel, instance.AppId, 180, isAppSettingsValid);
        string newAppSecret = EditableField(appSecretLabel, instance.AppSecret, 180, isAppSettingsValid);
        if (newAppId != instance.AppId || newAppSecret != instance.AppSecret)
        {
            instance.ClientVersions = new string[] {};
            instance.AppVersion = null;
            instance.AppId = newAppId;
            instance.AppSecret = newAppSecret;
            ManifestMod.GenerateManifest();
        }

        if (Event.current.type == EventType.Layout)
        {
            if(instance.ClientVersions != null && instance.ClientVersions.Length > 0)
            {
                isClientVersionExist = true;
            }
            else
            {
                isClientVersionExist = false;
            }
        }

        if(isClientVersionExist)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(appVersionLabel, GUILayout.Width(180), GUILayout.Height (16));
			GUI.color = Color.white;
            instance.SelectedVersionIndex = EditorGUILayout.Popup(instance.SelectedVersionIndex, instance.ClientVersions);
            if (instance.ClientVersions.Length > instance.SelectedVersionIndex &&
                instance.AppVersion != instance.ClientVersions[instance.SelectedVersionIndex])
            {
                instance.AppVersion = instance.ClientVersions[instance.SelectedVersionIndex];
            }

            GUI.enabled = !String.IsNullOrEmpty(instance.AppId) && !String.IsNullOrEmpty(instance.AppSecret);
            if (GUILayout.Button(reloadVersionsLabel))
            {
                RefreshVersions();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        else
        {
            GetVersionsButton(appVersionLabel, getVersionsLabel, 180, isVersionSettingsValid);
        }
	}

    private void RefreshVersions()
    {
        instance.VerifyAppKeys((NPNFError verifyError) => {
            if (verifyError == null)
            {
                CheckSettingStatus(NPNF.Admin.AdminManager.SettingsType.AppSettings, true);
                instance.GetVersions(instance.AdminId, instance.AdminSecret, NPNFSettings.Instance, (string[] versionsResponse, NPNFError error) => {
                    if (error != null)
                    {
                        if (error.Messages != null && error.Messages.Count > 0)
                        {
                            EditorGUILayout.HelpBox(error.Messages[0], MessageType.Error);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Error occurred while reloading data", MessageType.Error);
                        }
                    }
                    else
                    {
                        instance.ClientVersions = versionsResponse;

                        // find the index for the current verions after refresh
                        if (instance.AppVersion != null)
                        {
                            for (int i = 0; i < versionsResponse.Length; i++)
                            {
                                if (versionsResponse[i].Equals(instance.AppVersion))
                                {
                                    instance.SelectedVersionIndex = i;
                                    break;
                                }
                            }
                        }
                        else if (versionsResponse.Length > 0)
                        {
                            instance.SelectedVersionIndex = 0;
                            instance.AppVersion = versionsResponse[0];
                        }
                    }
                });
            }
            else
            {
				String message = verifyError.Messages[0];
				if (verifyError.Messages[0] == "Keys are invalid")
				{
					message = "App ID or App Secret is invalid. Please verify these settings.";
				}
				EditorUtility.DisplayDialog("NPNFSettings", message, "OK");
                instance.ClientVersions = new string[] {};
            }
        });
    }

    private void AdminKeyGUI()
    {
        GUI.color = Color.white;
        EditorGUILayout.HelpBox("Add the Admin keys associated with this game (for Asset Manager)", MessageType.None);
        instance.AdminId = EditableField(adminIdLabel, instance.AdminId, 180, isAdminSettingsValid);
        instance.AdminSecret = EditableField(adminSecretLabel, instance.AdminSecret, 180, isAdminSettingsValid);
    }

	private void AboutGUI ()
	{
		EditorGUILayout.HelpBox ("About the NPNF Platform SDK", MessageType.None);
		SelectableLabelField (sdkVersionLabel, NPNFSettings.SDK_VERSION);

		if (!NPNFSettings.IsValidVersion())
		{
			EditorGUILayout.HelpBox("Mismatch SDK Version", MessageType.Error);
		}
	}

    private string EditableField(GUIContent label, string value, int width = 180, bool isValid = true)
	{
		string ret = "";
		EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (label, GUILayout.Width (width), GUILayout.Height (16));
        if (isValid == false)
            GUI.color = Color.red;
        ret = EditorGUILayout.TextField(value, GUILayout.Height(16));
        GUI.color = Color.white;
		EditorGUILayout.EndHorizontal ();
		return ret;
    }

    private void GetVersionsButton (GUIContent label, GUIContent buttonLabel, int width = 180, bool isValid = true)
    {
        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (label, GUILayout.Width (width), GUILayout.Height (16));
        GUI.enabled = !String.IsNullOrEmpty(instance.AppId) && !String.IsNullOrEmpty(instance.AppSecret);
        if (isValid == false)
            GUI.color = Color.red;
        if (GUILayout.Button(buttonLabel))
        {
            RefreshVersions();
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal ();
    }

    private void VerifySettingsGUI()
    {
        EditorGUILayout.BeginVertical();
        GUI.enabled = true;
        if (GUILayout.Button("Verify Configuration"))
        {
            ResetVerifyStatus();
            instance.VerifyAppSettings(NPNFSettings.Instance, (NPNF.Admin.AdminManager.SettingsType type, bool isValid, string message) =>
            {
                CheckSettingStatus(type, isValid);
                verifyStatus += (string.IsNullOrEmpty(verifyStatus) ? "" : "\n") + message;
            });
        }
        EditorGUILayout.LabelField(verifyStatus, GUILayout.Height(48));
        EditorGUILayout.EndHorizontal();
    }

    private void ResetVerifyStatus()
    {
        verifyStatus = "";
        isAppSettingsValid = true;
        isAdminSettingsValid = true;
        isVersionSettingsValid = true;
    }

    /// <summary>
    /// Check settings status
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isValid"></param>
    private void CheckSettingStatus(NPNF.Admin.AdminManager.SettingsType type, bool isValid)
    {
        switch(type)
        {
            case NPNF.Admin.AdminManager.SettingsType.AppSettings:
                isAppSettingsValid = isValid;
                // If app settings is not correct, make version settings not correct too
                if (string.IsNullOrEmpty(instance.AppVersion))
                    isVersionSettingsValid = false;
                break;
            case NPNF.Admin.AdminManager.SettingsType.AdminSettings:
                isAdminSettingsValid = isValid;
                break;
            case NPNF.Admin.AdminManager.SettingsType.VersionSettings: 
                isVersionSettingsValid = isValid;
                break;
        }
    }

	private void SelectableLabelField (GUIContent label, string value)
	{
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (label, GUILayout.Width (180), GUILayout.Height (16));
		EditorGUILayout.SelectableLabel (value, GUILayout.Height (16));
		EditorGUILayout.EndHorizontal ();
	}

	public static void SetAppIdAndSecret(string appId, string appSecret)
	{
		if (appId == null || appSecret == null)
		{
			throw new ArgumentNullException("NPNF app Id and app secret cannot be null");
		}
		else
		{
			NPNFSettings.Instance.AppId = appId;
			NPNFSettings.Instance.AppSecret = appSecret;
			ManifestMod.GenerateManifest();
		}
	}

	public static void SetAppVersion(string value)
	{
		if (value == null)
		{
			Debug.Log("ERROR: App Version is null");
		}
		else
		{
			NPNFSettings.Instance.AppVersion = value;
		}
	}

    public static void ClearAllKeys()
    {
        NPNFSettings.Instance.AdminId = "";
        NPNFSettings.Instance.AdminSecret = "";
        NPNFSettings.Instance.AppId = "";
        NPNFSettings.Instance.AppSecret = "";
        NPNFSettings.Instance.AndroidGCMSenderID = "";
        NPNFSettings.Instance.AppVersion = "";
    }
}
