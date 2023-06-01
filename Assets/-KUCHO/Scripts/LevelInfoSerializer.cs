using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;
/*
public static class LevelInfoSerializer
{
	public static string GetLevelInfo(int index)
	{
		string fullPath = GetPath();
		var allLines = System.IO.File.ReadAllLines(fullPath);
		if (index >= 0)
		{
			string levelTitle = GetCleanTitleFromSceneFileName(Game.data.levelData[index].title);
			for (int i = 0; i < allLines.Count(); i++)
			{
				if (allLines[i].StartsWith(levelTitle))
				{
					return allLines[i];
				}
			}
		}

		return "";
	}

	public static void SaveCurrentLevelInfoIntoTextFile()
	{
		var levelDiff = MonoBehaviour.FindObjectOfType<LevelDifficulty>();
		SaveCurrentLevelInfoIntoTextFile(levelDiff);
	}

	public static void SaveCurrentLevelInfoIntoTextFile(LevelDifficulty levelDiff)
	{
		if (!levelDiff)
			return;
		var levelTitle = GetCleanTitleFromSceneFileName(SceneManager.GetActiveScene().name);
		if (levelTitle != "")
		{
			string fullPath = GetPath();
			if (!System.IO.File.Exists(fullPath))
			{
				var sr = System.IO.File.CreateText(fullPath);
				sr.Close();
			}

			var allLines0 = System.IO.File.ReadLines(fullPath);
			var allLines = allLines0.ToList();
			bool found = false;
			for (int i = 0; i < allLines.Count(); i++)
			{
				if (allLines[i].StartsWith(levelTitle))
				{
					allLines[i] = levelDiff.GetStringForLevelInfoFile(levelTitle); //reemplazo
					found = true;
					break;
				}
			}

			if (!found)
				allLines.Add(levelDiff.GetStringForLevelInfoFile(levelTitle));

			System.IO.File.WriteAllLines(fullPath, allLines);
			Debug.Log("LEVEL INFO DE " + levelTitle + " SALVADA A FILE");
		}
	}

	public static void ClearLevelInfoFile()
	{
		string fullPath = GetPath();
		if (System.IO.File.Exists(fullPath))
		{
			System.IO.File.Delete(fullPath);
			//System.IO.File.Create(fullPath);
		}

//		var allLines = new List<string>();

//		System.IO.File.WriteAllLines(fullPath, allLines);
	}

	public static string GetPath()
	{
		return KuchoHelper.GetCombinedDataPathForReadOnlyFiles("LevelInfo");
	}

	public static string GetCleanTitleFromSceneFileName(string t)
	{
		t = t.ToUpper();
		if (t.StartsWith("LEVEL "))
		{
			t = t.Remove(0, 6);
		}
		else if (t.StartsWith("LEVEL"))
		{
			t = t.Remove(0, 5);
		}
		if (t.StartsWith(" "))
		{
			t = t.Remove(0, 1);
		}
		return t;
	}
}
*/