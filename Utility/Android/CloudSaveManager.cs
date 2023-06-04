using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace SuspiciousGames.SellMe.Utility
{

    public class CloudSaveManager : MonoBehaviour
    {
        #region Constants

        /// <summary>
        /// Constant SaveGame FileName.
        /// </summary>
        private const string _saveGameName = "sellMe.save";

        #endregion

        #region Singleton
        /// <summary>
        /// Singleton instance of Cloud Manager.
        /// </summary>
        private static CloudSaveManager _instance;

        /// <summary>
        /// Singleton of CloudSaveManager.
        /// </summary>
        public static CloudSaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CloudSaveManager>();

                    if (_instance == null)
                    {
                        _instance = new GameObject("CloudSaveManager").AddComponent<CloudSaveManager>();
                    }
                    _instance.isProcessing = false;
                }

                return _instance;
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Lock for Data Processing
        /// </summary>
        public bool isProcessing { get; private set; }

        /// <summary>
        /// Currently loaded Data.
        /// </summary>
        public string loadedData { get; private set; }
        #endregion

        #region Public
        /// <summary>
        /// Load Data From SaveGame.
        /// </summary>
        /// <param name="afterLoadAction">Action to process SaveGame Data after Loading sucessfully.</param>
        public void LoadFromCloud(Action<string> afterLoadAction)
        {
            if (!isProcessing)
                StartCoroutine(LoadFromCloudRoutine(afterLoadAction));
        }

        /// <summary>
        /// Save Data to SaveGame.
        /// </summary>
        /// <param name="dataToSave">Data String that is saved to the saveGame</param>
        public void SaveToCloud(string dataToSave)
        {
            loadedData = dataToSave;
            isProcessing = true;
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(_saveGameName, DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnFileOpenToSave);
        }
        #endregion

        #region Private
        /// <summary>
        /// Converts SaveGame Data To String and saves it to loadedData.
        /// </summary>
        /// <param name="cloudData">SaveGameData</param>
        private void ProcessCloudData(byte[] cloudData)
        {
            if (cloudData == null)
            {
                Debug.Log("No Data saved to the cloud!");
                return;
            }

            string progress = BytesToString(cloudData);
            loadedData = progress;
        }

        /// <summary>
        /// Reads Data From SaveGame and Executes loadAction on loadedData after sucessfully loading.
        /// </summary>
        /// <param name="loadAction">Action to Execute after loading.</param>
        /// <returns></returns>
        private IEnumerator LoadFromCloudRoutine(Action<string> loadAction)
        {
            Debug.Log("Loading game progress from the cloud.");

            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                _saveGameName, //name of file.
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                OnFileOpenToLoad);

            isProcessing = true;

            while (isProcessing)
            {
                yield return null;
            }

            loadAction.Invoke(loadedData);
        }

        /// <summary>
        /// Callback from Save Function using Status and metaData from SaveGamefile to update file with loaded Data.
        /// </summary>
        /// <param name="status">Status for SaveGame</param>
        /// <param name="metaData">SaveGame MetaData</param>
        private void OnFileOpenToSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                byte[] data = StringToBytes(loadedData);

                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();

                SavedGameMetadataUpdate updatedMetadata = builder.Build();

                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metaData, updatedMetadata, data, OnGameSave);
            }
            else
            {
                Debug.LogWarning("Error opening Saved Game" + status);
            }
        }

        /// <summary>
        /// Callback from Load Function using Status and metaData from SaveGamefile to read file into loaded Data.
        /// </summary>
        /// <param name="status">Status for SaveGame</param>
        /// <param name="metaData">SaveGame Metadata</param>
        private void OnFileOpenToLoad(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status == SavedGameRequestStatus.Success)
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metaData, OnGameLoad);
            }
            else
            {
                Debug.LogWarning("Error opening Saved Game" + status);
            }
        }

        /// <summary>
        /// Reads Data if Status is Success.
        /// </summary>
        /// <param name="status">File Status.</param>
        /// <param name="bytes">Incoming Bytes.</param>
        private void OnGameLoad(SavedGameRequestStatus status, byte[] bytes)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning("Error Saving" + status);
            }
            else
            {
                ProcessCloudData(bytes);
            }

            isProcessing = false;
        }

        /// <summary>
        /// Writes Data if Status is Success.
        /// </summary>
        /// <param name="status">File Status.</param>
        /// <param name="metaData">Metadata of File.</param>
        private void OnGameSave(SavedGameRequestStatus status, ISavedGameMetadata metaData)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning("Error Saving" + status);
            }

            isProcessing = false;
        }

        /// <summary>
        /// Converts String into Bytes.
        /// </summary>
        /// <param name="stringToConvert"></param>
        /// <returns></returns>
        private byte[] StringToBytes(string stringToConvert)
        {
            return Encoding.UTF8.GetBytes(stringToConvert);
        }

        /// <summary>
        /// Converts Bytes to String.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

    }
}