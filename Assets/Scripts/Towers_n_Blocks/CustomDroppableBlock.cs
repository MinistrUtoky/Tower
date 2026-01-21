using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

internal class CustomDroppableBlock : AbstractDroppableBlock
{
    protected new void Awake()
    {
        base.Awake();
        Assert.IsTrue(tag == "TowerBlock");
        _onStack.AddListener(PlaceOnTop);
        _onStack.AddListener(FixOnTower);

        _onPerfectMatch.AddListener(PlacePerfectly);
        _onPerfectMatch.AddListener(FixOnTower);

        _onMiss.AddListener(BasicMiss);
    }

    public override void OnDrop()
    {
        IsStacked = false;
        Rigidbody.gravityScale = 10;
    }

    protected override bool CanStackOn(Collider2D other) => Mathf.Abs(transform.position.x - other.transform.position.x) 
                                                            < other.GetComponent<BoxCollider2D>().bounds.extents.x;
    

    protected override bool IsPerfect(Collider2D other) => Mathf.Abs(transform.position.x - other.transform.position.x)           
                                                            < other.GetComponent<BoxCollider2D>().bounds.extents.x * 0.35f;
    
    
    private void PlaceOnTop(Collider2D other)
    {
        AudioSingleton.Instance.PlaySfx(1, 0.5f);
        Rigidbody.transform.position = other.ClosestPoint(transform.position)
                                + new Vector2(0, Collider.size.y * Rigidbody.transform.localScale.y / 2f);

        Rigidbody.transform.DOScale(1.9f, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetLoops(4, LoopType.Yoyo); 
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
        Rigidbody.transform.position = other.transform.position
                                + new Vector3(0, Collider.size.y * Rigidbody.transform.localScale.y);

        Rigidbody.transform.DOScale(1.8f, 1f)
                            .SetEase(Ease.Linear)
                            .SetLoops(2, LoopType.Yoyo);
    }

    private IEnumerator MissAnimation()
    {
        Image.DOFade(0, 1f);
        Image.transform.DORotate(Vector3.left * Random.Range(-90f, 90f), 1f);
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
        this.enabled = false;
    }

}
