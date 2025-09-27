using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NothingAwaits.Core.UI
{
    public class MovementUI: MonoBehaviour
    {
        [SerializeField] private Image _arrowUpImage;
        [SerializeField] private Image _arrowDownImage;
        [SerializeField] private Image _arrowLeftImage;
        [SerializeField] private Image _arrowRightImage;
        [SerializeField] private TMP_Text _fText;

        private void Update()
        {
            ClearColor();
            if (Input.GetKey(KeyCode.W))
            {
                _arrowUpImage.color = Color.green;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _arrowDownImage.color = Color.green;
            }
            if (Input.GetKey(KeyCode.A))
            {
                _arrowLeftImage.color = Color.green;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _arrowRightImage.color = Color.green;
            }
        }

        private void ClearColor()
        {
            _arrowUpImage.color = Color.white;
            _arrowDownImage.color = Color.white;
            _arrowLeftImage.color = Color.white;
            _arrowRightImage.color = Color.white;
        }
    }
}