using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using WarOfGalaxiesApi.Controllers.Base;
using WarOfGalaxiesApi.DAL.Interfaces;
using WarOfGalaxiesApi.DAL.Models;
using WarOfGalaxiesApi.DTO.Enums;
using WarOfGalaxiesApi.DTO.Extends;
using WarOfGalaxiesApi.DTO.Models;
using WarOfGalaxiesApi.Statics;

namespace WarOfGalaxiesApi.Controllers
{
    public static class VerifyController
    {
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

        private static List<int> LockedPlanets = new List<int>();

        public static TblUserPlanets VerifyAllFleets(MainController controller, VerifyResourceDTO verifyData)
        {
            try
            {
                lock (LockedPlanets)
                {
                    // Eğer zaten verify ediliyor ise geri dön.
                    if (LockedPlanets.Contains(verifyData.UserPlanetID))
                        throw new Exception("Gezegen zaten doğrulanıyor.");

                    // Verify listesine ekle.
                    LockedPlanets.Add(verifyData.UserPlanetID);
                }

                // Gezegendeki filoları alıyoruz.
                List<TblFleets> fleets = controller.UnitOfWork.GetRepository<TblFleets>()
                    .Where(x => x.EndDate <= verifyData.VerifyDate)
                    .Where(x => x.DestinationUserPlanetId == verifyData.UserPlanetID || x.SenderUserPlanetId == verifyData.UserPlanetID)
                    .OrderBy(x => x.EndDate)
                    .Include(x => x.ReturnFleet)
                    .ToList();

                // Her bir filo hareketini execute ediyoruz.
                foreach (TblFleets fleet in fleets)
                {
                    // Savaş yada casusluk sırasında bütün filo yok edilirse dönecek filo kalmaz ve dönüş filosu silinir.
                    // Dönüş filosu silindiğinde tekrar o filoya ait işleri yapmamız gerekmez.
                    if (controller.UnitOfWork.GetRepository<TblFleets>().GetStateOfEntry(fleet) == EntityState.Deleted)
                        continue;

                    TblUserPlanets senderUser = null;
                    TblUserPlanets destinationUser = null;

                    // Gönderen gezegen var ise kaynaklarını doğruluyoruz.
                    if (fleet.SenderUserPlanetId.HasValue)
                        senderUser = VerifyPlanetResources(controller, new VerifyResourceDTO { UserPlanetID = fleet.SenderUserPlanetId.Value, VerifyDate = fleet.EndDate });

                    // Hedef de gezegen var ise kaynaklarını doğruluyoruz.
                    if (fleet.DestinationUserPlanetId.HasValue)
                        destinationUser = VerifyPlanetResources(controller, new VerifyResourceDTO { UserPlanetID = fleet.DestinationUserPlanetId.Value, VerifyDate = fleet.EndDate });

                    // Filo hareketini yapıyoruz.
                    HandlePlanetFleets(controller, senderUser, destinationUser, fleet, fleet.EndDate);

                    // Filo hareketlerini yaptıysak siliyoruz.
                    controller.UnitOfWork.GetRepository<TblFleets>().Delete(fleet);
                }

                // Son olarak gezegendeki diğer kaynakları ayarlıyoruz.
                TblUserPlanets userPlanet = VerifyPlanetResources(controller, verifyData);

                // Değişiklikleri kayıt ediyoruz.
                controller.UnitOfWork.SaveChanges();

                // Sonucu dönüyoruz.
                return userPlanet;
            }
            catch (Exception exc)
            {
                throw;
            }
            finally
            {
                lock (LockedPlanets)
                    LockedPlanets.Remove(verifyData.UserPlanetID);
            }
        }

        private static TblUserPlanets VerifyPlanetResources(MainController controller, VerifyResourceDTO verifyData)
        {
            // Kullanıcının gezegenini buluyoruz.i
            TblUserPlanets userPlanet = controller.UnitOfWork.GetRepository<TblUserPlanets>().Where(x => x.UserPlanetId == verifyData.UserPlanetID)
                .Include(x => x.TblUserPlanetBuildings)
                .Include(x => x.TblUserPlanetBuildingUpgs)
                .Include(x => x.TblUserPlanetDefenses)
                .Include(x => x.TblUserPlanetDefenseProgs)
                .Include(x => x.TblUserPlanetShips)
                .Include(x => x.TblUserPlanetShipProgs)
                .Include(x => x.TblUserResearchUpgs)
                .Include(x => x.User)
                .ThenInclude(x => x.TblUserResearches)
                .Include(x=>x.TblCordinates)
                .FirstOrDefault();

            // En son yine bütün kaynakları o saat için doğruluyoruz..
            VerifyPlanetDetails(controller, verifyData.VerifyDate, userPlanet);

            // Güncelleme tarihini değiştiriyoruz.
            userPlanet.LastUpdateDate = verifyData.VerifyDate;

            return userPlanet;
        }

        private static void VerifyPlanetDetails(MainController controller, DateTime verifyDate, TblUserPlanets userPlanet)
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
            if (buildingUpgrade != null && buildingUpgrade.EndDate <= verifyDate)
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

        private static void HandlePlanetFleets(MainController controller, TblUserPlanets userPlanet, TblUserPlanets destinationPlanet, TblFleets userFleet, DateTime actionDate)
        {
            if (userFleet.ReturnFleetId.HasValue) // Hedefe vardığımızda yapılacak olan.
            {
                switch ((FleetTypes)userFleet.FleetActionTypeId)
                {
                    case FleetTypes.Casusluk:
                        ExecuteSpyAction(controller, userPlanet, userFleet, actionDate, destinationPlanet);
                        break;
                    case FleetTypes.Saldır:
                        ExecuteWarAction(controller, userPlanet, userFleet, actionDate, destinationPlanet);
                        break;
                    case FleetTypes.Nakliye:
                        ExecuteTransportAction(controller, userPlanet, userFleet, actionDate, destinationPlanet);
                        break;
                    case FleetTypes.Sök:
                        ExecuteDebrisAction(controller, userPlanet, userFleet, actionDate, destinationPlanet);
                        break;
                    case FleetTypes.Konuşlandır:
                        break;
                    case FleetTypes.Sömürgeleştir:
                        break;
                }
            }
            else // Gezegene dönüşte yapılacak olan.
            {
                switch ((FleetTypes)userFleet.FleetActionTypeId)
                {
                    case FleetTypes.Casusluk:
                        ExecuteSpyReturnAction(destinationPlanet, userFleet);
                        break;
                    case FleetTypes.Saldır:
                        ExecuteWarReturnAction(controller, destinationPlanet, userFleet, actionDate, userPlanet);
                        break;
                    case FleetTypes.Nakliye: // Nakliye yapan geminin gezegene dönüşü.
                        ExecuteTransportReturnAction(controller, destinationPlanet, userFleet, actionDate, userPlanet);
                        break;
                    case FleetTypes.Sök:
                        ExecuteDebrisReturnAction(controller, destinationPlanet, userFleet, actionDate, userPlanet);
                        break;
                    case FleetTypes.Konuşlandır:
                        break;
                    case FleetTypes.Sömürgeleştir:
                        break;
                }
            }
        }

        #region Fleet Actions 

        private static void ExecuteDebrisAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            // Kordinat formatına dönüştürüyoruz.
            CordinateDTO destinationCordinate = CordinateExtends.ToCordinate(userFleet.DestinationCordinate);

            // Bu kordinattaki dataları alıyoruz.
            TblCordinates cordinateInfo = controller.UnitOfWork.GetRepository<TblCordinates>().FirstOrDefault(x => x.GalaxyIndex == destinationCordinate.GalaxyIndex && x.SolarIndex == destinationCordinate.SolarIndex && x.OrderIndex == destinationCordinate.OrderIndex);

            // Eğer kordinat var ise bizden önce gelen var ise onu da execute etmeliyiz.
            if (cordinateInfo != null)
            {
                // Kordinatı toplamadan önce kontrol etmeliyiz buraya giden filoları.
                int[] userPlanetIds = controller.UnitOfWork.GetRepository<TblFleets>()
                    .Where(x => x.FleetId != userFleet.FleetId)
                    .Where(x => x.DestinationCordinate == userFleet.DestinationCordinate)
                    .Where(x => x.ReturnFleetId.HasValue)
                    .Where(x => x.EndDate <= actionDate)
                    .Select(x => x.SenderUserPlanetId.Value)
                    .ToArray();

                // Gezegenleri onaylıyoruz.
                foreach (int userPlanetId in userPlanetIds)
                    VerifyAllFleets(controller, new VerifyResourceDTO { UserPlanetID = userPlanetId, VerifyDate = actionDate });

            }

            // Kordinatta bir şey yok ise sıfır veriyoruz.
            if (cordinateInfo == null)
            {
                cordinateInfo = new TblCordinates
                {
                    Metal = 0,
                    Crystal = 0,
                    Boron = 0
                };
            }

            // Gemi bilgisini alıyoruz.
            List<Tuple<Ships, int>> ships = FleetController.FleetDataToShipData(userFleet.FleetData);

            // İçerisinden geri dönüşümcüyü buluyoruz.
            Tuple<Ships, int> garbageShip = ships.Find(x => x.Item1 == Ships.EnkazToplamaGemisi);

            // Gegri dönüşümcülerin toplayabileceği miktar.
            double garbageSum = 0;

            // Eğer geri dönüşümcü var ise miktar ile çarpıp taşıma kapasitesini hesaplıyoruz.
            if (garbageShip != null)
                garbageSum = controller.StaticValues.GetShip(Ships.EnkazToplamaGemisi).CargoCapacity * garbageShip.Item2;

            // Alınacak olan kaynakları alıyoruz.
            double ownedMetal = cordinateInfo.Metal;
            double ownedCrystal = cordinateInfo.Crystal;
            double ownedBoron = cordinateInfo.Boron;

            // Toplamları.
            double totalCost = ownedMetal + ownedCrystal + ownedBoron;

            // Eğer kapasitemizin üstündeysek kapasiteye eşitliyoruz.
            if (garbageSum > totalCost)
                garbageSum = totalCost;

            // Taşınamayacak miktarı eşit oranda düşeceğiz.
            double ratio = (garbageSum / totalCost);

            if (double.IsNaN(ratio))
                ratio = 0;

            // Çarpıp taşınacak olan miktarı hesaplıyoruz.
            ownedMetal *= ratio;
            ownedCrystal *= ratio;
            ownedBoron *= ratio;

            cordinateInfo.Metal -= ownedMetal;
            cordinateInfo.Crystal -= ownedCrystal;
            cordinateInfo.Boron -= ownedBoron;

            userFleet.ReturnFleet.CarriedMetal += ownedMetal;
            userFleet.ReturnFleet.CarriedCrystal += ownedCrystal;
            userFleet.ReturnFleet.CarriedBoron += ownedBoron;

            // Mail bilgisini oluşturuyoruz.
            List<string> mail = new List<string>();
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE, userFleet.FleetActionTypeId));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.SökRapor));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETNAME, userPlanet.PlanetName));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_GARBAGE_METAL, ownedMetal));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_GARBAGE_CRYSTAL, ownedCrystal));
            mail.Add(MailEncoder.GetParam(MailEncoder.KEY_GARBAGE_BORON, ownedBoron));

            // Maili kullanıcıya iletiyoruz.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                UserId = userPlanet.UserId,
                IsReaded = false,
                MailCategoryId = (int)MailCategories.Gezegen,
                MailContent = MailEncoder.EncodeMail(mail),
                MailDate = actionDate
            });

        }

        private static void ExecuteDebrisReturnAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            List<string> defaultMailContent = new List<string>()
            {
                MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE_RETURN, userFleet.FleetActionTypeId),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETNAME, userPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_NEW_METAL, userFleet.CarriedMetal),
                MailEncoder.GetParam(MailEncoder.KEY_NEW_CRYSTAL, userFleet.CarriedCrystal),
                MailEncoder.GetParam(MailEncoder.KEY_NEW_BORON, userFleet.CarriedBoron)
            };

            // Gemileri alıyoruz. Kullanıcının gezegenine iade edeceğiz.
            List<Tuple<Ships, int>> ships = ExecuteAlwaysInReturn(userPlanet, userFleet);

            // Gönderilen gemileri string formatına dönüştürüyoruz.
            string shipsWithCounts = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, ships.Select(x => $"{(int)x.Item1}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.Item2}"));

            // Gemileri maile ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_SHIPS_ATTACKER, shipsWithCounts));

            // Nakliye türünü ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.SökRaporDönüş));

            // 1. mail her zaman gidiyor. 2.mail ise sadece gönderilen oyuncu değil ise gidiyor.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailDate = actionDate,
                MailCategoryId = (int)MailCategories.Gezegen,
                MailContent = MailEncoder.EncodeMail(defaultMailContent),
                UserId = userPlanet.UserId
            });
        }

        private static void ExecuteWarAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            #region Dataları getiriyoruz.

            // Saldıran gemiler.
            List<Tuple<Ships, int>> attackerShips = FleetController.FleetDataToShipData(userFleet.FleetData);

            // Savunan gemiler.
            List<TblUserPlanetShips> defenderShips = controller.UnitOfWork.GetRepository<TblUserPlanetShips>().Where(x => x.UserPlanetId == userFleet.DestinationUserPlanetId.Value).ToList();

            // Savunana ait savunma tesisleri.
            List<TblUserPlanetDefenses> defenderDefenses = controller.UnitOfWork.GetRepository<TblUserPlanetDefenses>().Where(x => x.UserPlanetId == userFleet.DestinationUserPlanetId.Value).ToList();

            // Hedef konumun kordinat bilgisini alıyoruz.
            TblCordinates destinationCordinate = controller.UnitOfWork.GetRepository<TblCordinates>().FirstOrDefault(x => x.UserPlanetId == destinationPlanet.UserPlanetId);

            #endregion

            #region Mail oluşturuyoruz.

            // Maili oluşturuyoruz.
            List<string> mailParams = new List<string>();

            #endregion

            #region Maillere Başlangıçdaki saldırı gemileri ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string beginingAttackerShips = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, attackerShips.Select(x => $"{(int)x.Item1}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.Item2}"));

            // Saldırı gemilerini koyuyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_SHIPS_ATTACKER, beginingAttackerShips));

            #endregion

            #region Maillere Başlangıçdaki savunma gemileri ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string beginingDefenseShips = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, defenderShips.Select(x => $"{x.ShipId}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.ShipCount}"));

            // Saldırı gemilerini koyuyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_SHIPS_DEFENDER, beginingDefenseShips));

            #endregion

            #region Maillere Başlangıçdaki savunma sistemlerini ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string beginingDefenses = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, defenderDefenses.Select(x => $"{x.DefenseId}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.DefenseCount}"));

            // Savunmaları koyuyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_DEFENSES, beginingDefenses));

            #endregion

            #region Gemi ve savunmaları parçalıyoruz. Normalde gurup halindeler.

            // Gemiler normalde grup halinde bu yüzden her birini eklememiz lazım.
            List<SimulationDTO> attackerShipsCombined = attackerShips.SelectMany(x =>
            {
                List<SimulationDTO> ships = new List<SimulationDTO>();
                TblShips shipData = controller.StaticValues.GetShip(x.Item1);
                for (int i = 1; i <= x.Item2; i++)
                {
                    ships.Add(new SimulationDTO
                    {
                        IsCivil = shipData.IsCivil,
                        Id = (int)x.Item1,
                        ShipData = shipData,
                        Health = shipData.ShipHealth,
                        Side = SimulationDTO.SimulationSides.Attacker
                    });
                }
                return ships;

            }).OrderBy(x => x.ShipData.ShipAttackDamage).ToList();

            // Savunmaya ait gemiler de grup halinde onları da tekliyoruz.
            List<SimulationDTO> defenderShipCombined = defenderShips.SelectMany(x =>
            {
                List<SimulationDTO> ships = new List<SimulationDTO>();
                TblShips shipData = controller.StaticValues.GetShip((Ships)x.ShipId);
                for (int i = 1; i <= x.ShipCount; i++)
                {
                    ships.Add(new SimulationDTO
                    {
                        IsCivil = shipData.IsCivil,
                        Id = x.ShipId,
                        ShipData = shipData,
                        Health = shipData.ShipHealth,
                        Side = SimulationDTO.SimulationSides.DefenderShip
                    });
                }
                return ships;

            }).ToList();

            // Savunmaya ait savunmaları da ekliyoruz listeye.
            defenderShipCombined.AddRange(defenderDefenses.SelectMany(x =>
            {
                List<SimulationDTO> defenses = new List<SimulationDTO>();
                TblDefenses defenseData = controller.StaticValues.GetDefense((Defenses)x.DefenseId);
                for (int i = 1; i <= x.DefenseCount; i++)
                {
                    defenses.Add(new SimulationDTO
                    {
                        IsCivil = false,
                        Id = x.DefenseId,
                        Side = SimulationDTO.SimulationSides.DefenderDefense,
                        DefenseData = defenseData,
                        Health = defenseData.DefenseHealth
                    });
                }
                return defenses;
            }));

            // Defans ise defansın saldırı değerine bakıyoruz değil ise gemilerin saldırı değerine bakıyoruz.
            defenderShipCombined = defenderShipCombined.OrderBy(x => x.ShipData != null ? x.ShipData.ShipAttackDamage : x.DefenseData.DefenseAttackDamage).ToList();

            // Enkaz oranını burada tutacağız.
            ResourcesDTO shipGarbage = new ResourcesDTO(0, 0, 0);

            #endregion

            #region Savaş simülasyonunu yapıyoruz.

            // İlk her zaman saldıran başlıyor.
            bool isAttackersTurn = true;

            int lastIndexOfAttacker = 0;
            int lastIndexOfDefender = 0;

            // Randomları yöneceğiz.
            Random randomizer = new Random();

            // İki taraftan birisinde savunma yada gemi kalmayana kadar devam ediyoruz.
            while (attackerShipsCombined.Count > 0 && defenderShipCombined.Count > 0)
            {
                // Eğer saldıran taraf ise burası çalışacak.
                if (isAttackersTurn)
                {
                    // Saldıran gemi.
                    SimulationDTO attackerShip = attackerShipsCombined[lastIndexOfAttacker];

                    // Atış yapabileceği sayı kadar atış yapıyoruz.
                    for (int ii = 0; ii < attackerShip.ShipData.ShipAttackQuantity; ii++)
                    {
                        // Eğer bir hedef yok ise yada canı 0dan az ise rastgele birisini alıyoruz.
                        if (attackerShip.Target == null || attackerShip.Target.Health <= 0)
                            attackerShip.Target = defenderShipCombined.OrderBy(x => x.IsCivil).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        // Hedef yok ise döngüyü bitir.
                        if (attackerShip.Target == null)
                            break;

                        // Şimdi hedefe saldırıyoruz.
                        attackerShip.Target.Health -= attackerShip.ShipData.ShipAttackDamage;

                        // Eğer hedef ölüyor ise listeden siliyoruz.
                        if (attackerShip.Target.Health <= 0)
                            defenderShipCombined.Remove(attackerShip.Target);
                    }

                    // Ve sıra kendisine geldiğinde bir sonraki gemiyi saldırtacağız.
                    lastIndexOfAttacker++;
                }
                else // Değil ise savunma tarafı saldıracak.
                {

                    // Savunan gemi yada savunma.
                    SimulationDTO defenderShipOrDefense = defenderShipCombined[lastIndexOfDefender];

                    // Sistemden yapılacak olan atış sayısı.
                    int shootCount = defenderShipOrDefense.Side == SimulationDTO.SimulationSides.DefenderShip ? defenderShipOrDefense.ShipData.ShipAttackQuantity : defenderShipOrDefense.DefenseData.DefenseAttackQuantity;

                    // Saldırı miktarı.
                    int damage = defenderShipOrDefense.Side == SimulationDTO.SimulationSides.DefenderShip ? defenderShipOrDefense.ShipData.ShipAttackDamage : defenderShipOrDefense.DefenseData.DefenseAttackDamage;

                    // Atış yapabileceği sayı kadar atış yapıyoruz.
                    for (int ii = 0; ii < shootCount; ii++)
                    {
                        // Eğer bir hedef yok ise yada canı 0dan az ise rastgele birisini alıyoruz.
                        if (defenderShipOrDefense.Target == null || defenderShipOrDefense.Target.Health <= 0)
                            defenderShipOrDefense.Target = attackerShipsCombined.OrderBy(x => x.IsCivil).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        if (defenderShipOrDefense.Target == null)
                            break;

                        // Şimdi hedefe saldırıyoruz.
                        defenderShipOrDefense.Target.Health -= damage;

                        // Eğer hedef ölüyor ise listeden siliyoruz.
                        if (defenderShipOrDefense.Target.Health <= 0)
                            attackerShipsCombined.Remove(defenderShipOrDefense.Target);
                    }

                    // Ve sıra kendisine geldiğinde bir sonraki gemiyi saldırtacağız.
                    lastIndexOfDefender++;
                }

                // Saldıran ve savunan sırayla saldıracak.
                if (!isAttackersTurn)
                {
                    // Saldırana sıra geçtiğinde emin olacağız saldıranın gemisi var mı?
                    if (lastIndexOfAttacker < attackerShipsCombined.Count)
                        isAttackersTurn = !isAttackersTurn;
                }
                else
                {
                    // Savunmaya sıra geçtiğinde emin olacağız savunmanın daha fazla birimi var mı?
                    if (lastIndexOfDefender < defenderShipCombined.Count)
                        isAttackersTurn = !isAttackersTurn;
                }

                // Eğer son saldıran gemiyi de saldırttıysak başa dönüyoruz.
                if (lastIndexOfAttacker >= attackerShipsCombined.Count && lastIndexOfDefender >= defenderShipCombined.Count)
                {
                    lastIndexOfAttacker = 0;
                    lastIndexOfDefender = 0;
                    isAttackersTurn = !isAttackersTurn;
                }
            }

            #endregion

            #region Kazanana ganimetlerini veriyoruz.

            // Yapılan işlemi yazıyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE, userFleet.FleetActionTypeId));

            // Yapılan işlemi yazıyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.SavaşRaporu));

            // Gönderen gezegenini ismi.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETNAME, userPlanet.PlanetName));

            // Gönderen gezegenin kordinatı.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate));

            // Hedef gezegenin ismi.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETNAME, destinationPlanet.PlanetName));

            // Hedef gezegenin kordinatı
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate));

            // Eğer saldıranda gemi var ise kazanmış demektir.
            if (attackerShipsCombined.Count > 0)
            {
                // Burada taşınabilecek olan kaynakları hesaplamamız lazım. Gemilerin kapasitesine bakmalıyız.
                double totalCapacity = attackerShipsCombined.Select(x => (double)x.ShipData.CargoCapacity).DefaultIfEmpty(0).Sum();

                // Alınacak olan kaynakları alıyoruz.
                double ownedMetal = destinationPlanet.Metal / 2;
                double ownedCrystal = destinationPlanet.Crystal / 2;
                double ownedBoron = destinationPlanet.Boron / 2;

                // Toplamları.
                double totalCost = ownedMetal + ownedCrystal + ownedBoron;

                // Eğer kapasitemizin üstündeysek kapasiteye eşitliyoruz.
                if (totalCapacity > totalCost)
                    totalCapacity = totalCost;

                // taşınamayacak miktarı eşit oranda düşeceğiz.
                double ratio = (totalCapacity / totalCost);

                // Çarpıp taşınacak olan miktarı hesaplıyoruz.
                ownedMetal *= ratio;
                ownedCrystal *= ratio;
                ownedBoron *= ratio;

                mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_METAL, ownedMetal));
                mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_CRYSTAL, ownedCrystal));
                mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_BORON, ownedBoron));

                // Kazanan arkadaşa kaynakların yarısını vereceğiz.
                userFleet.ReturnFleet.CarriedMetal += ownedMetal;
                userFleet.ReturnFleet.CarriedCrystal += ownedCrystal;
                userFleet.ReturnFleet.CarriedBoron += ownedBoron;

                // Tabiki alınan kaynakları gezegeden düşmemiz lazım.
                destinationPlanet.Metal -= ownedMetal;
                destinationPlanet.Crystal -= ownedCrystal;
                destinationPlanet.Boron -= ownedBoron;

            }
            else if (defenderShipCombined.Count > 0) // Saldıran kişi kaybettiyse 0 kaynak atıyoruz mail içerisine.
            {
                mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_METAL, 0));
                mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_CRYSTAL, 0));
                mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_BORON, 0));
            }

            // Kazananı basıyoruz. 0 ise saldıran kazanmıştır 1 ise savunan.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_WINNER, attackerShipsCombined.Count > 0 ? 0 : 1));

            #endregion

            #region Oranları alıyoruz dbden.

            // Defans birimlerinin taban yenilenme oranı.
            double globalDefenseRepeatRate = controller.StaticValues.GetParameter(ParameterTypes.DefenseRepairChance).ParameterFloatValue.Value;

            // Savunmaya ait gemilerin onarım şansı.
            double globalShipRepeatRate = controller.StaticValues.GetParameter(ParameterTypes.ShipRepairChange).ParameterFloatValue.Value;

            // Gemi enkaz oranı.
            double globalGarbageRate = controller.StaticValues.GetParameter(ParameterTypes.ShipGarbageRate).ParameterFloatValue.Value;

            #endregion

            #region Kaybedilen savunma birimlerinin onarımını yapıyoruz.

            // Kaybedilen savunmaya ait savunmalar.
            List<SimulationRepairDTO> lostDefensesOfDefender = new List<SimulationRepairDTO>();

            // Savunmaya ait tamir edilecek olan savunmaların miktarı.
            defenderDefenses.ForEach(e =>
            {
                SimulationRepairDTO sim = new SimulationRepairDTO
                {
                    Id = e.DefenseId,
                    LostCount = 0,
                    RepairCount = 0,
                    Side = SimulationDTO.SimulationSides.DefenderDefense
                };

                // Yok edilmiş gemi miktarını buluyoruz.
                int destroyedQuantity = e.DefenseCount - defenderShipCombined.Count(x => x.Id == e.DefenseId && x.Side == sim.Side);

                // Birimleri silerken x olasılıkla yenilenme şansı var.
                for (int ii = 1; ii <= destroyedQuantity; ii++)
                {
                    // Zar atıyoruz.
                    double rate = randomizer.NextDouble();

                    // Eğer attığımız oran beklediğimiz orandan fazla ise savunma birimini kaybettik.
                    if (rate > globalDefenseRepeatRate)
                        sim.LostCount++;
                    else // Aksi durumda sadece tamir edilenleri 1 arttırıyoruz.
                        sim.RepairCount++;
                }

                // Kaybedilen savunmaları düşüyoruz.
                e.DefenseCount -= sim.LostCount;

                // Eğer bütün savunma yok olduysa siliyoruz.
                if (e.DefenseCount <= 0)
                    controller.UnitOfWork.GetRepository<TblUserPlanetDefenses>().Delete(e);

                // Yok edilen miktarı listeye basıyoruz.
                lostDefensesOfDefender.Add(sim);
            });

            #endregion

            #region Kaybedilen savunma gemileri onarıyoruz.

            // Kaybedilen savunmaya ait filo.
            List<SimulationRepairDTO> lostShipsOfDefender = new List<SimulationRepairDTO>();

            // Savunmaya ait tamir edilecek gemi miktarı.
            defenderShips.ForEach(e =>
            {
                SimulationRepairDTO sim = new SimulationRepairDTO
                {
                    Id = e.ShipId,
                    LostCount = 0,
                    RepairCount = 0,
                    Side = SimulationDTO.SimulationSides.DefenderShip
                };

                // Yok edilmiş gemi miktarını buluyoruz.
                int destroyedQuantity = e.ShipCount - defenderShipCombined.Count(x => x.Id == e.ShipId && x.Side == sim.Side);

                // Birimleri silerken x olasılıkla yenilenme şansı var.
                for (int ii = 1; ii <= destroyedQuantity; ii++)
                {
                    // Zar atıyoruz.
                    double rate = randomizer.NextDouble();

                    // Eğer attığımız oran beklediğimiz orandan fazla ise savunma birimini kaybettik.
                    if (rate > globalDefenseRepeatRate)
                        sim.LostCount++;
                    else
                        sim.RepairCount++;
                }

                // Kaybettiği gemileri düşüyoruz.
                e.ShipCount -= sim.LostCount;

                // Gemi bilgisini alıyoruz.
                TblShips shipData = controller.StaticValues.GetShip((Ships)e.ShipId);

                // Enkaz alanına kaynak oluşturuyoruz.
                shipGarbage.Metal += (sim.LostCount * shipData.CostMetal) * globalGarbageRate;

                // Enkaz alanına kaynak oluşturuyoruz.
                shipGarbage.Crystal += (sim.LostCount * shipData.CostCrystal) * globalGarbageRate;

                // Enkaz alanına kaynak oluşturuyoruz.
                shipGarbage.Boron += (sim.LostCount * shipData.CostBoron) * globalGarbageRate;

                // Eğer gemisi kalmamış ise siliyoruz.
                if (e.ShipCount <= 0)
                    controller.UnitOfWork.GetRepository<TblUserPlanetShips>().Delete(e);

                // Kaybedilen gemi miktarı.
                lostShipsOfDefender.Add(sim);
            });

            #endregion

            #region Kaybedilen saldırı gemilerinin onarılması şansını kontrol ediyoruz.

            // Kullanıcının hiç kaybetmediği savaş gemileri.
            List<Tuple<Ships, int>> newAttackerShips = attackerShipsCombined.GroupBy(x => x.Id, (ship, shipCount) => new Tuple<Ships, int>((Ships)ship, shipCount.Count())).ToList();

            // Saldıran tarafa ait kaybedilen gemiler.
            List<SimulationRepairDTO> lostAttackerShips = new List<SimulationRepairDTO>();

            // Saldıran geminin dtolarını oluşturuyoruz.
            attackerShips.ForEach(e =>
            {
                SimulationRepairDTO sim = new SimulationRepairDTO
                {
                    Id = (int)e.Item1,
                    LostCount = 0,
                    RepairCount = 0,
                    Side = SimulationDTO.SimulationSides.Attacker
                };

                // Yeni kayıt bilgisini buluyoruz. BUradan kaybedilen gemi miktarını bulacağız.
                Tuple<Ships, int> newAttackerShip = newAttackerShips.Find(x => x.Item1 == e.Item1);

                // Toplan kaybedilen gemi miktarı. eğer yeni filo da bu gemiden yoksa demekki öncekilerin hepsi kaybedildi. Eğer var ise öncekilerin hepsinden yeni filodaki değeri çıkarıp buluyoruz.
                int totalLostCount = newAttackerShip == null ? e.Item2 : e.Item2 - newAttackerShip.Item2;

                // Kaybedilen bütün gemileri dönüyoruz.
                for (int ii = 1; ii <= totalLostCount; ii++)
                {
                    // Zar atıyoruz.
                    double rate = randomizer.NextDouble();

                    // Eğer attığımız oran beklediğimiz orandan fazla ise saldırı birimini kaybettik.
                    if (rate > globalShipRepeatRate)
                        sim.LostCount++;
                    else // Tamir edilen miktar.
                        sim.RepairCount++;
                }

                // Gemi bilgisini alıyoruz.
                TblShips shipData = controller.StaticValues.GetShip(e.Item1);

                // Enkaz alanına kaynak oluşturuyoruz.
                shipGarbage.Metal += (sim.LostCount * shipData.CostMetal) * globalGarbageRate;

                // Enkaz alanına kaynak oluşturuyoruz.
                shipGarbage.Crystal += (sim.LostCount * shipData.CostCrystal) * globalGarbageRate;

                // Enkaz alanına kaynak oluşturuyoruz.
                shipGarbage.Boron += (sim.LostCount * shipData.CostBoron) * globalGarbageRate;

                // Kurtarılan gemiler var ise listeye ekliyoruz.
                if (sim.LostCount < totalLostCount)
                {
                    // Eğer filo tamamen listeden silinmiş ise listeye ekliyoruz.
                    if (newAttackerShip == null)
                        newAttackerShips.Add(new Tuple<Ships, int>(e.Item1, sim.RepairCount));
                    else // Burada ise kayıt var sadece kayıttaki miktarı güncelleyeceğiz.
                    {
                        // Tuplelarda update yok bu yüzden sileceğiz ve tekrar yükleyeceğiz.
                        newAttackerShips.Remove(newAttackerShip);

                        // Tekrar ekliyoruz listeye.
                        newAttackerShips.Add(new Tuple<Ships, int>(e.Item1, newAttackerShip.Item2 + sim.RepairCount));
                    }
                }

                // Kaybedilen gemileri basıyoruz.
                lostAttackerShips.Add(sim);
            });

            #endregion

            #region Yok edilmiş saldırı gemilerini maile ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string destroyedAttackerShips = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, lostAttackerShips.Select(x => $"{x.Id}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.LostCount}"));

            // Maile ekliyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_DESTROYED_ATTACKER_SHIPS, destroyedAttackerShips));

            #endregion

            #region Yok edilmiş savunmaya ait gemileri maile ekliyoruz.

            // Yok edilen savunma gemileri alıyoruz.
            string destroyedDefenderShips = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, lostShipsOfDefender.Select(x => $"{x.Id}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.LostCount}"));

            // Maile ekliyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_DESTROYED_DEFENDER_SHIPS, destroyedDefenderShips));

            #endregion

            #region Yok edilmiş savunma sistemlerini maile ekliyoruz.

            // Yok edilen savunma sistemlerini alıyoruz.
            string destroyedDefenderDefenses = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, lostDefensesOfDefender.Select(x => $"{x.Id}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.LostCount}"));

            // Maile ekliyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_DESTROYED_DEFENDER_DEFENSES, destroyedDefenderDefenses));

            #endregion

            #region Onarılmış saldırı gemilerini maile ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string repairedAttackerShips = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, lostAttackerShips.Select(x => $"{x.Id}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.RepairCount}"));

            // Maile ekliyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_FIXED_ATTACKER_SHIPS, repairedAttackerShips));

            #endregion

            #region Onarılmış savunma gemilerini maile ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string repairedDefenderShips = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, lostShipsOfDefender.Select(x => $"{x.Id}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.RepairCount}"));

            // Maile ekliyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_FIXED_DEFENDER_SHIPS, repairedDefenderShips));

            #endregion

            #region Onarılmış savunma gemilerini maile ekliyoruz.

            // Yok edilen saldırı gemileri alıyoruz.
            string repairedDefenderDefenses = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, lostDefensesOfDefender.Select(x => $"{x.Id}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.RepairCount}"));

            // Maile ekliyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_FIXED_DEFENDER_DEFENSES, repairedDefenderDefenses));

            #endregion

            #region Filoyu güncelliyoruz.

            // Filodaki datayı güncelliyoruz.
            userFleet.ReturnFleet.FleetData = FleetController.ShipDataToStringData(newAttackerShips);

            // Eğer dönecek gemi kalmadı ise sistemden siliyoruz. Çünkü filonun bir dönüşü olmayacak.
            if (newAttackerShips.Count == 0)
                controller.UnitOfWork.GetRepository<TblFleets>().Delete(userFleet.ReturnFleetId.Value);

            #endregion

            #region Kordinata enkazı basıyoruz.

            // Kaynakları hedef kordinata koyuyoruz.
            destinationCordinate.Metal += shipGarbage.Metal;

            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_GARBAGE_METAL, shipGarbage.Metal));

            // Kaynakları hedef kordinata koyuyoruz.
            destinationCordinate.Crystal += shipGarbage.Crystal;

            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_GARBAGE_CRYSTAL, shipGarbage.Crystal));

            // Kaynakları hedef kordinata koyuyoruz.
            destinationCordinate.Boron += shipGarbage.Boron;

            // Kaynakları hedef kordinata koyuyoruz.
            mailParams.Add(MailEncoder.GetParam(MailEncoder.KEY_GARBAGE_BORON, shipGarbage.Boron));

            #endregion

            #region Savaşı raporluyoruz her iki kullanıcıya da.

            // Saldırana ait mail.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailCategoryId = (int)MailCategories.Savaş,
                MailContent = MailEncoder.EncodeMail(mailParams),
                MailDate = actionDate,
                UserId = userPlanet.UserId
            });

            // Savunana ait mail.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailCategoryId = (int)MailCategories.Savaş,
                MailContent = MailEncoder.EncodeMail(mailParams),
                MailDate = actionDate,
                UserId = destinationPlanet.UserId
            });

            #endregion

        }

        private static void ExecuteWarReturnAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            List<string> defaultMailContent = new List<string>()
            {
                MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE_RETURN, userFleet.FleetActionTypeId),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETNAME, destinationPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETNAME, userPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_NEW_METAL,userFleet.CarriedMetal),
                MailEncoder.GetParam(MailEncoder.KEY_NEW_CRYSTAL,userFleet.CarriedCrystal),
                MailEncoder.GetParam(MailEncoder.KEY_NEW_BORON,userFleet.CarriedBoron),
            };

            // Gemileri alıyoruz. Kullanıcının gezegenine iade edeceğiz.
            List<Tuple<Ships, int>> ships = ExecuteAlwaysInReturn(userPlanet, userFleet);

            // Gönderilen gemileri string formatına dönüştürüyoruz.
            string shipsWithCounts = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, ships.Select(x => $"{(int)x.Item1}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.Item2}"));

            // Gemileri maile ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_SHIPS_ATTACKER, shipsWithCounts));

            // Nakliye türünü ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.SavaşRaporuDönüş));

            // 1. mail her zaman gidiyor. 2.mail ise sadece gönderilen oyuncu değil ise gidiyor.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailDate = actionDate,
                MailCategoryId = (int)MailCategories.Gezegen,
                MailContent = MailEncoder.EncodeMail(defaultMailContent),
                UserId = userPlanet.UserId
            });
        }

        private static void ExecuteSpyAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            List<string> defaultMailContent = new List<string>()
            {
                MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE, userFleet.FleetActionTypeId),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETNAME, userPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETNAME, destinationPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate)
            };

            // Casusluk türünü ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.CasusRaporu));

            // Mail datasını yüklüyoruz. Raporlanan gezgendeki kaynaklar.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_METAL, destinationPlanet.Metal));
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_CRYSTAL, destinationPlanet.Crystal));
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_BORON, destinationPlanet.Boron));

            #region Binalar.

            // Binaları ve seviyeleri stringliyoruz.
            string buildingWithLevels = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, destinationPlanet.TblUserPlanetBuildings.Select(x => $"{x.BuildingId}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.BuildingLevel}"));

            // Ve mail datamıza ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_BUILDING_DEFENDER, buildingWithLevels));

            #endregion

            #region Gemiler.

            // Gemileri ve miktarlarını stringliyoruz.
            string shipsWithQuantity = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, destinationPlanet.TblUserPlanetShips.Select(x => $"{x.ShipId}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.ShipCount}"));

            // Ve mail datamıza ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_SHIPS_DEFENDER, shipsWithQuantity));

            #endregion

            #region Savunmalar

            // Savunmaları ve miktarlarını stringliyoruz.
            string defensesWithQuantity = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, destinationPlanet.TblUserPlanetDefenses.Select(x => $"{x.DefenseId}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.DefenseCount}"));

            // Ve mail datamıza ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_DEFENSES, defensesWithQuantity));

            #endregion

            #region Araştırmalar

            // araştırma ve seviyelerini stringliyoruz.
            string researchWithLevel = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, destinationPlanet.User.TblUserResearches.Select(x => $"{x.ResearchId}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.ResearchLevel}"));

            // Ve mail datamıza ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_RESEARCHES, researchWithLevel));

            #endregion

            // 1. mail her zaman gidiyor. 2.mail ise sadece gönderilen oyuncu değil ise gidiyor.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailDate = actionDate,
                MailCategoryId = (int)MailCategories.Casusluk,
                MailContent = MailEncoder.EncodeMail(defaultMailContent),
                UserId = userPlanet.UserId
            });
        }

        private static void ExecuteSpyReturnAction(TblUserPlanets userPlanet, TblFleets userFleet) => ExecuteAlwaysInReturn(userPlanet, userFleet);

        private static void ExecuteTransportAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            List<string> defaultMailContent = new List<string>()
            {
                MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE, userFleet.FleetActionTypeId),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETNAME, userPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETNAME, destinationPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate)
            };

            // Kaynakları ekliyoruz.
            destinationPlanet.Metal += userFleet.CarriedMetal;
            destinationPlanet.Crystal += userFleet.CarriedCrystal;
            destinationPlanet.Boron += userFleet.CarriedBoron;

            // Türünü ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.NakliyeRaporu));

            // Mail datasını yüklüyoruz. Taşınan kaynaklar.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_METAL, userFleet.CarriedMetal));
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_CRYSTAL, userFleet.CarriedCrystal));
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_NEW_BORON, userFleet.CarriedBoron));

            // Taşınan kaynakları siliyoruz.
            userFleet.ReturnFleet.CarriedMetal = 0;
            userFleet.ReturnFleet.CarriedCrystal = 0;
            userFleet.ReturnFleet.CarriedBoron = 0;

            // 1. mail her zaman gidiyor. 2.mail ise sadece gönderilen oyuncu değil ise gidiyor.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailDate = actionDate,
                MailCategoryId = (int)MailCategories.Gezegen,
                MailContent = MailEncoder.EncodeMail(defaultMailContent),
                UserId = userPlanet.UserId
            });

            // Eğer aynı kullanıcıya mail atıyorsak atmıyoruz dublike olmasın diye. Ayrıca atarken de geminin dönüyor olmasının hedef gezegen ile alakası yok.
            if (destinationPlanet.UserId != userPlanet.UserId)
            {
                controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
                {
                    IsReaded = false,
                    MailDate = actionDate,
                    MailCategoryId = (int)MailCategories.Gezegen,
                    MailContent = MailEncoder.EncodeMail(defaultMailContent),
                    UserId = destinationPlanet.UserId
                });
            }
        }

        private static void ExecuteTransportReturnAction(MainController controller, TblUserPlanets userPlanet, TblFleets userFleet, DateTime actionDate, TblUserPlanets destinationPlanet)
        {
            List<string> defaultMailContent = new List<string>()
            {
                MailEncoder.GetParam(MailEncoder.KEY_ACTION_TYPE_RETURN, userFleet.FleetActionTypeId),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETNAME, destinationPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_SENDERPLANETCORDINATE, userFleet.SenderCordinate),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETNAME, userPlanet.PlanetName),
                MailEncoder.GetParam(MailEncoder.KEY_DESTINATIONPLANETCORDINATE, userFleet.DestinationCordinate)
            };

            // Gemileri alıyoruz. Kullanıcının gezegenine iade edeceğiz.
            List<Tuple<Ships, int>> ships = ExecuteAlwaysInReturn(userPlanet, userFleet);

            // Gönderilen gemileri string formatına dönüştürüyoruz.
            string shipsWithCounts = string.Join(MailEncoder.KEY_MANY_ITEM_SEPERATOR, ships.Select(x => $"{(int)x.Item1}{MailEncoder.KEY_MANY_ITEM_KEY_VALUE_SEPERATOR}{x.Item2}"));

            // Gemileri maile ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_SHIPS_ATTACKER, shipsWithCounts));

            // Nakliye türünü ekliyoruz.
            defaultMailContent.Add(MailEncoder.GetParam(MailEncoder.KEY_MAIL_TYPE, (int)MailTypes.NakliyeRaporuDönüş));

            // 1. mail her zaman gidiyor. 2.mail ise sadece gönderilen oyuncu değil ise gidiyor.
            controller.UnitOfWork.GetRepository<TblUserMails>().Add(new TblUserMails
            {
                IsReaded = false,
                MailDate = actionDate,
                MailCategoryId = (int)MailCategories.Gezegen,
                MailContent = MailEncoder.EncodeMail(defaultMailContent),
                UserId = userPlanet.UserId
            });
        }

        private static List<Tuple<Ships, int>> ExecuteAlwaysInReturn(TblUserPlanets userPlanet, TblFleets userFleet)
        {
            // Gemileri alıyoruz. Kullanıcının gezegenine iade edeceğiz.
            List<Tuple<Ships, int>> ships = FleetController.FleetDataToShipData(userFleet.FleetData);

            // Dönen her bir gemiyi envantere ekliyoruz.
            foreach (Tuple<Ships, int> ship in ships)
            {
                // BU gezegende bu gemiden var mı?
                TblUserPlanetShips shipInPlanet = userPlanet.TblUserPlanetShips.FirstOrDefault(x => x.ShipId == (int)ship.Item1);

                // Eğer yok ise oluşturuyoruuz.
                if (shipInPlanet == null)
                    userPlanet.TblUserPlanetShips.Add(new TblUserPlanetShips { ShipId = (int)ship.Item1, ShipCount = ship.Item2, UserPlanetId = userFleet.DestinationUserPlanetId.Value });
                else // Eğer gezegende zaten var ise miktarıın arttırıyoruz.
                    shipInPlanet.ShipCount += ship.Item2;
            }

            // Şurada kaynakları geri veriyoruz kullanıcıya.
            userPlanet.Metal += userFleet.CarriedMetal;
            userPlanet.Crystal += userFleet.CarriedCrystal;
            userPlanet.Boron += userFleet.CarriedBoron;

            // Gemileri de dönüyoruz en son.
            return ships;
        }

        #endregion

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