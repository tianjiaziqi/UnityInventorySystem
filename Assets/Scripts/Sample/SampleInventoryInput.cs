using UnityEngine;
using UnityEngine.InputSystem;

namespace JZQ.InventorySystem.Sample
{
    /// <summary>
    /// Reads sample inventory input actions and exposes one-frame interaction state.
    /// </summary>
    public class SampleInventoryInput : MonoBehaviour
    {
        /// <summary>
        /// Gets whether the backpack toggle was pressed this frame.
        /// </summary>
        public bool BackpackPressed { get; private set; }

        /// <summary>
        /// Gets whether rotate input was pressed this frame.
        /// </summary>
        public bool RotatePressed { get; private set; }

        /// <summary>
        /// Gets the selected quick slot index for this frame.
        /// </summary>
        public int SlotSelection { get; private set; } = -1;

        /// <summary>
        /// Gets the scroll direction value for this frame.
        /// </summary>
        public float PrevNext { get; private set; }
        private SampleInventoryInputAction inputAction;

        private void Awake()
        {
            inputAction = new SampleInventoryInputAction();
        }

        private void OnEnable()
        {
            inputAction.Enable();
            RegisterEvents();
        }

        private void OnDisable()
        {
            UnregisterEvents();
            inputAction.Disable();
        }

        private void RegisterEvents()
        {
            UnregisterEvents();
            inputAction.Inventory.Switch.performed += OnBackPackPressed;
            inputAction.Inventory.Rotate.performed += OnRotatePressed;
            inputAction.Inventory.BackpackSlot1.performed += OnSlot1Selected;
            inputAction.Inventory.BackpackSlot2.performed += OnSlot2Selected;
            inputAction.Inventory.BackpackSlot3.performed += OnSlot3Selected;
            inputAction.Inventory.BackpackSlot4.performed += OnSlot4Selected;
            inputAction.Inventory.BackpackSlot5.performed += OnSlot5Selected;
            inputAction.Inventory.BackpackSlot6.performed += OnSlot6Selected;
            inputAction.Inventory.BackpackSlot7.performed += OnSlot7Selected;
            inputAction.Inventory.BackpackSlot8.performed += OnSlot8Selected;
            inputAction.Inventory.BackpackSlot9.performed += OnSlot9Selected;
            inputAction.Inventory.BackpackSlot10.performed += OnSlot10Selected;
            inputAction.Inventory.PrevNext.performed += OnPrevNext;
        }

        private void UnregisterEvents()
        {
            inputAction.Inventory.Switch.performed -= OnBackPackPressed;
            inputAction.Inventory.Rotate.performed -= OnRotatePressed;
            inputAction.Inventory.BackpackSlot1.performed -= OnSlot1Selected;
            inputAction.Inventory.BackpackSlot2.performed -= OnSlot2Selected;
            inputAction.Inventory.BackpackSlot3.performed -= OnSlot3Selected;
            inputAction.Inventory.BackpackSlot4.performed -= OnSlot4Selected;
            inputAction.Inventory.BackpackSlot5.performed -= OnSlot5Selected;
            inputAction.Inventory.BackpackSlot6.performed -= OnSlot6Selected;
            inputAction.Inventory.BackpackSlot7.performed -= OnSlot7Selected;
            inputAction.Inventory.BackpackSlot8.performed -= OnSlot8Selected;
            inputAction.Inventory.BackpackSlot9.performed -= OnSlot9Selected;
            inputAction.Inventory.BackpackSlot10.performed -= OnSlot10Selected;
            inputAction.Inventory.PrevNext.performed -= OnPrevNext;
        }

        private void OnBackPackPressed(InputAction.CallbackContext ctx)
        {
            BackpackPressed = true;
        }

        private void OnRotatePressed(InputAction.CallbackContext ctx)
        {
            RotatePressed = true;
        }

        private void OnSlot1Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 0;
        }

        private void OnSlot2Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 1;
        }

        private void OnSlot3Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 2;
        }

        private void OnSlot4Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 3;
        }

        private void OnSlot5Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 4;
        }

        private void OnSlot6Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 5;
        }

        private void OnSlot7Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 6;
        }

        private void OnSlot8Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 7;
        }

        private void OnSlot9Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 8;
        }

        private void OnSlot10Selected(InputAction.CallbackContext ctx)
        {
            SlotSelection = 9;
        }

        private void OnPrevNext(InputAction.CallbackContext ctx)
        {
            PrevNext = ctx.ReadValue<Vector2>().y;
        }

        private void ClearOneFrameInputs()
        {
            BackpackPressed = false;
            RotatePressed = false;
            SlotSelection = -1;
            PrevNext = 0f;
        }

        private void LateUpdate()
        {
            ClearOneFrameInputs();
        }
    }
}
