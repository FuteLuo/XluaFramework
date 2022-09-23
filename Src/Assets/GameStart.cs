using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

   

    public GameMode gameMode;
	// Use this for initialization
	void Start () {
        Manager.Event.Subscribe(10000, OnLuaInit);
        AppConst.gameMode = this.gameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        Manager.Lua.Init();
        
	}

    void OnLuaInit(object args)
    {
        Manager.Lua.StartLua("main");
        XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
        func.Call();
    }

    public void OnApplicationQuit()
    {
        Manager.Event.Unsubscribe(10000, OnLuaInit);
    }

}
