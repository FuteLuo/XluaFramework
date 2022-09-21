using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum GameMode
{
    EditorMode,
    PackageBundle,
    UpdateMode,
}

public class AppConst
{
    public const string BundleExtension = ".ab";
    public const string FileListName = "filelist.txt";

    public static GameMode gameMode = GameMode.EditorMode;

    //热更资源地址
    public const string ResourcesUrl = "http://192.168.0.103/AssetBundles";
}

 