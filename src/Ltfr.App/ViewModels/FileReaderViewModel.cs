using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ltfr.App
{
    public class FileReaderViewModel : BaseViewModel
    {
        private FileReaderManager _readerManager;
        private string _fileName;
        private bool _isReading;
        private string _fileContent;
        private CancellationTokenSource _ctk;
        private ObservableCollection<TextLine> _lines;
        private long _numberOfLines;

        public long NumberOfLines { get => _numberOfLines; set => SetValue(ref _numberOfLines, value); }

        public string FileContent { get => _fileContent; set => SetValue(ref _fileContent, value); }

        public bool IsReading { get => _isReading; set => SetValue(ref _isReading, value); }

        public ICommand OpenFileCommand { get; }

        public ICommand ReadCommand { get; }

        public ICommand CancelCommad { get; }

        public string FileName { get => _fileName; set => SetValue(ref _fileName, value); }

        public ObservableCollection<TextLine> Lines { get => _lines; private set => SetValue(ref _lines, value); }

        public FileReaderViewModel()
        {
            _lines = new ObservableCollection<TextLine>();
            _readerManager = new FileReaderManager();
            _readerManager.OnBlockRead += OnFileBlockRead;

            OpenFileCommand = new RelayCommand(OpenFile);
            CancelCommad = new RelayCommand(Cancel);
            ReadCommand = new RelayCommand(async () => await ReadFile());

            var file = @"C:\Users\balba\Desktop\Bigdata Workshp\ar-en.ar";
            _readerManager.SetFile(file);
            FileName = file;
            _ctk = new CancellationTokenSource();
        }

        private void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = "*.*",
                Filter = "Text documents (.txt)|*.txt | All Files|*.*"
            };

            var isFileSelected = dialog.ShowDialog();

            if (isFileSelected == true)
            {
                FileName = dialog.FileName;
                _readerManager.SetFile(FileName);
                _ctk = new CancellationTokenSource();
               
            }
        }

        private async Task ReadFile()
        {
            _ctk = new CancellationTokenSource();
            NumberOfLines = 0;

            IsReading = true;

            Lines.Clear();

            await _readerManager.ReadFile(_ctk.Token);
            
            IsReading = false;
        }

        private void Cancel()
        {
            _ctk?.Cancel();
        }

        private void OnFileBlockRead(object sender, System.Buffers.ReadOnlySequence<byte> e)
        {
            var data = Encoding.UTF8.GetString(e);

            Lines.Add(new TextLine(data));
            NumberOfLines++;
        }
    }
}
