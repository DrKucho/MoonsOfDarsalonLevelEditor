using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LevelDifficulty))]
[System.Serializable]
[CanEditMultipleObjects]
public class LevelDifficulty_Editor : Editor
{
    private LevelDifficulty sc;

    public override void OnInspectorGUI()
    {
        sc = (LevelDifficulty) target;
        if (GUILayout.Button("SAVE AND UPLOAD LEVEL TO STEAM"))
        {
            EditorCoroutines.StopAll();
            EditorCoroutines.Execute(SaveAndUploadLevelToSteam());
        }

        DrawDefaultInspector();
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            sc.InitialiseInEditor();
        }
    }

    public IEnumerator SaveAndUploadLevelToSteam()
    {
        var id = GameData.instance.GetSteamAppId();
        if (!CompareSteamAppIdFile(id)) // no existe o tenia otro numero?
        {
            CreateSteamAppIdTxtFile(id);
            Debug.LogError(" CONFIG TO LINK WITH STEAM WASN'T RIGHT, SO I FIXED IT, BUT NOW YOU HAVE TO REBOOT UNITY, NEXT TIME WILL WORK");
        }
        else
        {
            if (!StaticSteamManager.Initialized)
            {
                StaticSteamManager.Init();
            }

            if (StaticSteamManager.Initialized)
            {
                var sceneToBuildAndUpload = SceneManager.GetActiveScene();
                var sceneToBuildAndUploadNameToUpper = sceneToBuildAndUpload.name.ToUpper();
                if (sceneToBuildAndUpload.isDirty)
                {
                    //Debug.LogError("CAN'T UPLOAD THE CURRENT LEVEL, IT HAS UNSAVED CHANGES");
                    KuchoToolsMenu.InitialiseAllKuchoScripts(new MenuCommand(null, 1)); // 1 == allScene
                    EditorSceneManager.SaveScene(sceneToBuildAndUpload);
                }

                if (!sceneToBuildAndUploadNameToUpper.StartsWith("UGCLEVEL ") && !sceneToBuildAndUploadNameToUpper.StartsWith("UGC LEVEL"))
                {
                    Debug.LogError("THE ACTIVE LEVEL IS NOT NAMED PROPERLY? MAKE SURE IT STARTS WITH 'UGC LEVEL'");
                }
                else
                {
                    // necesito guardar tod0 esto de levelDiff porque al cargar scene de upload se pierde la referencia
                    var levelDiff = FindObjectOfType<LevelDifficulty>();
                    var previewImage = levelDiff.preview;
                    if (previewImage == null)
                    {
                        previewImage = Resources.Load("DefaultUGCLevelPreview") as Texture2D;
                    }

                    if (previewImage == null)
                    {
                        Debug.LogError("PREVIEW IMAGE CAN'T BE MISSING");
                    }
                    else
                    {
                        if (previewImage.width < 100 || previewImage.height < 100) // tamaño de las previews oficiales 242x120 , antes era 126
                        {
                            Debug.LogError("PREVIEW IMAGE IS TOO SMALL, MIN SIZE 100x100 pixels");
                        }
                        else
                        {
                            float aspectRatio = (float) previewImage.width / (float) previewImage.height;
                            if (aspectRatio <= 2 && aspectRatio >= 1)
                            {
                                Debug.LogError("PREVIEW IMAGE ASPECT RATIO IS BAD, GOOD ASPECT RATIOS GO FROM 1:1 to 2:1");
                            }
                            else
                            {

                                var tags = levelDiff.tags;
                                if (tags.Length > 6)
                                {
                                    Debug.LogError("TOO MANY TAGS, MAXIMUM = 6");
                                }
                                else
                                {
                                    if (!GotOneAndOnlyOneDifficultTag(tags))
                                    {
                                        Debug.LogError("WRONG NUMBER OF DIFFICULTY TAGS, ADD JUST ONE (EASY, NORMAL, DIFFICULT)");
                                    }
                                    else
                                    {
                                        if (!AtLeastOneLevelTypeTag(tags))
                                        {
                                            Debug.LogError("ADD AT LEAST ONE LEVEL TYPE TAG (ACTION, EXPLORATION, PUZZLE)");
                                        }
                                        else
                                        {
                                            var missions = levelDiff.mainGoal.GetDisplayName(false) + "\n";
                                            missions += levelDiff.specialGoal1.GetDisplayName(false) + "\n";
                                            missions += levelDiff.specialGoal2.GetDisplayName(false) + "\n";
                                            missions += levelDiff.specialGoal3.GetDisplayName(false) + "\n";

                                            var slash = Path.DirectorySeparatorChar;
                                            //var bundleName = "ugc_level_scenes_all.bundle";
                                            //var bundleFilePath = GetSourcePath(data.buildWinFull) + slash + bundleName; 
                                            var contentFolderName = "BundleToUpload";
                                            var currentDirectory = Directory.GetCurrentDirectory() + slash;
                                            var unityPathToContentFolder = "Assets" + slash + contentFolderName;
                                            var contentFolderPath = currentDirectory + unityPathToContentFolder;
                                            //var destinationPath = contentFolderPath + slash + bundleName;

                                            bool pathsGood = true;
                                            /*
                                            if (!File.Exists(bundleFilePath))
                                            {r
                                                Debug.LogError("bundle file not found" + bundleFilePath);
                                                pathsGood = false;
                                            }
                                            */
                                            if (!Directory.Exists(contentFolderPath))
                                            {
                                                Debug.LogError("content folder not found" + contentFolderPath);
                                                pathsGood = false;
                                            }

                                            if (pathsGood)
                                            {
                                                /*
                                                { //necesito copiarlo porque ha de estar en un folder el solito para crear el bundle
                                                    if (File.Exists(destinationPath))
                                                        File.Delete(destinationPath);
                                                    File.Copy(bundleFilePath, destinationPath);
                                                    Debug.Log("Bundle Copied to BundleToUpload folder...");
                                                }
                                                */
                                                //CreateSteamAppIdTxtFile(GameData.instance.GetSteamAppId());
                                                string title = null;

                                                // AssetBundle.UnloadAllAssetBundles(true);
                                                // var b = AssetBundle.LoadFromFile(bundleFilePath);
                                                // var scenePaths = b.GetAllScenePaths();
                                                // title = Path.GetFileNameWithoutExtension(scenePaths[0]);
                                                // title = GameData.GetCleanTitleFromSceneFileName(title);

                                                DeleteAllFiles(contentFolderPath, "*.*");

                                                string filePathToUpload = null;
                                                foreach (string filePath in Directory.GetFiles("Assets/-LEVELS", "*.unity"))
                                                {
                                                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                                                    string fileNameUpper = fileName.ToUpper();
                                                    if (fileNameUpper == sceneToBuildAndUploadNameToUpper) // el nivel abierto se ha encontrado en en folder
                                                    {
                                                        filePathToUpload = filePath;
                                                        title = GameData.GetCleanTitleFromSceneFileName(fileNameUpper);
                                                        var a = UnityEditor.AssetImporter.GetAtPath(filePath);
                                                        a.assetBundleName = "ugc level"; // un solo sistema, mi restaurador de materiales va a arreglar los shaders rotos
                                                    }
                                                    else
                                                    {
                                                        var a = UnityEditor.AssetImporter.GetAtPath(filePath);
                                                        a.assetBundleName = null;
                                                    }
                                                }

                                                if (filePathToUpload == null)
                                                    Debug.LogError("COULD NOT FIND A LEVEL TO UPLOAD? IT MUST BE INSIDE 'Assets/-LEVELS FOLDER', NAMED STARTING WITH 'UGC LEVEL'");
                                                else
                                                {
                                                    SteamWorkshop.SearchLevelsWithTitle('"' + title + '"');
                                                    float timeOut = Time.realtimeSinceStartup + 8;
                                                    bool timedOut = false;
                                                    float logTime = Time.realtimeSinceStartup;
                                                    while (SteamWorkshop.levelSearchInProgress)
                                                    {
                                                        if (Time.realtimeSinceStartup > logTime)
                                                        {
                                                            Debug.Log("SEARCHING FOR AN EXISTING LEVEL TITLE " + title + " IN STEAM WORKSHOP");
                                                            logTime += 1;
                                                        }

                                                        yield return null;
                                                        if (Time.realtimeSinceStartup > timeOut)
                                                        {
                                                            Debug.LogError("SEARCH TIME OUT");
                                                            timedOut = true;
                                                            EditorCoroutines.StopAll();
                                                            break;
                                                        }
                                                    }

                                                    if (!timedOut)
                                                    {
                                                        if (SteamWorkshop.matchedLevelsCount == 0)
                                                        {
                                                            BuildTarget buildTarget;
                                                            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
                                                                buildTarget = BuildTarget.StandaloneOSX;
                                                            else
                                                                buildTarget = BuildTarget.StandaloneWindows64;

                                                            BuildPipeline.BuildAssetBundles(unityPathToContentFolder, BuildAssetBundleOptions.None, buildTarget);
                                                            /*
                                                            var a = UnityEditor.AssetImporter.GetAtPath(filePath);
                                                            a.assetBundleName = "ugc level osx";
                                                            BuildPipeline.BuildAssetBundles(unityPathToContentFolder, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
                                                            a.assetBundleName = "ugc level win";
                                                            BuildPipeline.BuildAssetBundles(unityPathToContentFolder, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
                                                            */
                                                            DeleteAllFiles(contentFolderPath, contentFolderName);
                                                            DeleteAllFiles(contentFolderPath, "*.meta");
                                                            DeleteAllFiles(contentFolderPath, "*.manifest");
                                                            yield return new WaitForSecondsRealtime(0.5f); // necesario para que no salte un error de item no createble o no se que gaitas
                                                            Object o = AssetDatabase.LoadAssetAtPath<Object>(filePathToUpload);

                                                            List<string> tagList = new List<string>();
                                                            tagList.Add("LEVEL");
                                                            foreach (LevelDifficulty.TagType t in tags)
                                                            {
                                                                var tt = System.Enum.GetName(typeof(LevelDifficulty.TagType), t);
                                                                if (tt != null)
                                                                    tagList.Add(tt);
                                                            }

                                                            var tagArray = tagList.ToArray();

                                                            string description = "";
                                                            for (int i = 0; i < tagArray.Length; i++)
                                                            {
                                                                if (i == tagArray.Length - 1) // el ultimo?
                                                                    description += tagArray[i] + ".";
                                                                else
                                                                    description += tagArray[i] + ",";
                                                            }

                                                            description += "\n\n" + missions;

                                                            var previewPath = currentDirectory + AssetDatabase.GetAssetPath(previewImage);
                                                            if (!StaticSteamManager.Initialized)
                                                                StaticSteamManager.Init();
                                                            SteamWorkshop.UploadContent(title, description, contentFolderPath, tagArray, previewPath);
                                                        }
                                                        else
                                                        {
                                                            Debug.LogError("THERE IS ALREADY A LEVEL TITTLED '" + title + "' IN THE STEAM WORKSHOP, CHANGE TITLE AND TRY AGAIN");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("STEAM CONECTION NOT INITIALIZED?");
            }
        }
    }

    public static bool CompareSteamAppIdFile(uint appId)
    {
        var filename = "steam_appid.txt";
        string strCWD = Directory.GetCurrentDirectory();
        //string strSource = Path.Combine(Path.Combine(strCWD, path), filename);
        string strDest = Path.Combine(strCWD, filename);

        if (File.Exists(strDest))
        {
            var lines = File.ReadLines(strDest).ToArray();
            if (lines.Length > 0 && lines[0] == appId.ToString())
            {
                return true;
            }
        }

        return false;
    }
    public static void CreateSteamAppIdTxtFile(uint appId)
    {
        var filename = "steam_appid.txt";
        string strCWD = Directory.GetCurrentDirectory();
        //string strSource = Path.Combine(Path.Combine(strCWD, path), filename);
        string strDest = Path.Combine(strCWD, filename);

        using (StreamWriter sw = File.CreateText(strDest))
        {
            sw.WriteLine(appId.ToString());
        }

        if (File.Exists(strDest))
        {
            Debug.Log(string.Format("File SteamAppId (" + appId + ") sucessfully created"));
        }
        else
        {
            Debug.LogError(string.Format("File SteamAppId Could not be copied"));
        }
    }
    public static void DeleteAllFiles(string path, string searchString)
    {
        string[] filePaths = System.IO.Directory.GetFiles(path, searchString);
        foreach (string p in filePaths)
        {
            FileUtil.DeleteFileOrDirectory(p);
        }
    }

    public static bool AtLeastOneLevelTypeTag(LevelDifficulty.TagType[] tags)
    {
        foreach (LevelDifficulty.TagType t in tags)
        {
            if (t == LevelDifficulty.TagType.ACTION)
                return true;
            if (t == LevelDifficulty.TagType.EXPLORATION)
                return true;
            if (t == LevelDifficulty.TagType.PUZZLE)
                return true;
        }
        return false;
    }
    public static bool GotOneAndOnlyOneDifficultTag(LevelDifficulty.TagType[] tags)
    {
        int counter = 0;
        foreach (LevelDifficulty.TagType t in tags)
        {
            if (t == LevelDifficulty.TagType.EASY)
                counter++;
            if (t == LevelDifficulty.TagType.NORMAL)
                counter++;
            if (t == LevelDifficulty.TagType.DIFFICULT)
                counter++;
        }

        if (counter == 1)
            return true;
        return false;
    }
}