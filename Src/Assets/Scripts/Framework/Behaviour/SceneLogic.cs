﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLogic : LuaBehaviour {

    public string SceneName;

    Action m_LuaActive;
    Action m_LuaInActive;
    Action m_LuaOnEnter;
    Action m_LuaOnQuit;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_ScriptEnv.Get("OnActive", out m_LuaActive);
        m_ScriptEnv.Get("OnInActive", out m_LuaInActive);
        m_ScriptEnv.Get("OnEnter", out m_LuaOnEnter);
        m_ScriptEnv.Get("OnQuit", out m_LuaOnQuit);
    }

    public void OnActive()
    {
        if(m_LuaInActive != null)
        {
            m_LuaInActive.Invoke();
        }
    }

    public void OnInActive()
    {
        if (m_LuaInActive != null)
        {
            m_LuaInActive.Invoke();
        }
    }

    public void OnEnter()
    {
        if (m_LuaOnEnter != null)
        {
            m_LuaOnEnter.Invoke();
        }
    }

    public void OnQuit()
    {
        if (m_LuaOnQuit != null)
        {
            m_LuaOnQuit.Invoke();
        }
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaActive = null;
        m_LuaInActive = null;
        m_LuaOnEnter = null;
        m_LuaOnQuit = null;
    }
}
