namespace Domain.Services;

public interface IZPLConverterService
{
  byte[] ZPLToPDF(string zpl);
}