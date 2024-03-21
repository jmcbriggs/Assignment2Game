using UnityEngine;

public class GradientTest : MonoBehaviour
{

    public Gradient gradient = new Gradient();
    public GradientColorKey color1 = new GradientColorKey(Color.red, 0.0f);
    public GradientColorKey color2 = new GradientColorKey(Color.blue, 1.0f);

    public GradientAlphaKey alpha1 = new GradientAlphaKey(1.0f, 0.0f);
    public GradientAlphaKey alpha2 = new GradientAlphaKey(0.0f, 1.0f);
    void Start()
    {

        // Blend color from red at 0% to blue at 100%
        var colors = new GradientColorKey[2];
        colors[0] = color1;
        colors[1] = color2;

        // Blend alpha from opaque at 0% to transparent at 100%
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        // What's the color at the relative time 0.25 (25%) ?
      
    }

    private void FixedUpdate()
    {
        Material material = GetComponent<Renderer>().material;
        material.color = gradient.Evaluate(Time.time % 1);
    }
}