using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DevExpress.Mvvm;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using SignalREvaulation.Contracts.Models;

namespace SignalRClient
{
    public class ViewModel : ViewModelBase
    {
        private readonly RestClient _restClient;

        public ViewModel()
        {
            _restClient = new RestClient("http://localhost:9000");

            AddCommand = new DelegateCommand(AddNewPerson);
            DeleteCommand = new DelegateCommand(DeletePerson);
            RefreshCommand = new DelegateCommand(LoadPersons);
            UpdateCommand = new DelegateCommand(Update);

            Persons = new ObservableCollection<PersonDataViewModel>();

            InitPersonHub();
        }

        private async void InitPersonHub()
        {
            var hubConnection = new HubConnectionBuilder().WithUrl("http://localhost:9000/gateway/hub/person").Build();
            hubConnection.On<Person>("Add", OnNewPersonAdded);
            hubConnection.On<long>("Delete", OnPersonDeleted);
            hubConnection.On<Person>("Change", OnPersonChanged);

            await hubConnection.StartAsync();
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand UpdateCommand { get; }

        public string Name
        {
            get => GetProperty(() => Name);
            set => SetProperty(() => Name, value);
        }

        public string BirthDate
        {
            get => GetProperty(() => BirthDate);
            set => SetProperty(() => BirthDate, value);
        }

        public string BodySize
        {
            get => GetProperty(() => BodySize);
            set => SetProperty(() => BodySize, value);
        }

        public ObservableCollection<PersonDataViewModel> Persons
        {
            get => GetProperty(() => Persons);
            set => SetProperty(() => Persons, value);
        }

        public PersonDataViewModel SelectedPerson
        {
            get => GetProperty(() => SelectedPerson);
            set => SetProperty(() => SelectedPerson, value);
        }

        public void LoadPersons()
        {
            try
            {
                var restRequest = new RestRequest("person", Method.GET);
                var restResponse = _restClient.Execute<List<PersonDataViewModel>>(restRequest);
                Persons = new ObservableCollection<PersonDataViewModel>(restResponse.Data);
            }
            catch (Exception exception)
            {
                GetService<IMessageBoxService>().Show($"Ein Fehler ist aufgetreten: {Environment.NewLine}{exception}");
            }
        }

        public void Update()
        {
            var restRequest = new RestRequest("person", Method.PUT) { RequestFormat = DataFormat.Json };
            restRequest.AddBody(SelectedPerson);

            _restClient.Execute(restRequest);
        }

        public void DeletePerson()
        {
            var restRequest = new RestRequest("person/{id}", Method.DELETE);
            restRequest.AddUrlSegment("id",SelectedPerson?.Id);

            _restClient.Execute(restRequest);
        }

        public void AddNewPerson()
        {
            var person = new Person
            {
                BirthDate = BirthDate,
                Name = Name,
                BodySize = BodySize
            };
            var restRequest = new RestRequest("person", Method.POST) {RequestFormat = DataFormat.Json};
            restRequest.AddBody(person);

            _restClient.Execute(restRequest);
        }

        private void OnPersonChanged(Person person)
        {
            var changedPerson = GetPersonById(person.Id);
            GetService<IDispatcherService>().BeginInvoke(() =>
            {
                changedPerson.BirthDate = person.BirthDate;
                changedPerson.Name = person.Name;
                changedPerson.BodySize = person.BodySize;
            });
        }

        private void OnPersonDeleted(long personId)
        {
            var personToDelete = GetPersonById(personId);
            GetService<IDispatcherService>().BeginInvoke(() => Persons.Remove(personToDelete));
        }

        private void OnNewPersonAdded(Person person)
        {
            var newPersonDataViewModel = new PersonDataViewModel
            {
                Id = person.Id,
                BirthDate = person.BirthDate,
                BodySize = person.BodySize,
                Name = person.Name
            };
            GetService<IDispatcherService>().BeginInvoke(() => Persons.Add(newPersonDataViewModel));
        }

        private PersonDataViewModel GetPersonById(long personId)
        {
            var personToDelete = Persons.FirstOrDefault(p => p.Id == personId);
            return personToDelete;
        }
    }

    public class PersonDataViewModel : BindableBase
    {
        public long Id { get; set; }

        public string Name
        {
            get => GetProperty(() => Name);
            set => SetProperty(() => Name, value);
        }

        public string BirthDate
        {
            get => GetProperty(() => BirthDate);
            set => SetProperty(() => BirthDate, value);
        }

        public string BodySize
        {
            get => GetProperty(() => BodySize);
            set => SetProperty(() => BodySize, value);
        }
    }
}