using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace FenDraw.ViewModel
{
    class  MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private float _hue, _saturation, _luminosity, _linesize = 1;
        private double _width, _height;
        private Color _color;

        public float Hue
        {
            get => _hue;
            set
            {
                if (_hue != value)
                    Color = Color.FromHsla(value, _saturation, _luminosity);
            }
        }

        public float Saturation
        {
            get => _saturation;
            set
            {
                if (_saturation != value)
                    Color = Color.FromHsla(_hue, value, _luminosity);
            }
        }

        public float Luminosity
        {
            get => _luminosity;
            set
            {
                if (_luminosity != value)
                    Color = Color.FromHsla(_hue, _saturation, value);
            }
        }

        public float Size
        {
            get => _linesize;
            set
            {
                if (_linesize != value)
                    _linesize = value;
                    OnPropertyChanged();
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                if (_width != value)
                    _width = value;
                OnPropertyChanged();
            }
        }
        public double Height
        {
            get => _height;
            set
            {
                if (_height != value)
                    _height = value;
                OnPropertyChanged();
            }
        }
        public Color Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    _hue = _color.GetHue();
                    _saturation = _color.GetSaturation();
                    _luminosity = _color.GetLuminosity();

                    OnPropertyChanged("Hue");
                    OnPropertyChanged("Saturation");
                    OnPropertyChanged("Luminosity");
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
