// Интерфейс обратной фабрики. Но есть нюанс
internal interface ITower : IKillable
{
    public abstract int TotalFloors { get; }

    public abstract void Add(IDroppable towerBlock);
    public abstract void SpawnRandomDroppable();
}