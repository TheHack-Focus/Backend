namespace Backend.Models
{
    public class ErrorMessageModel
    {
        public string Message { get; set; }

        public ErrorMessageModel()
        {

        }

        public ErrorMessageModel(string message)
        {
            Message = message;
        }
    }
}
