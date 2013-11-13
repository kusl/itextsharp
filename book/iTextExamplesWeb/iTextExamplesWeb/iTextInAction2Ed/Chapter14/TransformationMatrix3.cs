/*
 * This class is part of the book "iText in Action - 2nd Edition"
 * written by Bruno Lowagie (ISBN: 9781935182610)
 * For more info, go to: http://itextpdf.com/examples/
 * This example only works with the AGPL version of iText.
 */
using System;
using System.IO;
using Ionic.Zip;
using iTextSharp.awt.geom;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace kuujinbo.iTextInAction2Ed.Chapter14 {
  public class TransformationMatrix3 : IWriter {
// ===========================================================================
    /** The resulting PDF. */
    public const String RESULT = "transformation_matrix3.pdf";
    /** A PDF with the iText logo that will be transformed. */
    public readonly string RESOURCE = Path.Combine(
      Utility.ResourcePdf, "logo.pdf"
    );
// ---------------------------------------------------------------------------
    public void Write(Stream stream) {
      using (ZipFile zip = new ZipFile()) {
        TransformationMatrix3 t = new TransformationMatrix3();
        zip.AddFile(RESOURCE, "");       
        zip.AddEntry(RESULT, t.CreatePdf());
        zip.Save(stream);             
      }
    }     
// ---------------------------------------------------------------------------    
    /**
     * Creates a PDF document.
     */
    public byte[] CreatePdf() {
      // step 1
      Rectangle rect = new Rectangle(-595, -842, 595, 842);
      using (MemoryStream ms = new MemoryStream()) {
        using (Document document = new Document(rect)) {
          // step 2
          PdfWriter writer = PdfWriter.GetInstance(document, ms);
          // step 3
          document.Open();
          // step 4
          PdfContentByte canvas = writer.DirectContent;
          // draw coordinate system
          canvas.MoveTo(-595, 0);
          canvas.LineTo(595, 0);
          canvas.MoveTo(0, -842);
          canvas.LineTo(0, 842);
          canvas.Stroke();
          // read the PDF with the logo
          PdfReader reader = new PdfReader(RESOURCE);
          PdfTemplate template = writer.GetImportedPage(reader, 1); 
          // add it
          canvas.SaveState();
          canvas.AddTemplate(template, 0, 0);
          AffineTransform af = new AffineTransform();
          af.Translate(-595, 0);
          af.Scale(0.5f, 0.5f);
          canvas.Transform(af);
          canvas.AddTemplate(template, 0, 0);
          canvas.ConcatCTM(AffineTransform.GetTranslateInstance(595, 595));
          canvas.AddTemplate(template, 0, 0);
          canvas.RestoreState();

          canvas.SaveState();
          af = new AffineTransform(1f, 0f, 0.4f, 1f, -750f, -650f);
          canvas.AddTemplate(template, af);
          canvas.RestoreState();
          
          canvas.SaveState();
          af = new AffineTransform(0, -1, -1, 0, 650, 0);
          canvas.AddTemplate(template, af);
          af = new AffineTransform(0, -0.2f, -0.5f, 0, 350, 0);
          canvas.AddTemplate(template, af);
          canvas.RestoreState();         
        }
        return ms.ToArray();
      }
    }
// ===========================================================================
  }
}