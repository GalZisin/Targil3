using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Targil3
{
    class ViewModel : INotifyPropertyChanged //IDataErrorInfo
    {

        public event PropertyChangedEventHandler PropertyChanged;
        private string url;
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
                StartCommand.RaiseCanExecuteChanged();
            
            }
        }
        private string _size;

        public string Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged("Size");
            }
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                _isBusy = value;
            }

        }
        public DelegateCommand StartCommand { get; set; }

        //public string Error
        //{
        //    get
        //    {
        //        return string.Empty;
        //    }
        //}

        //public string this[string columnName] => throw new NotImplementedException();

        public ViewModel()
        {
            Size = "Please Wait...";
            StartCommand = new DelegateCommand(() =>
            {
               
              
                Task task = WriteWebRequestSizeAsync(Url);
                OnPropertyChanged("Url");
            }, () => { return CanExecuteStartMethod() || IsBusy != false; });
        }

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }


        static public string FormatBytes(long bytes)
        {
            string[] magnitudes =
                new string[] { "GB", "MB", "KB", "Bytes" };
            long max =
                (long)Math.Pow(1024, magnitudes.Length);

            return string.Format("{1:##.##} {0}",
                magnitudes.FirstOrDefault(
                    magnitude =>
                    bytes > (max /= 1024)) ?? "0 Bytes",
              (decimal)bytes / (decimal)max);
        }

        public async Task WriteWebRequestSizeAsync(string url)
        {
            try
            {
                IsBusy = true;

                WebRequest webRequest = WebRequest.Create(url);
               
                WebResponse response = await webRequest.GetResponseAsync();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string text = await reader.ReadToEndAsync();
                    Size = FormatBytes(text.Length).ToString();
                }
            }
            catch (WebException)
            {
                // ...
            }
            catch (IOException)
            {

                // ...
            }
            catch (NotSupportedException)
            {
                // ...
            }
            finally
            {
                IsBusy = false;


            }


        }
        private bool CanExecuteStartMethod()
        {
            string str = "http";
            if (Url!=null)
            {
                return Url.StartsWith(str);
            }
        
            return false;
        }

    
    }
}
