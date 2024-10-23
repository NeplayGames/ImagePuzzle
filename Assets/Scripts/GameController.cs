using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ImagePuzzle
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]private int MaxColumns = 4;
        [SerializeField]private int MaxRows = 4;
        [SerializeField]private TextMeshProUGUI timePassed_TMP;

        [SerializeField]private TextMeshProUGUI stepTaken_TMP;
        [SerializeField]private TextMeshProUGUI TotalTime_TMP;
        [SerializeField]private TextMeshProUGUI stepTakenToFinished_TMP;
        [SerializeField]private GameObject gameFinishCanvas;
        [SerializeField]private AudioClip click;
        [SerializeField]private AudioClip cheer;
        [SerializeField]private Transform imagesParentGameObject;
        private EachImage[,] EachImages = new EachImage[4, 4];
        private int toAnimateI, toAnimateJ;
        private AudioSource audioSource;
        private int time = 0;
        private System.DateTime startTime;
        private bool gameOver = false;
        private int stepTaken = 0;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            for (int i = 0; i < MaxColumns; i++)
            {
                for (int j = 0; j < MaxRows; j++)
                {
                    EachImages[i, j] = imagesParentGameObject.GetChild(i * MaxColumns + j).gameObject.GetComponent<EachImage>();
                }
            }
            Shuffle();
            startTime = System.DateTime.UtcNow;
        }

        ///<summary>
        ///Suffle the image in different order each time the players plays the game
        ///</summary>
        public void Shuffle()
        {
            int i = MaxColumns - 1; int j = MaxRows - 1;
            for (int k = 0; k < 100; k++)
            {

                switch (Random.Range(0, 5))
                {
                    case 1:
                        i -= 1;
                        break;

                    case 2:
                        i += 1;

                        break;

                    case 3:
                        j += 1;
                        break;

                    case 4:
                        j -= 1;
                        break;

                    default:
                        break;

                }
                CheckIfTheValueIsInRange(ref i, ref j);
                if (FindIfEmptySpaceExits(i, j))
                {
                    Swap(toAnimateI, toAnimateJ, i, j);
                }
            }
        }

        ///<summary>
        ///Swap the image in the cavas and there relative information on the page
        ///</summary>
        private void Swap(int i, int j, int ri, int rj)
        {
            SwapImage(i, j, ri, rj);
            SwapPosition(i, j, ri, rj);
            EachImages[i, j].SetRowColumn(i, j);
            EachImages[ri, rj].SetRowColumn(ri, rj);
        }

        /// <summary>
        /// Swap the position of the cards
        /// </summary>
        private void SwapPosition(int i, int j, int ri, int rj)
        {
            Vector3 position = EachImages[i, j].transform.position;
            EachImages[i, j].transform.position = EachImages[ri, rj].transform.position;
            EachImages[ri, rj].transform.position = position;
        }

         /// <summary>
        /// Swap the image index in the 2D array
        /// </summary>
        private void SwapImage(int i, int j, int ri, int rj)
        {
            EachImage temp = EachImages[i, j];
            EachImages[i, j] = EachImages[ri, rj];
            EachImages[ri, rj] = temp;
        }

        ///<summary>
        ///Identify if the value doesnot go beyound the predefine range
        ///For instance on 3 * 3 puzzle there isn't 4th image
        ///</summary>
        private void CheckIfTheValueIsInRange(ref int ri, ref int rj)
        {
            if (ri < 0) ri = 0;
            if (ri > MaxColumns - 1) ri = MaxColumns - 1;
            if (rj < 0) rj = 0;
            if (rj > MaxRows - 1) rj = MaxRows - 1;
        }

        // Update is called once per frame
        void Update()
        {
            if (gameOver) return;
            CheckInput();
            System.TimeSpan ts = System.DateTime.UtcNow - startTime;
            if (ts.Seconds != time)
            {
                time = ts.Seconds;
                timePassed_TMP.text = "Total TiME : " + time;
            }
        }

        ///<summary>
        ///Check if the player touch/ press above the piece
        ///</summary>
        private void CheckInput()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    EachImage foundEachImage = hit.transform.GetComponent<EachImage>();
                    int CurrentRow = foundEachImage.CurrentRow;
                    int CurrentColumn = foundEachImage.CurrentColumn;
                    if (FindIfEmptySpaceExits(CurrentRow, CurrentColumn))
                        SwapImage(CurrentRow, CurrentColumn);
                    CheckForVictory();
                }
            }
        }

        private bool FindIfEmptySpaceExits(int CurrentRow, int CurrentColumn)
        {
            if (CurrentRow - 1 >= 0 && EachImages[CurrentRow - 1, CurrentColumn].isInActive)
            {
                toAnimateI = CurrentRow - 1; toAnimateJ = CurrentColumn;
                return true;

            }
            else if (CurrentColumn - 1 >= 0 && EachImages[CurrentRow, CurrentColumn - 1].isInActive)
            {

                toAnimateI = CurrentRow; toAnimateJ = CurrentColumn - 1;
                return true;
            }
            else if (CurrentRow + 1 < MaxRows && EachImages[CurrentRow + 1, CurrentColumn].isInActive)
            {

                toAnimateI = CurrentRow + 1; toAnimateJ = CurrentColumn;
                return true;
            }
            else if (CurrentColumn + 1 < MaxColumns && EachImages[CurrentRow, CurrentColumn + 1].isInActive)
            {

                toAnimateI = CurrentRow; toAnimateJ = CurrentColumn + 1;
                return true;
            }
            return false;
        }
        private void SwapImage(int CurrentRow, int CurrentColumn)
        {
            PlayAudio(click);
            var EachImageToAnimate = EachImages[CurrentRow, CurrentColumn];
            EachImageToAnimate.SetRowColumn(CurrentRow, CurrentColumn);
            stepTaken_TMP.text = "Steps Taken: " + stepTaken++;
            Swap(EachImageToAnimate.CurrentRow, EachImageToAnimate.CurrentColumn, toAnimateI, toAnimateJ);
            CheckForVictory();
        }

        private void PlayAudio(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        /// <summary>
        /// Identify if all the image piece are in right position
        /// </summary>
        private void CheckForVictory()
        {

            for (int i = 0; i < MaxColumns; i++)
            {
                for (int j = 0; j < MaxRows; j++)
                {
                    if (EachImages[i, j].CurrentRow != EachImages[i, j].row ||
                        EachImages[i, j].CurrentColumn != EachImages[i, j].column)
                        return;
                }
            }
            gameOver = true;
            PlayAudio(cheer);
            gameFinishCanvas.SetActive(true);
            TotalTime_TMP.text = "Total Time : " + time;
            stepTakenToFinished_TMP.text = "Total Steps : " + stepTaken;

        }
    }

}
