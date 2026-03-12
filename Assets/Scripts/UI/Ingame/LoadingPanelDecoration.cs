using System.Collections;
using UnityEngine;

namespace Tower
{
    [RequireComponent(typeof(Animator))]
    internal class LoadingPanelDecoration : MonoBehaviour
    {
        private void Awake()
        {
            StartCoroutine(HideAfterLoading());
        }

        private IEnumerator HideAfterLoading()
        {
            yield return new WaitForSeconds(TConfig.LOADING_TIME);
            GetComponent<Animator>().SetBool("HidingTime", true);
        }
    }
}