using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kreta.HttpService.Service;
using Kreta.Shared.Models.SchoolCitizens;
using Kreta.Shared.Responses;
using Kreta.Desktop.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kreta.Desktop.ViewModels.SchoolCitizens
{
    public partial class StudentViewModel : BaseViewModelWithAsyncInitialization
    {
        private readonly IStudentService? _studentService;

        [ObservableProperty]
        private ObservableCollection<string> _educationLevels;

        [ObservableProperty]
        private ObservableCollection<Student> _students = new();

        [ObservableProperty]
        private Student _selectedStudent;

        private string _selectedEducationLevel = string.Empty;
        public string SelectedEducationLevel
        {
            get => _selectedEducationLevel;
            set
            {
                SetProperty(ref _selectedEducationLevel, value);
                SelectedStudent.EducationLevel = _selectedEducationLevel;
            }
        }

        public StudentViewModel()
        {
            SelectedStudent = new Student();
            SelectedEducationLevel = string.Empty;
            EducationLevels = new ObservableCollection<string>();

            _studentService = new StudentService();
        }

        public StudentViewModel(IStudentService? studentService)
        {
            SelectedStudent = new Student();
            SelectedEducationLevel = string.Empty;
            EducationLevels = new ObservableCollection<string>();

            _studentService = studentService;
        }

        public async override Task InitializeAsync()
        {
            await UpdateView();
        }

        [RelayCommand]
        public async Task DoSave(Student newStudent)
        {
            if (_studentService is not null)
            {
                ControllerResponse result = new();
                if (newStudent.HasId)
                    result = await _studentService.UpdateAsync(newStudent);
                else
                    result = await _studentService.InsertAsync(newStudent);

                if (!result.HasError)
                {
                    await UpdateView();
                }
            }
        }

        [RelayCommand]
        public async Task DoRemove(Student studentToDelete)
        {
            if (_studentService is not null)
            {
                ControllerResponse result = await _studentService.DeleteAsync(studentToDelete.Id);
                if (result.IsSuccess)
                {
                    await UpdateView();
                }
            }
        }

        private async Task UpdateView()
        {
            if (_studentService is not null)
            {
                List<Student> students = await _studentService.SelectAllStudentAsync();
                Students = new ObservableCollection<Student>(students);
            }
        }

        [RelayCommand]
        void DoNewStudent()
        {
            SelectedStudent = new Student();
        }
    }
}
