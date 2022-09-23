using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public delegate void EventHandler(object args);

    Dictionary<int, EventHandler> m_Events = new Dictionary<int, EventHandler>();

    public void Subscribe(int id, EventHandler ev)
    {
        if (m_Events.ContainsKey(id))
            m_Events[id] += ev;
        else
            m_Events.Add(id, ev);
    }

    public void Unsubscribe(int id, EventHandler ev)
    {
        if (m_Events.ContainsKey(id))
        {
            if(m_Events[id] != null)
                m_Events[id] -= ev;
            if (m_Events[id] == null)
                m_Events.Remove(id);
        }
    }

    public void Fire(int id, object args = null)
    {
        EventHandler handler;
        if(m_Events.TryGetValue(id, out handler))
        {
            handler(args);
        }
    }
}
