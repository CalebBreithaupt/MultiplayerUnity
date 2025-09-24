using GDD4500.LAB01;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerParryMechanic : MonoBehaviour
{
    [SerializeField] Material _parryMaterial;
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _playerPrefab;
    private Material _currentMaterial;
    private PlayerInputHandler _inputHandler;
    [SerializeField] float _parryTime = 0;
    private bool _hasFired = false;
    private bool _isPlaying = false;

    public void Initialize(PlayerInputHandler playerInputHandler)
    {
        _inputHandler = playerInputHandler;
    }

    private void Update()
    {
        if (_parryTime > 0) 
        {
            _parryTime -= Time.deltaTime;

            if (_parryTime <= 0f && !_hasFired)
            {
                _hasFired = true;
                OnRelease();
            }
        }

    }

    public void OnCharge()
    {
        if (!_isPlaying && _playerPrefab.tag != "Dead")
        {
            _isPlaying = true;
            _playerPrefab.tag = "Parry";
            _currentMaterial = _player.GetComponent<MeshRenderer>().material;
            _player.GetComponent<MeshRenderer>().material = _parryMaterial;
            _parryTime = 1;
            _hasFired = false;
        }
    }

    public void OnRelease()
    {
        _playerPrefab.tag = "Player";
        if (_currentMaterial != null)
        {
            _player.GetComponent<MeshRenderer>().material = _currentMaterial;
        }
        _isPlaying = false;
    }
}
