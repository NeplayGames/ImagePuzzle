using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    public class EachImage : MonoBehaviour
    {
        [HideInInspector]
        ///Identify the initialRow of the image
        public int row;
      //  [HideInInspector]

        ///Identify the initial column of the image
        public int column;
      //  [HideInInspector]

        ///Indentify the current row of the image 
        ///This defines the current row position of the image in game
        public int CurrentRow ;
      //  [HideInInspector]
           ///Indentify the current column of the image 
        ///This defines the current column position of the image in game
        public int CurrentColumn;
      //  [HideInInspector]


        public GameObject imageObject;

        public bool isInActive = false;

      

         private void Awake() {
             string name = gameObject.name;
                row = CurrentRow = int.Parse(name) / 10;
                column = CurrentColumn = int.Parse(name) % 10;
                imageObject = this.gameObject;
               // if(isInActive) this.gameObject.SetActive(false);
        }

    }

