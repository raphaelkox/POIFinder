using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecretFinder : MonoBehaviour
{
    public Image Aim;
    public Text SecretsCounter;
    public RawImage CheatOverlay;
    public Toggle CheatToggle;

    [SerializeField] float maxDistance;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] float minActivationRatio;
    [SerializeField] float minAlpha;
    [SerializeField] float maxAlpha;

    public int SecretsTotal { get; private set; }
    public int SecretsFound { get; private set; }

    float lowestDistance;
    float scaleRange;
    float alphaRange;
    List<Transform> secretPoints = new List<Transform>();
    Transform nearestPoint;
    Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
    Camera cam;
    bool cheats = false;

    const string counterText = "Secrets Found: ";

    // Start is called before the first frame update
    void Start()
    {
        //correct maxDistance pixel for screen proportions
        var r = (float)Screen.width / 1920.0f;
        maxDistance *= r;
        maxDistance /= 2;
        Debug.Log(maxDistance);

        cam = Camera.main;

        var points = GameObject.FindGameObjectsWithTag("SecretPoint");
        SecretsTotal = points.Length;

        foreach (var point in points) {
            secretPoints.Add(point.transform);
        }

        Aim.rectTransform.localScale = new Vector3(maxScale, maxScale);
        var c = Aim.color;
        c.a = minAlpha;
        Aim.color = c;

        scaleRange = maxScale - minScale;
        alphaRange = maxAlpha - minAlpha;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            cheats = !cheats;
            CheatToggle.isOn = cheats;
            ToggleCheatCamera(cheats);
        }

        lowestDistance = float.PositiveInfinity;
        Vector3 p;
        float d, distRatio;

        if (secretPoints.Count < 1) {
            Aim.rectTransform.localScale = new Vector3(maxScale, maxScale);

            var color = Aim.color;
            color.a = 0;
            Aim.color = color;
            return;
        }

        foreach (var point in secretPoints) {
            p = cam.WorldToScreenPoint(point.position);

            d = Vector2.Distance(center, p);

            if (d < lowestDistance) {
                lowestDistance = d;
                nearestPoint = point;
            }
        }

        if (lowestDistance < maxDistance) {
            Debug.Log(lowestDistance);
            distRatio = lowestDistance / maxDistance;
            var scale = minScale + distRatio * scaleRange;
            Aim.rectTransform.localScale = new Vector3(scale, scale);

            var alpha = minAlpha + (1 - distRatio) * alphaRange;
            var color = Aim.color;
            color.a = alpha;
            Aim.color = color;

            if (distRatio < minActivationRatio) {
                SecretsFound++;
                secretPoints.Remove(nearestPoint);
                Destroy(nearestPoint.gameObject);
            }
        }
        else {
            Aim.rectTransform.localScale = new Vector3(maxScale, maxScale);

            var color = Aim.color;
            color.a = 0;
            Aim.color = color;
        }        

        SecretsCounter.text = counterText + SecretsFound + "/" + SecretsTotal;
    }

    public void ToggleCheatCamera(bool active) {
        CheatOverlay.enabled = active;
    }
}
