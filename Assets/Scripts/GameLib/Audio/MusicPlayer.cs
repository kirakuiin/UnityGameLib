using UnityEngine;
using GameLib.Common;

namespace GameLib.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : PersistentMonoSingleton<MusicPlayer>
    {
        [Tooltip("音源")]
        [SerializeField]
        private AudioSource source;
        
        /// <summary>
        /// 播放音轨。
        /// </summary>
        /// <param name="clip">声音片段</param>
        /// <param name="isLooping">是否循环播放</param>
        /// <param name="restart">是否重新开始播放</param>
        public void PlayTrack(AudioClip clip, bool isLooping, bool restart = false)
        {
            if (source.isPlaying)
            {
                if (!restart && source.clip == clip) return;
                source.Stop();
            }

            source.clip = clip;
            source.loop = isLooping;
            source.time = 0;
            source.Play();
        }
    }
}