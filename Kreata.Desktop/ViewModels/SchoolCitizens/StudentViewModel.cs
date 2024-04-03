using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kreta.HttpService.Service;
using Kreta.Shared.Models.SchoolCitizens;
using Kreta.Shared.Responses;
using Kreta.Desktop.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Kreta.Desktop.ViewModels.SchoolCitizens
{
    public partial class StudentViewModel : BaseViewModelWithAsyncInitialization
    {
        private readonly IStudentService? _studentService;

        [ObservableProperty]
        private ObservableCollection<string> _educationLevels;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteButtonVisible))]
        private ObservableCollection<Student> _students = new();

        [ObservableProperty]
        private Student? _selectedStudent;

        public bool IsDeleteButtonVisible => Students is not null && Students.Any();

        private string _selectedEducationLevel = string.Empty;
        public string SelectedEducationLevel
        {
            get => _selectedEducationLevel;
            set
            {
                SetProperty(ref _selectedEducationLevel, value);
                if (SelectedStudent is not null)
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
                    SelectedStudent = Students.FirstOrDefault(student =>student.Id == newStudent.Id);
                    if (SelectedStudent is null && result.Id!=Guid.Empty)
                        SelectedStudent = Students.FirstOrDefault(student =>student.Id == result.Id);
                    SelectedStudent = SelectedStudent ?? new();
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
                if (Students.Any())
                    SelectedStudent = Students.First();
                else
                    SelectedStudent = new();
            }
        }

        [RelayCommand]
        void DoNewStudent()
        {
            SelectedStudent = new Student();
        }
    }
}
