using Assets.Scripts.ApiModels;
using static Assets.Scripts.Pluigns.MailEncoder;

public interface IMailDetailItem
{
    void LoadContent(UserMailDTO mailData, MailDecodeDTO decodedData);
}
