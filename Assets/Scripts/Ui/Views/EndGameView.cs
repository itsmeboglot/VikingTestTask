using System;
using Core.UiSystem;
using TMPro;
using Ui.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Views
{
    public class EndGameView : WindowView<EndGameWindow>
    {
        public enum ButtonType
        {
            Restart,
            Exit
        }

        public event Action<ButtonType> OnClick;
        
        [SerializeField] private TMP_Text scoreTmp;
        [SerializeField] private Button restartBtn;
        [SerializeField] private Button exitBtn;

        public override void OnPop()
        {
            restartBtn.onClick.AddListener(() => OnClick?.Invoke(ButtonType.Restart));
            exitBtn.onClick.AddListener(() => OnClick?.Invoke(ButtonType.Exit));
        }
        
        public override void OnPush()
        {
            restartBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
        }

        public void SetScore(int value)
        {
            scoreTmp.text = value.ToString();
        }
    }
}