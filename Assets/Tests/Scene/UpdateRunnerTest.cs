using System.Globalization;
using GameLib.Common;
using UnityEngine;
using TMPro;

namespace Tests.Scene
{
    public class UpdateRunnerTest : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text frameText;
        
        [SerializeField]
        private TMP_Text secondText;
        
        [SerializeField]
        private TMP_Text timeText;

        private int _secCalledTime = 0;

        private int _frameCalledTime = 0;
        
        private UpdateRunner _runner;

        private void Awake()
        {
            _runner = GetComponent<UpdateRunner>();
            _runner.Subscribe(UpdateEachFrame);
            _runner.Subscribe(UpdateEachSecond, 1);
        }

        void UpdateEachSecond(float delta)
        {
            _secCalledTime += 1;
            secondText.text = $"seconds:{_secCalledTime.ToString()}";
        }

        void UpdateEachFrame(float delta)
        {
            _frameCalledTime += 1;
            frameText.text = $"frame:{_frameCalledTime.ToString()}";
        }

        private void OnDisable()
        {
            _runner.UnSubscribe(UpdateEachSecond);
            _runner.UnSubscribe(UpdateEachFrame);
        }

        private void Update()
        {
            timeText.text = $"time:{Time.time.ToString(CultureInfo.CurrentCulture)}s";
        }
    }
}