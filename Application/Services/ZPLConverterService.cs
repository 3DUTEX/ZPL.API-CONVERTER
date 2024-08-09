using Domain.Services;
using System.Text.RegularExpressions;
using BinaryKits.Zpl.Viewer;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Application.Services;

public class ZPLConverterService : IZPLConverterService
{
  public byte[] ZPLToPDF(string zplContent)
  {
    List<string> zpls = [];

    if (zplContent.Contains("~DGR:DEMO.GRF"))
    {
      zpls = SplitZpls(zplContent);
    }
    else
    {
      zpls.Add(zplContent);
    }

    var pdfs = new List<byte[]>();

    foreach (var zpl in zpls)
    {
      var imageBytes = ZPLToIMG(zpl);

      if (imageBytes is null) { continue; };

      var pdf = IMGToPDF(imageBytes);

      pdfs.Add(pdf);
    }

    var combinedPdfs = CombineAndSavePdf(pdfs);

    return combinedPdfs;
  }

  private byte[] IMGToPDF(byte[] imageBytes)
  {
    using var streamIMG = new MemoryStream();
    streamIMG.Write(imageBytes);

    using var document = new PdfDocument();

    PdfPage page = document.AddPage();

    XGraphics gfx = XGraphics.FromPdfPage(page);

    XImage image = XImage.FromStream(streamIMG);

#pragma warning disable CS0618 // Type or member is obsolete
    gfx.DrawImage(image, 0, 0, page.Width, page.Height);

    // saving in stream
    using var ms = new MemoryStream();
    document.Save(ms);

    // returning bytes
    return ms.ToArray();
  }

  #region Methods
  byte[] CombineAndSavePdf(List<byte[]> pdfs)
  {
    var outputDocument = new PdfDocument();

    foreach (var pdf in pdfs)
    {
      using var streamPdf = new MemoryStream();
      streamPdf.Write(pdf);

      // Abre o documento PDF existente
      var inputDocument = PdfReader.Open(streamPdf, PdfDocumentOpenMode.Import);

      // Copia todas as páginas para o documento de saída
      for (int i = 0; i < inputDocument.PageCount; i++)
      {
        PdfPage page = inputDocument.Pages[i];
        outputDocument.AddPage(page);
      }
    }

    using var ms = new MemoryStream();

    // Saving document in stream
    outputDocument.Save(ms);

    // returning bytes
    return ms.ToArray();
  }

  byte[]? ZPLToIMG(string zpl)
  {
    IPrinterStorage printerStorage = new PrinterStorage();

    var drawer = new ZplElementDrawer(printerStorage);

    var analyzer = new ZplAnalyzer(printerStorage);

    var analyzeInfo = analyzer.Analyze(zpl);

    var zplElements = analyzeInfo.LabelInfos.FirstOrDefault()?.ZplElements;

    if (zplElements is null) return null;

    return drawer.Draw(zplElements);
  }

  List<string> SplitZpls(string entryZpl)
  {
    var regexSplit = @"(?=~DGR:DEMO.GRF)";

    var zpls = Regex.Split(entryZpl, regexSplit)
        .Where(zpl => !string.IsNullOrEmpty(zpl))
        .ToList();

    return zpls;
  }
  #endregion
}