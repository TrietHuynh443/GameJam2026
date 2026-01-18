using System;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Image))]
    public class SplashImage: MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Image _image;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _image = GetComponent<Image>();

        }

        private void Update()
        {
            _image.sprite = _renderer.sprite;
        }
    }
}