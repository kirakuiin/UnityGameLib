using UnityEngine;

namespace GameLib.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
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
                source.time = 0;
            }

            source.clip = clip;
            source.loop = isLooping;
            source.Play();
        }

        /// <summary>
        /// 停止播放。
        /// </summary>
        public void Stop()
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }

        /// <summary>
        /// 暂停播放。
        /// </summary>
        public void Pause()
        {
            source.Pause();
        }
    }
}