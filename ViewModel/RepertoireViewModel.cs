using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MusicBand_Manager.Model;
using MusicBand_Manager.DAO;
using MusicBand_Manager.Tool;
using System.Windows.Controls;

namespace MusicBand_Manager.ViewModel
{
    public class RepertoireViewModel : INotifyPropertyChanged
    {
        private static RepertoireViewModel _instance;
        private readonly RepertoireSQLiteDAO _repertoireSQLiteDAO;
        private RepertoireSong _selectedSong;
        private RepertoireSong _uneditedSong;

        public static RepertoireViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RepertoireViewModel();

                return _instance;
            }
        }

        public ObservableCollection<RepertoireSong> RepertoireSongs { get; set; }

        public RepertoireSong SelectedSong
        {
            get => _selectedSong;
            set
            {
                if (value != null)
                {
                    _selectedSong = value;
                    _uneditedSong = new RepertoireSong()
                    {
                        Id = value.Id,
                        Title = value.Title,
                        Style = value.Style,
                        OriginalComposer = value.OriginalComposer,
                        Lyrics = value.Lyrics
                    };
                }
                else
                {
                    _selectedSong = null;
                }
                OnPropertyChanged(nameof(SelectedSong));
            }
        }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        private string _newSongTitle;
        public string NewSongTitle
        {
            get => _newSongTitle;
            set
            {
                if (_newSongTitle != value)
                {
                    _newSongTitle = value;
                    OnPropertyChanged(nameof(NewSongTitle));
                }
            }
        }

        private string _newSongStyle;
        public string NewSongStyle
        {
            get => _newSongStyle;
            set
            {
                if (_newSongStyle != value)
                {
                    _newSongStyle = value;
                    OnPropertyChanged(nameof(NewSongStyle));
                }
            }
        }

        private string _newSongOriginalComposer;
        public string NewSongOriginalComposer
        {
            get => _newSongOriginalComposer;
            set
            {
                if (_newSongOriginalComposer != value)
                {
                    _newSongOriginalComposer = value;
                    OnPropertyChanged(nameof(NewSongOriginalComposer));
                }
            }
        }


        private RepertoireViewModel()
        {
            _repertoireSQLiteDAO = new RepertoireSQLiteDAO();
            RepertoireSongs = new ObservableCollection<RepertoireSong>(_repertoireSQLiteDAO.GetAllRepertoireSongs());

            AddCommand = new RelayCommand((o) => AddSong(), (o) => CanAddSong());
            EditCommand = new RelayCommand((o) => EditSong(), (o) => CanEditSong());
            DeleteCommand = new RelayCommand((o) => DeleteSong(), (o) => CanDeleteSong());
        }

        private void AddSong()
        {
            var newSong = new RepertoireSong
            {
                Title = NewSongTitle,
                Style = NewSongStyle,
                OriginalComposer = NewSongOriginalComposer
            };

            RepertoireSongs.Add(newSong);
            SelectedSong = newSong;

            // Add song to the database
            _repertoireSQLiteDAO.AddRepertoireSong(newSong);

            // Clear the form inputs
            NewSongTitle = string.Empty;
            NewSongStyle = string.Empty;
            NewSongOriginalComposer = string.Empty;
        }

        private bool CanAddSong()
        {
            return !string.IsNullOrEmpty(NewSongTitle) && !string.IsNullOrEmpty(NewSongStyle) && !string.IsNullOrEmpty(NewSongOriginalComposer);
        }


        private void EditSong()
        {
            // Update song in the database
            _repertoireSQLiteDAO.UpdateRepertoireSong(SelectedSong);
            _uneditedSong.Title = SelectedSong.Title;
            _uneditedSong.Style = SelectedSong.Style;
            _uneditedSong.OriginalComposer = SelectedSong.OriginalComposer;
            _uneditedSong.Lyrics = SelectedSong.Lyrics;
        }

        private bool CanEditSong()
        {
            if (SelectedSong == null)
                return false;

            // Compare the Ids of the selected song and the unedited song
            if (SelectedSong.Id != _uneditedSong.Id)
                return false;

            // Compare each property value to check if any has changed
            if (SelectedSong.Title != _uneditedSong.Title ||
                SelectedSong.Style != _uneditedSong.Style ||
                SelectedSong.OriginalComposer != _uneditedSong.OriginalComposer ||
                SelectedSong.Lyrics != _uneditedSong.Lyrics)
            {
                return true; // At least one property has changed
            }

            return false; // No changes in any property
        }

        private bool CanDeleteSong()
        {
            return SelectedSong != null;
        }

        private void DeleteSong()
        {
            if (SelectedSong != null)
            {

                _repertoireSQLiteDAO.DeleteRepertoireSong(SelectedSong);
                RepertoireSongs.Remove(SelectedSong);
            }
        }

        public void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Get the edited cell's value
            var editedValue = ((TextBox)e.EditingElement).Text;

            // Get the edited song
            var editedSong = (RepertoireSong)e.Row.Item;

            // Update the selected song with the edited value
            if (e.Column.Header.ToString() == "Title")
                editedSong.Title = editedValue;
            else if (e.Column.Header.ToString() == "Style")
                editedSong.Style = editedValue;
            else if (e.Column.Header.ToString() == "Original Composer")
                editedSong.OriginalComposer = editedValue;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
