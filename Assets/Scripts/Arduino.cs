using UnityEngine;
using System.IO.Ports;

public class Arduino : MonoBehaviour
{
    SerialPort stream = new SerialPort("COM4", 9600); // Arduino'nun baðlý olduðu seri port ve baud rate
    public static bool forward,back,left,right,jump = false;
    void Start()
    {
        stream.Open(); // Seri portu aç

        // Eðer seri port açýlýrsa
        if (stream.IsOpen)
        {
            Debug.Log("Arduino connected");
        }
        else
        {
            Debug.LogError("Arduino connection failed");
        }
    }

    void Update()
    {
        // Eðer seri porttan veri varsa
        if (stream.IsOpen && stream.BytesToRead > 0)
        {
            string message = stream.ReadLine(); // Mesajý oku
           // Debug.Log(message); // Unity Debug.Log kýsmýna yaz

            if(message == "Button 1 Pressed")
            {
               forward= true;
            }
            else if (message=="Button 1 Release")
            {
                forward=false;
            }
            if (message == "Button 2 Pressed")
            {
                back= true;
            }
            else if (message == "Button 2 Release")
            {
                back = false;
            }
            if (message == "Button 3 Pressed")
            {
                left= true;
            }
            else if (message == "Button 3 Release")
            {
                left = false;
            }
            if (message == "Button 4 Pressed")
            {
                right= true;
                Debug.Log("RÝGHT TRUE");
            }
            else if (message == "Button 4 Release")
            {
                right = false;
            }
            if (message == "Button 5 Pressed")
            {
                jump= true;
            }
            else if (message == "Button 5 Release")
            {
                jump = false;
            }
        }
    }

    void OnApplicationQuit()
    {
        stream.Close(); // Uygulama kapatýldýðýnda seri portu kapat
    }
}
