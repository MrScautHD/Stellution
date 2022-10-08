using UnityEngine;
using Random = UnityEngine.Random;

public class AutoPilot : MonoBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float height = 0f;

    private float t = 0f;
    private Vector3 target;
    private float targetVelocity;

    private Chaos velocityChaos;
    private Chaos pitchChaos;
    private Chaos VelocityChaos
    {
        get { return velocityChaos ?? (velocityChaos = new Chaos());  }
    }
    private Chaos PitchChaos
    {
        get { return pitchChaos ?? (pitchChaos = new Chaos());  }
    }

    private float Pitch
    {
        get
        {
            var forward = transform.forward;
            var pitch = Vector3.Angle(new Vector3(forward.x, 0, forward.z), forward);
            if (forward.y > 0) pitch = -pitch;
            return pitch;
        }
    }
    
    // Use this for initialization
    void Start ()
    {
        targetVelocity = velocity;
    }

    // Update is called once per frame
    void Update ()
    {
        t += Time.deltaTime;
        if (t >= 1f)
        {
            t -= 1f;
            OnStep();
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        var pitch = Pitch;
        var maxRot = new Vector2(
            Mathf.Min(0, -45 - pitch),
            Mathf.Max(0, 45 - pitch));

        if (Physics.Raycast(ray, out hit, 20f))
        {

            if (hit.distance < height + 10f)
            {
                var rot = Mathf.Clamp(Time.deltaTime * -100f * (height  + 10f - hit.distance),
                    maxRot.x,
                    maxRot.y);
                transform.RotateAround(
                    transform.position,
                    transform.right,
                    rot);
            }
        }
        if (transform.position.y < height + 10f)
        {
            var rot = Mathf.Clamp(Time.deltaTime * -100f * (height + 10f - transform.position.y),
                maxRot.x,
                maxRot.y);
            transform.RotateAround(
                transform.position,
                transform.right,
                rot);
        }
        else if (transform.position.y > height + 30f)
        {
            var rot = Mathf.Clamp(Time.deltaTime * 10f * (transform.position.y - height - 30f),
                maxRot.x,
                maxRot.y);
            transform.RotateAround(
                transform.position,
                transform.right,
                rot);
        }

        var vChaos = VelocityChaos.Next;
        var pChaos = PitchChaos.Next;
        pitch = Pitch;
        maxRot = new Vector2(
            Mathf.Min(0, -45 - pitch),
            Mathf.Max(0, 45 - pitch));

        targetVelocity = 40f + vChaos * 60f;

        var pitchRot = Mathf.Clamp(Time.deltaTime * 100f * (pChaos - 0.5f), maxRot.x, maxRot.y);

        transform.RotateAround(
            transform.position,
            transform.right,
            pitchRot);
        
        velocity = Mathf.Lerp(velocity, targetVelocity, 0.1f * Time.deltaTime);
        var toTarget = target - transform.position;
        transform.LookAt(transform.position + 
                         Vector3.Lerp(
                             transform.forward,
                             toTarget.normalized,
                             Time.deltaTime));

        transform.position += transform.forward * Time.deltaTime * velocity * 1000f / 60f / 60f;
    }

    void OnStep()
    {
        target = new Vector3(Random.Range(-1000f, 1000f), transform.position.y, Random.Range(-1000f, 1000f));
    }

    private class Chaos
    {
        private float t;
        public Chaos()
        {
            t = Random.Range(0f, 1f);
        }

        public float Next
        {
            get
            {
                if (Mathf.Abs(t - 0.5f) < Mathf.Epsilon) t = Random.Range(0f, 1f);
                return t = (t < 0.5) ? t + 2 * t * t : t - 2 * (1 - t) * (1 - t);
            }
        }
    }
}