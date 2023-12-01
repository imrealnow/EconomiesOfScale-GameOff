using UnityEngine;

[CreateAssetMenu(fileName = "TextPopupManager", menuName = "SO/Managers/TextPopupManager")]
public class TextPopupManager : SManager
{
    public GameObject textPopupPrefab;
    public PoolManager poolManager;

    private PrefabPool popupPool;

    private void Start()
    {
        popupPool = poolManager.GetPool(textPopupPrefab);
    }

    public void ShowTextAt(string text, Vector2 position)
    {
        TextPopup textPopup = ShowText(text);
        textPopup.transform.position = position;
    }

    public void AttachTextToTransform(string text, Transform parentTransform)
    {
        ShowText(text).AttachToTransform(parentTransform);
    }

    private TextPopup ShowText(string text)
    {
        TextPopup textPopup = popupPool.GetUnusedObject().GetComponent<TextPopup>();
        textPopup.SetText(text);
        textPopup.gameObject.SetActive(true);
        return textPopup;
    }
}
