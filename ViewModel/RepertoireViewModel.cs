using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MusicBand_Manager.Model;
using MusicBand_Manager.DAO;
using MusicBand_Manager.Tool;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace MusicBand_Manager.ViewModel
{
    public class RepertoireViewModel : INotifyPropertyChanged
    {
        private static RepertoireViewModel _instance;
        private readonly RepertoireSQLiteDAO _repertoireSQLiteDAO;
        private readonly MemberSQLiteDAO _memberSQLiteDAO;
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

        public ObservableCollection<RepertoireSong> RepertoireSongs { get; set; }
        public ObservableCollection<Member> Members { get; set; }
        public InstrumentProgression SelectedInstrumentProgression { get; set; }
        public ICommand AddInstrumentProgressionCommand { get; private set; }
        public ICommand EditInstrumentProgressionCommand { get; private set; }
        public ICommand DeleteInstrumentProgressionCommand { get; private set; }
        public ICommand OpenLinkCommand { get; private set; }


        private void HandleLinkClick(object sender, MouseButtonEventArgs e)
        {
            var link = ((TextBlock)sender).Text;
            // Invoke the OpenLinkCommand with the link as the command parameter
            OpenLinkCommand?.Execute(link);
        }


        private void OpenLinkInBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Console.WriteLine($"Failed to open link: {ex.Message}");
            }
        }



        private bool CanAddInstrumentProgression()
        {
            return SelectedSong != null;
        }

        private bool CanEditInstrumentProgression(InstrumentProgression instrumentProgression)
        {
            return SelectedSong != null && instrumentProgression != null;
        }

        private bool CanDeleteInstrumentProgression(InstrumentProgression instrumentProgression)
        {
            return SelectedSong != null && instrumentProgression != null;
        }


        private RepertoireViewModel()
        {
            _repertoireSQLiteDAO = new RepertoireSQLiteDAO();
            _memberSQLiteDAO = new MemberSQLiteDAO();
            RepertoireSongs = new ObservableCollection<RepertoireSong>(_repertoireSQLiteDAO.GetAllRepertoireSongs());
            Members = new ObservableCollection<Member>(_memberSQLiteDAO.GetAllMembers());
            AddCommand = new RelayCommand((o) => AddSong(), (o) => CanAddSong());
            EditCommand = new RelayCommand((o) => EditSong(), (o) => CanEditSong());
            DeleteCommand = new RelayCommand((o) => DeleteSong(), (o) => CanDeleteSong());
            AddInstrumentProgressionCommand = new RelayCommand((o) => AddInstrumentProgression(), (o) => CanAddInstrumentProgression());
            EditInstrumentProgressionCommand = new RelayCommand((o) => EditInstrumentProgression(o as InstrumentProgression), (o) => CanEditInstrumentProgression(o as InstrumentProgression));
            DeleteInstrumentProgressionCommand = new RelayCommand((o) => DeleteInstrumentProgression(o as InstrumentProgression), (o) => CanDeleteInstrumentProgression(o as InstrumentProgression));
            OpenLinkCommand = new RelayCommand((o) => OpenLinkInBrowser(o as String));
        }

        private void AddInstrumentProgression()
        {
            if (SelectedSong != null)
            {
                var newInstrumentProgression = new InstrumentProgression()
                {
                    Instrument = "LambdaInstrument"
                };
                SelectedSong.InstrumentProgressions.Add(newInstrumentProgression);

                // Add instrument progression to the database
                _repertoireSQLiteDAO.AddInstrumentProgression(SelectedSong.Id, newInstrumentProgression);
            }
        }

        private void EditInstrumentProgression(InstrumentProgression instrumentProgression)
        {
            if (SelectedSong != null && instrumentProgression != null)
            {
                // Update instrument progression in the database
                _repertoireSQLiteDAO.EditInstrumentProgression(SelectedSong.Id, instrumentProgression);
            }
        }

        private void DeleteInstrumentProgression(InstrumentProgression instrumentProgression)
        {
            if (SelectedSong != null && instrumentProgression != null)
            {
                SelectedSong.InstrumentProgressions.Remove(instrumentProgression);

                // Delete instrument progression from the database
                _repertoireSQLiteDAO.DeleteInstrumentProgression(instrumentProgression);
            }
        }

        private void AddSong()
        {
            var newSong = new RepertoireSong
            {
                Title = NewSongTitle,
                Style = NewSongStyle,
                OriginalComposer = NewSongOriginalComposer,
                InstrumentProgressions = new List<InstrumentProgression>()
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
