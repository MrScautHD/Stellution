using UnityEngine;
using UnityEngine.UI;

namespace Mewlist.MassiveClouds
{
    [RequireComponent(typeof(MassiveCloudsEnvironment))]
    public class MassiveCloudsEnvironmentController : MonoBehaviour
    {
        [SerializeField] private MassiveCloudsEnvironment environment = null;

        [SerializeField] private Slider time = null;
        [SerializeField] private float timeChangeVelocity = 1f;
        [SerializeField] private Text hourText = null;

        private float currentHour;

        private float Hour
        {
            get
            {
                return time.value * 24;
            }
        }

        private void OnEnable()
        {
            if (time != null)
            {
                currentHour = Hour;
                ChangeTime(currentHour);
            }
            else
            {
                DynamicGI.UpdateEnvironment();
            }
        }

        private void OnDisable()
        {
            if (time != null) time.onValueChanged.RemoveListener(ChangeTime);
        }

        private void Update()
        {

            if (currentHour > Hour)
            {
                currentHour = Mathf.Max(Hour, currentHour - timeChangeVelocity * Time.deltaTime);
                ChangeTime(currentHour);
            }
            else if (currentHour < Hour)
            {
                currentHour = Mathf.Min(Hour, currentHour + timeChangeVelocity * Time.deltaTime);
                ChangeTime(currentHour);
            }
        }

        private void ChangeTime(float t)
        {
            environment.ChangeTime(t);
            var h = Mathf.FloorToInt(t);
            var m = Mathf.FloorToInt((t % 1f) * 60f);
            hourText.text = h.ToString().PadLeft(2) + ":" + m.ToString("00");
        }
    }
}