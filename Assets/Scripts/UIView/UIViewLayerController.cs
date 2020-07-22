using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UIViewLayerController: MonoBehaviour, IGameBase
{
    public UIViewLayer ViewLayer;
    //每一个界面的Order间隔
    private const int viewOrderStrp = 100;
    //最上层的Order值
    private int topOrder = 0;
    
    //保存这一层的窗口列表
    private List<UIViewBase> views = new List<UIViewBase>();

    //压入一个新的窗口（设置为最大的order)
    public void Push(UIViewBase view)
    {
        if (view.LayerController != null)
        {
            //本来就在这个队列中
            if (view.ViewOrder == topOrder)
                return;
            else
            {
                views.Remove(view);
                views.Add(view);
                topOrder += viewOrderStrp;
                view.ViewOrder = topOrder;
            }
        }
        else
        {
            views.Add(view);
            topOrder += viewOrderStrp;
            PushSingleView(view);
        }
    }
    
    //弹出一个指定的窗口
    public void Popup(UIViewBase view)
    {
        if (view == null)
            return;

        bool error = true;
        for (int i = views.Count - 1; i >= 0; i--)
        {
            if (views[i].GetInstanceID() == view.GetInstanceID())
            {
                views.RemoveAt(i);
                PopupSingleView(view);
                error = false;
                break;
            }
        }

        if (error)
        {
            Debug.LogError($"弹出失败，{ViewLayer}中没有{view.Config.viewName}");
            return;
        }

        RefreshTopOrder();
    }

    public UIViewBase[] PopupAll()
    {
        if (views.Count == 0)
            return null;

        UIViewBase current = null;
        UIViewBase[] allViews = views.ToArray();
        for (int i = views.Count - 1; i >= 0; i--)
        {
            current = views[i];
            views.RemoveAt(i);
            PopupSingleView(current);
        }

        topOrder = 0;
        return allViews;
    }
    
    //刷新界面的显示
    public bool RefreshView(bool alreadyCovered)
    {
        //如果已经覆盖了全部的屏幕
        if (alreadyCovered)
        {
            for (int i = views.Count - 1; i >= 0; i--)
            {
                if (views[i].Config.alwaysUpdate)
                    views[i].OnShow();
                else
                    views[i].OnHide();
            }

            return true;
        }
        else
        {
            bool covered = false;
            for (int i = views.Count - 1; i >= 0; --i)
            {
                if (views[i].Config.alwaysUpdate)
                    views[i].OnShow();
                else
                {
                    if (covered)
                        views[i].OnHide();
                    else
                        views[i].OnShow();
                }

                if (!covered)
                    covered = views[i].Config.coverScreen;
            }
            return covered;
        }
    }
    
    //压入单个界面
    private void PushSingleView(UIViewBase view)
    {
        if (view != null)
        {
            view.LayerController = this;
            view.OnPush();
            view.ViewOrder = topOrder;
        }
    }
    
    //弹出单个界面
    private void PopupSingleView(UIViewBase view)
    {
        if (view != null)
        {
            view.ViewOrder = 0;
            view.LayerController = null;
            view.OnPopup();
        }
    }
    
    //刷新最大的order
    private void RefreshTopOrder()
    {
        if (views.Count == 0)
            topOrder = 0;
        else
            topOrder = views[views.Count - 1].ViewOrder;
    }
    
    public void Init(params object[] args)
    {
        UIViewManager.Instance.MgrLog($"{Desc()}初始化{ViewLayer}");
    }

    public string Desc()
    {
        return "UIViewLayerController";
    }
}
