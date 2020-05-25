using UnityEngine;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ApiModels;
using Assets.Scripts.Pluigns;
using Assets.Scripts.Enums;
using TMPro;
using Assets.Scripts.Extends;
using System.Collections.Generic;
using System;

public class MailDetailItemOnlySpyController : MonoBehaviour, IMailDetailItem
{
    [Header("Dataları basarken kullanılacak olan template.")]
    public GameObject SpyMailTemplateItem;

    [Header("Kaynakları basacağımız alan.")]
    public Transform ResourceContent;

    [Header("Binaları basacağımız alan.")]
    public Transform BuildingContent;

    [Header("Araştırmaları basacağımız alan.")]
    public Transform ResearchContent;

    [Header("Gemileri basacağımız alan.")]
    public Transform ShipsContent;

    [Header("Savunmaları basacağımız alan.")]
    public Transform DefensesContent;

    public void LoadContent(UserMailDTO mailData, MailEncoder.MailDecodeDTO decodedData)
    {
        // Mailin türünü arıyoruz. Ona göre Dil dosyasından alacağız datayı.
        MailTypes mailType = decodedData.GetMailType();

        // Template yüklüyoruz dil dosyasından.
        string template = LanguageController.LC.GetText($"MT{(int)mailType}");

        #region Metal Kristal ve Bor datalarını basıyoruz.

        string metalQuantity = decodedData.GetValue(MailEncoder.KEY_NEW_METAL);
        string crystalQuantity = decodedData.GetValue(MailEncoder.KEY_NEW_CRYSTAL);
        string boronQuantity = decodedData.GetValue(MailEncoder.KEY_NEW_BORON);

        GameObject metalItem = Instantiate(SpyMailTemplateItem, ResourceContent);
        metalItem.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText("Metal"), ":", "orange", metalQuantity);

        GameObject crystalItem = Instantiate(SpyMailTemplateItem, ResourceContent);
        crystalItem.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText("Kristal"), ":", "orange", crystalQuantity);

        GameObject boronItem = Instantiate(SpyMailTemplateItem, ResourceContent);
        boronItem.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText("Bor"), ":", "orange", boronQuantity);

        #endregion

        #region Binalar

        decodedData.GetManyItem<Buildings>(MailEncoder.KEY_BUILDING_DEFENDER).ForEach(e =>
        {

            GameObject item = Instantiate(SpyMailTemplateItem, BuildingContent);
            item.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText($"B{(int)e.Item1}"), ":", "orange", LanguageController.LC.GetText("InciSeviye", e.Item2.ToString()));

        });

        #endregion

        #region Araştırmalar

        decodedData.GetManyItem<Researches>(MailEncoder.KEY_RESEARCHES).ForEach(e =>
        {

            GameObject item = Instantiate(SpyMailTemplateItem, ResearchContent);
            item.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText($"R{(int)e.Item1}"), ":", "orange", LanguageController.LC.GetText("InciSeviye", e.Item2.ToString()));

        });

        #endregion

        #region Ships

        decodedData.GetManyItem<Ships>(MailEncoder.KEY_SHIPS_DEFENDER).ForEach(e =>
        {

            GameObject item = Instantiate(SpyMailTemplateItem, ShipsContent);
            item.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText($"S{(int)e.Item1}"), ":", "orange", e.Item2.ToString());

        });

        #endregion

        #region Defenses

        decodedData.GetManyItem<Defenses>(MailEncoder.KEY_DEFENSES).ForEach(e =>
        {

            GameObject item = Instantiate(SpyMailTemplateItem, DefensesContent);
            item.GetComponent<TMP_Text>().text = TextExtends.MakeItColorize(LanguageController.LC.GetText($"S{(int)e.Item1}"), ":", "orange", e.Item2.ToString());

        });

        #endregion

    }
}
