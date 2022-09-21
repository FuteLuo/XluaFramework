using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/prefabs/testui.prefab.ab");
        yield return request;

        AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/ui/res/qq图片20220822140326.jpg.ab");
        yield return request1;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync("Assets/BuildResources/UI/Prefabs/TestUI.prefab");
        yield return bundleRequest;

        GameObject go = Instantiate(bundleRequest.asset) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
