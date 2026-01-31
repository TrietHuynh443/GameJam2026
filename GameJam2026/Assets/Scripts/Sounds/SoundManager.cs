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
    }
    public class SoundManager : PersistentMonoSingleton<SoundManager>
    {
        private readonly string ThemeSongSourceNormal = "Theme1";
        private readonly string ThemeSongSourceFast = "Theme2";
        
        private Dictionary<string, AudioSource> _audioSources;
        private AudioClip[] _themeClips;

        private AudioClip _maskedSfx;
    
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
        }

        private void Start()
        {
            _audioSources["Music"].clip = _themeClips[1];
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
            _audioSources["SE"].clip = _maskedSfx;
            _audioSources["SE"].Play();
        }
    }

}
