using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIViewState
{
    Invisible,
    Visible,
    Cache,
}

public class UIViewBase : MonoBehaviour
{
    public SO_UIViewConfig Config;

    //所属的层级的控制器
    [HideInInspector] public UIViewLayerController LayerController;
    //当前界面的状态
    private UIViewState viewState;
    //是否需要刷新
    private bool dirty = false;

    protected Canvas canvas;

    public UIViewState ViewState
    {
        get => viewState;
        set => viewState = value;
    }
    
    //设置界面层级
    public int ViewOrder
    {
        get => canvas.sortingOrder;
        set => canvas.sortingOrder = value;
    }

    private void InitCanvas()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
            canvas = gameObject.AddComponent<Canvas>();

        GraphicRaycaster caster = GetComponent<GraphicRaycaster>();
        if (caster == null)
            caster = gameObject.AddComponent<GraphicRaycaster>();
    }

    public void SetArguments(params object[] args)
    {
        dirty = true;
        UpdateArguments(args);
        
        if (LayerController == null) return;
        
        //如果当前这个界面就是显示的
        //或尽管隐藏也需要刷新
        //那么设定了参数直接就要刷新
        if(Config.alwaysUpdate || viewState == UIViewState.Visible)
            UpdateView();
    }

    protected virtual void UpdateArguments(params object[] args) { }
    
    //界面初始化
    public void Init()
    {
        InitCanvas();
        InitUIObjects();
        InitBG();
    }
    
    protected virtual void InitUIObjects() {}

    //初始化背景
    protected void InitBG()
    {
        if (Config.bgTriggerClose)
        {
            Transform bgTran = transform.Find("BG");
            if (bgTran == null)
            {
                GameObject bgObj = new GameObject("BG", typeof(RectTransform));
                bgTran = bgObj.transform;
                bgTran.SetParent(transform);
                bgTran.SetAsFirstSibling();
                RectTransform rt = bgObj.GetComponent<RectTransform>();
                rt.Normalize();
            }
            //查看BG上是否存在图片
            Image image = bgTran.GetComponent<Image>();
            if (image == null)
            {
                image = bgTran.gameObject.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0);
                CanvasRenderer cr = bgTran.GetComponent<CanvasRenderer>();
                cr.cullTransparentMesh = true;
            }

            image.raycastTarget = true;
            //BG是否存在点击事件
            EventTrigger eventTrigger = bgTran.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = bgTran.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(CloseWithEvent);
            eventTrigger.triggers.Add(entry);
        }
    }

    private void UpdateLayer()
    {
        if (canvas.overrideSorting == false)
        {
            canvas.overrideSorting = true;
            switch (Config.viewLayer)
            {
                case UIViewLayer.Background:
                    canvas.sortingLayerID = GameConst.SortingLayer_View_Background;
                    break;
                case UIViewLayer.Base:
                    canvas.sortingLayerID = GameConst.SortingLayer_View_Base;
                    break;
                case UIViewLayer.HUD:
                    canvas.sortingLayerID = GameConst.SortingLayer_View_HUD;
                    break;
                case UIViewLayer.Popup:
                    canvas.sortingLayerID = GameConst.SortingLayer_View_Popup;
                    break;
                case UIViewLayer.Top:
                    canvas.sortingLayerID = GameConst.SortingLayer_View_Top;
                    break;
                case UIViewLayer.Debug:
                    canvas.sortingLayerID = GameConst.SortingLayer_View_Debug;
                    break;
                default:
                    break;
            }
        }
    }

    //被压入窗口栈中
    public virtual void OnPush()
    {
        ViewState = UIViewState.Invisible;
        UpdateLayer();
    }

    public virtual void OnShow()
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
            UpdateLayer();
        }

        if (ViewState != UIViewState.Visible)
        {
            Vector3 pos = transform.localPosition;
            pos.z = 0;
            transform.localPosition = pos;

            ViewState = UIViewState.Visible;
        }

        if (dirty)
            UpdateView();
    }

    //更新UI
    public virtual void UpdateView()
    {
        dirty = false;
    }

    //隐藏
    public virtual void OnHide()
    {
        if (ViewState == UIViewState.Visible)
        {
            Vector3 pos = transform.localPosition;
            pos.z = GameConst.Infinity;
            transform.localPosition = pos;

            ViewState = UIViewState.Invisible;
        }
    }
    
    //被移出窗口栈
    public virtual void OnPopup()
    {
        if (ViewState == UIViewState.Cache)
            return;
        
        if (ViewState == UIViewState.Visible)
            OnHide();

        ViewState = UIViewState.Cache;
    }

    //将被移除
    public virtual void OnExit()
    {
        //如果不是缓存池状态，则需要先弹出
        if (ViewState != UIViewState.Cache)
            OnPopup();
    }

    public void Close()
    {
        UIViewManager.Instance.HideView(this);
    }

    //点击背景关闭窗口的回调
    protected virtual void CloseWithEvent(BaseEventData eventData)
    {
        UIViewManager.Instance.HideView(this);
    }
}