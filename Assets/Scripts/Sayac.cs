using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Sayac : MonoBehaviour
{
    public TextMeshProUGUI timerText,timertext2;
    public float totalTimeInSeconds = 300f; // Toplam s�re (saniye cinsinden)
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
            // Zaman� g�ncelle
            currentTime += Time.deltaTime;

            // Zaman� dakika ve saniye cinsine �evir
            float minutes = Mathf.Floor(currentTime / 60);
            float seconds = Mathf.RoundToInt(currentTime % 60);

            // Metin eleman�na g�ncellenmi� zaman� yazd�r
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Belirli bir s�re ge�ti�inde istedi�iniz bir i�lemi ger�ekle�tirin
            if (currentTime >= totalTimeInSeconds)
            {
                // �rne�in: Oyunu durdurabilir veya ba�ka bir �ey yapabilirsiniz.
                // Time.timeScale = 0; // Oyunu durdur
                isTimerRunning = false; // Timer'� durdur
            }
        }
    }

    // Timer'� durduran fonksiyon
    public void StopTimer()
    {
        isTimerRunning = false;
        timertext2.text = timerText.text;
    }
}
