namespace Kalantyr.Auth.InternalModels
{
    public class UserRecord
    {
        public uint Id { get; set; }

        public bool IsDisabled { get; set; }

        public string Login { get; set; }
    }
}
