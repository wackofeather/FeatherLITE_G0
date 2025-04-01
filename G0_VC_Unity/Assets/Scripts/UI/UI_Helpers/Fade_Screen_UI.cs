
using System.Threading.Tasks;
using UnityEngine;

public class Fade_Screen_UI : Screen_UI
{
    public float fadeSpeed; 
    public override async Task<bool> EnableScreen()
    {
        
        canvasGroup.alpha = 0;
        Container.SetActive(true);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;

            await Task.Yield();
        }
        return true;
    }
    public override async Task<bool> DisableScreen()
    {
        
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;

            await Task.Yield();
        }
        
        Container.SetActive(false);
        
        return true;
    }
    //ss
}
