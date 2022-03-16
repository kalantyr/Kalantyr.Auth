namespace Kalantyr.Auth.Models
{
    public class ResultDto<T>
    {
        public T Result { get; set; }

        public Error Error { get; set; }
        
        public static ResultDto<bool> Ok { get; } = new ResultDto<bool> { Result = true };
    }
}
