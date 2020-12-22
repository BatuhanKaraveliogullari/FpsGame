using UnityEngine;
using UnityEngine.UI;
 
public class HealthBarHandler : MonoBehaviour
{
    public Image HealthBarImage;//scenedeki healtbar imageleri

    private void Start()
    {
        HealthBarImage = GetComponent<Image>();//scale i kontrol etmek için bir atama

        SetHealthBarColor(Color.green);//başlangıç için health 100 yani max olduğu için green atama
    }

    public void SetHealthBarValue(float value)//değişen valueya göre yeni color atamsı
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

    public float GetHealthBarValue()//health barın anlık konumu ile ilgili değer get etme
    {
        if (HealthBarImage != null)
            return HealthBarImage.fillAmount;
        else
            return 0f;
    }

    public void SetHealthBarColor(Color healthColor)//health barın colorunu setleme
    {
        if (HealthBarImage != null)
            HealthBarImage.color = healthColor;
    }
}
