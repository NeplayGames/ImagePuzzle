using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

   
   public void StartGame(int i){
       SceneManager.LoadScene(i);
   } 
}