using UnityEngine;

internal interface IDroppable 
{
    public abstract BoxCollider2D Collider { get; }
    public abstract Rigidbody2D Rigidbody { get; }
    public abstract SpriteRenderer Image { get; }
    public abstract AbstractReleasable<Sprite> SpriteReleasable { get; }

    public abstract void OnInit(ITower tower);
    public abstract void OnDrop();
}
