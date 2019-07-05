using FinalAssignment.ViewModels;
using System.Windows;
using System.Windows.Documents;

namespace FinalAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainViewModel;
        }

        private void LoadInputButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            System.Nullable<bool> result = dlg.ShowDialog();

            string fileLocation = dlg.FileName;
            this.mainViewModel.LoadInput(fileLocation);
            RichTB.Document.Blocks.Clear();
            RichTB.Document.Blocks.Add(new Paragraph(new Run(mainViewModel.InputText)));
        }

        private void ProccessInputButton_Click(object sender, RoutedEventArgs e)
        {
            this.mainViewModel.InputText = new TextRange(RichTB.Document.ContentStart, RichTB.Document.ContentEnd).Text;
            this.mainViewModel.ProcessInput();
        }
    }
}
