using System.Collections.Generic;
using UnityEngine;

// Еще больше абстракции и точности описания
internal abstract class AbstractDroppableFactory : MonoBehaviour, ITower
{
    // В любой башне должны быть текущий предмет, собранные предметы, их число, сломалась ли башня
    private IDroppable _current;
    private Queue<IDroppable> _tower;
    public int TotalFloors { get; private set; } = 0;
    public bool IsAlive { get; private set; }

    protected IDroppable Current => _current;

    protected void Start()
    {
        _tower = new Queue<IDroppable>();
    }

    public virtual void Add(IDroppable towerBlock)
    {
        _tower.Enqueue(towerBlock);
        TotalFloors++;
        if (_tower.Count > TConfig.MAX_HEIGHT)
        {
            Destroy(_tower.Dequeue().Collider.transform.parent.gameObject);
        }
        SpawnRandomDroppable();
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
    }

    protected virtual void DropCurrent()
    {
        IDroppable current = Current;
        _current = null;
        current.OnDrop();
    }
}
