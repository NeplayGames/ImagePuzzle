using UnityEngine;

namespace ImagePuzzle
{
  public class EachImage : MonoBehaviour
  {
    [field: SerializeField] public bool isInActive { get; private set; } = false;
    public int row { get; private set; }
    public int column { get; private set; }
    public int CurrentRow { get; private set; }
    public int CurrentColumn { get; private set; }

    private void Awake()
    {
      string name = gameObject.name;
      row = CurrentRow = int.Parse(name) / 10;
      column = CurrentColumn = int.Parse(name) % 10;
    }

    public void SetRowColumn(int row, int column)
    {
      CurrentRow = row;
      CurrentColumn = column;
    }
  }
}