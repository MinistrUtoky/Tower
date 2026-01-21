using System.Collections.Generic;
using UnityEngine;

internal abstract class AbstractDroppableFactory : MonoBehaviour, ITower
{
    private IDroppable _current;
    private readonly LinkedList<IDroppable> _tower = new LinkedList<IDroppable>();
    public int TotalFloors { get; private set; } = 0;
    public bool IsAlive { get; private set; } = true;
    public bool IsHanging { get; private set; } = false;

    protected IDroppable Current => _current;

    protected void Start()
    {
        IsAlive = true;
    }

    public virtual void Add(IDroppable towerBlock)
    {
        _tower.AddLast(towerBlock);
        TotalFloors++;
        if (_tower.Count > TConfig.MAX_HEIGHT)
        {
            IDroppable toRemove = _tower.First.Value;
            _tower.RemoveFirst();
            Destroy(toRemove.Collider.transform.parent.gameObject);
        }
        SpawnRandomDroppable();
    }

    public virtual void RemoveTopmost()
    {
        if (TotalFloors > 0 & _tower.Count > 0)
        {
            TotalFloors--;
            IDroppable toRemove = _tower.Last.Value; 
            _tower.RemoveLast();
            Destroy(toRemove.Collider.transform.parent.gameObject);
        }
        else
            Debug.LogWarning("Trying to DESTROY, but alas - no blocks in Tower");
    }

    public abstract void TakeHit();

    public virtual void Die()
    {
        IsAlive = false;
    }

    public abstract void SpawnRandomDroppable();

    protected virtual void SpawnDroppable(IDroppable droppable)
    {
        droppable.OnInit(this);
        _current = droppable;
        IsHanging = true;
    }

    protected void DropCurrent()
    {
        IsHanging = false;
        _current.OnDrop();
    }
}
