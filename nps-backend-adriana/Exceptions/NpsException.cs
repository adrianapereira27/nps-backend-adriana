namespace nps_backend_adriana.Exceptions
{
    public class NpsException : Exception
    {
        public int ErrorCode { get; }

        // Construtor padrão
        public NpsException() { }

        // Construtor que aceita apenas a mensagem de erro
        public NpsException(string message)
            : base(message) { }

        // Construtor que aceita uma mensagem de erro e um código de erro
        public NpsException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        // Construtor que aceita uma mensagem de erro e uma exceção interna
        public NpsException(string message, Exception innerException)
            : base(message, innerException) { }

        // Construtor que aceita uma mensagem de erro, código de erro e exceção interna
        public NpsException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

    }
}
