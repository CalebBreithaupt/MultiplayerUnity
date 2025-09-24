using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using GDD4500.LAB01;

namespace GDD4500.LAB01
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private Material[] _PlayerMaterials;
        [SerializeField] private Material _GhostMaterial;
        private InputAction _move;
        private InputAction _shoot;
        private InputAction _parry;

        private bool _initialized;

        private PlayerMoveMechanic _moveMechanic;
        private PlayerShootMechanic _shootMechanic;
        private PlayerParryMechanic _parryMechanic;
        private PlayerInputContext _context;

        private bool _isShootAllowed = true;
        public bool _lockShooting = false;
        public bool IsShootAllowed
        {
            get => _isShootAllowed;
            set => _isShootAllowed = value;
        }

        private bool _isParryAllowed = true;
        public bool IsParryAllowed
        {
            get => _isParryAllowed;
            set => _isParryAllowed = value;
        }

        public void Initialize(PlayerInputContext context)
        {
            _context = context;
            this.name = $"Player {context.Index + 1}";
            
            // Enforce listening only to this scheme (extra safety; the manager already did ActivateControlScheme).
            context.Actions.bindingMask = InputBinding.MaskByGroup(context.SchemeName);
            var gameplayMap = context.Actions.FindActionMap("Gameplay", true);

            //Store the actions 
            _move = gameplayMap.FindAction("Move", true);
            _shoot = gameplayMap.FindAction("Shoot", true);
            _parry = gameplayMap.FindAction("Parry", true);

            // For simple button action subscription, don't forget to unsubscribe in OnDestroy!
            _shoot.performed += Shoot_Performed;

            _parry.performed += Parry_Performed;

            _initialized = true;

            // Enable if not already enabled by the manager.
            // gameplay.Enable();

            Debug.Log($"[Handler] Scheme = {context.SchemeName}");

            // Grab and init move
            _moveMechanic = GetComponent<PlayerMoveMechanic>();
            if (_moveMechanic != null) _moveMechanic.Initialize(this);

            // Grab and init shoot
            _shootMechanic = GetComponent<PlayerShootMechanic>();
             if (_shootMechanic != null)_shootMechanic.Initialize(this);

            // Grab and init parry
            _parryMechanic = GetComponent<PlayerParryMechanic>();
            if (_parryMechanic != null)_parryMechanic.Initialize(this);

            // Make the player persist between scenes
            DontDestroyOnLoad(this.gameObject);

            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = _PlayerMaterials[_context.Index];
            }
        }

        private void Update()
        {
            if (!_initialized) return;

            // Reads the move value (of Vector 2 type)
            Vector2 move = _move.ReadValue<Vector2>();

            // Reading the magnitude is relatively expensive so this logic should be optimized
            if (_moveMechanic != null && move.magnitude > 0)
            {
                _moveMechanic.DoMove(move);
            }
        }

        private void Shoot_Performed(InputAction.CallbackContext context)
        {
            if (!_isShootAllowed && _lockShooting || SceneManager.GetActiveScene().name != "Game") return;

            bool isPressed = context.ReadValueAsButton();   // true on press, false on release
            if (isPressed)
            {
                _shootMechanic.OnCharge();
            }
            else
            {
                _shootMechanic.OnRelease();
            }

        }

        private void Parry_Performed(InputAction.CallbackContext context)
        {
            if (!_isParryAllowed) return;

            bool isPressed = context.ReadValueAsButton();   // true on press, false on release
            if (isPressed)
            {
                _parryMechanic.OnCharge();
            }
        }

        private void OnBulletHit(GameObject bullet)
        {
            if (gameObject.tag == "Parry") return;
            _isShootAllowed = false;
            
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            gameObject.tag = "Dead";
            GameManager.Instance.OnGameCompleation(gameObject);
            BulletHandler[] bullets = FindObjectsOfType<BulletHandler>();
            foreach (BulletHandler b in bullets)
            {
                Destroy(b.gameObject);
            }
            PlayerInputHandler[] players = FindObjectsOfType<PlayerInputHandler>();
            foreach (PlayerInputHandler p in players)
            {
                p._lockShooting = true;
            }
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = _GhostMaterial;
            }
        }
    }
}
