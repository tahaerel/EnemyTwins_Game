using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Sayac : MonoBehaviour
{
    public TextMeshProUGUI timerText,timertext2;
    public float totalTimeInSeconds = 300f; // Toplam süre (saniye cinsinden)
    private float currentTime;
    private bool isTimerRunning = true;

    void Start()
    {
        currentTime = 0f;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            // Zamaný güncelle
            currentTime += Time.deltaTime;

            // Zamaný dakika ve saniye cinsine çevir
            float minutes = Mathf.Floor(currentTime / 60);
            float seconds = Mathf.RoundToInt(currentTime % 60);

            // Metin elemanýna güncellenmiþ zamaný yazdýr
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Belirli bir süre geçtiðinde istediðiniz bir iþlemi gerçekleþtirin
            if (currentTime >= totalTimeInSeconds)
            {
                // Örneðin: Oyunu durdurabilir veya baþka bir þey yapabilirsiniz.
                // Time.timeScale = 0; // Oyunu durdur
                isTimerRunning = false; // Timer'ý durdur
            }
        }
    }

    // Timer'ý durduran fonksiyon
    public void StopTimer()
    {
        isTimerRunning = false;
        timertext2.text = timerText.text;
    }
}
