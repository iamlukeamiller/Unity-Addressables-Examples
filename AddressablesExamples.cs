using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AddressablesExamples : MonoBehaviour
{

    // Expose an editable list of AssetLabelReferences in the editor for ease of updating
    public List<AssetLabelReference> labelsToLoad;

    // Hold a list of all returned IResourceLocations for downloading, instantiated privately for instant use
    private IList<IResourceLocation> _allLocations { get; } = new List<IResourceLocation>();

    /*************************************************************************************************************
     *
     * Example 1: Retrieving all Addressable locations by label
     *
     * This takes all of the AssetLabelReferences from the editor, converts to strings and passes to the
     * coroutine. The coroutine asynchronously parses every Addressable Asset that belongs to any of the indicated
     * labels and adds each location to the _allLocations list for processing separately.
     * 
     ************************************************************************************************************/

    // Method to grab all IResourceLocation names for all Addressables in the user's available Labels
    public void GetAddressableLocations() {

        // Create a new array of strings to hold the labelStrings of each of the user's label
        string[] labelStrings = new string[labelsToLoad.Count];

        // Loop through all of the added AssetLabelReferences and add their labelString to the array
        for(int i = 0; i < labelsToLoad.Count; i++) {
            labelStrings[i] = labelsToLoad[i].labelString;
        }

        // Pass the parsed labelStrings to the Coroutine to retrieve all of the locations
        StartCoroutine(GetAddressableLocationsByLabel(labelStrings));

    }

    IEnumerator GetAddressableLocationsByLabel(string[] labelStrings) {

        // Prepare the actual operation for execution, passing in the parsed labelStrings
        AsyncOperationHandle<IList<IResourceLocation>> handle = Addressables.LoadResourceLocationsAsync(labelStrings, Addressables.MergeMode.Union);
        
        // Fire the asynchronous operation
        yield return handle;

        // Loop through all of the retrieved IResourceLocations and handle them
        for(int i = 0; i < handle.Result.Count; i++) {

            // Convert the current IResourceLocation to a string
            string labelName = handle.Result[i].ToString();

            // Get the suffix of the current IResourceLocation (looking specifically for 'prefab')
            string labelSuffix = labelName.Substring(labelName.Length - 6);

            // Keep a list of all retrieved locations for downloading
            _allLocations.Add(handle.Result[i]);

            // NOTE: You can also look at the values of handle.Result to see the paths / locations of each Asset
            // being loaded in this process. I'm using it to detect when an asset ends in '.prefab' to also add
            // it to a separate list of locations, as I have some additional data processing steps for those.

        }

    }

    /*************************************************************************************************************
     *
     * Example 2: Actually load all Addressable locations by label
     *
     * This method simply passes a list of IResourceLocations to the coroutine, which takes that list of 
     * location and loads them. This should fire any downloads from remote locations that are required.
     *
     ************************************************************************************************************/

    public void LoadAllAddressables() {

        // Fire the LoadAddressableByLocation coroutine, passing in the list of ALL retrieved locations
        StartCoroutine(LoadAddressablesByLocation(_allLocations));

    }

    IEnumerator LoadAddressablesByLocation(IList<IResourceLocation> locations) {

        // Prepare the actual operation
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(locations, obj =>
            {
                //Gets called for every loaded asset
                Debug.Log("Successfully loaded " + obj.name);
            });

        // Fire the async operation
        yield return handle;

        // Add the successfully loaded GameObjects to a list for handling
        IList<GameObject> loadedObjects = handle.Result;

    }

    /*************************************************************************************************************
     *
     * Example 3: Check the download size of all Addressable Assets from a list of locations
     *
     * This method simply passes a list of IResourceLocations to the coroutine, which checks how much data in
     * bytes will be downloaded in order to cache all of the assets. 
     * 
     * Note: A returned value of 0 indicates all assets have already been cached and no download is necessary.
     *
     ************************************************************************************************************/

     public void CheckAssetDownloadSize() {

        StartCoroutine(CheckDownloadSize(_allLocations));

    }

    IEnumerator CheckDownloadSize(IList<IResourceLocation> locations) {

        // Prepare the actual operation
        AsyncOperationHandle<long> handle = Addressables.GetDownloadSizeAsync(locations);

        // Fire the async operation
        yield return handle;

        Debug.Log("Size of assets to be downloaded (if zero, all are cached): " + handle.Result);

    }

    /*************************************************************************************************************
     *
     * Example 4: Pre-download all Addressable Assets from a list of locations
     *
     * This method passes a list of IResourceLocations to the coroutine which downloads any that are not yet
     * cached locally on the user's system. It includes a while loop that will output current download status to
     * the console. 
     * 
     * Note: The DownloadDependenciesAsync() method has a second boolean parameter that releases the
     * async handle of each downloaded asset automatically. I'm unsure of the implications of this, but unless
     * I had it set to 'true' I was receiving errors in the console when attempting to clear the cache (see #5).
     *
     ************************************************************************************************************/

     public void DownloadAllAssets() {

        // Show the Downloading panel
        StartCoroutine(DoDownload(_allLocations));
    }

     // Coroutine to actually download all assets in the list of labels
    IEnumerator DoDownload(IList<IResourceLocation> locations)
    {
        var dl = Addressables.DownloadDependenciesAsync(locations, true);
        dl.Completed += (AsyncOperationHandle) =>
        {

            // Download complete, so notify the console
            Debug.Log("Download completed");

            // Add handling or firing of other methods / coroutines on a successful download
 
        };
 
        while (!dl.IsDone)
        {
            Debug.Log("Downloading Asset: " + dl.PercentComplete.ToString());
            yield return null;
        }
 
    }

    /*************************************************************************************************************
     *
     * Example 5: Clear all downloaded assets from the local cache
     *
     * This method simply fires the ClearDependencyCacheAsync() method, taking in a list of IResourceLocations
     * that represents the assets to clear. This is mostly for my own testing purposes, though it would allow a
     * user to clear their cache if they are having issues with downloaded assets or need to free up space on
     * their system.
     *
     * Note: This likely should be handled in a coroutine for better asynchronous handling and handling of any
     * errors or messages that are returned, but at this point all I needed was to be able to clear my cache.
     *
     ************************************************************************************************************/

    public void ClearCache() {

        Addressables.ClearDependencyCacheAsync(_allLocations);
        
    }
    

}
