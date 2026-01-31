using System;
using System.Collections.Generic;
using System.Linq;
using Extension;
using Unity.VisualScripting;
using UnityCommunity.UnitySingleton;
using UnityEngine;

namespace Sound
{
    public enum SoundEffectType
    {
        [SerializeAs("Masked")] Masked,
        [SerializeAs("Angry")] Angry,
        [SerializeAs("Click")] Click
    }
    public class SoundManager : PersistentMonoSingleton<SoundManager>
    {
        private readonly string ThemeSongSourceNormal = "Theme1";
        private readonly string ThemeSongSourceFast = "Theme2";
        
        private Dictionary<string, AudioSource> _audioSources;
        private AudioClip[] _themeClips;

        private AudioClip _maskedSfx;
        private AudioClip _angrySfx;
        private AudioClip _clickSfx;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _audioSources = gameObject.GetComponents<AudioSource>().ToDictionary(x => x.outputAudioMixerGroup.name);
            _themeClips = new AudioClip[3]
            {
                Resources.Load<AudioClip>(ThemeSongSourceNormal),
                Resources.Load<AudioClip>(ThemeSongSourceFast),
                Resources.Load<AudioClip>(ThemeSongSourceFast),
            };
            _maskedSfx = Resources.Load<AudioClip>("SE/Masked");
            _angrySfx = Resources.Load<AudioClip>("SE/Angry");
            _clickSfx = Resources.Load<AudioClip>("SE/Click");
        }

        private void Start()
        {
            _audioSources["Music"].clip = _themeClips[0];
            _audioSources["Music"].loop = true;
            _audioSources["Music"].Play();
        }

        public void ChangeThemeSong(int i)
        {
            if (i <= 0 || i >= _themeClips.Length)
            {
                return;
            }

            _audioSources["Music"].clip = _themeClips[i];
            _audioSources["Music"].Play();
        }
        
        public void PlaySoundEffect(SoundEffectType se)
        {
            var clip = GetClip(se);
            if(!clip) return;
            _audioSources["SE"].clip = clip;
            _audioSources["SE"].Play();
        }

        private AudioClip GetClip(SoundEffectType se)
        {
            switch (se)
            {
                case SoundEffectType.Masked:
                    return _maskedSfx;
                case SoundEffectType.Angry:
                    return _angrySfx;
                case SoundEffectType.Click:
                    return _clickSfx;
            }

            return null;
        }
    }

}
