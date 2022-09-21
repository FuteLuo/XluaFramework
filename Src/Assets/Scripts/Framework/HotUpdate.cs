using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour {

    byte[] m_ReadPathFileListData;
    byte[] m_ServerFileListData;

    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;
    }
    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
	IEnumerator DownLoadFile(DownFileInfo info, Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if(webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError("下载文件出错：" + info.url);
            yield break;
            //重试 拓展内容
        }
        info.fileData = webRequest.downloadHandler;
        if (Complete != null)
        {
            Complete.Invoke(info);
        }
        webRequest.Dispose();
    }

    /// <summary>
    /// 下载多个文件
    /// </summary>
    /// <param name="info"></param>
    /// <param name="Complete"></param>
    /// <returns></returns>
    IEnumerator DownLoadMultipleFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete, Action DownLoadAllComplete)
    {
        foreach(DownFileInfo info in infos)
        {
            yield return DownLoadFile(info, Complete);
        }
        if (DownLoadAllComplete != null)
            DownLoadAllComplete.Invoke();
    }

    /// <summary>
    /// 获取文件信息
    /// </summary>
    /// <returns></returns>
    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for(int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }
        return downFileInfos;
    }

    private void Start()
    {
        if (IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }
    #region 是否第一次安装
    
    private bool IsFirstInstall()
    {
        //判断只读目录是否存在版本文件
        bool isExitsReadPath = FileUtil.IsFileExist(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));

        //判断可读写目录是否存在版本文件
        bool isExitsReadWritePath = FileUtil.IsFileExist(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

        return isExitsReadWritePath && !isExitsReadPath;
    }
    #endregion

    #region 释放资源，下载资源，写入资源，接着检查更新
    private void ReleaseResources()
    {
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileListComplete));
    }

    private void OnDownLoadReadPathFileListComplete(DownFileInfo file)
    {
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownLoadMultipleFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));

    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();
    }
    

    private void OnReleaseFileComplete(DownFileInfo fileinfo)
    {
        Debug.Log("OnReleaseFileComplete" + fileinfo.url);
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileinfo.fileName);
        FileUtil.WriteFile(writeFile, fileinfo.fileData.data);
    }

    #endregion

    #region 从服务器下载热更新字段，如果一样则进入游戏，否则就更新最新资源
    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourcesUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo file)
    {
        m_ServerFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.ResourcesUrl);
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        for(int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if(!FileUtil.IsFileExist(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].fileName);
                downListFiles.Add(fileInfos[i]);
            }
        }
        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownLoadMultipleFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));
        }
        else
            EnterGame();
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo fileinfo)
    {
        Debug.Log("OnUpdateFileComplete" + fileinfo.url);
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileinfo.fileName);
        FileUtil.WriteFile(writeFile, fileinfo.fileData.data);
    }
    #endregion
    private void EnterGame()
    {
        Manager.Resource.ParseVersionFile();
        Manager.Resource.LoadUI("TestUI", OnCompleted);
    }

    

    private void OnCompleted(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
