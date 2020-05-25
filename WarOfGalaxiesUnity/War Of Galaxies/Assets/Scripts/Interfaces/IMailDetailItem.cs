using Assets.Scripts.ApiModels;
using static Assets.Scripts.Pluigns.MailEncoder;

namespace Assets.Scripts.Interfaces
{
    public interface IMailDetailItem
    {
        void LoadContent(UserMailDTO mailData, MailDecodeDTO decodedData);
    }
}
