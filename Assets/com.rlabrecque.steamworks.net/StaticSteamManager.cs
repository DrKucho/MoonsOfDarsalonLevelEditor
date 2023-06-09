// The SteamManager is designed to work with Steamworks.NET
// This file is released into the public domain.
// Where that dedication is not recognized you are granted a perpetual,
// irrevocable license to copy and modify this file as you see fit.
//
// Version: 1.0.13
#if UNITY_EDITOR
#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEditor;
using UnityEngine;
#if !DISABLESTEAMWORKS
using System.Collections;
using Steamworks;
#endif


//
// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
// It handles the basics of starting up and shutting down the SteamAPI for use.
//
[InitializeOnLoad]
public static class StaticSteamManager {
#if !DISABLESTEAMWORKS
	static bool s_EverInitialized = false;


	static bool m_bInitialized = false; 
	public static bool Initialized {
		get {
			return m_bInitialized;
		}
	}

	static SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	[AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText) {
		Debug.LogWarning(pchDebugText);
	}

#if UNITY_2019_3_OR_NEWER
	// In case of disabled Domain Reload, reset static members before entering Play Mode.
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		s_EverInitialized = false;
	}
#endif

	static StaticSteamManager()
	{
		Init();
		EditorApplication.update += Update;
		EditorApplication.quitting += OnDestroy;
	}

	private static AppId_t _appID;
	public static AppId_t appID
	{
		get
		{
			if (_appID.m_AppId == 0)
				_appID = new AppId_t(GameData.instance.GetSteamAppId());// MOD DEMO = 1644470 , MOD FULL = 1234180 
			return _appID;
		}
	}

	static public void Init() {   
		
		if(s_EverInitialized) {
			// This is almost always an error.
			// The most common case where this happens is when SteamManager gets destroyed because of Application.Quit(),
			// and then some Steamworks code in some other OnDestroy gets called afterwards, creating a new SteamManager.
			// You should never call Steamworks functions in OnDestroy, always prefer OnDisable if possible.
			throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");
		}

		if (!Packsize.Test()) { 
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
		}

		if (!DllCheck.Test()) {
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
		}

		try { 
			// If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
			// Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.
			// Note that this will run which ever version you have installed in steam. Which may not be the precise executable
			// we were currently running.

			// Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
			// remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
			// See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
			if (GameData.instance) // puede fallar si esta abriendo el proyecto por primera vez, parece que no enconetra gamedata porque no esta construida la asset database?
			{
				//var appID = new AppId_t(GameData.instance.GetSteamAppId()); // MOD DEMO = 1644470 , MOD FULL = 1234180 
				if (SteamAPI.RestartAppIfNecessary(appID))
				{
					Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");

					Application.Quit();
					return;
				}
			}
		}
		catch (System.DllNotFoundException e) { // We catch this exception here, as it will be the first occurrence of it.
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e);

			Application.Quit();
			return;
		}

		// Initializes the Steamworks API.
		// If this returns false then this indicates one of the following conditions:
		// [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
		// [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
		// [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
		// [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
		// [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
		// Valve's documentation for this is located here:
		// https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
		m_bInitialized = SteamAPI.Init(); 
		if (!m_bInitialized) {
			Debug.LogError("COULD NOT CONNECT... IS STEAM APP OPENED?");

			return;
		}

		s_EverInitialized = true;
	}

	// This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
	static void OnEnable() {


		if (!m_bInitialized) {
			return;
		}

		if (m_SteamAPIWarningMessageHook == null) {
			// Set up our callback to receive warning messages from Steam.
			// You must launch with "-debug_steamapi" in the launch args to receive warnings.
			m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
		}
	}

	// DEBO LLAMAR AQUI CUANDO LA APP CIERRE
	// OnApplicationQuit gets called too early to shutdown the SteamAPI.
	// Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
	// Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
	static void OnDestroy() {

		if (!m_bInitialized) {
			return;
		}

		SteamAPI.Shutdown();
	}

	public static void Update() { 
		if (!m_bInitialized) {
			return;
		}
		// Run Steam client callbacks
		SteamAPI.RunCallbacks();
	}
#else
	public static bool Initialized {
		get {
			return false;
		}
	}
#endif // !DISABLESTEAMWORKS
}
#endif
