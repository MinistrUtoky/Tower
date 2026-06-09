using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
internal abstract class AbstractDroppableBlock : MonoBehaviour, IDroppable
{
    [SerializeField]
    private SpriteRenderer _image;

    protected UnityEvent<Collider2D> _onStack = new();
    protected UnityEvent<Collider2D> _onPerfectMatch = new();
    protected UnityEvent _onMiss  = new();

    protected ITower Tower { get; private set; }
    protected bool IsStacked { get; set; } = false;

    public BoxCollider2D Collider { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    public AbstractReleasable<Sprite> SpriteReleasable { get; private set; } = new();

    public SpriteRenderer Image => _image;

    protected void Awake()
    {
        Assert.IsTrue(tag == "TowerBlock");
        Collider = GetComponent<BoxCollider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
        _onStack.AddListener(FreezeBlock);
        _onPerfectMatch.AddListener(FreezeBlock);
    }

    private void FreezeBlock(Collider2D other)
    {
        IsStacked = true;
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        Rigidbody.gravityScale = 0;
        Rigidbody.transform.rotation = other.transform.rotation;
    }

    public virtual void OnInit(ITower tower)
    {
        if (Tower != null)
        {
            Debug.LogError("The block cannot be initialized twice or have two towers as it's parent!");
            return;
        }
        if (InterplayData.Location.ReverseOverlap)
            Image.sortingOrder = (32699 - tower.TotalFloors) % 32700 + 3;
        else
            Image.sortingOrder = tower.TotalFloors % 32700 + 3;
        Tower = tower;
    }
    public abstract void OnDrop();

    protected abstract bool CanStackOn(Collider2D other);
    protected abstract bool IsPerfect(Collider2D other);
    private void StackOn(Collider2D other) => _onStack.Invoke(other);
    private void PerfectMatch(Collider2D other) => _onPerfectMatch.Invoke(other);
    private void Miss() => _onMiss.Invoke();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsStacked) 
            return;
        if (collision.gameObject.tag == "Death")
        {
            Miss();
        }
        else if (collision.gameObject.tag == "TowerBlock")
        {
            if (IsPerfect(collision.collider))
                PerfectMatch(collision.collider);
            else if (CanStackOn(collision.collider))
                StackOn(collision.collider);
            else
                Miss();
        }
    }
}
