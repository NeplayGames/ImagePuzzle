using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class GameController : MonoBehaviour
{
    public EachImage[,] EachImages = new EachImage[4, 4];

    Vector3 screenPositionToAnimate;
    private EachImage EachImageToAnimate;
    private int toAnimateI, toAnimateJ;

    [SerializeField] int MaxColumns = 4;
    [SerializeField] int MaxRows = 4;
    [SerializeField] Text stepTakenText;
    [SerializeField] Text TotalTime;
    [SerializeField] Text timePassed;
    [SerializeField] Text stepTakenToFinished;
    [SerializeField] GameObject gameFinished;
    EachImage transferPoint;

    AudioSource audioSource;
    [SerializeField] AudioClip click;
    [SerializeField] AudioClip cheer;
   
    [SerializeField] Transform go;
    int time = 0;
    public System.DateTime startTime;
    bool gameOver = false;
    int stepTaken = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < MaxColumns; i++)
        {
            for (int j = 0; j < MaxRows; j++)
            {
                EachImages[i, j] = go.GetChild(i * MaxColumns + j).gameObject.GetComponent<EachImage>();
            }
        }
        Shuffle();
        startTime = System.DateTime.UtcNow;
    }

    ///<summary>
    ///Suffle the image in different order each time the players plays the game
    ///</summary>
    Vector3 pos;
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
            pos = EachImages[i, j].transform.position;
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
        print("Swapped");
        EachImage temp = EachImages[i, j];
        EachImages[i, j] = EachImages[ri, rj];
        EachImages[ri, rj] = temp;
        Vector3 position = EachImages[i, j].imageObject.transform.position;
        EachImages[i, j].imageObject.transform.position = EachImages[ri, rj].imageObject.transform.position;
        EachImages[ri, rj].imageObject.transform.position = position;
        EachImages[i, j].CurrentRow = i;
        EachImages[i, j].CurrentColumn = j;
        EachImages[ri, rj].CurrentRow = ri;
        EachImages[ri, rj].CurrentColumn = rj;
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
            timePassed.text = "Total TiME : " + time;
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
                transferPoint = EachImages[toAnimateI, toAnimateJ];
                screenPositionToAnimate = transferPoint.transform.position;
                EachImage foundEachImage = hit.transform.GetComponent<EachImage>();
                int CurrentRow = foundEachImage.CurrentRow;
                int CurrentColumn = foundEachImage.CurrentColumn;
                if (FindIfEmptySpaceExits(CurrentRow, CurrentColumn))
                    IsImageAvailable(CurrentRow, CurrentColumn);
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
    private void IsImageAvailable(int CurrentRow, int CurrentColumn)
    {
        PlayAudio(click);
        EachImageToAnimate = EachImages[CurrentRow, CurrentColumn];
        EachImageToAnimate.CurrentRow = CurrentRow;
        EachImageToAnimate.CurrentColumn = CurrentColumn;
        stepTaken++;
        stepTakenText.text = "Steps Taken: " + stepTaken;
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
        gameFinished.SetActive(true);
        TotalTime.text = "Total Time : " + time;
        stepTakenToFinished.text = "Total Steps : " + stepTaken;

    }

    // private Vector3 GetScreenCordinate(int i, int j)
    // {
    //     float difference = 1f / (MaxColumns + 1f);
    //     Vector3 point = Camera.main.ViewportToWorldPoint(new Vector3(difference * (j * EachImages[i, j].transform.localScale.x  ), 1 - difference * (i* EachImages[i, j].transform.localScale.y  ), 0));
    //     point.z = 0;
    //     return point;
    // }


}
