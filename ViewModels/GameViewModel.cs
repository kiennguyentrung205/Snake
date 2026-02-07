using Snack.Infrastructure;
using Snack.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace Snack.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        // Kích thước board (số ô)
        public int Rows { get; } = 20;
        public int Columns { get; } = 20;

        // Kích thước mỗi ô (pixel) - để View dùng khi vẽ
        private int _cellSize = 20;
        public int CellSize
        {
            get => _cellSize;
            set { _cellSize = value; OnPropertyChanged(); }
        }

        private readonly DispatcherTimer _timer;
        private Direction _currentDirection = Direction.Right;
        private bool _isRunning;

        public GameState State { get; }

        private int _score;
        public int Score
        {
            get => _score;
            set { _score = value; OnPropertyChanged(); }
        }

        private string _statusText = "Ready";
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Cell> SnakeParts => State.SnakeParts;
        public Cell Food
        {
            get => State.Food;
            set
            {
                State.Food = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand ChangeDirectionCommand { get; }

        public GameViewModel()
        {
            State = new GameState(Rows, Columns);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150) // tốc độ rắn
            };
            _timer.Tick += TimerOnTick;

            StartCommand = new RelayCommand(_ => Start(), _ => !_isRunning);
            PauseCommand = new RelayCommand(_ => Pause(), _ => _isRunning);
            RestartCommand = new RelayCommand(_ => Restart());
            ChangeDirectionCommand = new RelayCommand(param =>
            {
                if (param is Direction dir)
                {
                    ChangeDirection(dir);
                }
            });

            InitGame();
        }

        private void InitGame()
        {
            SnakeParts.Clear();
            Score = 0;
            StatusText = "Ready";
            _currentDirection = Direction.Right;

            // tạo rắn ban đầu
            int midRow = Rows / 2;
            int midCol = Columns / 2;

            SnakeParts.Add(new Cell(midRow, midCol));
            SnakeParts.Add(new Cell(midRow, midCol - 1));
            SnakeParts.Add(new Cell(midRow, midCol - 2));

            SpawnFood();
        }

        private void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            StatusText = "Running";
            _timer.Start();
            RaiseCommandCanExecutes();
        }

        private void Pause()
        {
            if (!_isRunning) return;
            _isRunning = false;
            StatusText = "Paused";
            _timer.Stop();
            RaiseCommandCanExecutes();
        }

        private void Restart()
        {
            _timer.Stop();
            _isRunning = false;
            InitGame();
            RaiseCommandCanExecutes();
        }

        private void TimerOnTick(object? sender, EventArgs e)
        {
            MoveSnake();
        }

        private void MoveSnake()
        {
            if (SnakeParts.Count == 0) return;

            var head = SnakeParts[0];
            int newRow = head.Row;
            int newCol = head.Column;

            switch (_currentDirection)
            {
                case Direction.Up: newRow--; break;
                case Direction.Down: newRow++; break;
                case Direction.Left: newCol--; break;
                case Direction.Right: newCol++; break;
            }

            // va chạm tường
            if (newRow < 0 || newRow >= Rows || newCol < 0 || newCol >= Columns)
            {
                GameOver();
                return;
            }

            // va chạm thân
            foreach (var part in SnakeParts)
            {
                if (part.Row == newRow && part.Column == newCol)
                {
                    GameOver();
                    return;
                }
            }

            // Thêm đầu mới
            SnakeParts.Insert(0, new Cell(newRow, newCol));

            // Ăn thức ăn?
            if (newRow == Food.Row && newCol == Food.Column)
            {
                Score += 10;
                SpawnFood();
            }
            else
            {
                // không ăn -> bỏ đuôi
                SnakeParts.RemoveAt(SnakeParts.Count - 1);
            }
        }

        private void SpawnFood()
        {
            var rand = new Random();
            Cell newFood;

            while (true)
            {
                int r = rand.Next(0, Rows);
                int c = rand.Next(0, Columns);
                bool onSnake = false;
                foreach (var part in SnakeParts)
                {
                    if (part.Row == r && part.Column == c)
                    {
                        onSnake = true;
                        break;
                    }
                }

                if (!onSnake)
                {
                    newFood = new Cell(r, c);
                    break;
                }
            }

            Food = newFood;
        }

        private void ChangeDirection(Direction newDirection)
        {
            // Không cho quay 180 độ ngay lập tức
            if ((_currentDirection == Direction.Up && newDirection == Direction.Down) ||
                (_currentDirection == Direction.Down && newDirection == Direction.Up) ||
                (_currentDirection == Direction.Left && newDirection == Direction.Right) ||
                (_currentDirection == Direction.Right && newDirection == Direction.Left))
            {
                return;
            }

            _currentDirection = newDirection;
        }

        private void GameOver()
        {
            _timer.Stop();
            _isRunning = false;
            StatusText = "Game Over";
            RaiseCommandCanExecutes();
        }

        private void RaiseCommandCanExecutes()
        {
            (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PauseCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}