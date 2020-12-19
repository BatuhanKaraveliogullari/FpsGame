using UnityEngine;
using UnityEngine.UI;
 
public class HealthBarHandler : MonoBehaviour
{
    public Image HealthBarImage;

    private void Start()
    {
        HealthBarImage = GetComponent<Image>();

        SetHealthBarColor(Color.green);
    }

    public void SetHealthBarValue(float value)
    {
        if(HealthBarImage != null)
        {
            HealthBarImage.fillAmount = value;

            if (HealthBarImage.fillAmount < 0.3f)
            {
                SetHealthBarColor(Color.red);
            }
            else if (HealthBarImage.fillAmount < 0.5f)
            {
                SetHealthBarColor(Color.yellow);
            }
            else if (HealthBarImage.fillAmount <= 1)
            {
                SetHealthBarColor(Color.green);
            }
        }
    }

    public float GetHealthBarValue()
    {
        if (HealthBarImage != null)
            return HealthBarImage.fillAmount;
        else
            return 0f;
    }

    public void SetHealthBarColor(Color healthColor)
    {
        if (HealthBarImage != null)
            HealthBarImage.color = healthColor;
    }
}
