// Интерфейс продукта фабрики
using UnityEngine;

internal interface IDroppable
{
    // У любого падающего в итоге есть коллайдер. Остальное фабрике знать не обязательно.
    public abstract BoxCollider2D Collider { get; }

    // Решил что IStackable излишен и не обладает сигнатурными методами.
    public abstract void OnInit(ITower tower);
    public abstract void OnDrop();
}
