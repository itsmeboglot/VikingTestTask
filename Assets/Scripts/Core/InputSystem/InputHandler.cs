using Core.Signals;
using UnityEngine;
using Zenject;

namespace Core.InputSystem
{
    public class InputHandler : ITickable, ILateTickable 
    {
        private readonly SignalBus _signalBus;
        private readonly MovementSignal _movementSignal = new ();
        private readonly LookSignal _lookSignal = new ();
        
        private const float MouseSensitivity = 130f;
        
        public InputHandler(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Tick()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            
            _movementSignal.Input = new Vector2(horizontalInput, verticalInput);
            _signalBus.Fire(_movementSignal);
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _signalBus.Fire<AttackSignal>();
            }
        }

        public void LateTick()
        {
            float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
            
            _lookSignal.Input = new Vector2(mouseX, -mouseY);
            
            _signalBus.Fire(_lookSignal);
        }
    }
}
