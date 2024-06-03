public class ApplicationDatas
{
    public string FirstMail { get; private set; }
    public string Password { get; private set; }

    public ApplicationDatas(IConfiguration configuration)
    {
        FirstMail = configuration["ApplicationDatas:FirstMail"]!;
        Password = configuration["ApplicationDatas:Password"]!;
    }
}
