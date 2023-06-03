using System;
using UnityEngine;
using Steamworks;
using System.Collections.Generic;
using UnityEditor;

internal struct SteamWorkshopItem
{
    public string ContentFolderPath;
    public string Description;
    public string PreviewImagePath;
    public string[] Tags;
    public string Title;
}
//[InitializeOnLoad]
public static class SteamWorkshop
{
    static SteamWorkshopItem currentSteamWorkshopItem;
    static PublishedFileId_t publishedFileID;

    public static List<string> GetListOfSubscribedItemsPaths()
    {
        var subscribedCount = SteamUGC.GetNumSubscribedItems();
        PublishedFileId_t[] subscribedFiles = new PublishedFileId_t[subscribedCount];
        SteamUGC.GetSubscribedItems(subscribedFiles, (uint)subscribedFiles.Length);

        ulong sizeOnDisk = 0;
        string installLocation = string.Empty;
        uint timeStamp = 0;

        var result = new List<string>();

        foreach (var file in subscribedFiles)
        {
            SteamUGC.GetItemInstallInfo(file, out sizeOnDisk, out installLocation, 1024, out timeStamp);
            result.Add(installLocation);
        }

        return result;
    }

    public static AppId_t editorAppId = new AppId_t(GameData.instance.steamLevelEditorAppId);
    public static AppId_t consumerAppId = new AppId_t(GameData.instance.steamFullGameAppId);

    public static bool levelSearchInProgress;
    public static void SearchLevelsWithTitle(string filter)
    {
        levelSearchInProgress = true;
        //AppId_t id = new AppId_t(GameData.instance.GetSteamAppId());
        var qHandle = SteamUGC.CreateQueryAllUGCRequest(EUGCQuery.k_EUGCQuery_RankedByTextSearch, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
            editorAppId, consumerAppId, 1);
        if (qHandle != UGCQueryHandle_t.Invalid)
        {
            SteamUGC.SetSearchText(qHandle, filter);
            //SteamUGC.SetMatchAnyTag(qHandle, true);
            //SteamUGC.AddRequiredTag(qHandle, "LEVEL");

            var m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create();

            var call = SteamUGC.SendQueryUGCRequest(qHandle);
            m_SteamUGCQueryCompleted.Set(call, HandleLevelSearchResults );
        }
    }

    public static uint matchedLevelsCount;
    public static uint pageCount;

    public static void HandleLevelSearchResults(SteamUGCQueryCompleted_t param, bool bIOFailure)
    {
        levelSearchInProgress = false;
        matchedLevelsCount = param.m_unTotalMatchingResults;
        
        if (!bIOFailure)
        {
            if (param.m_eResult == EResult.k_EResultOK)
            {
                matchedLevelsCount = param.m_unTotalMatchingResults;

                pageCount = (uint) Mathf.Clamp((int) matchedLevelsCount / 50, 1, int.MaxValue);
                if (pageCount * 50 < matchedLevelsCount)
                    pageCount++;

                for (int i = 0; i < param.m_unNumResultsReturned; i++)
                {
                    SteamUGCDetails_t details;
                    SteamUGC.GetQueryUGCResult(param.m_handle, (uint) i, out details);
                }
            }
            else
            {
                Debug.LogError("HandleQueryCompleted Unexpected results, state = " + param.m_eResult.ToString());
            }
        }
        else
        {
            Debug.LogError("HandleQueryCompleted failed.");
        }
    }

    public static void UploadContent(string itemTitle, string itemDescription, string contentFolderPath, string[] tags, string previewImagePath)
    {
        currentSteamWorkshopItem = new SteamWorkshopItem
        {
            Title = itemTitle,
            Description = itemDescription,
            ContentFolderPath = contentFolderPath,
            Tags = tags,
            PreviewImagePath = previewImagePath
        };

        CreateItem();
        Debug.Log("UPLOADING CONTENT (" + itemTitle + ") TO STEAM WORKSHOP");

    }

    static void CreateItem()
    {
        //var id = new AppId_t(GameData.instance.steamFullGameAppId);//SteamUtils.GetAppID(); //segun la tipa de steam , la app puede tener su propio ID y operar con el , pero al crear objeto le decimos para que APP es
        var steamAPICall = SteamUGC.CreateItem(consumerAppId, EWorkshopFileType.k_EWorkshopFileTypeCommunity); 
        var steamAPICallResult = CallResult<CreateItemResult_t>.Create();
        steamAPICallResult.Set(steamAPICall, CreateItemResult);
    }

    static void CreateItemResult(CreateItemResult_t param, bool bIOFailure)
    {
        if (param.m_eResult == EResult.k_EResultOK)
        {
            publishedFileID = param.m_nPublishedFileId;
            UpdateItem();
        }
        else
        {
            Debug.Log("COULD NOT CREATE ITEM AT STEAM (" + param.m_eResult + ") , TRY REBOOTING STEAM CLIENT");
        }
    }

    static void UpdateItem()
    {
        var updateHandle = SteamUGC.StartItemUpdate(consumerAppId, publishedFileID);

        if (updateHandle != UGCUpdateHandle_t.Invalid)
        {
            SteamUGC.SetItemTitle(updateHandle, currentSteamWorkshopItem.Title);
            SteamUGC.SetItemDescription(updateHandle, currentSteamWorkshopItem.Description);
            SteamUGC.SetItemContent(updateHandle, currentSteamWorkshopItem.ContentFolderPath);
            SteamUGC.SetItemTags(updateHandle, currentSteamWorkshopItem.Tags);
            SteamUGC.SetItemPreview(updateHandle, currentSteamWorkshopItem.PreviewImagePath);
            SteamUGC.SetItemVisibility(updateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);

            var steamAPICall = SteamUGC.SubmitItemUpdate(updateHandle, "");
            var steamAPICallResult = CallResult<SubmitItemUpdateResult_t>.Create();
            steamAPICallResult.Set(steamAPICall, UpdateItemResult);
        }
        else
        {
            Debug.Log("Handle Mala");
        }
    }

    static void UpdateItemResult(SubmitItemUpdateResult_t param, bool bIOFailure) 
    {
        if (param.m_eResult == EResult.k_EResultOK)
        {
            Debug.Log("SUCCESSFULLY SUBMITTED LEVEL TO STEAM");
        }
        else
        {
            Debug.Log("COULD NOT SUBMIT LEVEL TO STEAM");
        }
    }
}