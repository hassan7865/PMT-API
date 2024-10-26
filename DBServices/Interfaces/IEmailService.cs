using DBServices.DTOS;

namespace DBServices.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailDTO request);
    }
}
