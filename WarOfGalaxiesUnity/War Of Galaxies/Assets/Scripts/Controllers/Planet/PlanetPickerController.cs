using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers.Planet
{
    public class PlanetPickerController : MonoBehaviour
    {
        public static PlanetPickerController PPC { get; set; }

        private void Awake()
        {
            if (PPC == null)
                PPC = this;
            else
                Destroy(gameObject);
        }

        [Header("Kullanıcının her bir gezegeni için üretilecek.")]
        public GameObject PlanetPickerItem;

        [Header("Gezegenler buraya koyulacak.")]
        public RectTransform PlanetPickerContent;

        private IEnumerator Start()
        {
            // Giriş yapana kadar bekliyoruz.
            yield return new WaitUntil(() => LoginController.LC.IsLoggedIn);

            // Gezegen seçilene kadar bekliyoruz.
            yield return new WaitUntil(() => GlobalPlanetController.GPC.CurrentPlanet != null);

            // Gezegenleri yüklüyoruz.
            ReLoadPlanets();
        }

        public void ReLoadPlanets()
        {
            // Eskilerini siliyoruz.
            foreach (Transform oldPlanet in PlanetPickerContent)
                Destroy(oldPlanet.gameObject);

            // Yenilerini basıyoruz.
            LoginController.LC.CurrentUser.UserPlanets.ForEach(e =>
            {
                GameObject planet = Instantiate(PlanetPickerItem, PlanetPickerContent);

                // Gezegen ismini hazırlıyoruz.
                string planetName = $"{e.PlanetName}{Environment.NewLine}<size=24><color=orange>({e.PlanetCordinate})</color></size>";

                // Gezegen ismini basıyoruz.
                planet.transform.Find("PlanetName").GetComponent<TMP_Text>().text = planetName;

                // Eğer seçili olan gezegen ise outline açılıyor
                if (e.UserPlanetId == GlobalPlanetController.GPC.CurrentPlanet.UserPlanetId)
                    planet.GetComponent<Image>().enabled = true;
                else // Gezegen ikonuna tıkladığımız da gezegeni yükleyeceğiz.
                    planet.GetComponent<Button>().onClick.AddListener(() => LoadSelectedPlanet(e.UserPlanetId));
            });
        }


        public void LoadSelectedPlanet(int userPlanetId)
        {
            // Tıklanılan gezegeni seçiyoruz.
            GlobalPlanetController.GPC.CurrentPlanet = LoginController.LC.CurrentUser.UserPlanets.Find(x => x.UserPlanetId == userPlanetId);

            // Gezegendeki binaları tekrar yüklüyoruz.
            foreach (BuildingController bc in FindObjectsOfType<BuildingController>())
                bc.LoadBuildingDetails();

            // Son aşama seçili olanın değişmesi için butonları tekrar yüklüyoruz.
            ReLoadPlanets();
        }

    }
}
