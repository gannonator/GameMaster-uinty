using System.Collections.Generic;
using UnityEngine;

namespace GameMaster
{

    public class LoadingOverlay : MonoBehaviour
    {
        public Canvas LoadingOverlayCanvas;
        public GameObject Container;
        internal float loadingAmount;
       // public SimpleLoadingIndicator simpleLoadingIndicator;
        public void Show()
        {
            StartCoroutine(FadeIn());
        }
        public void Hide()
        {
            StartCoroutine(FadeOut());
        }
         void Update()
        {
            //simpleLoadingIndicator.Amount = loadingAmount;
        }
        private IEnumerator<float> FadeOut()
        {
            float alphaPercentage = 0f;
            CanvasRenderer render = LoadingOverlayCanvas.GetComponent<CanvasRenderer>();
            while (alphaPercentage < 1f)
            {
                alphaPercentage += Time.deltaTime / 0.5f;
                render.SetAlpha(alphaPercentage);
                yield return 0f;
            }
            Container.SetActive(false);
        }

        private IEnumerator<float> FadeIn()
        {
            Container.SetActive(true);
            float alphaPercentage = 1f;
            CanvasRenderer render = LoadingOverlayCanvas.GetComponent<CanvasRenderer>();
            while (alphaPercentage > 0f)
            {
                alphaPercentage += Time.deltaTime / 0.5f;
                render.SetAlpha(alphaPercentage);
                yield return 0f;
            }
        }
    }
}
