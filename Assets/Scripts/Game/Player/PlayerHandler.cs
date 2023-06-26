using System;
using Cinemachine;
using UnityEngine;
using Zenject;

namespace Game.Player
{
    public class PlayerHandler : MonoBehaviour, IPlayerHandler, IPlayerInfo
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private CinemachineVirtualCamera playerVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera dollyCamera;
        [SerializeField] private int playerHealth;
        
        [Inject] private SignalBus _signalBus;
        
        public Transform Transform { get; private set; }
        public int MaxHealth => playerHealth;
        public bool IsDead => _player.IsDead;

        private PlayerController _player;

        private void Start()
        {
            playerVirtualCamera.gameObject.SetActive(false);
            dollyCamera.gameObject.SetActive(true);
        }

        public void SpawnPlayer()
        {
            if (_player == null)
            {
                var playerView = Instantiate(playerPrefab);
                _player = new PlayerController(_signalBus, playerView);
                Transform = playerView.transform;
                _player.Initialize(playerHealth);

                playerVirtualCamera.Follow = playerView.CameraTarget;
            }
            
            _player.Respawn(playerSpawnPoint);
            playerVirtualCamera.gameObject.SetActive(true);
            dollyCamera.gameObject.SetActive(false);
        }
    }
}