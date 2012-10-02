/*
 * This class is part of the book "iText in Action - 2nd Edition"
 * written by Bruno Lowagie (ISBN: 9781935182610)
 * For more info, go to: http://itextpdf.com/examples/
 * This example only works with the AGPL version of iText.
 */

using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

/**
 * First iText example: Hello World.
 */
namespace HelloWorldDirect
{
    class HelloWorldDirect
    {
        /** Path to the resulting PDF file. */
        public const String RESULT = "../../../../../results/part1/chapter01/hello_direct.pdf";

        /**
         * Creates a PDF file: hello_direct.pdf
         * @param    args    no arguments needed
         */
        static void Main(String[] args)
        {
            // step 1
            Document document = new Document();
            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, File.Create(RESULT));
            // step 3
            document.Open();
            // step 4
            PdfContentByte canvas = writer.DirectContentUnder;
            writer.CompressionLevel = 0;
            canvas.SaveState();                               // q
            canvas.BeginText();                               // BT
            canvas.MoveText(36, 788);                         // 36 788 Td
            canvas.SetFontAndSize(BaseFont.CreateFont(), 12); // /F1 12 Tf
            canvas.ShowText("Hello World");                   // (Hello World)Tj
            canvas.EndText();                                 // ET
            canvas.RestoreState();                            // Q
            // step 5
            document.Close();
        }
    }
}
