using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using GDD4500.LAB01;

namespace GDD4500.LAB01
{
    public class RematchButton : MonoBehaviour
    {

        private List<PlayerInputHandler> _existingPlayers;

        [SerializeField] private string _GameplaySceneName = "Game";
        [SerializeField] private Collider _startTrigger;
        [SerializeField] private TextMeshPro _playerCountText;

        bool _matchStarted = false;

        private void Start()
        {
            _existingPlayers = PlayerManager.Instance.GetPlayers();

            PlayerManager.Instance.OnPlayerJoined += OnPlayerJoined;
        }

        private void OnDestroy()
        {
            PlayerManager.Instance.OnPlayerJoined -= OnPlayerJoined;
        }

        private void OnPlayerJoined(PlayerInputContext ctx)
        {
            _existingPlayers.Add(ctx.Handler);

            Debug.Log($"Player {ctx.Index + 1} joined lobby");
        }

        private void Update()
        {
            if (_matchStarted) return;
            
            // Count the number of players in the start trigger
            int playersInTrigger = 0;
            foreach (var player in _existingPlayers)
            {
                if (_startTrigger.bounds.Contains(player.transform.position))
                {
                    playersInTrigger++;
                }
            }

            // If all players are in the start trigger, start the match
            if (_existingPlayers.Count > 0 && playersInTrigger == _existingPlayers.Count)
            {
                StartCoroutine(StartMatch());
            }

            UpdatePlayerCountUI(playersInTrigger);
        }

        private void UpdatePlayerCountUI(int playersInTrigger)
        {
            // Update the player count text
            _playerCountText.text = $"Rematch: {playersInTrigger}/{_existingPlayers.Count}";
        }

        private IEnumerator StartMatch()
        {
            PlayerInputHandler[] players = FindObjectsOfType<PlayerInputHandler>();

            foreach (PlayerInputHandler p in players)
            {
                p._lockShooting = false;
                p.gameObject.tag = "Player";

                MeshRenderer[] renderers = p.GetComponentsInChildren<MeshRenderer>();

                Material mat = Resources.Load<Material>("Materials/" + p.gameObject.name);
                if (mat == null)
                {
                    Debug.LogWarning("Material not found: " + p.gameObject.name);
                    continue;
                }

                foreach (MeshRenderer mr in renderers)
                {
                    mr.material = mat;
                }
            }

            yield return new WaitForSeconds(0.2f);

            GameManager.Instance.Rematch();
        }
    }
}
