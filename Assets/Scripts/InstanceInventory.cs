using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InstanceInventory : ScriptableObject
{
    public List<ItemInstance> items = new();
}