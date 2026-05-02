using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Manager<ObjectManager>
{
    private Dictionary<string, List<ObjectIdentifier>> _objectsByGroup = new();

    public void AddObject(ObjectIdentifier obj)
    {
        foreach (string group in obj.groups)
        {
            if (string.IsNullOrEmpty(group)) continue;

            if (!_objectsByGroup.ContainsKey(group))
            {
                _objectsByGroup[group] = new List<ObjectIdentifier>();
            }

            if (!_objectsByGroup[group].Contains(obj))
            {
                _objectsByGroup[group].Add(obj);
            }
        }
    }

    public void RemoveObject(ObjectIdentifier obj)
    {
        foreach (string group in obj.groups)
        {
            if (string.IsNullOrEmpty(group)) continue;

            if (_objectsByGroup.ContainsKey(group))
            {
                _objectsByGroup[group].Remove(obj);
                if (_objectsByGroup[group].Count == 0)
                {
                    _objectsByGroup.Remove(group);
                }
            }
        }
    }

    public List<ObjectIdentifier> GetGroup(string group)
    {
        if (_objectsByGroup.TryGetValue(group, out List<ObjectIdentifier> objects))
        {
            return new List<ObjectIdentifier>(objects);
        }
        return new List<ObjectIdentifier>();
    }

    public ObjectIdentifier GetObjectInGroup(string id, string group = "Default")
    {
        if (_objectsByGroup.TryGetValue(group, out List<ObjectIdentifier> objects))
        {
            return objects.Find(obj => obj.id == id);
        }
        return null;
    }
}