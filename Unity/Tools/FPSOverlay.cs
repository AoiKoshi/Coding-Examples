using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Threadwork.Tools
{
    public class FPSOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject overlay;

        private float timer = 0;
        private float averageTimer = 0;
        private float intervalPeriod = 1f;
        private float longPeriod = 10f;
        private List<float> secondFPSTracker = new List<float>();
        private List<float> longFPSTracker = new List<float>();
        private float fps;
        private float fpsMin;
        private float fpsMax;
        private float fpsAvg;

        [SerializeField] private Text fpsText;
        [SerializeField] private Text fpsMinText;
        [SerializeField] private Text fpsMaxText;
        [SerializeField] private Text fpsAvgText;

        public static FPSOverlay Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ToggleOverlay()
        {
            overlay.SetActive(!overlay.activeSelf);
        }

        private void Update()
        {
            if (!overlay.activeSelf) return;

            secondFPSTracker.Add(1f / Time.deltaTime);
            longFPSTracker.Add(1f / Time.deltaTime);

            timer += Time.deltaTime;
            averageTimer += Time.deltaTime;

            if (timer >= intervalPeriod)
            {
                CalculateFPS();
                fpsText.text = fps.ToString();
                fpsMinText.text = fpsMin.ToString();
                timer = 0;
            }

            if (averageTimer >= longPeriod)
            {
                CalculateAverageFPS();
                fpsMaxText.text = fpsMax.ToString();
                fpsAvgText.text = fpsAvg.ToString();
                averageTimer = 0;
            }
        }

        private void CalculateFPS()
        {
            float total = 0f;
            fpsMin = fpsMin == 0 ? secondFPSTracker[0] : fpsMin;

            foreach (float i in secondFPSTracker)
            {
                fpsMin = i < fpsMin ? i : fpsMin;
                total += i;
            }

            fpsMin = Mathf.RoundToInt(fpsMin);
            fps = Mathf.RoundToInt(total / secondFPSTracker.Count);
            secondFPSTracker.Clear();
        }

        private void CalculateAverageFPS()
        {
            float total = 0f;
            fpsMax = fpsMax = longFPSTracker[0]; ;
            fpsMin = 0; //Reset minimum fps

            foreach (float i in longFPSTracker)
            {
                fpsMax = i > fpsMax ? i : fpsMax;
                total += i;
            }

            fpsMax = Mathf.RoundToInt(fpsMax);
            fpsAvg = Mathf.RoundToInt(total / longFPSTracker.Count);
            longFPSTracker.Clear();
        }
    }
}
