using GameLib.Common;
using UnityEngine;
using UnityEngine.Audio;

namespace GameLib.Audio
{
    /// <summary>
    /// 音量混合配置器，用来设置音量混合器中的相关参数。
    /// </summary>
    public class AudioMixerConfigurator : PersistentMonoSingleton<AudioMixerConfigurator>
    {
        [Tooltip("音量混合器")]
        [SerializeField]
        private AudioMixer mixer;

        /// <summary>
        /// 音量由0.0001->1, 然而mixer是以分贝制工作的，所以需要用log10*系数来将音量
        /// 映射到分贝。之所以采用20是因为log10(0.0001)=-4*20 = -80 == 分贝的最低单位。
        /// </summary>
        private const int VolumeLog10Multiplier = 20;

        /// <summary>
        /// 设置混音器里的浮点变量。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetFloat(string key, float value)
        {
            mixer.SetFloat(key, GetVolumeInDecibels(value));
        }

        private float GetVolumeInDecibels(float volume)
        {
            if (volume <= 0)
            {
                volume = 0.0001f;
            }

            if (volume >= 1)
            {
                volume = 1;
            }

            return Mathf.Log10(volume) * VolumeLog10Multiplier;
        }
    }
}