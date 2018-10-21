namespace CustomerApi.ErrorHandling
{
    internal class EmailAlreadyInUseException : ApiException
    {
        public EmailAlreadyInUseException() : base(ErrorCodes.EmailInUse, "Email already in use.")
        {
        }
    }
}