using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
internal class LoadingPanelDecoration : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(HideAfterLoading());
    }

    // При открытии панели засекаем время и потом запускаем анимацию скрытия
    private IEnumerator HideAfterLoading()
    {
        yield return new WaitForSeconds(TConfig.LOADING_TIME);
        GetComponent<Animator>().SetBool("HidingTime", true);
    }
}
