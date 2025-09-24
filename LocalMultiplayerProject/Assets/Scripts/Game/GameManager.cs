using TMPro;
using UnityEngine;
using GDD4500.LAB01;
using NUnit.Framework.Internal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject _rematch;
    [SerializeField] GameObject _lobby;
    [SerializeField] GameObject _rematchText;
    [SerializeField] GameObject _lobbyText;
    [SerializeField] TextMeshPro _WinningText;
    [SerializeField] GameObject _WinningObject;
    private void Awake()
    {
        // Singleton boilerplate
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameCompleation(GameObject Losser)
    {
        PlayerInputHandler[] players = FindObjectsOfType<PlayerInputHandler>();
        foreach (PlayerInputHandler p in players)
        {
            p._lockShooting = true;
        }
        _rematch.SetActive(true); _lobby.SetActive(true);
        _rematchText.SetActive(true); _lobbyText.SetActive(true);
        _WinningText.text = Losser.name + " Losses";
        _WinningObject.SetActive(true);
    }

    public void Rematch()
    {
        _rematch.SetActive(false); _lobby.SetActive(false);
        _rematchText.SetActive(false); _lobbyText.SetActive(false);
        _WinningObject.SetActive(false);
    }
}
