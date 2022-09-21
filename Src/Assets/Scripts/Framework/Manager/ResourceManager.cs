using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UObject = UnityEngine.Object;

public class ResourceManager : MonoBehaviour {
    
    internal class BundleInfo
    {
        public string AssetsName;
        public string BundleName;
        public List<string> Dependences;
    }

    //存放Bundle信息的集合
    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();

    /// <summary>
    /// 解析版本文件
    /// </summary>
	public void ParseVersionFile()
    {
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(url);

        //解析文件信息
        for(int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');
            bundleInfo.AssetsName = info[0];
            bundleInfo.BundleName = info[1];
            bundleInfo.Dependences = new List<string>(info.Length - 2);
            for(int j = 2; j < info.Length; j++)
            {
                bundleInfo.Dependences.Add(info[j]);
            }

            Debug.Log(bundleInfo.AssetsName);

            m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);

            if (info[0].IndexOf("LuaScripts") > 0)
                Manager.Lua.LuaNames.Add(info[0]);
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="assetName">资源名</param>
    /// <param name="action">完成回调</param>
    /// <returns></returns>
    IEnumerator LoadBundleAsync(string assetName, Action<UObject> action = null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);
        List<string> dependences = m_BundleInfos[assetName].Dependences;
        if(dependences != null && dependences.Count > 0)
        {
            for(int i = 0; i < dependences.Count; i++)
            {
                yield return LoadBundleAsync(dependences[i]);
            }
        }

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;
        Debug.Log("LoadBundleAsync");
        if(action != null && bundleRequest != null)
        {
            action.Invoke(bundleRequest.asset);
        }
    }

    //Tag:卸载，Todo

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器环境加载资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    void EditorLoadAsset(string assetName, Action<UObject> action = null)
    {
        Debug.Log("This is Editor");
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UObject));
        if (obj == null)
            Debug.LogError("assets name is not exit:" + assetName);
        if (action != null)
            action.Invoke(obj);
    }
#endif


    private void LoadAsset(string assetName,Action<UObject> action)
    {
#if UNITY_EDITOR
        if (AppConst.gameMode == GameMode.EditorMode)
            EditorLoadAsset(assetName, action);
        else
#endif
            StartCoroutine(LoadBundleAsync(assetName, action));
    }

    //加载UI
    public void LoadUI(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), action);
    }
    //加载音乐
    public void LoadMusic(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }
    //加载音效
    public void LoadSound(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }
    //加载特效
    public void LoadEffect(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }
    //加载图片
    public void LoadSprite(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetSpritePath(assetName), action);
    }
    //加载场景
    public void LoadScene(string assetName, Action<UObject> action = null)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), action);
    }
    //加载lua
    public void LoadLua(string assetName, Action<UObject> action = null)
    {
        LoadAsset(assetName, action);
    }
    //加载实体
    public void LoadPrefab(string assetName, Action<UObject> action = null)
    {
        LoadAsset(assetName, action);
    }

}
