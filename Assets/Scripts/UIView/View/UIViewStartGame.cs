using UnityEngine;
using UnityEngine.UI;

public class UIViewStartGame: UIViewBase
{
    [SerializeField] private Button btnStartGame;

    protected override void InitUIObjects()
    {
        base.InitUIObjects();
        btnStartGame.onClick.AddListener(OnStartGameClick);
    }

    private void OnStartGameClick()
    {
        BattleManager.Instance.Run();
        UIViewManager.Instance.ShowView(UIViewName.Main);
        UIViewManager.Instance.ShowView(UIViewName.TopologicalMap);
        
        UIViewManager.Instance.HideView(this);
    }
}
