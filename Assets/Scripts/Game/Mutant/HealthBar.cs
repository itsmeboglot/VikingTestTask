using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Mutant
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }
        
        private void LateUpdate()
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                _mainCamera.transform.rotation * Vector3.up);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void UpdateBar(float value = 1f)
        {
            fillImage.DOFillAmount(value, 0.5f);
        }
    }
}