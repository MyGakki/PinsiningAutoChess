using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class UIViewManager: BaseManager<UIViewManager>
{
    public override string MgrName => "UIViewManager";

    [SerializeField] private RectTransform screenUIRoot;
    private Vector2 screenUICanvasRootSize;
    
    //配置文件
    private Dictionary<UIViewName, SO_UIViewConfig> uiViewConfigDic = new Dictionary<UIViewName, SO_UIViewConfig>();
    
    //屏幕UI的开启列表
    private List<UIViewBase> viewList = new List<UIViewBase>();
    private Dictionary<UIViewLayer, UIViewLayerController> viewLayerDic = new Dictionary<UIViewLayer, UIViewLayerController>();
    
    //临时缓冲区内的界面
    public int screenUITempCacheDepth = 0;

    [SerializeField] private List<UIViewLayerController> screenUIViewLayers;
    
    //常驻内存的界面
    private List<UIViewBase> screenUICache = new List<UIViewBase>();
    private List<UIViewBase> screenUITempCache = new List<UIViewBase>();

    public override void InitManager()
    {
        base.InitManager();

        screenUICanvasRootSize = screenUIRoot.sizeDelta;
        MgrLog($"屏幕UI Canvas大小{screenUICanvasRootSize}");

        InitViewConfig();
    }

    public async Task<UIViewBase> ShowView(UIViewName viewName, params object[] args)
    {
        SO_UIViewConfig config = GetConfig(viewName);
        if (config == null)
            return null;

        UIViewBase view = null;

        if (config.unique)
        {
            //界面是唯一打开的
            for (int i = 0; i < viewList.Count; i++)
            {
                if (viewList[i].Config.viewName == viewName)
                {
                    //打开过
                    view = viewList[i];
                    break;
                }
            }
            //界面被打开过
            if (view != null)
            {
                if (view.LayerController == null)
                {
                    Debug.LogError($"展示界面错误: {viewName}, 没有层级");
                    return null;
                }
                view.SetArguments(args);
                view.LayerController.Push(view);
            }
            else
            {
                 Task<UIViewBase> handle =  ShowViewFromCacheOrCreateNew(config, args);
                 await handle;
                 if (handle.Status == TaskStatus.RanToCompletion)
                 {
                     view = handle.Result;
                 }
            }
        }
        else
        {
            Task<UIViewBase> handle =  ShowViewFromCacheOrCreateNew(config, args);
            await handle;
            if (handle.Status == TaskStatus.RanToCompletion)
            {
                view = handle.Result;
            }
        }

        return view;

        //刷新显示、隐藏状态
        //UpdateViewHideState();
    }
    
    //关闭第一个同名界面
    public void HideView(UIViewName viewName)
    {
        for (int i = viewList.Count - 1; i >= 0; i--)
        {
            if (viewList[i].Config.viewName == viewName)
            {
                HideView(viewList[i]);
                return;
            }
        }
    }

    public void HideView(UIViewBase view)
    {
        if (view == null)
            return;

        if (view.LayerController != null)
        {
            viewList.Remove(view);
            view.LayerController.Popup(view);
            SchemeViewCache(view);
            UpdateViewHideState();
        }
        else
        {
            Debug.LogError($"关闭界面错误: {view.Config.viewName}, 没有层级");
        }
    }

    //关闭一层全部界面
    public void HideViews(UIViewLayer layer)
    {
        if (viewLayerDic.ContainsKey(layer) == false)
        {
            Debug.LogError($"隐藏层级失败: {layer}");
            return;
        }

        UIViewBase[] views = viewLayerDic[layer].PopupAll();
        if (views != null)
        {
            for (int i = 0; i < views.Length; i++)
            {
                viewList.Remove(views[i]);
                SchemeViewCache(views[i]);
            }

            UpdateViewHideState();
        }
    }

    public T GetViewByName<T>(UIViewName viewName)
        where T : UIViewBase
    {
        for (int i = 0; i < viewList.Count; ++i)
        {
            if(viewList[i].Config.viewName == viewName)
            {
                return viewList[i] as T;
            }
        }
        return null;
    }

    private void SchemeViewCache(UIViewBase view)
    {
        if (view != null)
        {
            switch (view.Config.cacheScheme)
            {
                case UIViewCacheScheme.Cache:
                    CacheView(view);
                    break;
                case UIViewCacheScheme.TempCache:
                    TempCacheView(view);
                    break;
                case UIViewCacheScheme.AutoRemove:
                    ReleaseView(view);
                    break;
            }
        }
    }
    
    //释放界面
    private void ReleaseView(UIViewBase view)
    {
        if (view != null)
        {
            view.OnExit();
            Destroy(view.gameObject);
        }
    }

    private void TempCacheView(UIViewBase view)
    {
        if (screenUITempCacheDepth <= 0)
            ReleaseView(view);
        
        screenUITempCache.Add(view);

        TidyTempCache();
    }

    private void TidyTempCache()
    {
        int removeCount = screenUITempCache.Count - screenUITempCacheDepth;
        while (removeCount > 0)
        {
            --removeCount;
            ReleaseView(screenUITempCache[0]);
            screenUITempCache.RemoveAt(0);
        }
    }

    private void CacheView(UIViewBase view)
    {
        if (screenUICache.Contains(view) == false)
        {
            screenUICache.Add(view);
        }
    }

    private SO_UIViewConfig GetConfig(UIViewName viewName)
    {
        if (uiViewConfigDic.ContainsKey(viewName) == false)
        {
            Debug.LogError($"获取配置失败: {viewName}");
            return null;
        }

        return uiViewConfigDic[viewName];
    }

    private void PushViewToLayer(UIViewBase view, params object[] args)
    {
        if (view != null)
        {
            view.SetArguments(args);
            viewList.Add(view);
            viewLayerDic[view.Config.viewLayer].Push(view);
        }
    }

    private UIViewBase GetViewFromCache(SO_UIViewConfig config)
    {
        if (config == null)
            return null;

        UIViewBase view = null;
        List<UIViewBase> cache = null;

        switch (config.cacheScheme)
        {
            case UIViewCacheScheme.Cache:
                cache = screenUICache;
                break;
            case UIViewCacheScheme.TempCache:
                cache = screenUITempCache;
                break;
        }

        if (cache != null)
        {
            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i].Config.viewName == config.viewName)
                {
                    view = cache[i];
                    cache.RemoveAt(i);
                    break;
                }
            }
        }

        return view;
    }

    private async Task<UIViewBase> ShowNewView(SO_UIViewConfig config)
    {
        if (viewLayerDic.ContainsKey(config.viewLayer) == false)
        {
              UtilityHelper.LogError("展示新窗口失败，Layer不存在");
              return null; 
        }

        Task<UIViewBase> handle = CreateUIView(config);
        await handle;
        if (handle.Status == TaskStatus.RanToCompletion && handle.Result != null)
        {
            handle.Result.Init();
            handle.Result.transform.SetParent(viewLayerDic[config.viewLayer].transform);
            handle.Result.GetComponent<RectTransform>().Normalize();
        }

        return handle.Result;
    }

    private async Task<UIViewBase> ShowViewFromCacheOrCreateNew(SO_UIViewConfig config, params object[] args)
    {
        UIViewBase view = GetViewFromCache(config);

        if (view == null)
        {
            Task<UIViewBase> handle = ShowNewView(config);
            await handle;
            if (handle.Status == TaskStatus.RanToCompletion)
            {
                view = handle.Result;
            }
        }
        
        if (view != null)
            PushViewToLayer(view, args);
        else
            Debug.LogError($"显示{config.viewName}失败");

        return view;
    }
    
    //刷新界面的隐藏情况
    private void UpdateViewHideState()
    {
        bool covered = false;
        covered = viewLayerDic[UIViewLayer.Debug].RefreshView(covered);
        covered = viewLayerDic[UIViewLayer.Top].RefreshView(covered);
        covered = viewLayerDic[UIViewLayer.HUD].RefreshView(covered);
        covered = viewLayerDic[UIViewLayer.Popup].RefreshView(covered);
        covered = viewLayerDic[UIViewLayer.Base].RefreshView(covered);
        covered = viewLayerDic[UIViewLayer.Background].RefreshView(covered);
    }

    private async Task<UIViewBase> CreateUIView(SO_UIViewConfig config)
    {
        var handle = Addressables.InstantiateAsync(config.Address);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            handle.Result.transform.CleanName();
            UIViewBase view = handle.Result.GetComponent<UIViewBase>();
            return view;
        }
        Debug.LogError($"加载{config.assetName}失败");
        return null;
    }

    private async void InitViewConfig()
    {
        var handle = Addressables.LoadAssetsAsync<SO_UIViewConfig>("UIConfig", null);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (SO_UIViewConfig config in handle.Result)
            {
                Debug.Log($"加载{config.assetName}界面配置文件");
                uiViewConfigDic.Add(config.viewName, config);
            }
            
            InitViewLayers();

            await ShowView(UIViewName.StartGame);
        }
    }

    private void InitViewLayers()
    {
        for (int i = 0; i < screenUIViewLayers.Count; i++)
        {
            screenUIViewLayers[i].Init();
            viewLayerDic.Add(screenUIViewLayers[i].ViewLayer, screenUIViewLayers[i]);
        }
    }

    public Vector2 ConvertWorldPositionToRootCanvasPosition(Vector3 worldPosition)
    {
        Vector2 pos = BattleFieldRenderer.Instance.BattleCamera.WorldToScreenPoint(worldPosition);
        
        pos.x = pos.x / BattleFieldRenderer.Instance.BattleCamera.pixelWidth * screenUICanvasRootSize.x - screenUICanvasRootSize.x * 0.5f;
        pos.y = pos.y / BattleFieldRenderer.Instance.BattleCamera.pixelHeight * screenUICanvasRootSize.y - screenUICanvasRootSize.y * 0.5f;
        return pos;
    }

    public Vector2 GetRelativePosition(Vector2 positionInCanvas)
    {
        return positionInCanvas / screenUICanvasRootSize;
    }
}
