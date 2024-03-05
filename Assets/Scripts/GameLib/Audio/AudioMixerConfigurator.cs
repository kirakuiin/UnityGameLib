using UnityEngine;
using UnityEngine.Audio;

namespace GameLib.Audio
{
    /// <summary>
    /// 音量混合配置器，用来设置音量混合器中的相关参数。
    /// </summary>
    public class AudioMixerConfigurator : MonoBehaviour
    {
        [Tooltip("音量混合器")]
        [SerializeField]
        private AudioMixer mixer;

        /// <summary>
        /// 音量由0.0001->1, 然而mixer是以分贝制工作的，所以需要用log10*系数来将音量
        /// 映射到分贝。之所以采用20是因为log10(0.0001)=-4*20 = -80 == 分贝的最低单位。
        /// </summary>
        private const int VolumeLog10Multiplier = 20;

        private const float VolumeMin = 0.0001f;

        /// <summary>
        /// 设置混音器里的浮点变量。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void SetFloat(string key, float value)
        {
            mixer.SetFloat(key, ChangeVolumeToDecibels(value));
        }

        private float ChangeVolumeToDecibels(float volume)
        {
            if (volume <= 0)
            {
                volume = VolumeMin;
            }

            if (volume >= 1)
            {
                volume = 1;
            }

            return Mathf.Log10(volume) * VolumeLog10Multiplier;
        }

        /// <summary>
        /// 获得混音器里的浮点变量。
        /// </summary>
        /// <remarks>如果值不存在则返回<see cref="VolumeMin"/></remarks>
        /// <param name="key">键</param>
        public float GetFloat(string key)
        {
            if (mixer.GetFloat(key, out float value))
            {
                return ChangeDecibelsToVolume(value);
            }
            else
            {
                return VolumeMin;
            }
        }

        private float ChangeDecibelsToVolume(float decibels)
        {
            if (decibels < -80)
            {
                decibels = -80;
            }

            if (decibels > 0)
            {
                decibels = 0;
            }

            return Mathf.Pow(10, decibels / VolumeLog10Multiplier);
        }
    }
}