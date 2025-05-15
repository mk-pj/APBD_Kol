namespace Kol.Middlewares;

public class NotFoundException(string msg) : Exception(msg);