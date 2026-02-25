using UnityEngine;
using UnityEngine.UI;

using TMPro;

using save;

/*
    - Im writing this script copying from https://www.youtube.com/watch?v=otDHGmncBQY originally
            I will most likely be improving this on my own but the base script and concept can be found there, its a really helpful tutorial so id recommend it to anyone looking
*/
public class CP_Controller : MonoBehaviour {
    [Header("data")]
    public float currentHue, currentSat, currentVal;
    
    // components
    [SerializeField] private RawImage hueImage, satValImage, outputImage;
    [SerializeField] private Slider hueSlider;
    [SerializeField] private TMP_InputField hexInputFeild;

    //
    private Texture2D hueTexture, svTexture, outputTexture;

    /// <summery> initalise everything nicly </summery>
    void Start() {
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();

        saveData currentSave = getData.viewSave();
        SetUsingString(currentSave.mainColor);
    }

    /// <summery> generate the image used for the Hue control </summery>
    private void CreateHueImage() {
        // generate the hue texture (it feels like i keep writing hue texture omfg)
        hueTexture = new Texture2D(16, 1);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "hueTexture";

        // loop through all pixels in the texture then set each pixel based on its height
        for (int i = 0; i < hueTexture.width; i++) hueTexture.SetPixel(i, 0, Color.HSVToRGB((float)i / hueTexture.width, 1, 1));

        hueTexture.Apply(); // update changes
        currentHue = 0; // set default hue

        // apply texture
        hueImage.texture = hueTexture;
    }

    /// <summery> generate the saturation and value texture </summery>
    private void CreateSVImage() {
        // setup the texture, same as the hue image pretty much
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "svTexture";

        // "nested loop" (we should call these 2D loops (kill me)) to set the color for the x and y values (yk cause its more than 1 digit tall this time) yap yap yap
        for (int y = 0; y < svTexture.height; y++) {
            for (int x = 0; x < svTexture.width; x++) {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                    currentHue,
                   (float)x / svTexture.width,
                   (float) y / svTexture.height
                ));
            }
        }

        svTexture.Apply(); // again save changes
        // default values
        currentSat = 0;
        currentHue = 0;

        // add the texture
        satValImage.texture = svTexture;
    }

    /// <summery> this one generates the output image (to display the chosen color) </summery>
    private void CreateOutputImage() {
        // setup the texture
        outputTexture = new Texture2D(1, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "outputTexture";

        // get the current color
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        // there has to be a better way of doing this
        for (int i = 0; i < outputTexture.height; i ++) outputTexture.SetPixel(0, i, currentColor);

        outputTexture.Apply();
        outputImage.texture = outputTexture;
    }

    /// <summery> this will be called whenever we are updating the output image </summery>
    private void UpdateOutputImage() {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal); // get the current color

        // i swear agian there has to be a better way of doing ts
        for (int i = 0; i < outputTexture.height; i ++) outputTexture.SetPixel(0, i, currentColor);

        outputTexture.Apply();
        hexInputFeild.text = ColorUtility.ToHtmlStringRGB(currentColor);

        Debug.Log($"color set to {currentColor}");

        saveData currentSave = getData.viewSave();
        currentSave.mainColor = $"#{ColorUtility.ToHtmlStringRGB(currentColor)}";
        getData.save(currentSave);
    }

    /// <summery> this will communicate with CP_ImageController to set the saturation and value </summery>
    public void SetSV(float S, float V) {
        currentSat = S;
        currentVal = V;

        UpdateOutputImage();
    }

    /// <summery> update SV on slider change (i got lazy with the commenting sorry)</summery>
    public void UpdateSVImage() {
        currentHue = hueSlider.value;

        // yayyyyyyy
        for (int y = 0; y < svTexture.height; y++) {
            for (int x = 0; x < svTexture.width; x++) {
                svTexture.SetPixel(x, y, Color.HSVToRGB(
                    currentHue,
                   (float)x / svTexture.width,
                   (float) y / svTexture.height
                ));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();
    }

    /// <summery> this will update the color on hex input </summery>
    public void OnTextInputChange() {
        if (hexInputFeild.text.Length  < 6) return;
        Color newColor;

        if (ColorUtility.TryParseHtmlString($"#{hexInputFeild.text}", out newColor))
            Color.RGBToHSV(newColor, out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;
        UpdateOutputImage();
    }

    /// <summery> set using a string </summery>
    public void SetUsingString(string input) {
        if (input.Length  < 7) return;
        Color newColor;

        if (ColorUtility.TryParseHtmlString(input, out newColor))
            Color.RGBToHSV(newColor, out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;
        UpdateOutputImage();
    }
}