using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using UnityEngine;

public static class PlayServicesManager
{
    #region Properties
    /// <summary>
    /// Returns if an an active PlayGamesPlatform exists
    /// </summary>
    public static bool IsInitialized => Social.Active != null;
    #endregion

    #region Public methods
    /// <summary>
    /// Initates the Play Games Builder and Play Games Platform.
    /// </summary>
    public static void InitiatePlayGames()
    {
        // Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
    }

    /// <summary>
    /// Logs in the user according to <paramref name="silentLogin"/>
    /// </summary>
    /// <param name="loginCallback"></param>
    /// <param name="silentLogin"></param>
    public static void Login(Action<bool> loginCallback, bool silentLogin)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            return;

        PlayGamesPlatform.Instance.Authenticate(loginCallback, silentLogin);
    }

    /// <summary>
    /// Logs in the user according to <paramref name="silentLogin"/>
    /// </summary>
    /// <param name="loginCallback"></param>
    /// <param name="silentLogin"></param>
    public static void Login(Action<bool, string> loginCallback, bool silentLogin)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
            return;

        PlayGamesPlatform.Instance.Authenticate(loginCallback, silentLogin);
    }
    #endregion
}
