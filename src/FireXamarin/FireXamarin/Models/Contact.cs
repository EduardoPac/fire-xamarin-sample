using BurgerMonkeys.Tools;

namespace FireXamarin.Models
{
    public class Contact : BaseModel
    {
        public string Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _locationName;
        public string LocationName
        {
            get => _locationName;
            set => SetProperty(ref _locationName, value);
        }

        private double _locationLatitude;
        public double LocationLatitude
        {
            get => _locationLatitude;
            set => SetProperty(ref _locationLatitude, value);
        }

        private double _locationLongitude;
        public double LocationLongitude
        {
            get => _locationLongitude;
            set => SetProperty(ref _locationLongitude, value);
        }

        #region Internal_List
        private bool _isFirst;
        public bool IsFirst
        {
            get => _isFirst;
            set => SetProperty(ref _isFirst, value);
        }

        private bool _isLast;
        public bool IsLast
        {
            get => _isLast;
            set => SetProperty(ref _isLast, value);
        }

        public bool Removed { get; set; }

        public bool HasLocation { get => LocationLatitude != 0; }

        public bool HasEmail { get => Email.IsNotNullOrWhiteSpace(); }

        #endregion

        public bool ValidatePropertiesRequired()
        {
            return !string.IsNullOrWhiteSpace(Id) &&
                   !string.IsNullOrWhiteSpace(Name) &&
                   !string.IsNullOrWhiteSpace(Phone);
        }
    }
}