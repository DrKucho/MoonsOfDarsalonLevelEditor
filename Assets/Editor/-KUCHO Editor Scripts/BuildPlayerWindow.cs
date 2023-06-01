
using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
//using Packages.Rider.Editor.Util;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;
using Application = UnityEngine.Application;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
[InitializeOnLoad]
public class BuildPlayerWindow : EditorWindow
{

    [MenuItem("Tools/Build Levels Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildPlayerWindow>(false, "Build Levels", true);
    }

    public BuildTarget buildTarget;

    Game gameScript;
    KuchoBuildData data;
    private GUIStyle onButton;
    private GUIStyle offButton;
    private GUIStyle actionButton;
    private GUIStyle secondaryActionButton;

    private bool userIsDeveloper;
    public void OnGUI()
    {
        float color;
        float skinMult;
        float skinAdd;
        if (EditorGUIUtility.isProSkin)
        {
            color = 1;
            skinMult = 1;
            skinAdd = 0;
        }
        else
        {
            color = 0;
            skinMult = 1f;
            skinAdd = -0.55f;
        }

        if (onButton == null || onButton.name == "" || true)
        {
            onButton = new GUIStyle(EditorStyles.miniButton);
            onButton.fontStyle = FontStyle.Bold;
        }

        onButton.normal.textColor = new Color(color, color, color, 1);

        if (offButton == null || offButton.name == "" || true)
        {
            offButton = new GUIStyle(EditorStyles.miniButton);
            offButton.fontStyle = FontStyle.Normal;
            offButton.normal.textColor = new Color(0.5f, 0.5f, 0.5f, 1) * skinMult + new Color(skinAdd, skinAdd, skinAdd, 0);
        }

        if (actionButton == null || actionButton.name == "" || true)
        {
            actionButton = new GUIStyle(EditorStyles.miniButton);
            actionButton.fontStyle = FontStyle.Bold;
            actionButton.normal.textColor = Color.yellow * skinMult + new Color(skinAdd, skinAdd, skinAdd, 0);
        }

        if (secondaryActionButton == null || secondaryActionButton.name == "" || true)
        {
            secondaryActionButton = new GUIStyle(EditorStyles.miniButton);
            secondaryActionButton.fontStyle = FontStyle.Bold;
            secondaryActionButton.normal.textColor = Color.cyan * skinMult + new Color(skinAdd, skinAdd, skinAdd, 0);
        }

        GUILayoutOption[] emptyGUILayoutOption = new GUILayoutOption[0];




        data = GameData.GetKuchoBuildData();

        if (!data)
            return;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("INJECT USER LEVELS 1 TO 5 INTO YOUR LOCAL STEAM COPY OF M.O.D.", actionButton))
            EditorCoroutines.Execute(BuildAddressablesCoroutine());
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("ONLY COPY PREVIOUSLY CREATED LEVELS INTO GAME", actionButton))
            CopyLevelFilesEveryWhere();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        EditorGUILayout.Space();

        switch (SystemInfo.operatingSystemFamily)
        {
            case (OperatingSystemFamily.Windows):
                data.buildWinFull = true;
                data.buildOsxFull = false;
                break;
            case (OperatingSystemFamily.MacOSX):
                data.buildWinFull = false;
                data.buildOsxFull = true;
                break;
            default:
                data.buildWinFull = false;
                data.buildOsxFull = false;
                break;
        }

        //EditorGUILayout.ObjectField("Data Found", data, typeof(KuchoBuildData), emptyGUILayoutOption);

        //data.gamePrefab = (GameObject) EditorGUILayout.ObjectField("GamePrefab", data.gamePrefab, typeof(GameObject), emptyGUILayoutOption);

        EditorGUILayout.LabelField("-PATHS", EditorStyles.boldLabel);
        
        //data.projectPath = EditorGUILayout.TextField("ProjectPath", data.projectPath, PathExists(data.projectPath, data.logDirs));
        data.projectPath = Directory.GetCurrentDirectory();
        
        EditorGUILayout.LabelField("SO FAMILY" + SystemInfo.operatingSystemFamily);
        //EditorGUILayout.LabelField("SO OP SYS" + SystemInfo.operatingSystem);
        EditorGUILayout.LabelField("UNIT PLAT" + Application.platform);
        data.gamePath = KuchoHelper.FixDirectorySeparators(StringHelper.RemoveStartAndEndSpacesAndFixOsxTerminalPath(data.gamePath));
        if (ShouldSetDefaultOSXSteamGamePath())
            data.gamePath = KuchoHelper.FixDirectorySeparators("/Users/kucho/Library/Application Support/Steam/steamapps/common/Moons Of Darsalon/Moons Of Darsalon OSX Demo.app");
        else if (ShouldSetDefaultWINSteamGamePath())
            data.gamePath = KuchoHelper.FixDirectorySeparators("C:/Program Files (x86)/Steam/steamapps/common/Moons Of Darsalon/Moons Of Darsalon WIN Demo.exe");
        else
            data.gamePath = EditorGUILayout.TextField("Local Steam Game Path", data.gamePath, PathExists(data.gamePath, data.logDirs));

        if (data.gamePath.ToUpper().Contains("DEMO")) // tiene demo?
            GameData.instance.targetGamePathIsDemo = true;
        else
            GameData.instance.targetGamePathIsDemo = false;

        data.motherProjectPath = KuchoHelper.FixDirectorySeparators(StringHelper.RemoveStartAndEndSpacesAndFixOsxTerminalPath(data.motherProjectPath));

        userIsDeveloper = GameData.instance.userIsDeveloper | Directory.GetCurrentDirectory().StartsWith("/Users/kucho");
        if (userIsDeveloper)
            data.motherProjectPath = EditorGUILayout.TextField("MotherProjectPath", data.motherProjectPath, PathExists(data.motherProjectPath, data.logDirs));

        //data.logDirs = EditorGUILayout.Toggle("Log Dirs", data.logDirs);

        EditorGUILayout.LabelField("-SOUNDS", EditorStyles.boldLabel);

        data.buildStartSound = (AudioClip) EditorGUILayout.ObjectField("Build Start", data.buildStartSound, typeof(AudioClip), false);
        data.finishedSound = (AudioClip) EditorGUILayout.ObjectField("Build Successful", data.finishedSound, typeof(AudioClip), false);
        //EditorGUILayout.LabelField("-INFO", EditorStyles.boldLabel);
        //data.targetStr = EditorGUILayout.TextField("TargetStr", data.targetStr);

        EditorUtility.SetDirty(data); // necesario para que se serialize al cerrar unity
        
        if (userIsDeveloper)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("PROCESS LEVEL-MAKER FOLDER INTO ADDRESSABLE GROUP & MAT DATABASE", secondaryActionButton))
            {
                AddressableExtensions.CleanGroup("DuoShared", true);
                //EditorCoroutines.Execute(AddressableExtensions.AddFolderIntoAddressableGroup("Assets/-LEVELS", "UserLevels"));
                EditorCoroutines.Execute(AddressableExtensions.AddFolderIntoAddressableGroup(GameData.MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER, "DuoShared"));
                //EditorCoroutines.Execute(AddressableExtensions.MakeAddressableAllAssetsInSelectedFolder(GameData.MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER + "/LevelObjects", "UserLevelAssets"));
                //EditorCoroutines.Execute(AddressableExtensions.MakeAddressableAllAssetsInSelectedFolder(GameData.MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER + "/Scripts", "UserLevelAssets"));
                //EditorCoroutines.Execute(AddressableExtensions.MakeAddressableAllAssetsInSelectedFolder(GameData.MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER + "/Terrain", "UserLevelAssets"));
                //EditorCoroutines.Execute(AddressableExtensions.MakeAddressableAllAssetsInSelectedFolder(GameData.MOTHER_AND_CHILD_PROJECTS_SHARED_FOLDER + "/TerrManager", "UserLevelAssets"));
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("COMMIT CHANGES", secondaryActionButton))
                    EditorCoroutines.Execute(Git_CommitChanges());
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("DOWNLOAD LEVEL MAKER UPDATE FROM GIT", secondaryActionButton))
                EditorCoroutines.Execute(Git_Pull());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("DOWNLOAD LEVEL MAKER UPDATE HARD MODE", secondaryActionButton))
                EditorCoroutines.Execute(Git_OverwriteAllLocalChangesFromRepo());
            EditorGUILayout.EndHorizontal();
        }

    }

    bool ShouldSetDefaultOSXSteamGamePath()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
        {
            if (userIsDeveloper)
                return false;
            if (data.gamePath.StartsWith("C:"))
                return true;
            if (data.gamePath.EndsWith(".exe"))
                return false;
            if (data.gamePath.Contains("Program Files"))
                return true;
            if (data.gamePath.Contains("x86"))
                return true;
        }
        return false;
    }

    bool ShouldSetDefaultWINSteamGamePath()
    {
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
        {
            if (userIsDeveloper)
                return false;
            if (data.gamePath.Contains("Library"))
                return true;
            if (data.gamePath.EndsWith(".app"))
                return false;
            if (data.gamePath.Contains("Application Support"))
                return true;
        }
        return false;
    }

    GUIStyle GetStyle(bool value)
    {
        if (value)
            return onButton;
        else
            return offButton;
    }

    public IEnumerator BuildAddressablesCoroutine()
    {
        if (SceneManager.GetActiveScene().isDirty)
        {
            Debug.LogError("CAN'T INJECT BECAUSE THE LEVEL OR CURRENT SCENE IS NOT SAVED");
        }
        else
        {
            KuchoBuildData.isBuildingNow = true;

            var sceneToBuildAndUpload = SceneManager.GetActiveScene();
            if (sceneToBuildAndUpload.isDirty)
            {
                KuchoToolsMenu.InitialiseAllKuchoScripts(new MenuCommand(null, 1)); // 1 == allScene
                EditorSceneManager.SaveScene(sceneToBuildAndUpload);
            }

            bool allGood = true;
            var userLevelAssetsGroup = AddressableExtensions.settings.FindGroup("UserLevels");

            for (int i = 1; i < 6; i++)
            {
                string fullFilePath = KuchoHelper.FixDirectorySeparators(levelsFullPath_child + "/Level USER " + i + ".unity");
                string filePath = KuchoHelper.FixDirectorySeparators("Assets/-LEVELS/Level USER " + i + ".unity");
                if (File.Exists(fullFilePath))
                {
                    Object o = AssetDatabase.LoadAssetAtPath<Object>(filePath);
                    AddressableExtensions.SetAddressableGroup(o, userLevelAssetsGroup, true, true);
                }
                else
                {
                    Debug.LogError("LEVEL " + i + " NOT FOUND, DID YOU DELETE THE FILE?, COPY ONE OF THE EXISTING LEVEL FILES AND RENAME IT");
                    allGood = false;
                }
            }

            var ugcAddressableAssetGroup = AddressableExtensions.settings.FindGroup("UGC_Level");
            AddressableExtensions.CleanGroup(ugcAddressableAssetGroup, true);
            foreach (string filePath in Directory.GetFiles("Assets/-LEVELS", "*.unity"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string fileNameUpper = fileName.ToUpper();
                if (fileNameUpper.StartsWith("UGCLEVEL ") || fileNameUpper.StartsWith("UGC LEVEL")) // es nivel que hay que subir 
                {
                    Object o = AssetDatabase.LoadAssetAtPath<Object>(filePath);
                    if (o)
                        AddressableExtensions.SetAddressableGroup(o, ugcAddressableAssetGroup, true, true);
                    break;
                }
            }

            if (allGood)
            {
                if (SceneManager.GetActiveScene().isDirty)
                {
                    Debug.LogError("CURRENT SCENE HAS NOT SAVED CHANGES I CAN'T START BUILD");
                    EditorExtras.PlayClip((AudioClip) data.errorsFoundSound, 0, false);
                }
                else
                {
                    allGood = true;
                    EditorExtras.ClearLog();
                    EditorExtras.PlayClip((AudioClip) data.buildStartSound, 0, false);

                    //scenesAndDifficultyManager = data.gamePrefab.GetComponent<ScenesAndDifficultyManager>();
                    // para evitar que unity reimporte cosas innecesariamente
                    //if (data.lastTarget == GetBuildTargetShortString(BuildTarget.StandaloneWindows64))
                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
                    {
                        if (data.buildWinFull)
                            yield return DoBuildAddressablesCoroutine(BuildTarget.StandaloneWindows64, false, false, " ------- ADDRESSABLES BUILD FOR WIN FINISHED -------");
                        if (data.buildOsxFull)
                            yield return DoBuildAddressablesCoroutine(BuildTarget.StandaloneOSX, false, true, " ------- ADDRESSABLES BUILD FOR OSX FINISHED -------");
                    }
                    else
                    {
                        if (data.buildOsxFull)
                            yield return DoBuildAddressablesCoroutine(BuildTarget.StandaloneOSX, false, true, " ------- ADDRESSABLES BUILD FOR OSX FINISHED -------");
                        if (data.buildWinFull)
                            yield return DoBuildAddressablesCoroutine(BuildTarget.StandaloneWindows64, false, false, " ------- ADDRESSABLES BUILD FOR WIN FINISHED -------");
                    }

                    CopyLevelFilesEveryWhere();

                    PlayEndOfBuildSound();
                }
            }

            KuchoBuildData.isBuildingNow = false;
        }
    }

    public static string levelsPath = "/Assets/-LEVELS";
    public static string levelsFullPath_child;
    public static string levelTemplatesPath = "/Assets/-KUCHO/LEVEL-MAKER/Level Templates";
    public static string levels_3DModelsPath = "/Assets/-KUCHO/Background3DModelPrefabs(UserLevels)";
    public static string levelTemplates_3DModelsPath = "/Assets/-KUCHO/LEVEL-MAKER/Level Templates Background3DModelPrefabs";

    void CopyLevelFilesEveryWhere()
    {
        string sourcePath = null;
        string destinationPath = null;

        if (data.buildWinFull)
        {
            sourcePath = GetSourcePath(true);
            destinationPath = GetGameBuildDestinationPath(true);
        }

        if (data.buildOsxFull)
        {
            sourcePath = GetSourcePath(false);
            destinationPath = GetGameBuildDestinationPath(false);
        }

        if (sourcePath == null || destinationPath == null)
            return;

        Debug.Log("COPING 'userlevels_assets_all.bundle' FROM " + sourcePath + " TO " + destinationPath) ;
        CopyBundleFileIntoGame(sourcePath, destinationPath, "userlevels_assets_all.bundle");
        CopyBundleFileIntoGame(sourcePath, destinationPath, "userlevels_scenes_all.bundle");

        if (false) // packedSeparatelly ?
        {
            sourcePath += "/userlevels_scenes_assets/-levels";
            destinationPath += "/userlevels_scenes_assets/-levels";
            CopyLevelFiles(sourcePath, destinationPath, " INTO GAME BUILD");
        }


        /*if (data.buildWinFull)
            CopyLevelFiles(GetSourcePath(true), GetGameBuildDestinationPath(true), " INTO GAME BUILD");

        if (data.buildOsxFull)
            CopyLevelFiles(GetSourcePath(false), GetGameBuildDestinationPath(false), " INTO GAME BUILD");    
        */

        if (Directory.Exists(data.motherProjectPath))
        {
            levelsFullPath_child = KuchoHelper.FixDirectorySeparators(data.projectPath + levelsPath);
            string motherLevelsPath = KuchoHelper.FixDirectorySeparators(data.motherProjectPath + levelsPath);
            string child3DModelsPath = KuchoHelper.FixDirectorySeparators(data.projectPath + levels_3DModelsPath);
            string mother3DModelsPath = KuchoHelper.FixDirectorySeparators(data.motherProjectPath + levels_3DModelsPath);

            if (Directory.Exists(motherLevelsPath))
            {
                for (int i = 1; i < 6; i++)
                {
                    string srcPath = KuchoHelper.FixDirectorySeparators(levelsFullPath_child + "/Level USER " + i + ".unity");
                    string destPath = KuchoHelper.FixDirectorySeparators(motherLevelsPath + "/Level USER " + i + ".unity");
                    if (File.Exists(srcPath))
                    {
                        if (File.Exists(destPath))
                        {
                            File.Delete(destPath);
                        }

                        File.Copy(srcPath, destPath);
                        Debug.Log("COPIED LEVEL USER " + i + " TO MOTHER PROJECT");
                    }

                    srcPath = KuchoHelper.FixDirectorySeparators(child3DModelsPath + "/USER " + i + " - BACKGROUND 3D MODELS.prefab");
                    destPath = KuchoHelper.FixDirectorySeparators(mother3DModelsPath + "/USER " + i + " - BACKGROUND 3D MODELS.prefab");
                    if (File.Exists(srcPath))
                    {
                        if (File.Exists(destPath))
                        {
                            File.Delete(destPath);
                        }

                        File.Copy(srcPath, destPath);
                        Debug.Log("COPIED 3DMODELS FOR LEVEL USER " + i + " TO MOTHER PROJECT");
                    }
                }
            }
            else
                Debug.LogError("I COULD NOT COPY FILES TO MOTHER PROJECT: FOLDER NOT FOUND");
        }
    }

    void CopyBundleFileIntoGame(string sourcePath, string destinationPath, string file_Name)
    {
        string filePath = sourcePath + "/" + file_Name;
        string destFilePath = destinationPath + Path.DirectorySeparatorChar + file_Name;
        if (File.Exists(destFilePath))
            File.Delete(destFilePath);
        if (File.Exists(filePath))
        {
            try
            {
                File.Copy(filePath, destFilePath);
                Debug.Log("COPIED FILE " + file_Name + " INTO GAME BUILD");
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Debug.LogError("IS LOCAL STEAM GAME PATH IN GREEN COLOR?");
                throw;
            }
        }
        else
            Debug.Log("FILE " + file_Name + " NOT FOUND ...???");
    }

    void CopyLevelFiles(string source, string destination, string copiedTo) 
    {
        bool allGood = true;
        if (!Directory.Exists(source))
        {
            Debug.LogError("SOURCE DIR NOT FOUND:" + source);
            allGood = false;
        }

        if (!Directory.Exists(destination))
        {
            Debug.LogError("DESTINATION DIR NOT FOUND:" + destination);
            allGood = false;
        }

        if (allGood)
        {
            var files = Directory.GetFiles(source, "*");
            foreach (string filePath in files)
            {
                string file_Name = Path.GetFileName(filePath);
                if (file_Name.StartsWith("leveluser") && file_Name.EndsWith(".unity.bundle"))
                {
                    string destFilePath = destination + Path.DirectorySeparatorChar + file_Name;
                    if (File.Exists(destFilePath))
                        File.Delete(destFilePath);
                    if (File.Exists(filePath))
                        File.Copy(filePath, destFilePath);
                    else
                        Debug.LogError("FILE " + filePath + " NOT FOUND ...???");
                    Debug.Log("COPIED FILE " + file_Name + copiedTo);
                }
            }
        }
    }

    string GetSourcePath(bool win)
    {
        if (win)
            return KuchoHelper.FixDirectorySeparators(Directory.GetCurrentDirectory() + "/Library/com.unity.addressables/aa/Windows/StandaloneWindows64");
        else
        {
            return KuchoHelper.FixDirectorySeparators(Directory.GetCurrentDirectory() + "/Library/com.unity.addressables/aa/OSX/StandaloneOSX");
        }
    }

    string GetGameBuildDestinationPath(bool win)
    {
        if (win)
        {
            string result = data.gamePath.Substring(0, data.gamePath.Length - 4);// le quito el ".exe"
            result += @"_Data\StreamingAssets\aa\StandaloneWindows64";
            return result;
        }
        else
        {
            string result = data.gamePath; 
            if (!result.EndsWith(".app"))
                result += ".app";
            result += "/Contents/Resources/Data/StreamingAssets/aa/StandaloneOSX";
            return result;
        }
    }

    public IEnumerator DoBuildAddressablesCoroutine(BuildTarget _target, bool releaseMode, bool demo, string finishedText)
    {

        bool success = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, _target);
        AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = 3; // al hacer debug en el proceso de unity este dato era un 3 , pero no se que es
        AddressableAssetSettings.BuildPlayerContent();

        // no tengo forma de saber si ha habido errores

        Debug.Log(finishedText);
        yield return null;
    }

    public static bool allGood;


    void PlayEndOfBuildSound()
    {

        EditorExtras.PlayClip((AudioClip) data.finishedSound, 0, false);

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets(); // para que garde el scriptable object data
    }


    GUIStyle PathExists(string path, bool print)
    {
        if (path == null)
            return "";
        GUIStyle s = new GUIStyle(EditorStyles.textField);
        if (!string.IsNullOrEmpty(path))
        {
            s.fontStyle = FontStyle.Bold;

            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                path = path.Substring(0, path.Length - 1);

            bool exist = false;
            if (path.EndsWith(".exe"))
                exist = File.Exists(path);
            else
                exist = Directory.Exists(path);

            if (exist)
            {
                if (print)
                {
                    var a = Directory.GetDirectories(path); // para poder debuguear y pillar los nombres raros con espacios y simbolos,, 
                    Debug.Log("DIRECTORIOS EN " + path);
                    string allDirs = "";
                    foreach (string st in a)
                        allDirs += st + "\n";
                    Debug.Log("DIRECTORIOS EN " + path + "\n" + allDirs);
                }

                s.normal.textColor = new Color(0, 0.5f, 0, 1);
                return s;
            }
            else
            {
                s.normal.textColor = new Color(0.5f, 0, 0, 1);
                return s;
            }
        }

        s.normal.textColor = new Color(0, 0, 0, 1);
        return s;
    }

    public static class AddressableExtensions
    {
        public static AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        public static IEnumerator AddFolderIntoAddressableGroup(string path, string groupName)
        {
            var mainGroup = settings.FindGroup(groupName);
            var userLevelAssetsGroup = settings.FindGroup("UserLevelAssets");
            if (!mainGroup)
                mainGroup = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    string[] searches = {"*.asset", "*.mat", "*.png", "*.psd", "*.tga", "*.shader", "*.unity"};
                    int counter = 0;
                    foreach (string searchString in searches)
                    {
                        foreach (string file in Directory.EnumerateFiles(path, searchString, SearchOption.AllDirectories))
                        {
                            if (false && file.EndsWith("UserLevelAssetsForceLoadMaterial.mat"))
                            {
                                Object o = AssetDatabase.LoadAssetAtPath<Object>(file);
                                SetAddressableGroup(o, userLevelAssetsGroup, true, true);
                            }
                            else if (file.EndsWith("LevelObjects Data.asset") || file.EndsWith("FarBackground Data.asset")) // no incluir en duoshared
                            {
                            }
                            else if (!file.EndsWith("Sticker.png") && !file.EndsWith("Sticker.psd"))
                            {
                                Object o = AssetDatabase.LoadAssetAtPath<Object>(file);
                                SetAddressableGroup(o, mainGroup, true, true);
                                counter++;
                                if (counter > 3)
                                {
                                    yield return null;
                                    counter = 0;
                                }
                            }
                        }
                    }

                    Debug.Log(" DONE INCLUDING ASSETS IN ADDRESSABLE GROUP");
                }
                else
                {
                    Debug.Log("File");
                }
            }
            else
            {
                Debug.Log("Not in assets folder");
            }
        }

        public static void SetAddressableGroup(Object obj, AddressableAssetGroup group, bool log, bool simplifyName)
        {
            if (obj)
            {
                //var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings)
                {
                    var assetpath = AssetDatabase.GetAssetPath(obj);
                    var guid = AssetDatabase.AssetPathToGUID(assetpath);

                    var e = settings.CreateOrMoveEntry(guid, group, false, false);
                    var entriesAdded = new List<AddressableAssetEntry> {e};

                    group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
                    if (simplifyName)
                    {
                        foreach (AddressableAssetEntry entry in entriesAdded)
                            entry.SetAddress(Path.GetFileNameWithoutExtension(entry.address), false);

                        group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entriesAdded, false, true);
                    }

                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
                    if (log)
                        Debug.Log("ADDED " + obj.name + " TO ADDRESSABLES GROUP " + group.name);

                }
            }
            else
            {
                //WTF?
            }
        }

        public static void CleanGroup(string groupName, bool log)
        {
            var group = settings.FindGroup(groupName);
            CleanGroup(group, log);
        }

        public static void CleanGroup(AddressableAssetGroup group, bool log)
        {
            if (group)
            {
                if (settings)
                {
                    var itemsToRemove = group.entries.ToList();
                    foreach (AddressableAssetEntry e in itemsToRemove)
                        group.RemoveAssetEntry(e, false);
                    group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, itemsToRemove, false, true);
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, itemsToRemove, true, false);

                    if (log)
                        Debug.Log("REMOVED ALL ENTRIES FROM ADDRESSABLES GROUP " + group.name);
                }
            }
            else
            {
                Debug.Log("COULD NOT FIND ADDRESSABLES GROUP " + group.name);
            }
        }
    }

    public class CreateAssetBundles
    {
        [MenuItem ("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles ()
        {
            BuildPipeline.BuildAssetBundles ("Assets/BundleToUpload", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        } 
    }
    

    public static IEnumerator Git_CommitChanges()
    {
        string currentDir = Directory.GetCurrentDirectory();
        
        Debug.Log("ADDING CHANGES");
        var request = ShellHelper.ProcessCommand("git add .", currentDir);
        while (ShellHelper.running)
            yield return null;
        if (!ShellHelper.hasError)
        {
            Debug.Log("CHECKING STATUS CHANGES");
            request = ShellHelper.ProcessCommand("git status", currentDir);
            while (ShellHelper.running)
                yield return null;
            if (!ShellHelper.hasError)
            {
                var comment = System.DateTime.Now.ToString();
                comment = comment.Replace(' ', '_');
                Debug.Log("COMMITING CHANGES WITH COMMENT " + comment);
                request = ShellHelper.ProcessCommand("git commit -m " + "\"" + comment + "\"", currentDir);
                while (ShellHelper.running)
                    yield return null;

                if (!ShellHelper.hasError)
                {
                    Debug.Log("PUSHING CHANGES");
                    request = ShellHelper.ProcessCommand("git push", "ALL DONE COMMITING AND PUSHING TO THE CLOUD", "SOME SHIT HAPPENED", currentDir);
                    while (ShellHelper.running)
                        yield return null;
                }
            }
        }
    }

    public static IEnumerator Git_Pull()
    {
        Debug.Log("DOWNLOADING CHANGES FROM GIT");
        var request = ShellHelper.ProcessCommand("git pull", Directory.GetCurrentDirectory());
        while (ShellHelper.running)
            yield return null;

        if (!ShellHelper.hasError)
        {
            AssetDatabase.Refresh();
            Debug.Log("GETTING LAST COMMIT COMMENT");
            var qm = "\"";
            request = ShellHelper.ProcessCommand("git log --pretty=format:" + qm + "%s" + qm + " -n 1", Directory.GetCurrentDirectory());
            while (ShellHelper.running)
                yield return null;
        }
    }
    public static IEnumerator Git_OverwriteAllLocalChangesFromRepo()
    {
        Debug.Log("RESETTING LOCAL CHANGES");
        var request = ShellHelper.ProcessCommand("git reset --hard origin/main", Directory.GetCurrentDirectory());
        while (ShellHelper.running)
            yield return null;
        EditorCoroutines.Execute(Git_Pull());
    }

}


#endif