using System;
using ZondervanLibrary.SharedLibrary;

namespace ZondervanLibrary.Harvester.Wpf.Dialogs.Repository.ViewModels
{
    public class SushiJournalRepositoryViewModel : ViewModelBase, IJournalRepositoryViewModel
    {
        public String Name => "SUSHI";

        public String Description => "SUSHI";

        private String _sushiUrl;
        public String SushiUrl
        {
            get => _sushiUrl;
            set => RaiseAndSetIfPropertyChanged(ref _sushiUrl, value);
        }

        private String _requestorId;
        public String RequestorId
        {
            get => _requestorId;
            set => RaiseAndSetIfPropertyChanged(ref _requestorId, value);
        }

        private String _customerId;
        public String CustomerId
        {
            get => _customerId;
            set => RaiseAndSetIfPropertyChanged(ref _customerId, value);
        }
    }
}
