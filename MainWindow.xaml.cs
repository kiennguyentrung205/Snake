using Snack.Models;
using Snack.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace Snack
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // DataContext đã set trong XAML
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not GameViewModel vm)
                return;

            Direction? dir = null;

            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    dir = Direction.Up;
                    break;
                case Key.Down:
                case Key.S:
                    dir = Direction.Down;
                    break;
                case Key.Left:
                case Key.A:
                    dir = Direction.Left;
                    break;
                case Key.Right:
                case Key.D:
                    dir = Direction.Right;
                    break;
            }

            if (dir.HasValue)
            {
                vm.ChangeDirectionCommand.Execute(dir.Value);
            }
        }
    }
}