using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour {

    //缓存ui
    Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();

    //ui分组
    Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();

    private Transform m_EntityParent;

    private void Awake()
    {
        m_EntityParent = this.transform.parent.Find("Entity");
    }

    public void SetEntityGroup(List<string> group)
    {
        for (int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(m_EntityParent, false);
            m_Groups[group[i]] = go.transform;
        }
    }

    Transform GetGroup(string group)
    {
        if (!m_Groups.ContainsKey(group))
        {
            Debug.LogError("Group is not exist");
        }
        return m_Groups[group];
    }


    public void ShowEntity(string Name, string group, string luaName)
    {
        GameObject entity = null;
        if (m_Entities.TryGetValue(Name, out entity))
        {
            EntityLogic Logic = entity.GetComponent<EntityLogic>();
            Logic.OnShow();
            return;
        }

        Manager.Resource.LoadPrefab(Name, (UnityEngine.Object obj) =>
        {
            entity = Instantiate(obj) as GameObject;
            Transform parent = GetGroup(group);
            entity.transform.SetParent(parent, false);
            m_Entities.Add(Name, entity);
            EntityLogic entityLogic = entity.AddComponent<EntityLogic>();
            entityLogic.Init(luaName);
            entityLogic.OnShow();
        });
    }
}
