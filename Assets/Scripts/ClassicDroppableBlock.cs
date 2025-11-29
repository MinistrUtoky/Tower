using DG.Tweening;
using System.Collections;
using UnityEngine;

internal class ClassicDroppableBlock : AbstractDroppableBlock
{
    [SerializeField]
    private ParticleSystem _perfectEffect;

    protected new void Awake()
    {
        base.Awake();
        _onStack.AddListener(PlaceOnTop);
        _onStack.AddListener(FixOnTower);

        _onPerfectMatch.AddListener(PlacePerfectly);
        _onPerfectMatch.AddListener(FixOnTower);

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
                < other.GetComponent<BoxCollider2D>().bounds.extents.x * 0.9f;
    }

    protected override bool IsPerfect(Collider2D other)
    {
        return Mathf.Abs(transform.position.x - other.transform.position.x)
                < other.GetComponent<BoxCollider2D>().bounds.extents.x * 0.25f;
    }

    // В этом типе блоков идеальный мэтч отличается от обычного только плейсментом при попадании и анимацией
    private void PlaceOnTop(Collider2D other)
    {
        AudioSingleton.Instance.PlaySfx(1, 0.5f);
        Transform t = Rigidbody.transform;
        // вспомнил, что bounds.extents определяет bounding box коллайдера, а не его собственные размерности 
        // заменил магическую константу определяющим ее размером объекта
        t.position = other.ClosestPoint(transform.position)
                                + new Vector2(0, Collider.size.y * t.localScale.y / 2f);
        // эффект отскакивания капибар при падении
        Image.transform.DOPunchPosition(Vector3.up, 1f, 2, 1f, false).SetEase(Ease.OutQuad);
    }

    private void FixOnTower(Collider2D other)
    {
        Tower.Add(this);
        this.enabled = false;
    }

    private void BasicMiss()
    {
        AudioSingleton.Instance.PlaySfx(2, 0.5f);
        Collider.enabled = false;
        Tower.TakeHit();
        Tower.SpawnRandomDroppable();
        StartCoroutine(MissAnimation());
    }

    private void PlacePerfectly(Collider2D other)
    {
        AudioSingleton.Instance.PlaySfx(3, 0.5f);
        Collider.transform.position = other.transform.position
                                + new Vector3(0, Collider.size.y * Collider.transform.localScale.y);
        _perfectEffect.Play();
    }

    private IEnumerator MissAnimation()
    {
        Image.DOFade(0, 1f);
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
        this.enabled = false;
    }
}
