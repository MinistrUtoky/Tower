using DG.Tweening;
using System.Collections;
using UnityEngine;

internal class BombDroppableBlock : AbstractDroppableBlock
{
    [SerializeField]
    private ParticleSystem _perfectEffect;

    protected new void Awake()
    {
        base.Awake();
        _onStack.AddListener(Boom);

        _onPerfectMatch.AddListener(PerfectBoom);

        _onMiss.AddListener(BasicMiss);
    }

    public override void OnDrop()
    {
        IsStacked = false;
        Rigidbody.gravityScale = 5;
    }

    protected override bool CanStackOn(Collider2D other)
    {
        return Mathf.Abs(transform.position.x - other.transform.position.x)
                < other.GetComponent<BoxCollider2D>().bounds.extents.x * 0.5f;
    }

    protected override bool IsPerfect(Collider2D other)
    {
        return Mathf.Abs(transform.position.x - other.transform.position.x)
                < other.GetComponent<BoxCollider2D>().bounds.extents.x * 0.3f;
    }

    private void Boom(Collider2D other)
    {
        if (Tower.TotalFloors > 0)
        {
            AudioSingleton.Instance.PlaySfx(1, 0.5f);
            Tower.TakeHit();
            Tower.SpawnRandomDroppable();

            Tower.RemoveTopmost();

            Destroy(transform.parent.gameObject);
            this.enabled = false;
        }
        else
            BasicMiss();
    }
   
    private void BasicMiss()
    {
        AudioSingleton.Instance.PlaySfx(2, 0.5f);
        Collider.enabled = false;
        Tower.SpawnRandomDroppable();
        StartCoroutine(MissAnimation());
    }

    private void PerfectBoom(Collider2D other)
    {
        if (Tower.TotalFloors > 1)
        {
            AudioSingleton.Instance.PlaySfx(3, 0.5f);
            _perfectEffect.Play();
            Tower.TakeHit();
            Tower.TakeHit();
            Tower.SpawnRandomDroppable();

            Tower.RemoveTopmost();
            Tower.RemoveTopmost();

            Destroy(transform.parent.gameObject);
            this.enabled = false;
        }
        else if (Tower.TotalFloors == 1)
            Boom(other);
        else
            BasicMiss();
    }

    private IEnumerator MissAnimation()
    {
        Image.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
        this.enabled = false;
    }
}
