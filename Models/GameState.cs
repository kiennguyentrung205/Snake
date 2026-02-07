using System.Collections.ObjectModel;

namespace Snack.Models
{
    public class GameState
    {
        public ObservableCollection<Cell> SnakeParts { get; set; }
        public Cell Food { get; set; }

        public int Rows { get; }
        public int Columns { get; }

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            SnakeParts = new ObservableCollection<Cell>();
        }
    }
}