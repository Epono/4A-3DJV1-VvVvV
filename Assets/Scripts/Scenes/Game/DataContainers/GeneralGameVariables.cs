using UnityEngine;
using System.Collections;

[System.Serializable]
public class GeneralGameVariables {
    [SerializeField]
    float _gameDuration = 180.0f;

    public float GameDuration {
        get { return _gameDuration; }
        set { _gameDuration = value; }
    }

    [SerializeField]
    float _coinSelectionRadius = 10;

    public float CoinSelectionRadius {
        get { return _coinSelectionRadius; }
        set { _coinSelectionRadius = value; }
    }

    [SerializeField]
    float _turnDuration = 20.0f;

    public float TurnDuration {
        get { return _turnDuration; }
        set { _turnDuration = value; }
    }

    [SerializeField]
    float _simulationDuration = 5.0f;

    public float SimulationDuration {
        get { return _simulationDuration; }
        set { _simulationDuration = value; }
    }

    [SerializeField]
    float _intervalBetweenRPCs = 0.1f;

    public float IntervalBetweenRPCs {
        get { return _intervalBetweenRPCs; }
        set { _intervalBetweenRPCs = value; }
    }

    [SerializeField]
    AudioClip _backgroundMusic;

    public AudioClip BackgroundMusic {
        get { return _backgroundMusic; }
        set { _backgroundMusic = value; }
    }

    [SerializeField]
    GameObject _trapPrefab;

    public GameObject TrapPrefab {
        get { return _trapPrefab; }
        set { _trapPrefab = value; }
    }

    [SerializeField]
    int _trapCoinsLostCount = 5;

    public int TrapCoinsLostCount {
        get { return _trapCoinsLostCount; }
        set { _trapCoinsLostCount = value; }
    }

    [SerializeField]
    Material _trapRadiusMaterial;

    public Material TrapRadiusMaterial {
        get { return _trapRadiusMaterial; }
        set { _trapRadiusMaterial = value; }
    }

    [SerializeField]
    Material _pathMaterial;

    public Material PathMaterial {
        get { return _pathMaterial; }
        set { _pathMaterial = value; }
    }

    [SerializeField]
    Material _coinSelectionMateriel;

    public Material CoinSelectionMateriel {
        get { return _coinSelectionMateriel; }
        set { _coinSelectionMateriel = value; }
    }
};
