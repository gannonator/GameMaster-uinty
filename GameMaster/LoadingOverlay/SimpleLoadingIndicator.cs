using UnityEngine;
using UnityEngine.UI;

public class SimpleLoadingIndicator : MonoBehaviour
{
    public Image LoadingImage;
    public float Amount ;

    float _amount = 1;

    private void Update()
    {
        if(Amount <= 0)
        {
            Amount = 1;
        }
        LoadingImage.fillAmount = Amount;
    }
}