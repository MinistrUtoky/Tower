internal interface IKillable
{
    public bool IsAlive { get; }

    public abstract void TakeHit();
    public abstract void Die();
}
