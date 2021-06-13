using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightController : MonoBehaviour
{
    public Light directionalLight;
    public Material waterMaterial;
    public Gradient lightIntensityOvertime;
    public Gradient lightColorOvertime;
    public Gradient oceanColorOvertime;

    public Vector3 startRotation = Vector3.zero;
    public Vector3 endRotation = Vector3.zero;

    public float globalDeltaAcceleration = 20f;

    private Color currentLightColor;
    private Color currentOceanColor;
    private float currentLightIntensity;
    private Quaternion currentRotation;

    private Color startingWaterShallowColor;
    private Color startingWaterDeepColor;

    void Start()
    {
        startingWaterShallowColor = waterMaterial.GetColor("_DepthGradientShallow");
        startingWaterDeepColor = waterMaterial.GetColor("_DepthGradientDeep");
    }

    void OnDestroy()
    {
        waterMaterial.SetColor("_DepthGradientShallow", startingWaterShallowColor);
        waterMaterial.SetColor("_DepthGradientDeep", startingWaterDeepColor);
    }

    //between 0-1
    public void UpdateDaylight(float time)
    {
        time = Mathf.Clamp01(time);

        currentLightIntensity = lightIntensityOvertime.Evaluate(time).a;
        currentLightColor = lightColorOvertime.Evaluate(time);
        currentOceanColor = oceanColorOvertime.Evaluate(time);
        currentRotation = Quaternion.Slerp(Quaternion.Euler(startRotation), Quaternion.Euler(endRotation), time);

        directionalLight.intensity = Mathf.MoveTowards(directionalLight.intensity, currentLightIntensity, globalDeltaAcceleration * Time.deltaTime);
        directionalLight.color = Color.Lerp(directionalLight.color, currentLightColor, globalDeltaAcceleration * Time.deltaTime);
        directionalLight.transform.rotation = Quaternion.RotateTowards(directionalLight.transform.rotation, currentRotation, globalDeltaAcceleration * Time.deltaTime);

        waterMaterial.SetColor("_DepthGradientDeep", currentOceanColor);
    }
}
