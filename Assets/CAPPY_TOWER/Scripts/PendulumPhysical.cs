using DG.Tweening;
using System.Collections;
using UnityEngine;

internal class PendulumPhysical : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rb;

    [SerializeField]
    private float _speed;
    [SerializeField, Range(1, 179)]
    private float _maxAngle;

    private Tweener _pendulumTweenner;
    private float _currentTweenerCoef = 1f;

    private float _swingTime = 2f;

    private void Start()
    {
        _pendulumTweenner = _rb.transform.DOLocalRotate(new Vector3(0, 0, _maxAngle), _swingTime)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        AudioSingleton.Instance.ChangeLoopedPitch(_speed / 15000f * _currentTweenerCoef);
        StartCoroutine(StartWhooshesDelayed(_swingTime/3f));
    }

    private static IEnumerator StartWhooshesDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSingleton.Instance.PlaySfxLooped(4, 0.5f);
    }

    public void SpeedUpPendulum()
    {
        _currentTweenerCoef += 0.02f;
        if (_currentTweenerCoef > 2f) _currentTweenerCoef = 2f;
        _pendulumTweenner.timeScale = _currentTweenerCoef;
        AudioSingleton.Instance.ChangeLoopedPitch(_speed / 15000f * _currentTweenerCoef);
    }
}
