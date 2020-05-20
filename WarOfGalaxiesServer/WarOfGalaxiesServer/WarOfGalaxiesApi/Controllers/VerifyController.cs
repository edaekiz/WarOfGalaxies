using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public static class VerifyController
    {

        /// <summary>
        /// Doğrulananların listesi.
        /// </summary>
        public static List<int> Verifies = new List<int>();

        /// <summary>
        ///  Üretim binaları ve depoları. Önce kaynak sonra depo.
        /// </summary>
        public static Buildings[] ResouceBuildings = new Buildings[] {
                    Buildings.MetalMadeni,
                    Buildings.MetalDeposu,
                    Buildings.KristalMadeni,
                    Buildings.KristalDeposu,
                    Buildings.BoronMadeni,
                    Buildings.BoronDeposu
                };

        /// <summary>
        /// Gezegene ait kaynakları kullanmadan önce çağrılması gereken methot.
        /// Bütün kaynak hesaplamalarını yeniden yapar.
        /// </summary>
        /// <param name="verifyData"></param>
        /// <returns></returns>
        public static bool VerifyPlanetResources(MainController controller, VerifyResourceDTO verifyData)
        {

            #region Doğrulamadan önce zaten doğruluyor mu diye kontrol ediyoruz.

            lock (Verifies)
            {
                // Eğer zaten listede ve verify ediliyor ise hata dön.
                if (Verifies.Contains(verifyData.UserPlanetID))
                    throw new Exception("Zaten bir doğrulama yapılıyor.");

                // Listeye ekliyoruz.
                Verifies.Add(verifyData.UserPlanetID);
            }

            #endregion

            try
            {
                // Sistem tarihi.
                DateTime currentDate = DateTime.UtcNow;

                // Kullanıcının gezegenini buluyoruz.i
                TblUserPlanets userPlanet = controller.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserPlanetId == verifyData.UserPlanetID)
                    .Include(x => x.TblUserPlanetBuildings)
                    .Include(x => x.TblUserPlanetBuildingUpgs)
                    .Include(x => x.TblUserPlanetDefenses)
                    .Include(x => x.TblUserPlanetDefenseProgs)
                    .Include(x => x.TblUserPlanetShips)
                    .Include(x => x.TblUserPlanetShipProgs)
                    .Include(x => x.TblUserResearchUpgs)
                    .Include(x => x.TblFleetsDestinationUserPlanet)
                    .Include(x => x.TblFleetsSenderUserPlanet)
                    .Include(x => x.User)
                    .ThenInclude(x => x.TblUserResearches)
                    .FirstOrDefault();

                // Bu gezegenden yada bu gezegene yapılan filo hareketleri.
                List<TblFleets> userFleets = new List<TblFleets>();

                // Gönderen gezegen isek her türlü alıyoruz listeye.
                userFleets.AddRange(userPlanet.TblFleetsSenderUserPlanet);

                // Eğer hedef gezegen biz isek sadece bize geldikleri durumda verify ediyoruz.
                userFleets.AddRange(userPlanet.TblFleetsDestinationUserPlanet.Where(x => x.DestinationUserPlanetId == verifyData.UserPlanetID && !x.IsReturning));

                // Filoları hareket tarihine göre sıralıyoruz.
                IOrderedEnumerable<TblFleets> orderedUserFleets = userFleets.OrderBy(x =>
                {
                    DateTime GetHalfOfFlyDate = x.BeginDate.AddSeconds((x.EndDate - x.BeginDate).TotalSeconds / 2);
                    DateTime GetEndFlyDate = x.EndDate;
                    return GetHalfOfFlyDate <= currentDate ? GetHalfOfFlyDate : GetEndFlyDate;
                });

                // Her bir filo hedefe ulaşmadan önce kaynakları doğruluyoruz.
                foreach (TblFleets userFleet in orderedUserFleets)
                {
                    // Hedefe varış süresini hesaplıyoruz.
                    DateTime halfOfFlyDate = userFleet.BeginDate.AddSeconds((userFleet.EndDate - userFleet.BeginDate).TotalSeconds / 2);

                    // Göndericiye dönüş süresini hesaplıyoruz.
                    DateTime endFlyDate = userFleet.EndDate;

                    // Hedef ulaştıysak işlemleri hallediyoruz..
                    if (currentDate >= halfOfFlyDate && !userFleet.IsReturning)
                    {
                        // Kaynaklar doğrulanıyor.
                        VerifyPlanetResources(controller, halfOfFlyDate, userPlanet);

                        // Güncelleme tarihini değiştiriyoruz.
                        userPlanet.LastUpdateDate = halfOfFlyDate;

                        if (userFleet.DestinationUserPlanetId.HasValue && userPlanet.UserPlanetId != userFleet.DestinationUserPlanetId)
                        {
                            // Gezegeni doğruluyoruz.
                            bool isVerified = VerifyPlanetResources(controller, new VerifyResourceDTO { UserPlanetID = userFleet.DestinationUserPlanetId.Value });

                            // EĞer kaynak doğrulama başarısız olduysa değer false dönüyoruz.
                            if (!isVerified)
                                return false;

                            // Burada hedef gezegene kaynakları yükleyeceğiz.
                            HandleFleetActions(controller, userPlanet, userFleet);

                            // Artık geri dönüyor.
                            userFleet.IsReturning = true;
                        }

                    }

                    // Dönüş tamamlandıysa işleyip siliyoruz.
                    if (currentDate >= endFlyDate && userFleet.IsReturning && userFleet.SenderUserPlanetId == verifyData.UserPlanetID)
                    {
                        // Kaynaklar doğrulanıyor.
                        VerifyPlanetResources(controller, endFlyDate, userPlanet);

                        // Güncelleme tarihini değiştiriyoruz.
                        userPlanet.LastUpdateDate = endFlyDate;

                        // Burada hedef gezegene kaynakları yükleyeceğiz.
                        HandleFleetActions(controller, userPlanet, userFleet);

                        // Kaydı siliyoruz.
                        controller.UnitOfWork.GetRepository<TblFleets>().Delete(userFleet);
                    }
                }

                // En son yine bütün kaynakları o saat için doğruluyoruz..
                VerifyPlanetResources(controller, currentDate, userPlanet);

                // Güncelleme tarihini değiştiriyoruz.
                userPlanet.LastUpdateDate = currentDate;

                // Değişiklikleri kayıt ediyoruz.
                controller.UnitOfWork.SaveChanges();

                // Gezegendeki üretimleri doğruladığımızı söylüyoruz.
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
            finally
            {
                // Listeden siliyoruz.
                lock (Verifies)
                    Verifies.Remove(verifyData.UserPlanetID);
            }
        }

        private static void VerifyPlanetResources(MainController controller, DateTime verifyDate, TblUserPlanets userPlanet)
        {
            #region Üretim hesaplamaları.

            for (int ii = 0; ii < ResouceBuildings.Length; ii += 2)
            {
                // Kaynak binası.
                Buildings resourceBuilding = ResouceBuildings[ii];

                // Kaynak deposu.
                Buildings resourceStorageBuilding = ResouceBuildings[ii + 1];

                // En son güncellemeden bu yana geçen süreyi buluyoruz.
                double passedSeconds = (verifyDate - userPlanet.LastUpdateDate).TotalSeconds;

                #region Kaynak Binaları

                // Gezegendeki kaynak deposunu buluyoruz..
                TblUserPlanetBuildings planetStorageBuilding = userPlanet.TblUserPlanetBuildings.FirstOrDefault(x => x.BuildingId == (int)resourceStorageBuilding);

                // Gezegendeki kaynak deposunun yükseltmesini buluyoruz.
                TblUserPlanetBuildingUpgs planetStorageBuildingUpg = userPlanet.TblUserPlanetBuildingUpgs.FirstOrDefault(x => x.BuildingId == (int)resourceStorageBuilding);

                // Gezegendeki kaynak binasını buluyoruz.
                TblUserPlanetBuildings planetResourceBuilding = userPlanet.TblUserPlanetBuildings.FirstOrDefault(x => x.BuildingId == (int)resourceBuilding);

                // Gezegendeki kaynak binasının yükseltmesini buluyoruz.
                TblUserPlanetBuildingUpgs planetResourceBuildingUpg = userPlanet.TblUserPlanetBuildingUpgs.FirstOrDefault(x => x.BuildingId == (int)resourceBuilding);

                #endregion

                #region Kaynak Hesaplama

                // Eğer kaynak binası yükseltiliyor ve yükseltme tamamlanmış ise yeni ve eski seviyedeki üretimleri ayrı ayrı hesaplamak gerkeiyor.
                if (planetResourceBuildingUpg != null && planetResourceBuildingUpg.EndDate <= verifyDate)
                {
                    #region Kaynak binasının yükseltmesi tamamlandıktan sonraki hesaplama

                    // Yükseltmeden sonra geçen süreyi hesaplıyoruz.
                    double passedSecondsInNewLevel = (verifyDate - planetResourceBuildingUpg.EndDate).TotalSeconds;

                    // Yükseltmeden önceki geçen süreyi buluyoruz.
                    double passedSecondsInPrevLevels = passedSeconds - passedSecondsInNewLevel;

                    // Önceki seviyede toplam üretilen kaynak miktarını hesaplıyoruz.
                    double metalProduceQuantity = controller.StaticValues.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSecondsInPrevLevels / 3600);

                    // Yeni seviyede toplam üretilen kaynak miktarını hesaplıyoruz.
                    metalProduceQuantity += controller.StaticValues.GetBuildingProdPerHour(resourceBuilding, planetResourceBuildingUpg.BuildingLevel) * (passedSecondsInNewLevel / 3600);

                    // Kaynak depo kapasitesi.
                    double metalBuildingCapacity = controller.StaticValues.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                    // Gezegendeki kaynağı yükseltiyoruz.
                    UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacity, metalProduceQuantity);

                    // Yükseltmeyi siliyoruz.
                    controller.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Delete(planetResourceBuildingUpg);

                    // Eğer bina yok ise ozaman binayı oluşturacağız.
                    if (planetResourceBuilding == null)
                    {
                        // Ve binayı sisteme ekliyoruz.
                        controller.UnitOfWork.GetRepository<TblUserPlanetBuildings>().Add(new TblUserPlanetBuildings
                        {
                            BuildingLevel = 1,
                            BuildingId = (int)resourceBuilding,
                            UserPlanetId = userPlanet.UserPlanetId
                        });
                    }
                    else // Zaten kaynak binası var ise seviyesini yükseltiyoruz.
                        planetResourceBuilding.BuildingLevel = planetResourceBuildingUpg.BuildingLevel;

                    #endregion
                }
                else // Eğer üretim binası yükseltimiyor yada henüz tamamlanmamış ise yükseltmeden önceki seviyeye göre hesaplama yapmamız yeterli.
                {
                    // Metal deposu yükseltiliyor mu?
                    if (planetStorageBuildingUpg != null)
                    {
                        // Yükseltmenin bittiğinden emin oluyoruz.
                        if (planetStorageBuildingUpg.EndDate < verifyDate)
                        {
                            #region Deponun yükseltilmesinden önceki hesaplama.

                            // Deponun yükseltmeden önceki geçen süre.
                            double passedSecondsInPrevStorage = (planetStorageBuildingUpg.EndDate - userPlanet.LastUpdateDate).TotalSeconds;

                            // Deponun yükseltmeden önceki kapasitesi.
                            double metalBuildingCapacityInPrevStorage = controller.StaticValues.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                            // Metal binasının depo yükseltilene kadar ürettiği toplam miktar.
                            double metalProduceQuantityInPrevStorage = controller.StaticValues.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSecondsInPrevStorage / 3600);

                            // Gezegendeki kaynağı yükseltiyoruz.
                            UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacityInPrevStorage, metalProduceQuantityInPrevStorage);

                            #endregion

                            #region Depo yükseltme işlemi.

                            // Yükseltme bilgisini siliyoruz.
                            controller.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Delete(planetStorageBuildingUpg);

                            // Metal deposunu inşaa etmemiz gerekiyor ise inşaa edeceğiz.
                            if (planetStorageBuilding == null)
                            {
                                // Ve binayı sisteme ekliyoruz.
                                controller.UnitOfWork.GetRepository<TblUserPlanetBuildings>().Add(new TblUserPlanetBuildings
                                {
                                    BuildingLevel = 1,
                                    BuildingId = (int)resourceStorageBuilding,
                                    UserPlanetId = userPlanet.UserPlanetId
                                });
                            }
                            else // Zaten kaynak deposu var ise seviyesini yükseltiyoruz.
                                planetStorageBuilding.BuildingLevel = planetStorageBuildingUpg.BuildingLevel;

                            #endregion

                            #region Deponun yükseltilmesinden sonraki hesaplama.

                            // Deponun yükseltmesinden sonraki geçen süre.
                            double passedSecondsInNextStorage = (verifyDate - planetStorageBuildingUpg.EndDate).TotalSeconds;

                            // Deponun yükseltmeden sonraki kapasitesi.
                            double metalBuildingCapacityInNextStorage = controller.StaticValues.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                            // Metal binasının yükseltmeden sonraki geçen sürede ürettiği metal miktarı.
                            double metalProduceQuantityInNextStorage = controller.StaticValues.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSecondsInNextStorage / 3600);

                            // Gezegendeki kaynağı yükseltiyoruz.
                            UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacityInNextStorage, metalProduceQuantityInNextStorage);

                            #endregion

                        }
                    }
                    else // Yükseltilmiyor ise standart ekleme çıkarma yapacağız.
                    {
                        #region Kaynak üretimi (Herhangi bir yükseltme olmadan.)

                        // Toplam geçen sürede kaynak binasının ürettiği toplam üretim.
                        double metalProduceQuantity = controller.StaticValues.GetBuildingProdPerHour(resourceBuilding, planetResourceBuilding == null ? 0 : planetResourceBuilding.BuildingLevel) * (passedSeconds / 3600);

                        // Metal binasının kapasitesini hesaplıyoruz.
                        double metalBuildingCapacity = controller.StaticValues.GetBuildingStorage(resourceStorageBuilding, planetStorageBuilding == null ? 0 : planetStorageBuilding.BuildingLevel);

                        // Gezegendeki kaynağı yükseltiyoruz.
                        UpdateUserPlanetResources(userPlanet, resourceBuilding, metalBuildingCapacity, metalProduceQuantity);

                        #endregion
                    }
                }

                #endregion
            }

            #endregion

            #region Diğer tesisler

            // Tesis üretimlerini verify ediyoruz.
            // Yükseltme bilgisi.
            TblUserPlanetBuildingUpgs buildingUpgrade = userPlanet.TblUserPlanetBuildingUpgs.FirstOrDefault();

            // Eğer yüksletme var ise yükseltiyoruz.
            if (buildingUpgrade != null)
            {
                // Gezegendeki kaynak binasını buluyoruz.
                TblUserPlanetBuildings building = userPlanet.TblUserPlanetBuildings.FirstOrDefault(x => x.BuildingId == buildingUpgrade.BuildingId);

                // Eğer bina yok ise ozaman binayı oluşturacağız.
                if (building == null)
                {
                    // Ve binayı sisteme ekliyoruz.
                    controller.UnitOfWork.GetRepository<TblUserPlanetBuildings>().Add(new TblUserPlanetBuildings
                    {
                        BuildingLevel = 1,
                        BuildingId = buildingUpgrade.BuildingId,
                        UserPlanetId = userPlanet.UserPlanetId
                    });
                }
                else // Zaten kaynak binası var ise seviyesini yükseltiyoruz.
                    building.BuildingLevel = buildingUpgrade.BuildingLevel;

                // Yükseltmeyi siliyoruz.
                controller.UnitOfWork.GetRepository<TblUserPlanetBuildingUpgs>().Delete(buildingUpgrade);
            }


            #endregion

            #region Araştırmalar

            // Devam eden araştırmaları kontrol ediyoruz biten var ise bitiriyoruz.
            TblUserResearchUpgs researchUpg = userPlanet.TblUserResearchUpgs.FirstOrDefault();

            // Eğer araştırma var ise siliyoruz.
            if (researchUpg != null)
            {
                // Eğer tamamlanmış ise yükseltmeyi tamamlıyoruz.
                bool isCompleted = researchUpg.EndDate <= verifyDate;

                // Tamamlanmış ise araştırmalara ekliyoruz yada güncelliyoruz.
                if (isCompleted)
                {
                    // Eğer 1.seviye ise ilk defa eklenecek.
                    if (researchUpg.ResearchTargetLevel == 1)
                    {
                        controller.UnitOfWork.GetRepository<TblUserResearches>().Add(new TblUserResearches
                        {
                            ResearchId = researchUpg.ResearchId,
                            ResearchLevel = researchUpg.ResearchTargetLevel,
                            UserId = userPlanet.UserId
                        });
                    }
                    else
                    {
                        // Var olan yükseltme.
                        TblUserResearches existsUpgrade = userPlanet.User.TblUserResearches.FirstOrDefault(x => x.ResearchId == researchUpg.ResearchId && x.UserId == researchUpg.UserId);

                        // Araştırmaların seviyesini yükseltiyoruz.
                        existsUpgrade.ResearchLevel = researchUpg.ResearchTargetLevel;
                    }

                    // Yükseltmeyi siliyoruz.
                    controller.UnitOfWork.GetRepository<TblUserResearchUpgs>().Delete(researchUpg);
                }
            }

            #endregion

            #region Gemiler

            // Tersaneyi buluyoruz.
            TblUserPlanetBuildings shipyard = userPlanet.TblUserPlanetBuildings.FirstOrDefault(x => x.BuildingId == (int)Buildings.Tersane);

            // Tersane seviyesini buluyoruz.
            int shipyardLevel = shipyard == null ? 0 : shipyard.BuildingLevel;

            // İlk geminin üretiminin hepsi beklenen süreden önce tamamlandı. Ve bizim bir sonraki üretime başlamamız lazım.
            // Bir sonraki üretimi hesaplarken kaç sanie olacağını belirtiyoruz.
            DateTime? lastVerifyDateInShipyard = null;

            // Silinecek olanları burada tutuyoruz.
            List<TblUserPlanetShipProgs> toRemoveShipProgs = new List<TblUserPlanetShipProgs>();

            // Her bir üretimi dönüyoruz.
            foreach (TblUserPlanetShipProgs userPlanetShipProg in userPlanet.TblUserPlanetShipProgs)
            {
                // Bir geminin üretim süresi.
                double shipBuildTime = controller.StaticValues.CalculateShipCountdown((Ships)userPlanetShipProg.ShipId, shipyardLevel);

                // Son onaylanma tarihi bir öncekinin bitiş tarihi.
                if (!userPlanetShipProg.LastVerifyDate.HasValue)
                    userPlanetShipProg.LastVerifyDate = lastVerifyDateInShipyard;

                // Son doğrulamadan bu yana geçen süre.
                double passedSeconds = (verifyDate - userPlanetShipProg.LastVerifyDate.Value).TotalSeconds;

                // Toplam üretilen gemi sayısı.
                int producedCount = (int)(passedSeconds / shipBuildTime);

                // Eğer üretim yok ise güncel üretimdeyiz.
                if (producedCount == 0)
                    break;

                // Eğer olandan fazla ürettiysek üretebileceğimiz sınıra getiriyoruz.
                if (producedCount > userPlanetShipProg.ShipCount)
                    producedCount = userPlanetShipProg.ShipCount;

                // Üretilmesi için geçen süreyi buluyoruz.
                passedSeconds = shipBuildTime * producedCount;

                // Son doğrulama tarihini güncelliyoruz.
                userPlanetShipProg.LastVerifyDate = userPlanetShipProg.LastVerifyDate.Value.AddSeconds(passedSeconds);

                // Son doğrulama süresini veriyoruz bunun üzerinden hesaplayacağız.
                lastVerifyDateInShipyard = userPlanetShipProg.LastVerifyDate;

                // Gezegende bulunan benzer gemi.
                TblUserPlanetShips userPlanetShip = userPlanet.TblUserPlanetShips.FirstOrDefault(x => x.ShipId == userPlanetShipProg.ShipId);

                // Eğer gezegende bu gemiden yok ise ekliyoruz.
                if (userPlanetShip == null)
                {
                    // Veritabanına gemiyi ekliyoruz.
                    controller.UnitOfWork.GetRepository<TblUserPlanetShips>().Add(new TblUserPlanetShips
                    {
                        ShipId = userPlanetShipProg.ShipId,
                        ShipCount = producedCount,
                        UserPlanetId = userPlanet.UserPlanetId,
                    });
                }
                else
                {
                    // Sadece miktarı güncelliyoruz.
                    userPlanetShip.ShipCount += producedCount;
                }

                // Üretim miktarını azaltıyoruz.
                userPlanetShipProg.ShipCount -= producedCount;

                // Eğer üretilebilecek gemi kalmamış ise veritabanından siliyoruz.
                if (userPlanetShipProg.ShipCount <= 0)
                    toRemoveShipProgs.Add(userPlanetShipProg);
                else // Eğer hala üretim  var ise diğerlerine geçmemize gerek yok.
                    break;
            }

            // Silinecek olanları siliyoruz.
            toRemoveShipProgs.ForEach(e => controller.UnitOfWork.GetRepository<TblUserPlanetShipProgs>().Delete(e));

            #endregion

            #region Savunmalar

            // Robot fabrikasını buluyoruz.
            TblUserPlanetBuildings robotFac = userPlanet.TblUserPlanetBuildings.FirstOrDefault(x => x.BuildingId == (int)Buildings.RobotFabrikası);

            // Robot seviyesini buluyoruz.
            int robotFacLevel = robotFac == null ? 0 : robotFac.BuildingLevel;

            // İlk savunmanın üretiminin hepsi beklenen süreden önce tamamlandı. Ve bizim bir sonraki üretime başlamamız lazım.
            // Bir sonraki üretimi hesaplarken kaç saniye olacağını belirtiyoruz.
            DateTime? lastVerifyDateInDefense = null;

            // Silinecek olanları burada tutuyoruz.
            List<TblUserPlanetDefenseProgs> toRemoveDefenseProgs = new List<TblUserPlanetDefenseProgs>();

            // Her bir üretimi dönüyoruz.
            foreach (TblUserPlanetDefenseProgs userPlanetDefenseProg in userPlanet.TblUserPlanetDefenseProgs)
            {
                // Bir savunmanın üretim süresi.
                double defenseBuildTime = controller.StaticValues.CalculateDefenseCountdown((Defenses)userPlanetDefenseProg.DefenseId, robotFacLevel);

                // Son onaylanma tarihi bir öncekinin bitiş tarihi.
                if (!userPlanetDefenseProg.LastVerifyDate.HasValue)
                    userPlanetDefenseProg.LastVerifyDate = lastVerifyDateInDefense;

                // Son doğrulamadan bu yana geçen süre.
                double passedSeconds = (verifyDate - userPlanetDefenseProg.LastVerifyDate.Value).TotalSeconds;

                // Toplam üretilen gemi sayısı.
                int producedCount = (int)(passedSeconds / defenseBuildTime);

                // Eğer üretim yok ise güncel üretimdeyiz.
                if (producedCount == 0)
                    break;

                // Eğer olandan fazla ürettiysek üretebileceğimiz sınıra getiriyoruz.
                if (producedCount > userPlanetDefenseProg.DefenseCount)
                    producedCount = userPlanetDefenseProg.DefenseCount;

                // Üretilmesi için geçen süreyi buluyoruz.
                passedSeconds = defenseBuildTime * producedCount;

                // Son doğrulama tarihini güncelliyoruz.
                userPlanetDefenseProg.LastVerifyDate = userPlanetDefenseProg.LastVerifyDate.Value.AddSeconds(passedSeconds);

                // Son doğrulama süresini veriyoruz bunun üzerinden hesaplayacağız.
                lastVerifyDateInDefense = userPlanetDefenseProg.LastVerifyDate;

                // Gezegende bulunan benzer savunmalar.
                TblUserPlanetDefenses userPlanetDefense = userPlanet.TblUserPlanetDefenses.FirstOrDefault(x => x.DefenseId == userPlanetDefenseProg.DefenseId);

                // Eğer gezegende bu savunmadan yok ise ekliyoruz.
                if (userPlanetDefense == null)
                {
                    // Kaydı listeye de ekliyoruz ki aynı savunmadan tekrar üretim gelirse listeye basalım.
                    controller.UnitOfWork.GetRepository<TblUserPlanetDefenses>().Add(new TblUserPlanetDefenses
                    {
                        DefenseId = userPlanetDefenseProg.DefenseId,
                        DefenseCount = producedCount,
                        UserPlanetId = userPlanet.UserPlanetId,
                    });
                }
                else
                {
                    // Sadece miktarı güncelliyoruz.
                    userPlanetDefense.DefenseCount += producedCount;
                }

                // Üretim miktarını azaltıyoruz.
                userPlanetDefenseProg.DefenseCount -= producedCount;

                // Eğer üretilebilecek savunma kalmamış ise veritabanından siliyoruz.
                if (userPlanetDefenseProg.DefenseCount <= 0)
                    toRemoveDefenseProgs.Add(userPlanetDefenseProg);
                else // Eğer hala üretim  var ise diğerlerine geçmemize gerek yok.
                    break;
            }

            // Silinecek olan üretimleri siliyoruz.
            toRemoveDefenseProgs.ForEach(e => controller.UnitOfWork.GetRepository<TblUserPlanetDefenseProgs>().Delete(e));

            #endregion

        }

        private static void HandleFleetActions(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet)
        {
            if (userFleet.IsReturning) // Kaynağa döndüğünde yapılacak olan.
            {
                switch ((FleetTypes)userFleet.FleetActionTypeId)
                {
                    case FleetTypes.Casusluk:
                        break;
                    case FleetTypes.Saldır:
                        break;
                    case FleetTypes.Nakliye: // Nakliye yapan geminin gezegene dönüşü.
                        {
                            // Gemileri alıyoruz. Kullanıcının gezegenine iade edeceğiz.
                            List<Tuple<Ships, int>> ships = FleetController.FleetDataToShipData(userFleet.FleetData);
                            foreach (Tuple<Ships, int> ship in ships)
                            {
                                // BU gezegende bu gemiden var mı?
                                TblUserPlanetShips shipInPlanet = userPlanet.TblUserPlanetShips.FirstOrDefault(x => x.ShipId == (int)ship.Item1);

                                // Eğer yok ise oluşturuyoruuz.
                                if (shipInPlanet == null)
                                    userPlanet.TblUserPlanetShips.Add(new TblUserPlanetShips { ShipId = (int)ship.Item1, ShipCount = ship.Item2, UserPlanetId = userFleet.SenderUserPlanetId });
                                else // Eğer gezegende zaten var ise miktarıın arttırıyoruz.
                                    shipInPlanet.ShipCount += ship.Item2;
                            }

                            // Şurada kaynakları geri veriyoruz kullanıcıya.
                            userPlanet.Metal += userFleet.CarriedMetal;
                            userPlanet.Crystal += userFleet.CarriedCrystal;
                            userPlanet.Boron += userFleet.CarriedBoron;
                        }
                        break;
                    case FleetTypes.Konuşlandır:
                        break;
                    case FleetTypes.Sömürgeleştir:
                        break;
                }
            }
            else // Hedef vardığında yapılacak olan.
            {
                switch ((FleetTypes)userFleet.FleetActionTypeId)
                {
                    case FleetTypes.Casusluk:
                        break;
                    case FleetTypes.Saldır:
                        break;
                    case FleetTypes.Nakliye:
                        {
                            /// Hedef gezegen.
                            TblUserPlanets destinationPlanet = userPlanet;

                            // Eğer hedef gezegen biz değil isek hedef gezegeni buluyoruz.
                            if (userFleet.DestinationUserPlanetId.HasValue && userPlanet.UserPlanetId != userFleet.DestinationUserPlanetId)
                                destinationPlanet = controller.UnitOfWork.GetRepository<TblUserPlanets>().FirstOrDefault(x => x.UserPlanetId == userFleet.DestinationUserPlanetId.Value);

                            // Kaynakları ekliyoruz.
                            destinationPlanet.Metal += userFleet.CarriedMetal;
                            destinationPlanet.Crystal += userFleet.CarriedCrystal;
                            destinationPlanet.Boron += userFleet.CarriedBoron;

                            // Taşınan kaynakları siliyoruz.
                            userFleet.CarriedMetal = 0;
                            userFleet.CarriedCrystal = 0;
                            userFleet.CarriedBoron = 0;
                        }
                        break;
                    case FleetTypes.Konuşlandır:
                        break;
                    case FleetTypes.Sömürgeleştir:
                        break;
                }
            }
        }

        /// <summary>
        /// Verilen türe göre kaynakları günceller.
        /// </summary>
        /// <param name="userPlanet">Kullanıcının hangi gezegeninin kaynakları güncellenecek.</param>
        /// <param name="building">Hangi kaynak için yapılacak.</param>
        /// <param name="capacity">Kaynak depo kapasitesi.</param>
        /// <param name="quantity">Kaynak miktarı.</param>
        public static void UpdateUserPlanetResources(TblUserPlanets userPlanet, Buildings building, double capacity, double quantity)
        {
            switch (building)
            {
                case Buildings.MetalMadeni:

                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (userPlanet.Metal < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        userPlanet.Metal += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (userPlanet.Metal > capacity)
                            userPlanet.Metal = capacity;
                    }

                    break;
                case Buildings.KristalMadeni:

                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (userPlanet.Crystal < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        userPlanet.Crystal += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (userPlanet.Crystal > capacity)
                            userPlanet.Crystal = capacity;
                    }

                    break;
                case Buildings.BoronMadeni:

                    // Eğer depoda yeterince yer var ise kaynakları depoya koyacağız.
                    if (userPlanet.Boron < capacity)
                    {
                        // Kaynakları depoya koyuyoruz.
                        userPlanet.Boron += quantity;

                        // Eğer kaynak depo sınırına ulaştıysak fazlalığı siliyoruz.
                        if (userPlanet.Boron > capacity)
                            userPlanet.Boron = capacity;
                    }

                    break;
                default:
                    break;
            }
        }

    }
}