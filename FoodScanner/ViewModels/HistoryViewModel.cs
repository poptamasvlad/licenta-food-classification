using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodScanner.Models;

namespace FoodScanner.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<ScanResult> ScanHistory { get; } = new();

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set => SetProperty(ref _isEmpty, value);
        }

        public HistoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Istoric scanări";
        }

        public async Task LoadHistoryAsync()
        {
            IsBusy = true;
            ScanHistory.Clear();

            var results = await _databaseService.GetAllScansAsync();
            foreach (var item in results)
                ScanHistory.Add(item);

            IsEmpty = ScanHistory.Count == 0;
            IsBusy = false;
        }

        public async Task DeleteScanAsync(ScanResult scan)
        {
            await _databaseService.DeleteScanAsync(scan);
            ScanHistory.Remove(scan);
            IsEmpty = ScanHistory.Count == 0;
        }
    }
}
