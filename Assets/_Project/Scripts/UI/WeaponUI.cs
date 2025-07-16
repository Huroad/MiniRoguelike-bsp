using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct WeaponUIEntry
{
    public WeaponType type;
    public GameObject uIObject;
}

public class WeaponUI : MonoBehaviour
{
    public List<WeaponUIEntry> weaponUIEntries; 

    private Dictionary<WeaponType, GameObject> weaponUIDict;

    void Awake()
    {
        weaponUIDict = new Dictionary<WeaponType, GameObject>();
        foreach (var entry in weaponUIEntries)
        {
            if (!weaponUIDict.ContainsKey(entry.type))
                weaponUIDict.Add(entry.type, entry.uIObject);
        }
    }

    public void ShowWeaponUI(WeaponType type)
    {
        print(type.ToString());
        foreach (var pair in weaponUIDict)
            pair.Value.SetActive(pair.Key == type);
    }
}
