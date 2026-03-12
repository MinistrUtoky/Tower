using UnityEngine;

public abstract class AbstractPanelManager : MonoBehaviour
{
    [SerializeField]
    private Transform _panel;
    protected void SetActive(GameObject openable, bool isActive)
    {
        AudioSingleton.Instance.PlaySfx(0, 0.5f);
        openable.SetActive(isActive);
    }
    public void OpenPanel() => SetActive(_panel.gameObject, true);
    public void ClosePanel() => SetActive(_panel.gameObject, false);
}
