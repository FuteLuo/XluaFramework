using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour {

    public GameMode gameMode;
	// Use this for initialization
	void Start () {
        AppConst.gameMode = this.gameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        Manager.Lua.Init(
            ()=>
            {
                Manager.Lua.StartLua("main");
                XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
                func.Call();
            }
            );
        
	}

}
