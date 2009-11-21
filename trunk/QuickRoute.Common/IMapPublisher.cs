using System;
using System.Collections.Generic;

namespace QuickRoute.Common
{
  public interface IMapPublisher
  {
    string WebServiceUrl { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    PublishResult Publish(MapInfo map);
    GetAllCategoriesResult GetAllCategories();
    GetAllMapsResult GetAllMaps();
    ConnectResult Connect();
  }

  public class MapInfo
  {
    public int ID { get; set; }
    public int UserID { get; set; }
    public int CategoryID { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public string Organiser { get; set; }
    public string Country { get; set; }
    public string Discipline { get; set; }
    public string RelayLeg { get; set; }
    public string MapName { get; set; }
    public string ResultListUrl { get; set; }
    public string Comment { get; set; }
    public byte[] MapImageData { get; set; }
    public byte[] BlankMapImageData { get; set; }
    public string MapImageFileExtension { get; set; }

    public override string ToString()
    {
      return Name + 
        (Date != DateTime.MinValue ? " [" + Date.ToShortDateString() + "]" : "");
    }
  }

  public class Category
  {
    public int ID { get; set; }
    public int UserID { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }



  public class PublishResult
  {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string URL { get; set; }
  }

  public class GetAllMapsResult
  {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public List<MapInfo> Maps { get; set; }
  }

  public class GetAllCategoriesResult
  {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public List<Category> Categories { get; set; }
  }

  public class ConnectResult
  {
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public string Version { get; set; }
  }
}