using MusicBand_Manager.DAO;
using MusicBand_Manager.Model;
using MusicBand_Manager.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicBand_Manager.ViewModel
{
    public class MemberViewModel : INotifyPropertyChanged
    {
        private static MemberViewModel _instance;
        private readonly MemberSQLiteDAO _memberSQLiteDAO;
        private Member _selectedMember;
        private Member _uneditedMember;

        public static MemberViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MemberViewModel();

                return _instance;
            }
        }

        public ObservableCollection<Member> Members { get; set; }

        public Member SelectedMember
        {
            get => _selectedMember;
            set
            {
                if (value != null)
                {
                    _selectedMember = value;
                }
                else
                {
                    _selectedMember = null;
                }
                OnPropertyChanged(nameof(SelectedMember));
            }
        }

        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        private string _newMemberFullName;
        public string NewMemberFullName
        {
            get => _newMemberFullName;
            set
            {
                if (_newMemberFullName != value)
                {
                    _newMemberFullName = value;
                    OnPropertyChanged(nameof(NewMemberFullName));
                }
            }
        }

        private DateTime _newMemberArrivalDate;
        public DateTime NewMemberArrivalDate
        {
            get => _newMemberArrivalDate;
            set
            {
                if (_newMemberArrivalDate != value)
                {
                    _newMemberArrivalDate = value;
                    OnPropertyChanged(nameof(NewMemberArrivalDate));
                }
            }
        }

        private MemberViewModel()
        {
            _memberSQLiteDAO = new MemberSQLiteDAO();
            Members = new ObservableCollection<Member>(_memberSQLiteDAO.GetAllMembers());

            AddCommand = new RelayCommand((o) => AddMember(), (o) => CanAddMember());
            EditCommand = new RelayCommand((o) => EditMember(), (o) => CanEditMember());
            DeleteCommand = new RelayCommand((o) => DeleteMember(), (o) => CanDeleteMember());
        }

        private void AddMember()
        {
            var newMember = new Member
            {
                FullName = NewMemberFullName,
                ArrivalDate = NewMemberArrivalDate
            };

            Members.Add(newMember);
            SelectedMember = newMember;

            // Add song to the database
            _memberSQLiteDAO.AddMember(newMember);

            // Clear the form inputs
            NewMemberFullName = string.Empty;
            NewMemberArrivalDate = DateTime.Now;
        }

        private bool CanAddMember()
        {
            return !string.IsNullOrEmpty(NewMemberFullName);
        }


        private void EditMember()
        {
            // Update song in the database
            _memberSQLiteDAO.UpdateMember(SelectedMember);
            _uneditedMember.FullName = SelectedMember.FullName;
            _uneditedMember.ArrivalDate = SelectedMember.ArrivalDate;
            _uneditedMember.DepartureDate = SelectedMember.DepartureDate;
        }

        private bool CanEditMember()
        {
            if (SelectedMember == null)
                return false;
            else
            {
                if (_uneditedMember == null)
                {
                    _uneditedMember = new Member()
                    {
                        Id = SelectedMember.Id,
                        FullName = SelectedMember.FullName,
                        ArrivalDate = SelectedMember.ArrivalDate,
                        DepartureDate = SelectedMember.DepartureDate
                    };
                }

                if (SelectedMember.Id != _uneditedMember.Id)
                {
                    // Retrieve the unedited member with the same ID
                    Member? uneditedMember = Members.FirstOrDefault(x => x.Id == _uneditedMember.Id);

                    if (uneditedMember != null)
                    {
                        // Update the selected member's properties with the unedited member's values
                        uneditedMember.FullName = _uneditedMember.FullName;
                        uneditedMember.ArrivalDate = _uneditedMember.ArrivalDate;
                        uneditedMember.DepartureDate = _uneditedMember.DepartureDate;
                    }
                    _uneditedMember = null;
                    return false;
                }

                // Compare each property value to check if any has changed
                if (SelectedMember.FullName != _uneditedMember.FullName ||
                    SelectedMember.ArrivalDate != _uneditedMember.ArrivalDate ||
                    SelectedMember.DepartureDate != _uneditedMember.DepartureDate)
                {
                    return true; // At least one property has changed
                }

                return false; // No changes in any property
            }

            // Compare the Ids of the selected member and the unedited member
            
        }

        private bool CanDeleteMember()
        {
            return SelectedMember != null;
        }

        private void DeleteMember()
        {
            if (SelectedMember != null)
            {

                bool success = _memberSQLiteDAO.DeleteMember(SelectedMember);
                if (success)
                {
                    Members.Remove(SelectedMember);
                }
            }
        }

        public void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Get the edited cell's value
            var editedValue = ((TextBox)e.EditingElement).Text;

            // Get the edited song
            var editedMember = (Member)e.Row.Item;

            // Update the selected song with the edited value
            if (e.Column.Header.ToString() == "FullName")
                editedMember.FullName = ((TextBox)e.EditingElement).Text;
            else if (e.Column.Header.ToString() == "ArrivalDate")
                editedMember.ArrivalDate = ((DatePicker)e.EditingElement).SelectedDate != null ? (DateTime)((DatePicker)e.EditingElement).SelectedDate : DateTime.Now;
            else if (e.Column.Header.ToString() == "DepartureDate")
                editedMember.DepartureDate = ((DatePicker)e.EditingElement).SelectedDate;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
