
namespace Ordering.Application.Extensions
{
    public class ValidationException: ApplicationException
    {
        public Dictionary<string, string[]> Errors { get; }
        public ValidationException(string name, object key) : base("One or more validation error(s) occured") 
        {
            Errors = new Dictionary<string, string[]>();
        }
    }
}
