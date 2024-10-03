using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Scripts
{
    public class TouchManager : MonoBehaviour
    {
        public InputActionReference DragAction;

        private HandleMovementSystem _handleMovementSystem;

        private void Start()
        {
            _handleMovementSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<HandleMovementSystem>();
        }

        private void OnEnable()
        {
            DragAction.action.performed += OnDragPerformed;
        }

        private void OnDisable()
        {
            DragAction.action.performed -= OnDragPerformed;
        }

        private void OnDragPerformed(InputAction.CallbackContext context)
        {
            _handleMovementSystem.TouchDelta = context.ReadValue<Vector2>();
        }
    }
}
