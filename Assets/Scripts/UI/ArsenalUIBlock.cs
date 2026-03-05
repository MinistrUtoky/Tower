using Arsenal;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ArsenalUIBlock : MonoBehaviour
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TMP_Text _nameText;
    // Temporary just probabilities. Later to be changed for Bar or Line Chart.
    [SerializeField]
    private TMP_Text _probabilityCurve;

    public Sprite Icon { get; private set; }
    public string Name { get; private set; }
    public float[] PerTurnProbabilities { get; private set; }

    public void Init(ArsenalManager parenalManager, Sprite icon, string name, float[] perTurnProbabilities)
    {
        Icon = icon;
        Name = name;
        PerTurnProbabilities = perTurnProbabilities;

        GetComponent<Button>().onClick.AddListener(() => parenalManager.SwitchInArsenal(this));
        _icon.sprite = icon;
        if (!_nameText) return;
        _nameText.text = name;
        if (!_probabilityCurve) return;
        StringBuilder sb = new StringBuilder();
        foreach (float p in perTurnProbabilities)
        {
            sb.Append(p); sb.Append(' ');
        }
        _probabilityCurve.text = sb.ToString();
    }
}
