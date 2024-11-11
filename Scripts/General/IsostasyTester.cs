using UnityEngine;
using System;
using TMPro;

public class IsostasyTester : MonoBehaviour
{
    private const int minOceanicCrustThickness = 6000; // minimum oceanic crust thickness due to magma supply variability
    private const int maxOceanicCrustThickness = 10000; // maximum oceanic crust thickness in meters
    private const int minContinentalCrustThickness = 30000; // minimum continental crust thickness in meters
    private const int maxContinentalCrustThickness = 70000; // maximum continental crust thickness in meters
    public int oceanicThickness = 8000;
    public int continentalThickness = 50000;

    public TectonicColumn oceanicColumn;
    public float oceanicScale = 1f;
    public TectonicColumn continentalColumn;
    public float continentalScale = 1f;

    public Transform oceanicParent;
    public Transform continentalParent;  

    private RectTransform o_seaLevelImage, o_mantleImage, o_lowerCrustImage, o_upperCrustImage;
    private RectTransform c_seaLevelImage, c_mantleImage, c_lowerCrustImage, c_upperCrustImage;

    private TextMeshProUGUI continentalElevationText;
    private TextMeshProUGUI oceanicElevationText;
    void Update()
    {
        oceanicThickness = (int)Math.Clamp(oceanicThickness, minOceanicCrustThickness, maxOceanicCrustThickness);
        continentalThickness = (int)Math.Clamp(continentalThickness, minContinentalCrustThickness, maxContinentalCrustThickness);

        oceanicColumn = new TectonicColumn(0/*, oceanicThickness*/);
        continentalColumn = new TectonicColumn(1/*, continentalThickness*/);

        if(o_seaLevelImage == null)
        {
            o_seaLevelImage = oceanicParent.GetChild(0).gameObject.GetComponent<RectTransform>();
            o_mantleImage = oceanicParent.GetChild(3).gameObject.GetComponent<RectTransform>();
            o_lowerCrustImage = oceanicParent.GetChild(2).gameObject.GetComponent<RectTransform>();
            o_upperCrustImage = oceanicParent.GetChild(1).gameObject.GetComponent<RectTransform>();

            oceanicElevationText = o_upperCrustImage.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }
        if(c_seaLevelImage == null)
        {
            c_seaLevelImage = continentalParent.GetChild(0).gameObject.GetComponent<RectTransform>();
            c_mantleImage = continentalParent.GetChild(3).gameObject.GetComponent<RectTransform>();
            c_lowerCrustImage = continentalParent.GetChild(2).gameObject.GetComponent<RectTransform>();
            c_upperCrustImage = continentalParent.GetChild(1).gameObject.GetComponent<RectTransform>();

            continentalElevationText = c_upperCrustImage.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        }

        ApplyContinental();
        ApplyOceanic();
    }

    private void ApplyContinental()
    {
        int _mantleThickness = continentalColumn.lithosphericMantle.thickness;
        int _lCrustThickness = continentalColumn.lowerCrust.thickness;
        int _uCrustThickness = continentalColumn.upperCrust.thickness;

        float mantleImageSize = (float)_mantleThickness * continentalScale * 0.001f;
        float lowerCrustImageSize = (float)(_lCrustThickness) * continentalScale * 0.001f;
        float upperCrustImageSize = (float)(_uCrustThickness) * continentalScale * 0.001f;

        float referenceDepthUI = c_seaLevelImage.anchoredPosition.y - (float)continentalColumn.referenceDepth * continentalScale * 0.001f;
        float manteImageOffset = referenceDepthUI + mantleImageSize * 0.5f;
        float lowerCrustImageOffset = referenceDepthUI + mantleImageSize + lowerCrustImageSize * 0.5f;
        float upperCrustImageOffset = referenceDepthUI + mantleImageSize + lowerCrustImageSize + upperCrustImageSize * 0.5f;

        c_mantleImage.anchoredPosition = new Vector2(0, manteImageOffset);
        c_lowerCrustImage.anchoredPosition = new Vector2(0, lowerCrustImageOffset);
        c_upperCrustImage.anchoredPosition = new Vector2(0, upperCrustImageOffset);

        c_mantleImage.sizeDelta = new Vector2(100f, mantleImageSize);
        c_lowerCrustImage.sizeDelta = new Vector2(100f, lowerCrustImageSize);
        c_upperCrustImage.sizeDelta = new Vector2(100f, upperCrustImageSize);

        continentalElevationText.text = "" + ((_mantleThickness + _lCrustThickness + _uCrustThickness) - continentalColumn.referenceDepth);
    }

    private void ApplyOceanic()
    {
        int _mantleThickness = oceanicColumn.lithosphericMantle.thickness;
        int _lCrustThickness = oceanicColumn.lowerCrust.thickness;
        int _uCrustThickness = oceanicColumn.upperCrust.thickness;

        float mantleImageSize = (float)_mantleThickness * oceanicScale * 0.001f;
        float lowerCrustImageSize = (float)(_lCrustThickness) * oceanicScale * 0.001f;
        float upperCrustImageSize = (float)(_uCrustThickness) * oceanicScale * 0.001f;

        float referenceDepthUI = o_seaLevelImage.anchoredPosition.y - (float)oceanicColumn.referenceDepth * oceanicScale * 0.001f;
        float manteImageOffset = referenceDepthUI + mantleImageSize * 0.5f;
        float lowerCrustImageOffset = referenceDepthUI + mantleImageSize + lowerCrustImageSize * 0.5f;
        float upperCrustImageOffset = referenceDepthUI + mantleImageSize + lowerCrustImageSize + upperCrustImageSize * 0.5f;

        o_mantleImage.anchoredPosition = new Vector2(0, manteImageOffset);
        o_lowerCrustImage.anchoredPosition = new Vector2(0, lowerCrustImageOffset);
        o_upperCrustImage.anchoredPosition = new Vector2(0, upperCrustImageOffset);

        o_mantleImage.sizeDelta = new Vector2(100f, mantleImageSize);
        o_lowerCrustImage.sizeDelta = new Vector2(100f, lowerCrustImageSize);
        o_upperCrustImage.sizeDelta = new Vector2(100f, upperCrustImageSize);

        oceanicElevationText.text = "" + ((_mantleThickness + _lCrustThickness + _uCrustThickness) - oceanicColumn.referenceDepth);
    }
}
