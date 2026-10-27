using TMPro;
using UnityEngine;
using Zenject;

public sealed class DeathCounterView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI preyCounter;
    [SerializeField] private TextMeshProUGUI predatorCounter;

    private AnimalStatistics _statistics;

    [Inject]
    private void Construct(AnimalStatistics statistics)
    {
        _statistics = statistics;
        _statistics.Changed += Refresh;
        Refresh();
    }

    private void Refresh()
    {
        if (preyCounter != null)
            preyCounter.text = $"Dead prey: {_statistics.DeadPrey}";

        if (predatorCounter != null)
            predatorCounter.text = $"Dead predators: {_statistics.DeadPredators}";
    }

    private void OnDestroy()
    {
        if (_statistics != null)
            _statistics.Changed -= Refresh;
    }
}