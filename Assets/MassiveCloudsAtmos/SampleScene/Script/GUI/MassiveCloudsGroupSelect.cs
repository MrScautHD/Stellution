#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mewlist.MassiveClouds;
using UnityEngine;
using UnityEngine.UI;

public class MassiveCloudsGroupSelect : MonoBehaviour
{
	[Serializable]
	public class Group
	{
		public string Name;
		public List<MassiveCloudsStylizedCloudProfile> Profiles = new List<MassiveCloudsStylizedCloudProfile>();
	}
	
	public GameObject ItemTemplate;
	public Transform OptionsRoot;
	public List<Group> Groups = new List<Group>();
	public int initialGroup;

	public bool build = false;

	// Use this for initialization
	void Start ()
	{
		Select(initialGroup);
		GetComponent<Dropdown>().value = initialGroup;
	}
	
	// Update is called once per frame
	private void OnValidate()
	{
#if UNITY_EDITOR
		if (build)
		{
			build = false;
			var sep = Path.DirectorySeparatorChar;
			var rootDirPath = Application.dataPath +
			        sep + "MassiveClouds" +
					sep + "Profile" +
			        sep + "Stylized Clouds";

			var dropdown = GetComponent<Dropdown>();
			dropdown.options.Clear();
			Groups.Clear();
			var dirInfo = new DirectoryInfo(rootDirPath);
			var dirs = dirInfo.GetDirectories();
			foreach (var dir in dirs)
			{
				Debug.Log(dir.Name);
				dropdown.options.Add(new Dropdown.OptionData(dir.Name));
				var fileInfo = new DirectoryInfo(dir.FullName);
				var files = fileInfo.GetFiles();
				var group = new Group();
				Groups.Add(group);
				group.Name = dir.Name;
				foreach (var file in files)
				{
					if (file.Name.EndsWith(".asset"))
					{
						var assetPath = "Assets" +
						                sep + "MassiveClouds" +
						                sep + "Profile" +
						                sep + "Stylized Clouds" +
						                sep + dir.Name +
						                sep + file.Name;
						var profile = AssetDatabase.LoadAssetAtPath<MassiveCloudsStylizedCloudProfile>(assetPath);
						group.Profiles.Add(profile);
						Debug.Log(profile.name);
					}
				}
			}
		}
#endif
	}

	public void Select(int groupIndex)
	{
		foreach (Transform o in OptionsRoot)
		{
			if (ItemTemplate != o.gameObject)
				Destroy(o.gameObject);
		}
		var groupName = GetComponent<Dropdown>().options[groupIndex].text;
		var targetGroups = Groups.Where(group => group.Name == groupName);
		foreach (var targetGroup in targetGroups)
		{
			foreach (var profile in targetGroup.Profiles)
			{
				var option = Instantiate(ItemTemplate) as GameObject;
				var toggle = option.GetComponent<Toggle>();
				var changer = option.GetComponent<MassiveCloudsProfileChanger>();
				toggle.isOn = false;
				changer.SetProfile(profile);
				option.SetActive(true);
				option.transform.SetParent(OptionsRoot, false);
			}
		}
	}
}
