using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
class CustomTiledImporterForBlocks : Tiled2Unity.ICustomTiledImporter
{

    public void HandleCustomProperties(UnityEngine.GameObject gameObject,
        IDictionary<string, string> props)
    {
        // Does this game object have a spawn property?
        if (!props.ContainsKey("multiplication"))
            return;

        // Are we spawning an Appearing Block?
		if ((props["multiplication"] != "BlocApparaissantMegaman1")
		    && (props["multiplication"] != "BlocApparaissantMegaman2")
		    && (props["multiplication"] != "BlocApparaissantMegaman3")
			&& (props["multiplication"] != "BlocApparaissantMetroid"))
            return;

        // Load the prefab assest and Instantiate it
		string prefabPath = "";
		if (props["multiplication"] == "BlocApparaissantMegaman1")
			prefabPath = "Assets/Partie jeu de plates-formes/Prefabs/Décors/BlocApparaissantMegaman1.prefab";
		else if (props["multiplication"] == "BlocApparaissantMegaman2")
			prefabPath = "Assets/Partie jeu de plates-formes/Prefabs/Décors/BlocApparaissantMegaman2.prefab";
		else if (props["multiplication"] == "BlocApparaissantMegaman3")
			prefabPath = "Assets/Partie jeu de plates-formes/Prefabs/Décors/BlocApparaissantMegaman3.prefab";
		else if (props["multiplication"] == "BlocApparaissantMetroid")
			prefabPath = "Assets/Partie jeu de plates-formes/Prefabs/Décors/BlocApparaissantMetroid.prefab";
        UnityEngine.Object spawn = 
            AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
        if (spawn != null)
        {
            GameObject spawnInstance = 
                (GameObject)GameObject.Instantiate(spawn);
            spawnInstance.name = spawn.name;

            // Use the position of the game object we're attached to
            spawnInstance.transform.parent = gameObject.transform;
            spawnInstance.transform.localPosition = Vector3.zero;
        }
    }

    public void CustomizePrefab(UnityEngine.GameObject prefab)
    {
        // Do nothing
    }
}