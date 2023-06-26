using System;
using Core.UiSystem;
using TMPro;
using Ui.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Views
{
    public class GameMenuView : WindowView<GameMenuWindow>
    {
        public enum ButtonType
        {
            Play,
            Exit
        }

        public event Action<ButtonType> OnClick;
        
        [SerializeField] private Button playBtn;
        [SerializeField] private Button exitBtn;

        public override void OnPop()
        {
            playBtn.onClick.AddListener(() => OnClick?.Invoke(ButtonType.Play));
            exitBtn.onClick.AddListener(() => OnClick?.Invoke(ButtonType.Exit));
        }
        
        public override void OnPush()
        {
            playBtn.onClick.RemoveAllListeners();
            exitBtn.onClick.RemoveAllListeners();
        }
    }
}