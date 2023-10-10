using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Gameframe.SaveLoad;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using AbilitySystem.Authoring;
using System.Linq;

public class ResourceManager: Singleton<ResourceManager>
{
    public readonly string PrefabPath = "Prefabs";
    public readonly string GameComponentDataPath = "GameComponentData";
    public readonly string DefaultDeviceInfoPath = "DefaultDeviceInfo";
    public readonly string AttributeDir = "Attributes";
    public readonly string AttributeHandlerDir = "AttributeHandlers";
    public readonly string GameplayEffectDir = "GameplayEffects";

    private SaveLoadManager localDeviceStorageManager;
    private AbstractAbilityScriptableObject[] _allAbilities;
    public ResourceManager() { 
        localDeviceStorageManager = SaveLoadManager.Create("BaseDirectory", "SavedDevice", SerializationMethodType.JsonDotNet);
    }
    public GameObject LoadPrefab(string filename){
        var path = Path.Combine(PrefabPath, filename);
        var prefab = Resources.Load<GameObject>(path);
        if(prefab == null){
            Debug.LogWarning("Cannot find prefab: " + path);
        }
        return prefab;
    }

    public List<GameComponentData> LoadAllGameComponentData(){
        var data = Resources.LoadAll<GameComponentData>(GameComponentDataPath);
        var dataList = new List<GameComponentData>();
        foreach(var d in data){
            dataList.Add(d);
        }
        return dataList;
    }

    public DeviceInfo LoadDefaultDeviceInfo(){
        var text = (TextAsset)Resources.Load(DefaultDeviceInfoPath);
        var info = JsonConvert.DeserializeObject<DeviceInfo>(text.text);
        Debug.Assert(info != null);
        return info;
    }

    public DeviceInfo LoadLocalDeviceInfo(string name){
        string filename = name + ".json";
        var deviceInfo = localDeviceStorageManager.Load<DeviceInfo>(filename);
        return deviceInfo;
    }

    public void SaveLocalDeviceInfo(DeviceInfo info, string name){
        Debug.Assert(info != null);
        string filename = name + ".json";
        localDeviceStorageManager.Save(info, filename);
    }

    public AttributeScriptableObject LoadAttribute(string name){
        var path = Path.Combine(AttributeDir, name);
        var attribute = Resources.Load<AttributeScriptableObject>(path);
        if(attribute == null){
            Debug.LogWarning("Cannot load attribute: " + path);
        }
        return attribute;
    }

    public AbstractAttributeEventHandler LoadAttributeEventHandler(string name){
        var path = Path.Combine(AttributeHandlerDir, name);
        var handler = Resources.Load<AbstractAttributeEventHandler>(path);
        if(handler == null){
            Debug.LogWarning("Cannot load attribute event handler: " + path);
        }
        return handler;
    }

    public GameplayEffectScriptableObject LoadGameplayEffect(string name){
        var path = Path.Combine(GameplayEffectDir, name);
        var effect = Resources.Load<GameplayEffectScriptableObject>(path);
        if(effect == null){
            Debug.LogWarning("Cannot load gameplay effect: " + path);
        }
        return effect;
    }

    public GameplayEffectScriptableObject LoadEmptyGameplayEffect(){
        var path = Path.Combine(GameplayEffectDir, "EmptyGameplayEffect");
        var effect = Resources.Load<GameplayEffectScriptableObject>(path);
        if(effect == null){
            Debug.LogWarning("Cannot load gameplay effect: " + path);
        }
        return effect;
    }

    public GameObject LoadPlayerObject(){
        var path = Path.Combine(PrefabPath, "Player");
        var player = Resources.Load<GameObject>(path);
        if(player == null){
            Debug.LogWarning("Cannot load player object: " + path);
        }
        return player;
    }
    public AbstractAbilityScriptableObject GetAbilityScriptableObjectByName(string name){
        if (_allAbilities == null){
            _allAbilities = Resources.FindObjectsOfTypeAll<DisplayableAbilityScriptableObject>();
        }
        var result = _allAbilities.ToList().FindAll((ability) => ability.AbilityName == name);
        if (result.Count == 0){
            Debug.LogWarning("Cannot find ability: " + name);
            return null;
        }
        if (result.Count > 1){
            Debug.LogWarning("Find more than one ability with the same name: " + name);
        }
        return result.First();
    }

}
