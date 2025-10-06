using StarterAssets;
using UnityEngine;

namespace NothingAwaits.Core.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private ThirdPersonController _playerController;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private StarterAssetsInputs _playerInputs;

        private bool _playerControllerWasEnabled;
        private bool _playerAnimatorWasEnabled;
        private bool _wasCursorLocked;
        private CursorLockMode _previousCursorLockMode;

        public void DisablePlayerControl()
        {
            _wasCursorLocked = Cursor.lockState == CursorLockMode.Locked;
            _previousCursorLockMode = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (_playerController != null)
            {
                _playerControllerWasEnabled = _playerController.enabled;
                _playerController.enabled = false;
            }

            if (_playerAnimator != null)
            {
                _playerAnimatorWasEnabled = _playerAnimator.enabled;
                _playerAnimator.enabled = false;
            }
            
            if (_playerInputs != null)
            {
                _playerInputs.cursorInputForLook = false;
                _playerInputs.cursorLocked = false;
                _playerInputs.move = Vector2.zero;
                _playerInputs.jump = false;
                _playerInputs.sprint = false;
            }
        }

        public void EnablePlayerControl(bool immediateCursorLock = false)
        {
            if (immediateCursorLock)
            {
                _wasCursorLocked = true;
            }

            if (_wasCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = _previousCursorLockMode;
                Cursor.visible = true;
            }
            
            if (_playerController != null)
            {
                _playerController.enabled = _playerControllerWasEnabled;
            }
            
            if (_playerAnimator != null)
            {
                _playerAnimator.enabled = _playerAnimatorWasEnabled;
            }
            
            if (_playerInputs != null)
            {
                _playerInputs.cursorInputForLook = true;
                _playerInputs.cursorLocked = _wasCursorLocked;
                
                _playerInputs.move = Vector2.zero;
                _playerInputs.jump = false;
                _playerInputs.sprint = false;
            }
        }
    }
}