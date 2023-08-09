using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{

    
    public void Anasayfa()
    {
        SceneManager.LoadScene("Anasayfa");
    }
    public void Giris()
    {
        SceneManager.LoadScene("Giris");
    }
    public void Hakkimizda()
    {
        SceneManager.LoadScene("Iletisim");
    }
    public void TaraSayfa()
    {
        SceneManager.LoadScene("TaraSayfa");
    }
    public void Imzalar()
    {
        SceneManager.LoadScene("Imzalar");
    }
    public void Cikis()
    {
        Debug.Log("Uygulamadan Çıkış Yapıldı");
        Application.Quit();
    }

    

}
