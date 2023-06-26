using Core.UiSystem;
using DG.Tweening;
using TMPro;
using Ui.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Views
{
    public class PlayerHudView : WindowView<PlayerHudWindow>
    {
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TMP_Text scoreTmp;

        public void UpdateHealthBar(float value)
        {
            healthBarFill.DOFillAmount(value, 0.5f);
        }
        
        public void UpdateScore(int value)
        {
            scoreTmp.text = value.ToString();
        }
    }
}