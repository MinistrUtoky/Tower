using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
internal abstract class AbstractDroppableBlock : MonoBehaviour, IDroppable
{
    // компонент, содержащий изображение блока
    [SerializeField]
    private SpriteRenderer _image;

    // переделываю под Builder + DI для того, чтобы эффективно наследовать и кастомизировать
    protected UnityEvent<Collider2D> _onStack = new();
    protected UnityEvent<Collider2D> _onPerfectMatch = new();
    protected UnityEvent _onMiss  = new();

    // Таким образом мы изолируем все основные компоненты шаблона в рамках класса DroppableBlock
    // А вся кастомная функциональность отдельных блоков реализуется уже в наследниках этого класса
    protected ITower Tower { get; private set; }
    protected bool IsStacked { get; set; } = false;

    public BoxCollider2D Collider { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }

    protected SpriteRenderer Image => _image;

    protected void Awake()
    {
        Assert.IsTrue(tag == "TowerBlock");
        Collider = GetComponent<BoxCollider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
        _onStack.AddListener(FreezeBlock);
        _onPerfectMatch.AddListener(FreezeBlock);
    }

    // Все блоки пока что застывают при накладывании. Возможно в будущем переедет в какой-то специальный подтип.
    private void FreezeBlock(Collider2D other)
    {
        IsStacked = true;
        Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        Rigidbody.gravityScale = 0;
        Rigidbody.transform.rotation = other.transform.rotation;
    }

    // virtual потому что такая опция может понадобиться в случае расширения свойств спавна
    // (допустим, захочу создать тикающую бомбу поставить ей спавн таймер - зачем ветвить методы если есть этот)
    // ну а базовая имплементация универсальна - блок без башни жить не может
    public virtual void OnInit(ITower tower)
    {
        // добавил вывод ошибки
        if (Tower != null)
        {
            Debug.LogError("The block cannot be initialized twice or have two towers as it's parent!");
            return;
        }
        Image.sortingOrder = tower.TotalFloors % 32765 + 3;
        Tower = tower;
    }
    // не все падающие блоки не застаканы по дефолту и падают с гравитацией 5
    public abstract void OnDrop();

    // абстрагируем условие стака, чтобы можно было делать интересные подтипы матчей
    protected abstract bool CanStackOn(Collider2D other);
    protected abstract bool IsPerfect(Collider2D other);
    private void StackOn(Collider2D other) => _onStack.Invoke(other);

    // Зачем отдельно идеальные метчи? Для разных блоков - забавные условия идеального выполнения.
    // Это не всегда связано с попаданием. Для одних блоков идеальное условие -
    // их уничтожение; для других - попасть в башню, но не на верхний этаж; и т.д.
    // Более того не все они стакаются. Может захочется чтобы они как в мэтч-3 просто уничтожались.
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
