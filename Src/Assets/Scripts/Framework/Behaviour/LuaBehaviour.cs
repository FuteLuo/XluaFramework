using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour {
    private LuaEnv m_LuaEnv = Manager.Lua.LuaEnv;
    protected LuaTable m_ScriptEnv;
    private Action m_LuaOnInit;
    //private Action m_LuaStart;
    private Action m_LuaUpdate;
    private Action m_LuaOnDestory;

    public string luaName;
    // Use this for initialization
    private void Awake() {
        m_ScriptEnv = m_LuaEnv.NewTable();
        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = m_LuaEnv.NewTable();
        meta.Set("__index", m_LuaEnv.Global);
        m_ScriptEnv.SetMetaTable(meta);
        meta.Dispose();

        m_ScriptEnv.Set("self", this);


    }

    public virtual void Init(string luaName)
    {
        m_LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), luaName, m_ScriptEnv);

        //m_ScriptEnv.Get("Awake", out m_LuaAwake);
        //m_ScriptEnv.Get("Start", out m_LuaStart);
        m_ScriptEnv.Get("Update", out m_LuaUpdate);
        m_ScriptEnv.Get("OnInit", out m_LuaOnInit);

        if (m_LuaOnInit != null)
        {
            m_LuaOnInit.Invoke();
        }
    }


    // Update is called once per frame
    void Update () {
        if (m_LuaUpdate != null)
        {
            m_LuaUpdate.Invoke();
        }
    }

    protected virtual void Clear()
    {
        m_LuaOnDestory = null;
        if (m_ScriptEnv != null)
        {   
            m_ScriptEnv.Dispose();
        }
        m_ScriptEnv = null;
        m_LuaUpdate = null;
        m_LuaOnInit = null;
        
    }

    private void OnDestroy()
    {
        if (m_LuaOnDestory != null)
        {
            m_LuaOnDestory.Invoke();
        }
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
