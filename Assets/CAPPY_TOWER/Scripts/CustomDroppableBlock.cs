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
        // Этот блок типо летит быстрееы
        GetComponent<Rigidbody2D>().gravityScale = 10;
    }

    protected override bool CanStackOn(Collider2D other) => Mathf.Abs(transform.position.x - other.transform.position.x) 
                                                            < other.GetComponent<BoxCollider2D>().bounds.extents.x;
    

    protected override bool IsPerfect(Collider2D other) => Mathf.Abs(transform.position.x - other.transform.position.x)           
                                                            < other.GetComponent<BoxCollider2D>().bounds.extents.x * 0.35f;
    
    
    private void PlaceOnTop(Collider2D other)
    {
        AudioSingleton.Instance.PlaySfx(1, 0.5f);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        rb.transform.position = other.ClosestPoint(transform.position)
                                + new Vector2(0, col.size.y * rb.transform.localScale.y / 2f);
        
        // Они растут! Как грибы!
        rb.transform.DOScale(1.9f, 0.5f)
                            .SetEase(Ease.Linear)
                            .SetLoops(4, LoopType.Yoyo); 
    }

    // По плану добавить сюда двойные баллы для этого блока
    private void FixOnTower(Collider2D other)
    {
        Tower.Add(this);
        this.enabled = false;
    }

    private void BasicMiss()
    {
        AudioSingleton.Instance.PlaySfx(2, 0.5f);
        GetComponent<BoxCollider2D>().enabled = false;
        Tower.TakeHit();
        StartCoroutine(MissAnimation());
    }

    // А сюда еще двойные двойные баллы
    private void PlacePerfectly(Collider2D other)
    {
        AudioSingleton.Instance.PlaySfx(3, 0.5f);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.transform.position = other.transform.position
                                + new Vector3(0, GetComponent<BoxCollider2D>().size.y * rb.transform.localScale.y);

        rb.transform.DOScale(1.8f, 1f)
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
